using GameLogic.Actions;
using GameLogic.Actions.ActionTypes;
using GameLogic.Core;
using GameLogic.Logic;
using GameLogic.Models;
using Jables_Protocol;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using Server.Networking;
using SharedModels;
using SharedModels.Core;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;

// Must be strictly defined as it can be "ambigous" with the primitive double 
using Double = GameLogic.Actions.ActionTypes.Double;

namespace Server.GameControl
{
    /// <summary>
    /// Manages the server-side game session for a connected client.
    /// </summary>
    /// <remarks>
    /// Acts as the central controller for a single player's game session, responsible for:
    /// <list type="bullet">
    /// <item><description>Receiving and deserializing player commands from the client.</description></item>
    /// <item><description>Executing game actions such as bet, hit, stand, double, and insure.</description></item>
    /// <item><description>Coordinating dealer logic and round resolution.</description></item>
    /// <item><description>Sending game state updates back to the client.</description></item>
    /// </list>
    /// </remarks>
    /// <author>Evan Travis, Brodie Arkell</author>
    public class GameManager
    {
        // create game
        /// <summary>
        /// The client connection associated with this game session.
        /// </summary>
        private readonly ClientConnection _connection;

        /// <summary>
        /// The current game instance being managed.
        /// </summary>
        private Game _game;

        /// <summary>
        /// The player participating in this game session.
        /// </summary>
        private Player _player;

        // serializers
        /// <summary>
        /// Serializer used to convert game update data into network packets.
        /// </summary>
        private readonly GameUpdateSerializer _gameUpdateSerializer = new GameUpdateSerializer();

        /// <summary>
        /// Callback action used to log game events.
        /// </summary>
        private readonly Action<string> _OnLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        /// <param name="connection">The client connection for this game session.</param>
        /// <param name="OnLog">A callback action for logging game events.</param>
        /// <remarks>
        /// Creates a new game with a minimum bet of 10 and a three-deck shoe,
        /// and adds a default player to the game.
        /// </remarks>
        public GameManager(ClientConnection connection, Action<string> OnLog)
        {
            _connection = connection;
            _OnLog = OnLog;

            Debug.WriteLine("Game Session created for Client");
            _OnLog("Game Session Created for Client");

            // initialize the new game
            _game = new Game(10, new Shoe(3));

            // Add player to game
            _player = new Player("Brodie Arkell", 1000);
            _game.AddPlayer(_player);
        }

        /// <summary>
        /// Callback invoked when a message is received from the client.
        /// </summary>
        /// <param name="sender">The client connection that sent the message.</param>
        /// <param name="data">The raw byte data received from the client.</param>
        /// <remarks>
        /// Deserializes the incoming packet using the Jables Protocol and
        /// routes it to the appropriate handler based on packet type.
        /// </remarks>
        // this is the callback function that will run when message received from client
        // we need to use the Jables_Protocol to deserialize the message here 
        public void OnMessageReceived(ClientConnection sender, byte[] data)
        {
            ///TODO: deserialize the message using custom protocol
            /// and then 
            try {
                // deserialize the packets wrapper
                Packet packet = Packet.FromBytes(data);

                Debug.WriteLine($"Received packet of type: {packet.Type}");

                if (packet.Type == PacketType.PlayerAction) {
                    HandlePlayerCommand(packet.Payload);
                }else if (packet.Type == PacketType.Disconnect)
                {
                    HandleDisconnect();
                }
            }

            catch (Exception ex) {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            Debug.WriteLine("Client meassage received, size of: " + data.Length + " bytes.");
        }

        /// <summary>
        /// Deserializes and routes a player command payload to the appropriate action handler.
        /// </summary>
        /// <param name="payload">The raw byte payload containing the player command.</param>
        private void HandlePlayerCommand(byte[] payload)
        {
            // Deserialize the payloads command
            // using private variables at the top of the class
            //PlayerCommandDto playerCommand = _commandSerializer.Deserialize(payload);

            // using the static deserializer
            PlayerCommandDto playerCommand = PlayerCommandSerializer.Deserialize(payload);

            Debug.WriteLine($"Game Command: {playerCommand.Action}, Bet: {playerCommand.BetAmount}");

            switch (playerCommand.Action) {
                case PlayerAction.Bet:
                    ExecuteBet(playerCommand.BetAmount);
                    break;

                case PlayerAction.Hit:
                    ExecuteHit();
                    break;

                case PlayerAction.Stand:
                    ExecuteStand();
                    break;

                case PlayerAction.Double:
                    ExecuteDouble();
                    break;

                case PlayerAction.Insure:
                    ExecuteInsure();
                    break;

                default:
                    Debug.WriteLine($"Unknown packet: {playerCommand.GetType().Name}");
                    break;
            }
        }

        /// <summary>
        /// Executes a bet action for the current player.
        /// </summary>
        /// <param name="betAmount">The amount the player wishes to bet.</param>
        /// <remarks>
        /// If the betting player is the last in the player list, initial cards are dealt.
        /// Sends a game update to the client after execution.
        /// </remarks>
        /// TODO: I think all of these will need some sort of continue/update to tell the process to start again for the next player
        /// (Side note: they already move the player forward but they gotta tell the server with some sort of function)
        private void ExecuteBet(double betAmount)
        {
            bool IsEndRound = false;
            Bet action = new Bet(_player.Name, betAmount, _game);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Bet action did not complete successfully for: {_player.Name}");
                return;
            }

            _OnLog($"Bet: {_player.Name} {betAmount}");
            Debug.WriteLine($"Bet: {_player.Name}");

            // if we are on the last player then deal initial cards
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.DealInitialCards(_game);
            }

            ROUND_RESULT result = _game.RoundResult(_player);

            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
        }

        /// <summary>
        /// Executes a hit action for the current player.
        /// </summary>
        /// <remarks>
        /// If the player busts and is the last player, the dealer plays their turn
        /// and the round is ended. Sends a game update to the client after execution.
        /// </remarks>
        private void ExecuteHit()
        {
            bool IsEndRound = false;
            Hit action = new Hit(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Hit action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Hit: {_player.Name}");
            _OnLog($"[Hit]: {_player.Name}");

            // Check if bust
            if (HandHelper.IsBust(_player.Hand)) {
                Debug.WriteLine($"Bust: {_player.Name}");
                _OnLog($"[Bust]: {_player.Name}");

                // If its the last player who has now busted then the dealer shall go
                if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                    DealerLogic.PlayTurn(_game);

                    IsEndRound = true;
                }
            }

            ROUND_RESULT result = _game.RoundResult(_player);

            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
            if (IsEndRound) {
                _game.EndRound();
                SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
            }

        }

        /// <summary>
        /// Executes a stand action for the current player.
        /// </summary>
        /// <remarks>
        /// If the standing player is the last in the player list, the dealer plays their turn.
        /// Ends the round and sends a game update to the client after execution.
        /// </remarks>
        private void ExecuteStand()
        {
            bool IsEndRound = true;
            Stand action = new Stand(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Stand action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Stand: {_player.Name}");
            _OnLog($"[Stand] {_player.Name}");

            // If its the last player who has now stood then the dealer shall go
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.PlayTurn(_game);
            }

            ROUND_RESULT result = _game.RoundResult(_player);

            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
            _game.EndRound();
            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);

        }

        /// <summary>
        /// Executes a double down action for the current player.
        /// </summary>
        /// <remarks>
        /// If the doubling player is the last in the player list, the dealer plays their turn.
        /// Ends the round and sends a game update to the client after execution.
        /// </remarks>
        private void ExecuteDouble()
        {
            bool IsEndRound = true;
            Double action = new Double(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Double action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Double: {_player.Name}");
            _OnLog($"[Double] {_player.Name}");

            // If its the last player who has now busted then the dealer shall go
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.PlayTurn(_game);
            }

            ROUND_RESULT result = _game.RoundResult(_player);

            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
            _game.EndRound();
            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
        }

        /// <summary>
        /// Executes an insurance action for the current player.
        /// </summary>
        /// <remarks>
        /// Does not end the round. Sends a game update to the client after execution.
        /// </remarks>
        private void ExecuteInsure()
        {
            Insure action = new Insure(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Insure action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Insure: {_player.Name}");
            _OnLog($"[Insure] {_player.Name}");
            bool IsEndRound = false;

            ROUND_RESULT result = _game.RoundResult(_player);

            SendGameUpdate(IsEndRound, _player, actionResult.Success, result);
        }

        /// <summary>
        /// Builds and sends a game state update packet to the connected client.
        /// </summary>
        /// <param name="IsEndRound">Indicates whether the current round has ended.</param>
        /// <param name="player">The player whose state is being reported.</param>
        /// <param name="actionResult">Indicates whether the last action was successful.</param>
        /// <param name="roundWin">The round result for the player.</param>
        /// <remarks>
        /// Constructs a <see cref="GameUpdateDto"/> containing player state, dealer cards,
        /// game state, and round outcome, then serializes and sends it as a packet.
        /// </remarks>
        private void SendGameUpdate(bool IsEndRound, Player player, bool actionResult, ROUND_RESULT roundWin)
        {
            List<CardDto> dealerCards = new List<CardDto>();
            PlayerDto playerDto = new PlayerDto(player);
            GameResult gameresult = GameResult.DEFAULT_RESULT;


            foreach (Card card in _game.Dealer.Hand.Cards) {
                CardDto cardDto = new CardDto() {
                    Rank = card.Rank,
                    Suit = card.Suit,
                };

                dealerCards.Add(cardDto);
            }

            if (player.Balance >= 1200) {
                gameresult = GameResult.PLAYER_WIN;
            }
            else if (player.Balance <= 0 && player.CurrentBet <= 0) {
                gameresult = GameResult.PLAYER_LOSE;
            }

            GameUpdateDto dto = new GameUpdateDto() {
                Player = playerDto,

                IsEndRound = IsEndRound,

                GameState = _game.GameState.State,

                DealerCardCount = HandHelper.CardCount(_game.Dealer.Hand),
                DealerCards = dealerCards,

                CurrentPlayerIndex = _game.CurrentPlayerIndex,

                ActionResult = actionResult,
                RoundWin = roundWin,

                gameResult = gameresult
            };

            SendPacket(PacketType.GameUpdate, _gameUpdateSerializer.Serialize(dto));

            if (gameresult == GameResult.PLAYER_WIN) {
                SendPacket(PacketType.EndGame, PictureSerializer.SerializePic("Winner"));
            }
            else if (gameresult == GameResult.PLAYER_LOSE) {
                SendPacket(PacketType.EndGame, PictureSerializer.SerializePic("Loser"));
            }
        }

        private void HandleDisconnect()
        {
            Debug.WriteLine($"Client {_player.Name} disconnected.");
            _OnLog($"Client {_player.Name} disconnected.");
            _connection.Disconnect();  
        }


        /// <summary>
        /// Serializes and sends a packet of the specified type to the connected client.
        /// </summary>
        /// <param name="type">The type of packet to send.</param>
        /// <param name="payload">The serialized byte payload to include in the packet.</param>
        private void SendPacket(PacketType type, byte[] payload)
        {
            Packet packet = new Packet {
                Type = type,
                PayloadSize = payload.Length,
                Payload = payload
            };

            _connection.Send(packet.ToBytes());
        }
    }
}