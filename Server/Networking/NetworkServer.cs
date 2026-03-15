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
        private ConcurrentBag<ClientConnection> clients = new();
        private GameManager _session;

        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Debug.WriteLine("Server listening...");

            _session = new GameManager();
        
            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();

                ClientConnection connection = new ClientConnection(tcpClient, _session.HandleMessage);

                clients.Add(connection);
                _session.AddClient(connection);

                connection.Start();

                Debug.WriteLine("Client Connected");
                
            }
        
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
