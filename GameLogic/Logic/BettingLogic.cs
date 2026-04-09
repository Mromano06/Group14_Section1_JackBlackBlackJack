using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Core;

namespace GameLogic.Logic
{
    /// <summary>
    /// Provides static utility methods for handling player betting logic.
    /// </summary>
    /// <remarks>
    /// Encapsulates validation and execution of bet placement,
    /// ensuring bets meet minimum requirements and the player has sufficient balance.
    /// </remarks>
    /// <author>Evan Travis</author>
    public static class BettingLogic
    {
        /// <summary>
        /// Places a bet for the specified player in the current game.
        /// </summary>
        /// <param name="player">The player placing the bet.</param>
        /// <param name="game">The current game instance used for minimum bet validation.</param>
        /// <param name="amount">The amount to bet.</param>
        /// <returns>The confirmed bet amount.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="amount"/> is less than or equal to the game's minimum bet.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the player does not have sufficient balance to cover the bet.
        /// </exception>
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