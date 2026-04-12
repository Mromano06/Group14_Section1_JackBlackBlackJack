using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{
    /// <summary>
    /// Represents a collection of cards held by a player or dealer.
    /// </summary>
    /// <remarks>
    /// A hand consists of a list of <see cref="Card"/> objects.
    /// This class is used to manage and display a player's/dealer's cards.
    /// </remarks>
    public class Hand 
    {
        /// <summary>
        /// Gets or sets the list of cards in the hand.
        /// </summary>
        public List<Card> Cards { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hand"/> class.
        /// </summary>
        /// <remarks>
        /// Creates an empty list of cards.
        /// </remarks>
        public Hand()
        {
            Cards = new List<Card>();
        }

        /// <summary>
        /// Returns a string representation of the hand.
        /// </summary>
        /// <returns>
        /// A comma-separated list of cards (e.g., "AH, 10D, KS").
        /// </returns>
        /// <remarks>
        /// This method is primarily used for debugging and testing purposes.
        /// </remarks>
        public override string ToString()
        {
            string result = "";

            foreach (Card card in Cards)
            {
                result += card.ToString();
                
                if (Cards.IndexOf(card) != (Cards.Count() - 1)) {
                    result += ", ";
                }
            }

            return result;
        }
    }
}
