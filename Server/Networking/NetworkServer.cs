using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Server.Networking
{
    public class NetworkServer
    {
        private TcpListener _listener;

        private ConcurrentBag<ClientConnection> clients = new();

        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Debug.WriteLine("Server listening...");
        
            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();

                ClientConnection client = new ClientConnection(tcpClient);

                clients.Add(client);

                client.Start();

                Debug.WriteLine("Client Connected");
                
            }
        
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
