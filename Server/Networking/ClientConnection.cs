using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server.Networking
{
    /// <summary>
    /// Represents a single connected client on the server.
    /// </summary>
    /// <remarks>
    /// This class manages communication with the client by maintaining
    /// separate asynchronous send/receive loops. It also provides
    /// callback hooks for received messages, disconnect events, and logging.
    /// </remarks>
    public class ClientConnection
    {
        /// <summary>
        /// The TCP client associated with this connection.
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The network stream used to read and write data for this client.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Reference to the network server that owns this connection (unsused).
        /// </summary>
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();

        /// <summary>
        /// Token source used to end the send and receive loops.
        /// </summary>
        private readonly Action<ClientConnection, byte[]> _onMessageReceived; // callback function to send message to handler 

        /// <summary>
        /// Callback function to send message to handler.
        /// </summary>
        private readonly Action<ClientConnection> _onDisconnect; // callback function to notify handler of disconnection

        /// <summary>
        /// Callback function to notify handler of disconnection
        /// </summary>
        private readonly Action<string> log;

        /// <summary>
        /// Callback used for logging server messages.
        /// </summary>
        private ConcurrentQueue<byte[]> sendQueue = new();

        /// <summary>
        /// Queue of outgoing messages waiting to be sent to the client.
        /// </summary>
        public bool IsConnected => _client?.Connected ?? false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnection"/> class.
        /// </summary>
        /// <param name="client">The connected TCP client.</param>
        /// <param name="onMessageReceived">
        /// Callback invoked when data is received from the client.
        /// </param>
        /// <param name="onDisconnect">
        /// Callback invoked when the client disconnects.
        /// </param>
        /// <param name="onLog">
        /// Callback used for logging connection-related events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="client"/> or <paramref name="onMessageReceived"/> is null.
        /// </exception>
        public ClientConnection(TcpClient client, Action<ClientConnection, byte[]> onMessageReceived, Action<ClientConnection> onDisconnect) // messagereceived is callback function
        {
            this._client = client ?? throw new ArgumentNullException(nameof(_client));
            this.stream = client.GetStream();
            _onMessageReceived = onMessageReceived ?? throw new ArgumentNullException(nameof(onMessageReceived)); // callback function
            _onDisconnect = onDisconnect;
        }

        /// <summary>
        /// Starts the asynchronous send and receive loops for this client.
        /// </summary>
        /// <remarks>
        /// Two background tasks are launched:
        /// one for receiving incoming messages and one for sending queued outgoing messages.
        /// </remarks>
        public void Start()
        {
            _ = Task.Run(ReceiveLoop);
            _ = Task.Run(SendLoop);
        }

        /// <summary>
        /// Queues data to be sent to the client.
        /// </summary>
        /// <param name="data">The byte array to send.</param>
        public void Send(byte[] data)
        {
            sendQueue.Enqueue(data);
        }

        /// Continuously listens for incoming data from the client.
        /// </summary>
        /// <remarks>
        /// Data is read from the network stream into a buffer, trimmed to the
        /// actual number of bytes read, and then passed to the message handler callback.
        /// The loop exits if the client disconnects, cancellation is requested,
        /// or an exception occurs.
        /// </remarks>
        /// <returns>A task representing the asynchronous receive operation.</returns>
        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096]; // adjust size later
            try
            {

                while (!cancellation.Token.IsCancellationRequested && IsConnected) /// TODO: create logic for deserializing client packets
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellation.Token);
                    if (bytesRead == 0)
                        break; // connection closed

                    // reduce buffer to size read
                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);

                    // forward message to the game session handler
                    _onMessageReceived(this, data);
                }
            }
            catch (Exception ex)
                {
                Debug.WriteLine(ex.ToString());
                }
            finally {
                Disconnect();
            }
        }

        /// <summary>
        /// Continuously sends queued messages to the client.
        /// </summary>
        /// <remarks>
        /// This loop checks the outgoing message queue for pending data.
        /// If data is available, it is written to the network stream.
        /// If the queue is empty, the loop pauses briefly before checking again.
        /// </remarks>
        /// <returns>A task representing the asynchronous send operation.</returns>
        private async Task SendLoop()
        {
            try
            {

                while (!cancellation.Token.IsCancellationRequested && IsConnected)
                {
                    if (sendQueue.TryDequeue(out byte[]? data))
                    {
                        if (data == null || data.Length == 0)
                            continue;
                        // send() to the client
                        await stream.WriteAsync(data, 0, data.Length, cancellation.Token);
                    }
                    else
                    {
                        await Task.Delay(5, cancellation.Token); // delay if nothing to send
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {

                Disconnect();
            }
        }

        /// <summary>
        /// Disconnects the client and cleans up connection resources.
        /// </summary>
        /// <remarks>
        /// This method cancels active send and receive operations, closes the
        /// network stream and TCP client, and notifies the disconnect handler.
        /// </remarks>
        public void Disconnect()
        {
            cancellation.Cancel();
            try { stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
            _onDisconnect?.Invoke(this); // notify handler of disconnection

        }
    }
}
