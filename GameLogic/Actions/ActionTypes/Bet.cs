using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;
using GameLogic.Core;

namespace GameLogic.Actions.ActionTypes
{
    /// <summary>
    /// Represents a bet action performed by a player during a game round.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IAction"/> to encapsulate the logic for validating
    /// and executing a player's bet, including balance checks and turn validation.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Bet : IAction
    {
        /// <summary>
        /// The name of the player placing the bet.
        /// </summary>
        private string _playerName;

        /// <summary>
        /// The amount being wagered.
        /// </summary>
        private double _amount;

        /// <summary>
        /// Gets a human-readable description of the bet action.
        /// </summary>
        public string Description => $"{_playerName} bets {_amount}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Bet"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player placing the bet.</param>
        /// <param name="amount">The amount to bet.</param>
        /// <param name="game">The current game instance used for validation.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="game"/> or <paramref name="playerName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="amount"/> is less than the game's minimum bet.
        /// </exception>
        public Bet(string playerName, double amount, Game game)
        {
            if (game == null) {
                throw new ArgumentNullException("Game is null");
            }
            if (playerName == null) {
                throw new ArgumentNullException(nameof(playerName));
            }
            if (amount < game.MinBet) {
                throw new ArgumentException($"Bet cannot be less that minimum bet (${game.MinBet})");
            }
            _playerName = playerName;
            _amount = amount;
        }

        /// <summary>
        /// Determines whether the bet action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the bet is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Validates the following conditions:
        /// <list type="bullet">
        /// <item><description>The player exists in the game.</description></item>
        /// <item><description>It is currently the player's turn.</description></item>
        /// <item><description>The player has not already placed a bet this round.</description></item>
        /// <item><description>The player has sufficient balance to cover the bet.</description></item>
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
            // Player can't bet if already have a bet this round
            if (player.CurrentBet > 0)
                return false;
            // Player must have enough balance
            if (_amount > player.Balance)
                return false;
            return true;
        }

        /// <summary>
        /// Executes the bet action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the bet.
        /// </returns>
        /// <remarks>
        /// On success, deducts the bet amount from the player's balance
        /// and sets their current bet for the round.
        /// </remarks>
        public ActionResult Execute(Game game)
        {
            if (!IsExecutable(game))
                return ActionResult.Failed("Invalid bet");
            var player = game.GetPlayer(_playerName);
            // Deduct from balance and set current bet
            player.Balance -= _amount;
            player.CurrentBet = _amount;
            return ActionResult.Successful($"{_playerName} bet {_amount}");
        }
    }
}
