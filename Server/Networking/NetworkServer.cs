using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Server.Networking
{
    public class NetworkServer
    {
        private TcpListener _listener;

        private List<ClientConnection> clients = new List<ClientConnection>();

        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Console.WriteLine("Server listening...");
        
            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();

                ClientConnection client = new ClientConnection(tcpClient);

                clients.Add(client);

                client.Start();

                Console.WriteLine("Client Connected");
                
            }
        
        }
    }
}
