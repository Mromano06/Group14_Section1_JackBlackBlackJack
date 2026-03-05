using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Policy;



/// <summary>
/// Network client responsible for sending and receiving messages/commands
/// from the server. Works with a send queue to avoid blocking the UI.
/// </summary>
namespace Client.Networking
{
    public class NetworkClient
    {

        private TcpClient client;
        private NetworkStream stream;

        // Queue to hold outgoing commands/messages (thread-safe)
        // will change from string to command object once command object is implemented
        private readonly ConcurrentQueue<string> sendQueue = new();

        // Token used to cancel the send/receive loops
        private readonly CancellationTokenSource cancellation = new();

        /// <summary>
        /// Event raised whenever a message is received from the server.
        /// Your ViewModel can subscribe to update UI/game state.
        /// </summary>
        public event Action<string>? MessageReceived;

        /// <summary>
        /// Indicates if the client is currently connected to the server.
        /// </summary>
        public bool IsConnected => client?.Connected ?? false;

        /// <summary>
        /// Connects to the server at the specified host/port and starts
        /// the send and receive loops.
        /// </summary>
        public async Task Connect(string host, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(host, port); // attempt connection to server

            stream = client.GetStream(); // get stream to server

            // Start the send loop in a background task
            _ = Task.Run(SendLoop);

            // Start the receive loop in a background task
            _ = Task.Run(ReceiveLoop);
        }

        /// TODO: Change to command object eventually
        public void EnqueueCommand(string command)
        {
            /// Change to command object eventually
            sendQueue.Enqueue(command);
        }


        /// <summary>
        /// Enqueues a message/command to be sent to the server.
        /// </summary>
        public void Send(string message)
        {
            sendQueue.Enqueue(message);
        }


        /// <summary>
        /// Continuously sends messages from the queue to the server.
        /// This runs on a background thread to prevent UI blocking.
        /// </summary>
        private async Task SendLoop()
        {
            while (!cancellation.Token.IsCancellationRequested)
            {

                /// This is where we would add our custom protocol instead of just queuing and sending string decoded
                if (sendQueue.TryDequeue(out string message))
                {
                    byte[] data = Encoding.UTF8.GetBytes(message + "\n");

                    await stream.WriteAsync(data, 0, data.Length); // placeholder async write
                }
                else
                {
                    await Task.Delay(5);
                }
            }

        }

        /// <summary>
        /// Continuously reads messages from the server and invokes the
        /// MessageReceived event.
        /// </summary>
        private async Task ReceiveLoop()
        {
            var reader = new StreamReader(stream, Encoding.UTF8);

            while (!cancellation.Token.IsCancellationRequested)
            {
                string line = await reader.ReadLineAsync();

                if (line == null)
                    break;

                HandleMessage(line);
            }
        }

        /// <summary>
        /// Handles a single message received from the server.
        /// Currently, just raises MessageReceived event.
        /// Later, you can parse your custom protocol here.
        /// </summary>
        private void HandleMessage(string message)
        {
            Console.WriteLine("Server:" + message); /// add better logging here
            
            /// TODO:
            /// parse message from server to handle message and update the UI
            /// Example: if message starts with "Player_Hit", update player state
        }


        /// <summary>
        /// Disconnects the client and stops all background loops.
        /// </summary>
        public void Disconnect()
        {
            cancellation.Cancel();
            stream?.Close();
            client?.Close();
        }





    }
}
