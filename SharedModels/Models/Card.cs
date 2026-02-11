using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{
    
    [Serializable]
    public class Card 
    {
        private char _rank; // K, Q, J, T, 2-9, A
        private char _suit; // H, D, C, S

        public char Rank
        {
            get => _rank;
            set {
                if (!"A23456789TJQK".Contains(value))
                    throw new ArgumentException("Invalid rank");
                _rank = value;
            }
        }

        public char Suit
        {
            get => _suit;
            set
            {
                if (!"HDCS".Contains(value))
                    throw new ArgumentException("Invalid suit. Must be H, D, C, or S");
                _suit = value;
            }
        }
    }
}
