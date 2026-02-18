using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    public class Bet : IAction
    {
        private string _playerName;
        private double _amount;

        public string Description => $"{_playerName} bets {_amount}";

        public Bet(string playerName, double amount, Game game)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }

            if (amount < game.MinBet) {
                throw new ArgumentException($"Bet cannot be less that minimum bet (${game.MinBet})");
            }

            _playerName = playerName;
            _amount = amount;
        }

        public bool IsExecutable(Game game)
        {
            Player player = game.GetPlayer(_playerName);

            if (player == null) {
                return false;
            }

            // Must be the player turn
            if (game.GetCurrentPlayer().Name != _playerName) {
                return false;
            }

            // Player can't bet if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            // Player can't bet if they dont have enough balance
            if (_amount > player.Balance)
                return false;

            // Player can't bet less than 5 or if they have already bet
            if (_amount < 5 || _amount > 0)
                return false;

            return true; 
        }

        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game))
                return ActionResult.Failed("Invalid bet");

            var player = game.GetPlayer(_playerName);

            // Deduct from balance and set current bet
            player.Balance -= _amount;
            player.CurrentBet = _amount;

            return ActionResult.Successful($"{_playerName} bet {_amount}");
        }
    }
}
