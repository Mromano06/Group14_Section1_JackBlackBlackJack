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
    public class NetworkServer
    {
        private TcpListener _listener;
        private Dictionary<ClientConnection, GameManager> _clients = new();
        private readonly PlayerSerializer _playerSerializer = new PlayerSerializer();

        // logger
        public event Action<string> OnLog; 

        // invoking the action for logging
        private void Log(string message)
        {
            OnLog?.Invoke(message);
        }

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
                ClientConnection connection = new ClientConnection(tcpClient, HandleClientMessage);

                // start send receive
                connection.Start();

                GameManager _session = new GameManager(connection, OnLog); // create a game manager for this connection

                _clients[connection] = _session; // add connection to pool

                PlayerDto playerDto = new PlayerDto();
                playerDto.Name = "Evan Travis";
                playerDto.CardCount = 0;
                playerDto.Hand = null;
                playerDto.CurrentBet = 0;
                playerDto.HasDoubled = false;
                playerDto.HasInsured = false;
                playerDto.ActionCount = 0;
                playerDto.Balance = 999.99;

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
            }
        
        }

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

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
