using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using Jables_Protocol;
using Server.GameControl;
using SharedModels.Models;
using SharedModels.Core;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Networking
{
    /// <summary>
    /// Represents the main TCP server responsible for handling client connections.
    /// </summary>
    /// <remarks>
    /// This server listens for incoming client connections, creates a <see cref="ClientConnection"/>
    /// for each client, and associates it with a <see cref="GameManager"/> session.
    /// It also handles message routing, disconnections, and initial data transmission.
    /// </remarks>
    public class NetworkServer
    {
        /// <summary>
        /// TCP listener used to accept incoming client connections.
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// Mapping of active client connections to their respective game sessions.
        /// </summary>
        private Dictionary<ClientConnection, GameManager> _clients = new();

        /// <summary>
        /// Serializer used to convert player objects into byte arrays.
        /// </summary>
        private readonly PlayerSerializer _playerSerializer = new PlayerSerializer();

        /// <summary>
        /// Event triggered when a log message is generated (logger).
        /// </summary>
        public event Action<string> OnLog;

        /// <summary>
        /// Invokes the logging event.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        /// <summary>
        /// Starts the server and begins listening for client connections.
        /// </summary>
        /// <param name="port">The port number to listen on.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method runs indefinitely, accepting new clients and initializing
        /// their corresponding game sessions.
        /// </remarks>
        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Debug.WriteLine("Server listening...");
            Log("Server listening...");
        
            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                Debug.WriteLine("Client Connected");
                Log("Client Connected");

                // create connection with a callback
                ClientConnection connection = new ClientConnection(tcpClient, HandleClientMessage, HandleClientDisconnect);

                // start send receive
                connection.Start();

                GameManager _session = new GameManager(connection, OnLog); // create a game manager for this connection

                _clients[connection] = _session; // add connection to pool

                // DO NOT SET THE HAND OR YOU WILL BREAK EVERYTHING
                Player player = new Player() {
                    Name = "Brodie Arkell",
                    CurrentBet = 0,
                    HasDoubled = false,
                    HasInsured = false,
                    ActionCount = 0,
                    Balance = 1000
                };

                PlayerDto playerDto = new PlayerDto(player);

                Debug.WriteLine("Attempted to send player to client");
                Log("Send Player Initialization");

                byte[] buffer = _playerSerializer.Serialize(playerDto);

                Packet packetToSend = new Packet {
                    Type = PacketType.Player,
                    PayloadSize = buffer.Length,
                    Payload = buffer
                };

                byte[] packetBytes = packetToSend.ToBytes();

                Packet newPacket = Packet.FromBytes(packetBytes);
                PlayerDto decodedPlayer = PlayerSerializer.Deserialize(newPacket.Payload);
                Debug.WriteLine($"TESTING TYPE: {newPacket.Type}");
                Debug.WriteLine($"Player Name: {decodedPlayer.Name}");
                Debug.WriteLine($"Player Balance: {decodedPlayer.Balance}");
                Debug.WriteLine($"Player CardCount: {decodedPlayer.CardCount}");
                Debug.WriteLine($"Player CurrentBet: {decodedPlayer.CurrentBet}");
                Debug.WriteLine($"Player HasDoubled: {decodedPlayer.HasDoubled}");
                Debug.WriteLine($"Player HasInsured: {decodedPlayer.HasInsured}");
                Debug.WriteLine($"Player ActionCount: {decodedPlayer.ActionCount}");

                connection.Send(packetBytes);


                /// Send Win and loss Images to client at start
                byte[] endGamePacketW = PictureSerializer.SerializePic("Winner");

                Packet packetW = new Packet
                {
                    Type = PacketType.EndGame,
                    PayloadSize = endGamePacketW.Length,
                    Payload = endGamePacketW
                };
                byte[] endGamePacketL = PictureSerializer.SerializePic("Loser");

                Packet packetL = new Packet
                {
                    Type = PacketType.EndGame,
                    PayloadSize = endGamePacketL.Length,
                    Payload = endGamePacketL
                };

                connection.Send(packetW.ToBytes());
                connection.Send(packetL.ToBytes());

            }
        
        }

        /// <summary>
        /// Handles incoming messages from a client.
        /// </summary>
        /// <param name="client">The client that tried to send the message.</param>
        /// <param name="data">The raw byte, data buffer received.</param>
        private void HandleClientMessage(ClientConnection client, byte[] data)
        {
            if (_clients.TryGetValue(client, out var session)) {

                session.OnMessageReceived(client, data); // send message to game manager
            }
            else
            {
                Debug.WriteLine("No session found for client");
                Log("No Session Found for Client");
            }

        }

        /// <summary>
        /// Handles client disconnection events.
        /// </summary>
        /// <param name="client">The client that wants to disconnected.</param>
        private void HandleClientDisconnect(ClientConnection client)
        {
            if (_clients.TryGetValue(client, out var session))
            {
                _clients.Remove(client); // remove from pool
                Debug.WriteLine("Client Disconnected");
                Log("Client Disconnected");
            }
        }

        /// <summary>
        /// Stops the server by closing the listener.
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
        }
    }
}
