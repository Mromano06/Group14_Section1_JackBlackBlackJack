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
        private readonly PlayerCommandSerializer _commandSerializer = new PlayerCommandSerializer();
        private readonly GameStateSerializer _gameStateSerializer = new GameStateSerializer();

        public GameManager(ClientConnection connection)
        {
            _connection = connection;

            // initialize the new game
            _game = new Game(5, new Shoe(3));

            // Add player to game
            _player = new Player("Brodie Arkell", 1000);
            _game.AddPlayer(_player);

            ///TODO: once we have a game loop add whatever is needed for Game Manager here
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

                HandleCommand(packet.Payload);
            }

            catch (Exception e) {
                Debug.WriteLine($"Error: {e.Message}");
            }
            
            Debug.WriteLine("Client meassage received, size of: " +  data.Length + " bytes.");

            ///This method should lead into HandleMessage (pass DTO into params)

        }

        private void HandleCommand(byte[] payload)
        {
            // Deserialize the payloads command
            PlayerCommandDto playerCommand = _commandSerializer.Deserialize(payload);

            Debug.WriteLine($"[GAME] Command: {playerCommand.Action}, Bet: {playerCommand.BetAmount}");

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
        /// (Side note: they already move the player forware but they gotta tell the server with some sort of function)
        private void ExecuteBet(double betAmount)
        {
            Bet action = new Bet(_player.Name, betAmount, _game);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Bet action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Bet: {_player.Name}");

            // if we are on the last player then deal initial cards
            if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                DealerLogic.DealInitialCards(_game);
            }
        }

        private void ExecuteHit()
        {
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

                    _game.EndRound();
                }
            }
            else {
                // TODO: some sort of continue/update 
            }
        }

        private void ExecuteStand()
        {
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

                _game.EndRound();
            }
        }

        private void ExecuteDouble()
        {
            Double action = new Double(_player.Name);
            ActionResult actionResult = action.Execute(_game);

            if (!actionResult.Success) {
                Debug.WriteLine($"Double action did not complete successfully for: {_player.Name}");
                return;
            }

            Debug.WriteLine($"Double: {_player.Name}");

            // Check if bust
            if (HandHelper.IsBust(_player.Hand)) {
                Debug.WriteLine($"Bust: {_player.Name}");

                // If its the last player who has now busted then the dealer shall go
                if (_player.Name == _game.Players[_game.MaxPlayers - 1].Name) {
                    DealerLogic.PlayTurn(_game);

                    _game.EndRound();
                }
            }
            else {
                // TODO: some sort of continue/update 
            }
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
        }

        public void HandleMesssage(ClientConnection sender, byte[] data)
        {
            ///TODO: Create the logic that will manage and change the game based on messages from client



            /// Might lead into another method that would deal with either the game or responding to client (sender.Send(data))


            Debug.WriteLine("test");
        }

    }
}
