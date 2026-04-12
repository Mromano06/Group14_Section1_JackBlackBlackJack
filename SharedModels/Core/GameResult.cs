using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Core
{
    /// <summary>
    /// Represents the possible results of a full game.
    /// </summary>
    /// <remarks>
    /// This enumeration is used to indicate the result of a completed game. 
    /// </remarks>
    public enum GameResult
    {
        /// <summary>
        /// Default state when no result has been determined.
        /// </summary>
        DEFAULT_RESULT,
        /// <summary>
        /// Win state when the player reaches a balance of 1200+
        /// </summary>
        PLAYER_WIN,
        /// <summary>
        /// Loss state when the player reaches a balance 0
        /// </summary>
        PLAYER_LOSE
    }
}
