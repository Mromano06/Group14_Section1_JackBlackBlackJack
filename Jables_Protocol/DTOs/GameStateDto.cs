using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a <see cref="GameStateEnum"/>.
    /// </summary>
    public class GameStateDto
    {
        public GameStateEnum GameState { get; set; }
    }
}
