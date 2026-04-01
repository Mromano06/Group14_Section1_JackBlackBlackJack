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
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;


// Must be strictly defined as it can be "ambigous" with the primitive double 
using Double = GameLogic.Actions.ActionTypes.Double;

namespace Server.GameControl
{
    public class GameManager
    {
        // create game
        private readonly ClientConnection _connection;
        private Game _game;
        private Player _player;

        // serializers
        private readonly GameUpdateSerializer _gameUpdateSerializer = new GameUpdateSerializer();
        private readonly Action<string> _OnLog;

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
                }
            }

            catch (Exception ex) {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            
            Debug.WriteLine("Client meassage received, size of: " +  data.Length + " bytes.");
        }

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

            SendGameUpdate(IsEndRound, _player);
        }

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

            // Check if bust
            if (HandHelper.IsBust(_player.Hand)) {
                Debug.WriteLine($"Bust: {_player.Name}");

                // If its the last player who has now busted then the dealer shall go
                if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                    DealerLogic.PlayTurn(_game);

                    IsEndRound = true;
                }
            }

            if (IsEndRound) {
                _game.EndRound();
            }

            SendGameUpdate(IsEndRound, _player);
        }

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

            // If its the last player who has now stood then the dealer shall go
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.PlayTurn(_game);
            }

            _game.EndRound();
            SendGameUpdate(IsEndRound, _player);

        }

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

            // If its the last player who has now busted then the dealer shall go
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.PlayTurn(_game);
            }

            _game.EndRound();
            SendGameUpdate(IsEndRound, _player);
        }

        private void ExecuteInsure()
        {
            Insure action = new Insure(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Insure action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Insure: {_player.Name}");

            bool IsEndRound = false;

            SendGameUpdate(IsEndRound, _player);
        }

        private void SendGameUpdate(bool IsEndRound, Player player)
        {
            List<CardDto> dealerCards = new List<CardDto>();
            PlayerDto playerDto = new PlayerDto(player);

            foreach (Card card in _game.Dealer.Hand.Cards) {
                CardDto cardDto = new CardDto() {
                    Rank = card.Rank,
                    Suit = card.Suit,
                };

                dealerCards.Add(cardDto);
            }

            GameUpdateDto dto = new GameUpdateDto() {
                Player = playerDto,

                IsEndRound = IsEndRound,

                GameState = _game.GameState.State,

                DealerCardCount = HandHelper.CardCount(_game.Dealer.Hand),
                DealerCards = dealerCards,

                CurrentPlayerIndex = _game.CurrentPlayerIndex
            };

            SendPacket(PacketType.GameUpdate, _gameUpdateSerializer.Serialize(dto));
        }

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
