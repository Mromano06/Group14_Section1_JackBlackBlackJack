using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.Networking
{
    public class ClientConnection
    {
        private TcpClient _client;
        private NetworkStream stream;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();

        // turn this from a string to a DTO
        private ConcurrentQueue<string> sendQueue = new();



        public ClientConnection(TcpClient client)
        {
            this._client = client;
            this.stream = client.GetStream();
        }

        public void Start()
        {
            _ = Task.Run(ReceiveLoop);
            _ = Task.Run(SendLoop);
        }

        private async Task ReceiveLoop()
        {
            while (true) /// TODO: create logic for deserializing client packets
            {
                //Packet packet = await PacketSerializer.ReadPacket(stream);

                //HandlePacket(packet);
            }
        }

        ///TODO: make changes so that we can use custom protocol
        private async Task SendLoop()
        {
            while (!cancellation.Token.IsCancellationRequested)
            {
                if (sendQueue.TryDequeue(out string? message))
                {
                    if (message == null)
                        continue;

                    byte[] data = Encoding.UTF8.GetBytes(message);

                    await stream.WriteAsync(data, 0, data.Length);
                }
                else 
                {
                    await Task.Delay(5);
                }
            }
        }
    }
}
