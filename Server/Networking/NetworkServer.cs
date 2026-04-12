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
        /// The passcode clients must provide to join the game.
        /// Change this to whatever code you want players to enter.
        /// </summary>
        private const int ServerPasscode = 1234;

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

            while (true) {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                Debug.WriteLine("Client Connected - awaiting login");
                Log("Client Connected - awaiting login");

                // Handle each client's login on its own background task so the
                // accept loop can keep receiving new connections while we wait.
                _ = Task.Run(() => HandleLoginAsync(tcpClient));

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
            else {
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
            if (_clients.TryGetValue(client, out var session)) {
                _clients.Remove(client); // remove from pool
                Debug.WriteLine("Client Disconnected");
                Log("Client Disconnected");
            }
        }

        /// <summary>
        /// Reads the first packet from a newly connected TCP client, checks the passcode,
        /// and either admits the client into a game session or sends a denial and closes the socket.
        /// </summary>
        private async Task HandleLoginAsync(TcpClient tcpClient)
        {
            var loginSerializer = new LoginSerializer();
            var loginResponseSerializer = new LoginResponseSerializer();

            try {
                NetworkStream stream = tcpClient.GetStream();

                // Read the login packet (up to 256 bytes is plenty for name + int)
                byte[] buffer = new byte[256];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0) {
                    Debug.WriteLine("Client disconnected before sending login");
                    tcpClient.Close();
                    return;
                }

                byte[] raw = new byte[bytesRead];
                Array.Copy(buffer, raw, bytesRead);

                Packet loginPacket = Packet.FromBytes(raw);

                if (loginPacket.Type != PacketType.LoginRequest) {
                    Debug.WriteLine("First packet was not a LoginRequest — rejecting");
                    SendLoginResponse(stream, loginResponseSerializer, false, "Expected a login request.");
                    tcpClient.Close();
                    return;
                }

                LoginDto loginDto = LoginSerializer.Deserialize(loginPacket.Payload);
                Debug.WriteLine($"Login attempt — Name: '{loginDto.PlayerName}', Passcode: {loginDto.Passcode}");
                Log($"Login attempt from '{loginDto.PlayerName}'");

                if (loginDto.Passcode != ServerPasscode) {
                    Debug.WriteLine("Wrong passcode — rejecting client");
                    Log($"Login denied for '{loginDto.PlayerName}' — wrong passcode");
                    SendLoginResponse(stream, loginResponseSerializer, false, "Wrong passcode. Access denied.");
                    tcpClient.Close();
                    return;
                }

                // Passcode correct — send acceptance and set up game session
                SendLoginResponse(stream, loginResponseSerializer, true, "Welcome to JackBlack!");
                Log($"Login accepted for '{loginDto.PlayerName}'");

                AcceptClient(tcpClient, loginDto.PlayerName);
            }
            catch (Exception ex) {
                Debug.WriteLine($"Login error: {ex.Message}");
                try { tcpClient.Close(); } catch { }
            }
        }

        /// <summary>
        /// Writes a <see cref="LoginResponseDto"/> directly to the stream before handing
        /// the socket off to a <see cref="ClientConnection"/> (which takes over the stream).
        /// </summary>
        private static void SendLoginResponse(NetworkStream stream, LoginResponseSerializer serializer, bool accepted, string message)
        {
            var dto = new LoginResponseDto { Accepted = accepted, Message = message };
            byte[] payload = serializer.Serialize(dto);
            Packet pkt = new Packet {
                Type = PacketType.LoginResponse,
                PayloadSize = payload.Length,
                Payload = payload
            };
            byte[] pktBytes = pkt.ToBytes();
            stream.Write(pktBytes, 0, pktBytes.Length);
        }

        /// <summary>
        /// Finishes setting up a verified client: creates the <see cref="ClientConnection"/>,
        /// <see cref="GameManager"/>, and sends the initial player/image packets.
        /// </summary>
        private void AcceptClient(TcpClient tcpClient, string playerName)
        {
            ClientConnection connection = new ClientConnection(tcpClient, HandleClientMessage, HandleClientDisconnect);
            connection.Start();

            GameManager session = new GameManager(connection, OnLog, playerName);
            _clients[connection] = session;

            // Send initial player state
            Player player = new Player() {
                Name = playerName,
                CurrentBet = 0,
                HasDoubled = false,
                HasInsured = false,
                ActionCount = 0,
                Balance = 1000
            };

            PlayerDto playerDto = new PlayerDto(player);
            byte[] playerPayload = _playerSerializer.Serialize(playerDto);
            Packet playerPacket = new Packet {
                Type = PacketType.Player,
                PayloadSize = playerPayload.Length,
                Payload = playerPayload
            };
            connection.Send(playerPacket.ToBytes());

            // Send win/loss images
            byte[] winPayload = PictureSerializer.SerializePic("Winner");
            connection.Send(new Packet { Type = PacketType.EndGame, PayloadSize = winPayload.Length, Payload = winPayload }.ToBytes());

            byte[] lossPayload = PictureSerializer.SerializePic("Loser");
            connection.Send(new Packet { Type = PacketType.EndGame, PayloadSize = lossPayload.Length, Payload = lossPayload }.ToBytes());

            Debug.WriteLine($"Client '{playerName}' fully admitted to game session");
            Log($"Client '{playerName}' admitted to game session");
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