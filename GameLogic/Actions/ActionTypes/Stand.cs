using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    public class Stand : IAction
    {
        private string _playerName;

        public string Description => $"{_playerName} stands";

        public Stand(string playerName)
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

            // Player can't stand if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            return true;
        }

        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game)) {
                return ActionResult.Failed("Cannot stand at this time");
            }

            Player player = game.GetPlayer(_playerName);

            game.NextPlayer();

            return ActionResult.Successful($"{_playerName} stood");
        }
    }
}
