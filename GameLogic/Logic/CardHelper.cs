using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Logic
{
    /// <summary>
    /// Provides static utility methods for card operations.
    /// </summary>
    /// <remarks>
    /// Encapsulates logic for retrieving card values and generating standard decks,
    /// following standard blackjack card valuation rules.
    /// </remarks>
    /// <author>Evan Travis</author>
    public static class CardHelper
    {
        /// <summary>
        /// Gets the blackjack value of the specified card.
        /// </summary>
        /// <param name="card">The card to evaluate.</param>
        /// <returns>
        /// The integer value of the card:
        /// <list type="bullet">
        /// <item><description>Ace returns 11.</description></item>
        /// <item><description>King, Queen, Jack, and Ten return 10.</description></item>
        /// <item><description>Numbered cards return their face value.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="card"/> is null.
        /// </exception>
        public static int GetCardValue(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            return card.Rank switch {
                'A' => 11,
                'K' or 'Q' or 'J' or 'T' => 10,
                _ => card.Rank - '0' // For numbers, removing the char '0' gets the int value
            };
        }

        /// <summary>
        /// Creates a standard 52-card deck containing all suits and ranks.
        /// </summary>
        /// <returns>
        /// A <see cref="List{Card}"/> containing 52 cards across four suits:
        /// Hearts, Diamonds, Clubs, and Spades, each with ranks Ace through King.
        /// </returns>
        public static List<Card> CreateStandardDeck()
        {
            List<Card> cards = new List<Card>();
            char[] suits = { 'H', 'D', 'C', 'S' };
            char[] ranks = { 'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K' };
            for (int i = 0; i < suits.Length; i++) {
                for (int j = 0; j < ranks.Length; j++) {
                    cards.Add(new Card {
                        Suit = suits[i],
                        Rank = ranks[j]
                    });
                }
            }
            return cards;
        }
    }
}