using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jables_Protocol;
using Jables_Protocol.Serializers;
using Jables_Protocol.DTOs;
using SharedModels.Core;
using Client.ViewModels;
using System.Windows.Threading;
using System.Windows.Media.Converters;
using System.Security.RightsManagement;



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
        public event Action<CardDto> PlayerCardUpdate;
        public event Action<CardDto> DealerCardUpdate;
        public event Action<double> PlayerMoneyUpdate;
        public event Action<double> PlayerBetUpdate;
        public event Action<bool> RoundCheckUpdate;
        public event Action<ROUND_RESULT> RoundResultUpdate;
        public event Action<int> PlayerIndexUpdate;
        public double LatestPlayerMoney { get; private set; }

        // Queue to hold outgoing commands/messages (thread-safe)
        // will change from string to command object once command object is implemented
        private readonly ConcurrentQueue<byte[]> sendQueue = new();

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

        /// <summary>
        /// Enqueues a message/command to be sent to the server.
        /// </summary>
        public void Send(byte[] data)
        {
            sendQueue.Enqueue(data);
        }

        /// <summary>
        /// Handles a single message received from the server.
        /// Currently, just raises MessageReceived event.
        /// Later, you can parse your custom protocol here.
        /// </summary>
        private void HandleMessage(byte[] data)
        {
            Console.WriteLine("Server:" + data); /// add better logging here

            // Deserialize the packet

            // Sort data into player/dealer cards

            // MainWindowViewModel _mainWindow = new MainWindowViewModel();
            /// TODO:
            /// parse message from server to handle message and update the UI
            /// Example: if message starts with "Player_Hit", update player state

            Packet packet = Packet.FromBytes(data);

            switch (packet.Type)
            {
                //case SharedModels.Core.PacketType.Error: // Not sure what to do with Error yet.
                // Player
                case PacketType.Player:
                    {
                        PlayerDto dto = PlayerSerializer.Deserialize(packet.Payload);
                        sendPlayerMoneyUpdate(dto);
                        break;
                    }

                //case player action
                case PacketType.PlayerAction: { PlayerCommandDto dto = PlayerCommandSerializer.Deserialize(packet.Payload); break; }

                //case state update
                case PacketType.StateUpdate: { GameStateDto dto = GameStateSerializer.Deserialize(packet.Payload); break; }

                // GameUpdate
                case PacketType.GameUpdate:
                    {
                        GameUpdateDto dto = GameUpdateSerializer.Deserialize(packet.Payload);
                        handleGameUpdateDto(dto);
                        break;
                    }

                //Card Dealt
                case PacketType.CardDealt: { CardDto dto = CardSerializer.Deserialize(packet.Payload); break; }

                //Hand Dealt
                case PacketType.HandDealt: { HandDto dto = HandSerializer.Deserialize(packet.Payload); break; }

                    //Join request
            }

        }

        /// <summary>
        /// Continuously sends messages from the queue to the server.
        /// This runs on a background thread to prevent UI blocking.
        /// </summary>
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
        /// Continuously reads messages from the server and invokes the
        /// MessageReceived event.
        /// </summary>
        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096]; // adjust size later
            try
            {

                while (!cancellation.Token.IsCancellationRequested && IsConnected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellation.Token);
                    if (bytesRead == 0)
                        break; // connection closed

                    // reduce buffer to size read
                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);

                    // forward message to the game session handler
                    HandleMessage(data);
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
        /// Disconnects the client and stops all background loops.
        /// </summary>
        public void Disconnect()
        {
            cancellation.Cancel();
            stream?.Close();
            client?.Close();
        }

        public void handleGameUpdateDto(GameUpdateDto gameUpdateDto)
        {
            // if player cards not null send them to UI

            if (gameUpdateDto.ActionResult != false)
            {

                if (gameUpdateDto.Player.Hand != null)
                {
                    // send all cards in the update
                    for (int i = 0; i < gameUpdateDto.Player.CardCount; i++)
                    {

                        CardDto cardDto = new CardDto();
                        cardDto.Rank = gameUpdateDto.Player.Hand[i].Rank; // get the first card rank and suit
                        cardDto.Suit = gameUpdateDto.Player.Hand[i].Suit;
                        sendPlayerCardUpdate(cardDto);
                    }
                    Debug.WriteLine("Sending player cards to dispatcher");
                }

                // if dealer cards not null send them to UI
                if (gameUpdateDto.DealerCards != null)
                {
                    // send all cards in the update
                    for (int i = 0; i < gameUpdateDto.DealerCardCount; i++)
                    {

                        CardDto cardDto = new CardDto();
                        cardDto.Rank = gameUpdateDto.DealerCards[i].Rank; // get the first card rank and suit
                        cardDto.Suit = gameUpdateDto.DealerCards[i].Suit;
                        sendDealerCardUpdate(cardDto);
                    }
                    Debug.WriteLine("Sending dealer cards to dispatcher");
                }
                sendPlayerBetUpdate(gameUpdateDto.Player.CurrentBet);
                sendPlayerMoneyUpdate(gameUpdateDto.Player);
                sendPlayerIndex(gameUpdateDto.CurrentPlayerIndex);


                if (gameUpdateDto.IsEndRound)
                {
                    sendRoundResult(gameUpdateDto.RoundWin);
                    sendRoundCheck(gameUpdateDto.IsEndRound);
                }

            }
            else
            {
                Debug.WriteLine("Action result was false, not sending updates to dispatcher");
            }

        }

        public void sendPlayerCardUpdate(CardDto cardDto)
        {
            PlayerCardUpdate?.Invoke(cardDto);
        }

        public void sendDealerCardUpdate(CardDto cardDto)
        {
            // send card dto
            DealerCardUpdate?.Invoke(cardDto);
        }

        public void sendPlayerMoneyUpdate(PlayerDto player)
        {
            if (player == null)
            {
                Debug.WriteLine("Player Dto Was Empty");
                return;
            }

            if (player.Balance >= 0)
            {
                Debug.WriteLine("sending player balance to dispatcher");
                LatestPlayerMoney = player.Balance;
                // send player money to UI
                PlayerMoneyUpdate?.Invoke(player.Balance);
            }

        }

        public void sendPlayerBetUpdate(double amount)
        {


            if (amount >= 0)
            {
                Debug.WriteLine("sending player bet amount to dispatcher");
                // send player bet to UI
                PlayerBetUpdate?.Invoke(amount);
            }

        }

        public void sendRoundCheck(bool roundCheck)
        {
            if (roundCheck)
            {
                Debug.WriteLine("Sending round check");
                RoundCheckUpdate?.Invoke(roundCheck);
            }
        }

        public void sendRoundResult(ROUND_RESULT result)
        {
            Debug.WriteLine("Sending round result");
            RoundResultUpdate?.Invoke(result);
        }

        public void sendPlayerIndex(int index)
        {
            Debug.WriteLine("Sending player index: " + index);
            // send player index to UI
            PlayerIndexUpdate?.Invoke(index);
        }
    }
}
