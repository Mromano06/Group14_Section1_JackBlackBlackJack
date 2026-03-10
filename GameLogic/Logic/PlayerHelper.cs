using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Logic
{
    public static class PlayerHelper
    {
        public static void PlayerRoundReset(Player player)
        {
            player.CurrentBet = 0;
            player.HasDoubled = false;
            player.HasInsured = false;
            player.ActionCount = 0;
        }
    }
}
