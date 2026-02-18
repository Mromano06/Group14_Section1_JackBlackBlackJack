using GameLogic.Core;
using GameLogic.Logic;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;


// TODO: If we want split, a players hand has to be a List<Hand> hands so we can track multiple hands
//       Also, we will have to track doubling by hand since each hand in a split could double (or split again)
//       Therefore.... scoped to V2

namespace GameLogic.Actions.ActionTypes
{
    public class Split : IAction
    {
        private string _playerName;

        public string Description => $"{_playerName} splits";

        public Split(string playerName)
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

            // Player can't split if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;

            // Player can't split if they dont have a high enough balance
            if (player.Balance < player.CurrentBet)
                return false;

            // Player cant split if their cards arent the same (We will need a List<Hand> hands to hold multiple hands)
            if (HandHelper.CanSplit(player.Hand))
                return false;

            // TODO: Track IsExecutable by each hand

            return true;
        }

        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game)) {
                return ActionResult.Failed("Cannot split at this time");
            }

            Player player = game.GetPlayer(_playerName);

            // Deduct from balance and set current bet
            player.Balance -= player.CurrentBet;

            player.CurrentBet *= 2;

            // TODO: would require sending cards to seperate hands
            var card = game.Shoe.DrawCard();
            player.Hand.Cards.Add(card);

            // TODO: Hand value would be by hand again
            int handValue = HandHelper.CalculateHandValue(player.Hand);
            bool isBust = HandHelper.IsBust(player.Hand);

            game.NextPlayer();

            // TODO: Message would have to mention multiple hands
            return ActionResult.Successful($"{_playerName} split, drew {card}, and hand value is now: {handValue}");
        }
    }
}
