using GameLogic.Core;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameLogic.Actions
{
    /// <summary>
    /// Defines the contract for all game actions that can be performed by a player.
    /// </summary>
    /// <remarks>
    /// All action types such as Hit, Stand, Bet, Double, Split, and Insure
    /// must implement this interface to ensure consistent validation and execution behaviour.
    /// </remarks>
    /// <author>Evan Travis</author>
    public interface IAction
    {
        /// <summary>
        /// Determines whether the action can be executed in the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// <c>true</c> if the action is valid and executable; otherwise, <c>false</c>.
        /// </returns>
        bool IsExecutable(Game game);

        /// <summary>
        /// Executes the action against the current game state.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating success or failure of the action.
        /// </returns>
        ActionResult Execute(Game game);

        /// <summary>
        /// Gets a human-readable description of the action.
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Represents the result of an executed game action.
    /// </summary>
    /// <remarks>
    /// Encapsulates whether the action succeeded or failed,
    /// along with a descriptive message for logging or display purposes.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class ActionResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the action succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a message describing the outcome of the action.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a failed <see cref="ActionResult"/> with the specified message.
        /// </summary>
        /// <param name="message">A message describing why the action failed.</param>
        /// <returns>An <see cref="ActionResult"/> with <see cref="Success"/> set to <c>false</c>.</returns>
        public static ActionResult Failed(string message)
        {
            return new ActionResult { Success = false, Message = message };
        }

        /// <summary>
        /// Creates a successful <see cref="ActionResult"/> with the specified message.
        /// </summary>
        /// <param name="message">A message describing the successful outcome.</param>
        /// <returns>An <see cref="ActionResult"/> with <see cref="Success"/> set to <c>true</c>.</returns>
        public static ActionResult Successful(string message)
        {
            return new ActionResult { Success = true, Message = message };
        }
    }
}