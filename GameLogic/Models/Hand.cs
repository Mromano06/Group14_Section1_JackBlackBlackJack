using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Models 
{

    [Serializable]
    public class Hand 
    {
        private List<Card> _cards;
        private int _handValue;
        private bool _isBust;
        private bool _isBlackjack;
        private bool _isSoft;

        public List<Card> Cards
        {
            get => _cards;
            set {
                if (value == null || value.Count == 0)
                    throw new ArgumentException("Hand must contain at least one card");
                _cards = value;
                // CalculateHandValue();
            }
        }

        public int HandValue
        {
            get => _handValue;
            set {
                if (value < 0)
                    throw new ArgumentException("Hand value cannot be negative");
                _handValue = value;
            }
        }

        public bool IsBust
        {
            get => _isBust;
            set => _isBust = value;
        }

        public bool IsBlackjack
        {
            get => _isBlackjack;
            set => _isBlackjack = value;
        }

        public bool IsSoft
        {
            get => _isSoft;
            set => _isSoft = value;
        }

        // Hand value calculation logic can be implemented in a separate class :)
    }
}
