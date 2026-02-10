using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Models 
{

    [Serializable]
    public class Player 
    {
        private string _name;
        private Hand _hand;
        private double _currency;

        public string Name
        {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Player name cannot be empty");
                _name = value;
            }
        }

        public Hand Hand
        {
            get => _hand;
            set {
                if (value == null)
                    throw new ArgumentNullException("Hand cannot be null");
                _hand = value;
            }
        }

        public double Currency
        {
            get => _currency;
            set {
                if (value < 0)
                    throw new ArgumentException("Currency cannot be negative");
                _currency = value;
            }
        }
    }
}
