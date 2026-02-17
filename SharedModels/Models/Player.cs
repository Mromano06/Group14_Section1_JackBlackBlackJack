using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{

    [Serializable]
    public class Player 
    {
        private string _name = "Anonymous";
        public Hand Hand { get; set; }
        public double CurrentBet { get; set; }
        public bool HasDoubled { get; set; }
        private double _balance;

        public Player()
        {
            Hand = new Hand();
        }
        public Player(string name, double startingBalance = 100.00)
        {
            Name = name;
            Balance = startingBalance;
            Hand = new Hand();
        }

        public string Name
        {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Player name cannot be empty");
                _name = value;
            }
        }

        public double Balance
        {
            get => _balance;
            set {
                if (value < 0)
                    throw new ArgumentException("Balance cannot be negative");
                _balance = value;
            }
        }
    }
}
