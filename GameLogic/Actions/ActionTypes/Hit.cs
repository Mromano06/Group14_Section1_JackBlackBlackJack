using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    /// <summary>
    /// Represents a hit action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's hit, including bust checks, turn validation,
    /// and drawing an additional card.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Hit : IAction
    {
        /// <summary>
        /// The name of the player performing the hit.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// Gets a human-readable description of the hit action.
        /// </summary>
        public string Description => $"{_playerName} hits";

        /// <summary>
        /// Initializes a new instance of the <see cref="Hit"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player performing the hit.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="playerName"/> is null.
        /// </exception>
        public Hit(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerName = playerName;
        }

        /// <summary>
        /// Determines whether the hit action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the hit is valid and executable; otherwise, <c>false</c>.
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
            // Player can't hit if they've already busted
            if (HandHelper.IsBust(player.Hand))
                return false;
            return true;
        }

        /// <summary>
        /// Executes the hit action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the hit.
        /// </returns>
        /// <remarks>
        /// On success, draws one card and adds it to the player's hand.
        /// If the drawn card causes a bust, the player is moved on immediately.
        /// Otherwise, the player's action count is incremented.
        /// </remarks>
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