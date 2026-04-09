using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Logic
{
    /// <summary>
    /// Provides static utility methods for managing player state.
    /// </summary>
    /// <remarks>
    /// Encapsulates helper logic for resetting player data between rounds.
    /// </remarks>
    /// <author>Evan Travis</author>
    public static class PlayerHelper
    {
        /// <summary>
        /// Resets the specified player's round-specific state to its default values.
        /// </summary>
        /// <param name="player">The player to reset.</param>
        /// <remarks>
        /// Clears the following fields at the end of each round:
        /// <list type="bullet">
        /// <item><description>Current bet is set to zero.</description></item>
        /// <item><description>Double down status is cleared.</description></item>
        /// <item><description>Insurance status is cleared.</description></item>
        /// <item><description>Action count is reset to zero.</description></item>
        /// </list>
        /// </remarks>
        public static void PlayerRoundReset(Player player)
        {
            player.CurrentBet = 0;
            player.HasDoubled = false;
            player.HasInsured = false;
            player.ActionCount = 0;
        }
    }
}