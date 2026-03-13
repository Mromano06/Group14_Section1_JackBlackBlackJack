using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.DTOs
{
    internal class PlayerCommandDto
    {
        public PlayerAction Action { get; set; }
        public int BetAmount { get; set; }
    }
}
