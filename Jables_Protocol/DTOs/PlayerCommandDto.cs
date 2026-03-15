using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.DTOs
{
    public class PlayerCommandDto
    {
        public PlayerAction Action { get; set; }
        public double BetAmount { get; set; }
    }
}
