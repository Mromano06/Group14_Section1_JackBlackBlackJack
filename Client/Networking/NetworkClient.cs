using Client.ViewModels;
using Jables_Protocol;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using SharedModels.Core;
using SharedModels.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using System.Windows.Threading;
using SharedModels.Logging;



/// <summary>
/// Network client responsible for sending and receiving messages/commands
/// from the server. Works with a send queue to avoid blocking the UI.
/// </summary>
namespace Client.Networking
{
    /// <summary>
    /// Network client responsible for handling all communication with the game server.
    /// </summary>
    /// <remarks>
    /// This class serves as the central networking layer of the application.
    /// It is responsible for:
    /// <list type="bullet">
    /// <item><description>Establishing TCP connections to the server</description></item>
    /// <item><description>Sending serialized player commands</description></item>
    /// <item><description>Receiving and deserializing server packets</description></item>
    /// <item><description>Dispatching updates to the UI via events</description></item>
    /// </list>
    /// 
    /// Uses background tasks and a thread-safe queue to prevent blocking the UI thread.
    /// </remarks>
    public class NetworkClient
    {
        /// <summary>
        /// Underlying TCP client.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Network stream used for sending and receiving data.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Raised when a player card dto is received from the server.
        /// </summary>
        public event Action<CardDto> PlayerCardUpdate;

        /// <summary>
        /// Raised when a dealer card dto is received from the server.
        /// </summary>
        public event Action<CardDto> DealerCardUpdate;

        /// <summary>
        /// Raised when the player's money is updated.
        /// </summary>
        public event Action<double> PlayerMoneyUpdate;

        /// <summary>
        /// Raised when the player's bet amount is updated.
        /// </summary>
        public event Action<double> PlayerBetUpdate;

        /// <summary>
        /// Raised when the round state changes.
        /// </summary>
        public event Action<bool> RoundCheckUpdate;

        /// <summary>
        /// Raised when a round result is received.
        /// </summary>
        public event Action<ROUND_RESULT> RoundResultUpdate;

        /// <summary>
        /// Raised when a game result is received.
        /// </summary>
        public event Action<GameResult> GameResultUpdate;

        /// <summary>
        /// Raised when the active player index changes.
        /// </summary>
        public event Action<int> PlayerIndexUpdate;

        /// <summary>
        /// Raised when the game ends.
        /// </summary>
        public event Action<string> EndGameUpdate;

        /// <summary>
        /// Stores the most recent player balance received from the server.
        /// </summary>
        public double LatestPlayerMoney { get; private set; }

        /// <summary>
        /// Stores the most recent game result.
        /// </summary>
        public GameResult? LastGameResult { get; set; }

        /// <summary>
        /// Queue to hold outgoing commands/messages (thread-safe)
        /// will change from string to command object once command object is implemented
        /// </summary>
        private readonly ConcurrentQueue<byte[]> sendQueue = new();

        /// <summary>
        /// Token used to cancel the send/receive loops
        /// </summary>
        private CancellationTokenSource cancellation = new();

        /// <summary>
        /// Raised when the server responds to a login attempt.
        /// bool = accepted, string = message from server.
        /// </summary>
        public event Action<bool, string>? LoginResponseReceived;

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
            cancellation = new CancellationTokenSource(); // reset cancellation token

            client = new TcpClient();
            await client.ConnectAsync(host, port); // attempt connection to server

            stream = client.GetStream(); // get stream to server

            // Start the send loop in a background task
            _ = Task.Run(SendLoop);

            // Start the receive loop in a background task
            _ = Task.Run(ReceiveLoop);
        }

        /// <summary>
        /// Serializes and queues a login request packet to be sent to the server.
        /// </summary>
        /// <param name="passcode">The passcode the player entered.</param>
        /// <param name="playerName">The player's chosen display name.</param>
        public void SendLoginRequest(int passcode, string playerName)
        {
            var dto = new Jables_Protocol.DTOs.LoginDto { Passcode = passcode, PlayerName = playerName };
            var serializer = new Jables_Protocol.Serializers.LoginSerializer();
            byte[] payload = serializer.Serialize(dto);

            Jables_Protocol.Packet pkt = new Jables_Protocol.Packet {
                Type = SharedModels.Core.PacketType.LoginRequest,
                PayloadSize = payload.Length,
                Payload = payload
            };

            Send(pkt.ToBytes());
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
            Console.WriteLine("Server:" + data);

            // Deserialize the packet

            // Sort data into player/dealer cards

            Packet packet = Packet.FromBytes(data);

            switch (packet.Type) {
                case PacketType.LoginResponse: {
                        LoginResponseDto dto = LoginResponseSerializer.Deserialize(packet.Payload);
                        FileLogger.Log($"[LOGIN] Accepted: {dto.Accepted}, Message: {dto.Message}");
                        LoginResponseReceived?.Invoke(dto.Accepted, dto.Message);
                        break;
                    }

                case PacketType.Player: {
                        PlayerDto dto = PlayerSerializer.Deserialize(packet.Payload);
                        FileLogger.Log($"[PLAYER] Balance: {dto.Balance}");
                        sendPlayerMoneyUpdate(dto);
                        break;
                    }

                //case player action
                case PacketType.PlayerAction: { PlayerCommandDto dto = PlayerCommandSerializer.Deserialize(packet.Payload); break; }

                //case state update
                case PacketType.StateUpdate: { GameStateDto dto = GameStateSerializer.Deserialize(packet.Payload); break; }

                // GameUpdate
                case PacketType.GameUpdate: {
                        GameUpdateDto dto = GameUpdateSerializer.Deserialize(packet.Payload);
                        FileLogger.Log($"[GAME UPDATE] Balance: {dto.Player.Balance}, Bet: {dto.Player.CurrentBet}, RoundWin: {dto.RoundWin}, GameResult: {dto.gameResult}, IsEndRound: {dto.IsEndRound}");
                        handleGameUpdateDto(dto);
                        break;
                    }

                //Card Dealt
                case PacketType.CardDealt: { CardDto dto = CardSerializer.Deserialize(packet.Payload); break; }

                //Hand Dealt
                case PacketType.HandDealt: { HandDto dto = HandSerializer.Deserialize(packet.Payload); break; }

                //Join request
                //End Game
                case PacketType.EndGame: {
                        string gameResult = PictureSerializer.DeserializePic(packet.Payload);
                        FileLogger.Log($"[END GAME] Result: {gameResult}");
                        handleEndGame(gameResult);
                        break;
                    }
            }

        }

        /// <summary>
        /// Continuously sends messages from the queue to the server.
        /// This runs on a background thread to prevent UI blocking.
        /// </summary>
        private async Task SendLoop()
        {
            try {

                while (!cancellation.Token.IsCancellationRequested && IsConnected) {
                    if (sendQueue.TryDequeue(out byte[]? data)) {
                        if (data == null || data.Length == 0)
                            continue;
                        // send() to the client
                        await stream.WriteAsync(data, 0, data.Length, cancellation.Token);
                    }
                    else {
                        await Task.Delay(5, cancellation.Token); // delay if nothing to send
                    }
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
            finally {
                Disconnect();
            }

        }

        /// <summary>
        /// Continuously reads messages from the server and invokes the
        /// MessageReceived event.
        /// </summary>
        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[2048576]; // adjust size later
            try {

                while (!cancellation.Token.IsCancellationRequested && IsConnected) {
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
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
            finally {
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

        /// <summary>
        /// Handles game update packets and dispatches relevant UI updates.
        /// </summary>
        /// <param name="gameUpdateDto">Deserialized game update data.</param>
        public void handleGameUpdateDto(GameUpdateDto gameUpdateDto)
        {
            // if player cards not null send them to UI

            if (gameUpdateDto.ActionResult != false) {

                if (gameUpdateDto.Player.Hand != null) {
                    // send all cards in the update
                    for (int i = 0; i < gameUpdateDto.Player.CardCount; i++) {

                        CardDto cardDto = new CardDto();
                        cardDto.Rank = gameUpdateDto.Player.Hand[i].Rank; // get the first card rank and suit
                        cardDto.Suit = gameUpdateDto.Player.Hand[i].Suit;
                        sendPlayerCardUpdate(cardDto);
                    }
                    Debug.WriteLine("Sending player cards to dispatcher");
                }

                // if dealer cards not null send them to UI
                if (gameUpdateDto.DealerCards != null) {
                    // send all cards in the update
                    for (int i = 0; i < gameUpdateDto.DealerCardCount; i++) {

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

                if (gameUpdateDto.gameResult == GameResult.PLAYER_WIN || gameUpdateDto.gameResult == GameResult.PLAYER_LOSE) {
                    sendGameResult(gameUpdateDto.gameResult);
                }
                else if (gameUpdateDto.IsEndRound) {
                    sendRoundResult(gameUpdateDto.RoundWin);
                    sendRoundCheck(gameUpdateDto.IsEndRound);
                }

            }
            else {
                Debug.WriteLine("Action result was false, not sending updates to dispatcher");
            }

        }

        /// <summary>
        /// Raises the <see cref="PlayerCardUpdate"/> event to notify subscribers of a new player card.
        /// </summary>
        /// <param name="cardDto">The card dto received from the server.</param>
        /// <remarks>
        /// This method acts as a bridge between the networking layer and the UI,
        /// allowing ViewModels to update the player's hand when new cards are dealt.
        /// </remarks>
        public void sendPlayerCardUpdate(CardDto cardDto)
        {
            PlayerCardUpdate?.Invoke(cardDto);
        }

        /// <summary>
        /// Raises the <see cref="DealerCardUpdate"/> event to notify subscribers of a new dealer card.
        /// </summary>
        /// <param name="cardDto">The card dto received from the server.</param>
        /// <remarks>
        /// Used to update the dealer's visible hand in the UI.
        /// </remarks>
        public void sendDealerCardUpdate(CardDto cardDto)
        {
            // send card dto
            DealerCardUpdate?.Invoke(cardDto);
        }

        /// <summary>
        /// Raises the <see cref="PlayerMoneyUpdate"/> event to update the player's balance.
        /// </summary>
        /// <param name="player">The player dto containing updated balance information.</param>
        /// <remarks>
        /// Ensures the player object is valid and the balance is non-negative before dispatching.
        /// Also updates the cached <see cref="LatestPlayerMoney"/> value.
        /// </remarks>
        public void sendPlayerMoneyUpdate(PlayerDto player)
        {
            if (player == null) {
                Debug.WriteLine("Player Dto Was Empty");
                return;
            }

            if (player.Balance >= 0) {
                Debug.WriteLine("sending player balance to dispatcher");
                LatestPlayerMoney = player.Balance;
                // send player money to UI
                PlayerMoneyUpdate?.Invoke(player.Balance);
            }

        }

        /// <summary>
        /// Raises the <see cref="PlayerBetUpdate"/> event to update the player's current bet.
        /// </summary>
        /// <param name="amount">The updated bet amount.</param>
        /// <remarks>
        /// Only dispatches the update if the amount is non-negative.
        /// </remarks>
        public void sendPlayerBetUpdate(double amount)
        {


            if (amount >= 0) {
                Debug.WriteLine("sending player bet amount to dispatcher");
                // send player bet to UI
                PlayerBetUpdate?.Invoke(amount);
            }

        }

        /// <summary>
        /// Raises the <see cref="RoundCheckUpdate"/> event to indicate a round state change.
        /// </summary>
        /// <param name="roundCheck">Boolean indicating if the round has ended or reset.</param>
        /// <remarks>
        /// Typically used by ViewModels to reset round-specific UI state.
        /// </remarks>
        public void sendRoundCheck(bool roundCheck)
        {
            if (roundCheck) {
                Debug.WriteLine("Sending round check");
                RoundCheckUpdate?.Invoke(roundCheck);
            }
        }

        /// <summary>
        /// Raises the <see cref="RoundResultUpdate"/> event to notify subscribers of the round result.
        /// </summary>
        /// <param name="result">The result of the round.</param>
        /// <remarks>
        /// Used to trigger result displays or transitions between rounds.
        /// </remarks>
        public void sendRoundResult(ROUND_RESULT result)
        {
            Debug.WriteLine("Sending round result");
            RoundResultUpdate?.Invoke(result);
        }

        /// <summary>
        /// Raises the <see cref="GameResultUpdate"/> event to notify subscribers of the game outcome.
        /// </summary>
        /// <param name="result">The final result of the game.</param>
        /// <remarks>
        /// Typically triggers navigation to victory or loss screens.
        /// </remarks>
        public void sendGameResult(GameResult result)
        {
            Debug.WriteLine("Sending game result");
            LastGameResult = result;
            GameResultUpdate?.Invoke(result);
        }

        /// <summary>
        /// Raises the <see cref="PlayerIndexUpdate"/> event to indicate the active player index.
        /// </summary>
        /// <param name="index">The index of the current player.</param>
        /// <remarks>
        /// Useful for multiplayer or turn-based UI updates.
        /// </remarks>
        public void sendPlayerIndex(int index)
        {
            Debug.WriteLine("Sending player index: " + index);
            // send player index to UI
            PlayerIndexUpdate?.Invoke(index);
        }

        public void handleEndGame(string gameResult)
        {
            Debug.WriteLine("Sending end game result: " + gameResult);
            EndGameUpdate?.Invoke(gameResult);
        }

        public void sendDisconnect()
        {
            LastGameResult = GameResult.DEFAULT_RESULT;
            Disconnect();
        }
    }
}