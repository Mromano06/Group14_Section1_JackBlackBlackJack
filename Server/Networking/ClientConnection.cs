using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server.Networking
{
    public class ClientConnection
    {
        private TcpClient _client;
        private NetworkStream stream;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private readonly Action<ClientConnection, byte[]> _onMessageReceived; // callback function to send message to handler 
        private readonly Action<ClientConnection> _onDisconnect; // callback function to notify handler of disconnection
        private readonly Action<string> log;


        // queue for outgoing messages
        private ConcurrentQueue<byte[]> sendQueue = new();

        // check for connection
        public bool IsConnected => _client?.Connected ?? false;

        public ClientConnection(TcpClient client, Action<ClientConnection, byte[]> onMessageReceived, Action<ClientConnection> onDisconnect) // messagereceived is callback function
        {
            this._client = client ?? throw new ArgumentNullException(nameof(_client));
            this.stream = client.GetStream();
            _onMessageReceived = onMessageReceived ?? throw new ArgumentNullException(nameof(onMessageReceived)); // callback function
            _onDisconnect = onDisconnect;
        }

        // start async tasks: the send/receive loops that run concurrently
        public void Start()
        {
            _ = Task.Run(ReceiveLoop);
            _ = Task.Run(SendLoop);
        }

        public void Send(byte[] data)
        {
            sendQueue.Enqueue(data);
        }

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

        public void Disconnect()
        {
            cancellation.Cancel();
            try { stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
            _onDisconnect?.Invoke(this); // notify handler of disconnection

        }
    }
}
