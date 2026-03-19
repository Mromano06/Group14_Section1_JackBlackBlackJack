using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    public class GameUpdateDto
    {
        public double BetSize {  get; set; }
        public int CardCount { get; set; }
        public List<CardDto>? Cards { get; set; }
        public GameStateEnum GameState { get; set; }
        public int DealerCardCount {  get; set; }
        public List<CardDto>? DealerCards { get; set; }
        public int CurrentPlayerIndex { get; set; }
    }
}
