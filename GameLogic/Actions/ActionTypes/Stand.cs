using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    /// <summary>
    /// Represents a stand action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's stand, ending their turn without drawing additional cards.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Stand : IAction
    {
        /// <summary>
        /// The name of the player performing the stand.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// Gets a human-readable description of the stand action.
        /// </summary>
        public string Description => $"{_playerName} stands";

        /// <summary>
        /// Initializes a new instance of the <see cref="Stand"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player performing the stand.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="playerName"/> is null.
        /// </exception>
        public Stand(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerName = playerName;
        }

        /// <summary>
        /// Determines whether the stand action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the stand is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Validates the following conditions:
        /// <list type="bullet">
        /// <item><description>The player exists in the game.</description></item>
        /// <item><description>It is currently the player's turn.</description></item>
        /// <item><description>The player has not already busted.</description></item>
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
            // Player can't stand if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;
            return true;
        }

        /// <summary>
        /// Executes the stand action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the stand.
        /// </returns>
        /// <remarks>
        /// On success, advances the turn to the next player without modifying
        /// the current player's hand or balance.
        /// </remarks>
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