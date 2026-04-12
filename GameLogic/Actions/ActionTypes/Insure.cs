using GameLogic.Core;
using GameLogic.Logic;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Actions.ActionTypes
{
    /// <summary>
    /// Represents an insurance action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's insurance bet, including balance checks, turn validation,
    /// and dealer upcard verification.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Insure : IAction
    {
        /// <summary>
        /// The name of the player performing the insurance action.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// Gets a human-readable description of the insure action.
        /// </summary>
        public string Description => $"{_playerName} insures";

        /// <summary>
        /// Initializes a new instance of the <see cref="Insure"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player performing the insurance action.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="playerName"/> is null.
        /// </exception>
        public Insure(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerName = playerName;
        }

        /// <summary>
        /// Determines whether the insurance action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the insurance action is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Validates the following conditions:
        /// <list type="bullet">
        /// <item><description>The player exists in the game.</description></item>
        /// <item><description>It is currently the player's turn.</description></item>
        /// <item><description>The dealer's upcard is an Ace.</description></item>
        /// <item><description>The player has not already performed an action this round.</description></item>
        /// <item><description>The player has sufficient balance to cover half their current bet.</description></item>
        /// </list>
        /// </remarks>
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
            if (game.Dealer.Hand.Cards.Count() > 0) {
                // Dealer must be showing an Ace
                if (game.Dealer.Hand.Cards[0].Rank != 'A')
                    return false;
            }
            // Player can't insure if they have already played an action
            if (player.ActionCount > 0)
                return false;
            // Player can't insure if they dont have a high enough balance
            if (player.Balance + player.CurrentBet < (player.CurrentBet * 1.5))
                return false;
            return true;
        }

        /// <summary>
        /// Executes the insurance action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the insurance action.
        /// </returns>
        /// <remarks>
        /// On success, deducts half the player's current bet from their balance
        /// and increases their total bet by 50%, marking them as insured.
        /// </remarks>
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