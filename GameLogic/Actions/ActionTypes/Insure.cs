using GameLogic.Core;
using GameLogic.Logic;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Actions.ActionTypes
{
    public class Insure : IAction
    {
        private string _playerName;

        public string Description => $"{_playerName} insures";

        public Insure(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }

            _playerName = playerName;
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

            // Player can't insure if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            // Player can't insure if they have already played an action
            if (player.ActionCount <= 0)
                return false;

            // Player can't insure if they dont have a high enough balance
            if (player.Balance < (player.CurrentBet * 1.5))
                return false;

            // Player can't insure if they have already insure this round
            if (player.HasInsured)
                return false;

            return true;
        }

        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game)) {
                return ActionResult.Failed("Cannot double at this time");
            }

            Player player = game.GetPlayer(_playerName);

            // Deduct from balance and set current bet
            player.Balance -= player.CurrentBet * 0.5;

            player.CurrentBet *= 1.5;

            player.HasInsured = true;

            return ActionResult.Successful($"{_playerName} insured");
        }
    }
}
