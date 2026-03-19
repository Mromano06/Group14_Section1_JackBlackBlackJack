using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    public class Hit : IAction
    {
        private string _playerName;
        public string Description => $"{_playerName} hits";

        public Hit(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }

            _playerName = playerName;
        }

        public bool IsExecutable(Game game)
        {
            Player player;
            try {
                player = game.GetPlayer(_playerName);
            }
            catch (ArgumentException) {
                return false;
            }

            if (player == null) {
                return false;
            }

            // Must be the player turn
            if (game.GetCurrentPlayer().Name != _playerName) {
                return false; 
            }

            // Player can't hit if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            return true;
        }

        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game)) {
                return ActionResult.Failed("Cannot hit at this time");
            }

            Player player = game.GetPlayer(_playerName);

            Card card = game.Shoe.DrawCard();
            HandHelper.AddCardToHand(player.Hand, card);

            int handValue = HandHelper.CalculateHandValue(player.Hand);
            bool isBust = HandHelper.IsBust(player.Hand);

            if (isBust) {
                game.NextPlayer();
                return ActionResult.Successful($"{_playerName} drew {card} and busted with {handValue}");
            }

            player.ActionCount++;

            return ActionResult.Successful($"{_playerName} drew {card}, hand value: {handValue}");
        }
    }
}
