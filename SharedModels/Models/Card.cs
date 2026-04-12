using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{
    /// <summary>
    /// Represents a standard playing card.
    /// </summary>
    /// <remarks>
    /// A card consists of a rank and a suit:
    /// - Rank: A, 2–9, T (10), J, Q, K
    /// - Suit: H (Hearts), D (Diamonds), C (Clubs), S (Spades)
    /// </remarks>
    public class Card 
    {
        /// <summary>
        /// The rank of the card (e.g., A, 2–9, T, J, Q, K).
        /// </summary>
        private char _rank;

        /// <summary>
        /// The suit of the card (H, D, C, S).
        /// </summary>
        private char _suit;

        /// <summary>
        /// Gets or sets the rank of the card.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid rank is assigned.
        /// </exception>
        public char Rank
        {
            get => _rank;
            set {
                if (!"A23456789TJQK".Contains(value))
                    throw new ArgumentException("Invalid rank");
                _rank = value;
            }
        }

        /// <summary>
        /// Gets or sets the suit of the card.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid suit is assigned.
        /// </exception>
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

        /// <summary>
        /// Returns a string representation of the card.
        /// </summary>
        /// <returns>
        /// A two-character string combining rank and suit.
        /// </returns>
        /// <remarks>
        /// This is primarily used for debugging/testing purposes.
        /// </remarks>        
        public override string ToString()
        {
            return $"{Rank}{Suit}";
        }
    }
}
