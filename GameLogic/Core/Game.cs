using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Models;
using SharedModels.Models;

namespace GameLogic.Core {
    public class Game 
    {
        private Dealer Dealer { get; set; }
        private List<Player> Players { get; set; }
        private Shoe Shoe { get; set; }
        private GameState State { get; set; }
        public double MinBet { get; set; }
        private int MaxPlayers = 5;
        private double PayoutRatio = 1.5;
        private double InsurancePayoutRatio = 2.0;

        public Game(double minBet, Shoe shoe) 
        {
            if (minBet <= 0) {
                throw new ArgumentException("Minimum bet must be greater than zero");
            }
            MinBet = minBet;
            Shoe = shoe ?? throw new ArgumentNullException(nameof(shoe));
            Dealer = new Dealer();
            Players = new List<Player>();
            // State = GameState.State;
        }
    }
}
