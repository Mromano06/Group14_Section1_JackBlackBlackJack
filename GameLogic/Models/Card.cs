using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Models 
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

        public int GetValue()
        {
            return Rank switch 
            {
                'A' => 11,

                'K' or 'Q' or 'J' or 'T' => 10,

                _ => Rank - '0' // For numbers, removing the char '0' gets the int value
            };
        }

        // Overriding ToString to simplify the display of the cards (For testing)
        public override string ToString()
        {
            return $"{Rank}{Suit}";
        }
    }
}
