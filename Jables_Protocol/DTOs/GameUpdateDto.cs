using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    public class GameUpdateDto
    {
        public PlayerDto Player { get; set; }
        public bool IsEndRound { get; set; }
        public GameStateEnum GameState { get; set; }
        public int DealerCardCount {  get; set; }
        public List<CardDto>? DealerCards { get; set; }
        public int CurrentPlayerIndex { get; set; }
    }
}
