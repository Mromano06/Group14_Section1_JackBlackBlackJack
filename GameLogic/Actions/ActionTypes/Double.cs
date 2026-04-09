using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;
namespace GameLogic.Actions.ActionTypes
{
    /// <summary>
    /// Represents a double down action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's double down, including balance checks, turn validation,
    /// and drawing an additional card.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Double : IAction
    {
        /// <summary>
        /// The name of the player performing the double down.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// Gets a human-readable description of the double action.
        /// </summary>
        public string Description => $"{_playerName} doubles";

        /// <summary>
        /// Initializes a new instance of the <see cref="Double"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player performing the double down.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="playerName"/> is null.
        /// </exception>
        public Double(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerName = playerName;
        }

        /// <summary>
        /// Determines whether the double down action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the double down is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Validates the following conditions:
        /// <list type="bullet">
        /// <item><description>The player exists in the game.</description></item>
        /// <item><description>It is currently the player's turn.</description></item>
        /// <item><description>The player has not already performed an action this round.</description></item>
        /// <item><description>The player has sufficient balance to double their current bet.</description></item>
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
            // Player can't double if they have already completed an action
            if (player.ActionCount > 0)
                return false;
            // Player can't double if they dont have a high enough balance
            if (player.Balance < (player.CurrentBet * 2))
                return false;
            return true;
        }

        /// <summary>
        /// Executes the double down action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the double down.
        /// </returns>
        /// <remarks>
        /// On success, deducts the current bet from the player's balance, doubles their bet,
        /// draws one additional card, and advances to the next player.
        /// If the drawn card causes a bust, the player is moved on immediately.
        /// </remarks>
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
            Card card = game.Shoe.DrawCard();
            HandHelper.AddCardToHand(player.Hand, card);
            int handValue = HandHelper.CalculateHandValue(player.Hand);
            bool isBust = HandHelper.IsBust(player.Hand);
            if (isBust) {
                game.NextPlayer();
                return ActionResult.Successful($"{_playerName} doubled, drew {card} and busted with {handValue}");
            }
            player.ActionCount++;
            game.NextPlayer();
            return ActionResult.Successful($"{_playerName} doubled, drew {card}, and hand value is now: {handValue}");
        }
    }
}