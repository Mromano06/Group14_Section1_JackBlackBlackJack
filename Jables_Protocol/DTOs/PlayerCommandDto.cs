using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Represents a player's command sent from the client to the server, including the action to perform and any associated bet amount.
    /// </summary>
    public class PlayerCommandDto
    {
        public PlayerAction Action { get; set; }
        public double BetAmount { get; set; }
    }
}
