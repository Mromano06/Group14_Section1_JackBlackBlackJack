using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a full game update sent between server and client.
    /// Contains the current game state, player data, dealer information, and round results.
    /// </summary>
    public class GameUpdateDto
    {
        /// <summary>
        /// Gets or sets the current player's data.
        /// </summary>
        public PlayerDto Player { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current round has ended.
        /// </summary>
        public bool IsEndRound { get; set; }

        /// <summary>
        /// Gets or sets the current state of the game.
        /// </summary>
        public GameStateEnum GameState { get; set; }

        /// <summary>
        /// Gets or sets the number of dealer cards.
        /// Used during serialization/deserialization to determine how many cards to read.
        /// </summary>
        public int DealerCardCount { get; set; }

        /// <summary>
        /// Gets or sets the list of dealer cards.
        /// May be null or empty depending on the game state (e.g., before cards are revealed).
        /// </summary>
        public List<CardDto>? DealerCards { get; set; }

        /// <summary>
        /// Gets or sets the index of the current player whose turn it is.
        /// </summary>
        public int CurrentPlayerIndex { get; set; }

        /// <summary>
        /// Gets or sets the result of the most recent player action.
        /// Typically indicates success or failure of an action (e.g., hit, stand, bet).
        /// </summary>
        public bool ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the outcome of the round for the player.
        /// </summary>
        public ROUND_RESULT RoundWin { get; set; }

        /// <summary>
        /// Gets or sets the overall game result.
        /// This may include win/loss conditions or final game summary data.
        /// </summary>
        public GameResult gameResult { get; set; }
    }
}