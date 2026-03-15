using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using Server.GameControl;

namespace Server.Networking
{
    public class NetworkServer
    {
        private TcpListener _listener;
        private Dictionary<ClientConnection, GameManager> _clients = new();

        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Debug.WriteLine("Server listening...");

        
            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                Debug.WriteLine("Client Connected");

                // create connection with a callback
                ClientConnection connection = new ClientConnection(tcpClient, HandleClientMessage);

                // start send receive
                connection.Start();

                GameManager _session = new GameManager(connection); // create a game manager for this connection
                _clients[connection] = _session; // add connection to pool

                Debug.WriteLine("Game Session created for Client");
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
            }

        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
