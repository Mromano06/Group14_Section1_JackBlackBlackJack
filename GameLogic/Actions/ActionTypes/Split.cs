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
    /// <summary>
    /// Represents a split action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's split, including balance checks, turn validation,
    /// and card matching verification.
    /// <para>
    /// Note: Full split functionality requiring multiple hands per player is scoped to V2.
    /// Current implementation is a partial placeholder.
    /// </para>
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Split : IAction
    {
        /// <summary>
        /// The name of the player performing the split.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// Gets a human-readable description of the split action.
        /// </summary>
        public string Description => $"{_playerName} splits";

        /// <summary>
        /// Initializes a new instance of the <see cref="Split"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player performing the split.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="playerName"/> is null.
        /// </exception>
        public Split(string playerName)
        {
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerName = playerName;
        }

        /// <summary>
        /// Determines whether the split action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the split is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Validates the following conditions:
        /// <list type="bullet">
        /// <item><description>The player exists in the game.</description></item>
        /// <item><description>It is currently the player's turn.</description></item>
        /// <item><description>The player has not already busted.</description></item>
        /// <item><description>The player has sufficient balance to match their current bet.</description></item>
        /// <item><description>The player's hand contains two cards of the same rank.</description></item>
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Executes the split action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the split.
        /// </returns>
        /// <remarks>
        /// On success, deducts the current bet from the player's balance, doubles their bet,
        /// draws one additional card, and advances to the next player.
        /// <para>
        /// Note: Full multi-hand tracking and per-hand messaging is pending V2 implementation.
        /// </para>
        /// </remarks>
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