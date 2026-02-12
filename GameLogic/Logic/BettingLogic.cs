using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Core;

namespace GameLogic.Logic {
    public static class BettingLogic 
    {
        public static double PlaceBet(Player player, Game game, double amount) 
        { 
            if (amount <= game.MinBet) {
                throw new ArgumentException("Bet amount must be greater than minimum bet");
            }

            if (player.Balance < amount) {
                throw new InvalidOperationException(player.Name + " does not have enough balance to place this bet");
            }

            player.Balance -= amount;

            return amount;
        }

    }
}
