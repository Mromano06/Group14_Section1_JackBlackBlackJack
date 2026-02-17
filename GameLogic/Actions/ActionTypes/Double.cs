using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    public class Double : IAction
    {
        private string _playerName;

        public string Description => $"{_playerName} doubles";

        public Double(string playerName)
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

            // Player can't double if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            // Player can't double if they dont have a high enough balance
            if (player.Balance < player.CurrentBet)
                return false;

            // Player can't double if they have already doubled this round
            if (player.HasDoubled)
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
            player.Balance -= player.CurrentBet;

            player.CurrentBet *= 2;

            player.HasDoubled = true;

            var card = game.Shoe.DrawCard();
            player.Hand.Cards.Add(card);

            int handValue = HandHelper.CalculateHandValue(player.Hand);
            bool isBust = HandHelper.IsBust(player.Hand);

            if (isBust) {
                game.NextPlayer();
                return ActionResult.Successful($"{_playerName} doubled, drew {card} and busted with {handValue}");
            }

            game.NextPlayer();

            return ActionResult.Successful($"{_playerName} doubled, drew {card}, and hand value is now: {handValue}");
        }
    }
}
