using SharedModels.Models;

namespace GameLogic.Logic
{
    /// <summary>
    /// Provides static utility methods for evaluating and manipulating player hands.
    /// </summary>
    /// <remarks>
    /// Encapsulates blackjack hand logic including value calculation, bust detection,
    /// blackjack and soft hand detection, split eligibility, and hand manipulation utilities.
    /// </remarks>
    /// <author>Evan Travis</author>
    public static class HandHelper
    {
        /// <summary>
        /// Calculates the total value of the specified hand.
        /// </summary>
        /// <param name="hand">The hand to evaluate.</param>
        /// <returns>The integer value of the hand.</returns>
        /// <remarks>
        /// Aces are initially counted as 11 and reduced to 1 as needed
        /// to prevent the hand from exceeding 21.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the calculated hand value is negative.
        /// </exception>
        public static int CalculateHandValue(Hand hand)
        {
            int handValue = 0;
            int aceCount = 0;
            foreach (Card card in hand.Cards) {
                if (card.Rank == 'A') {
                    aceCount++;
                    handValue += 11;
                }
                else {
                    handValue += CardHelper.GetCardValue(card);
                }
            }
            if (handValue > 21 && aceCount > 0) {
                while (handValue > 21 && aceCount > 0) {
                    handValue -= 10;
                    aceCount--;
                }
            }
            if (handValue < 0) {
                throw new ArgumentException("Hand value cannot be negative");
            }
            return handValue;
        }

        // Flags
        /// <summary>
        /// Determines whether the specified hand has busted.
        /// </summary>
        /// <param name="hand">The hand to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the hand value exceeds 21; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBust(Hand hand)
        {
            return CalculateHandValue(hand) > 21;
        }

        /// <summary>
        /// Determines whether the specified hand is a blackjack.
        /// </summary>
        /// <param name="hand">The hand to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the hand contains exactly two cards totalling 21; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlackjack(Hand hand)
        {
            return hand.Cards.Count == 2 && CalculateHandValue(hand) == 21;
        }

        /// <summary>
        /// Determines whether the specified hand is a soft hand.
        /// </summary>
        /// <param name="hand">The hand to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the hand contains an Ace currently counted as 11
        /// and the total value does not exceed 21; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Since <see cref="CalculateHandValue"/> accounts for Aces being worth 1 or 11,
        /// a hand value of 21 or less with an Ace present indicates the Ace is counted as 11,
        /// making it a soft hand.
        /// </remarks>
        public static bool IsSoft(Hand hand)
        {
            bool hasAce = false;
            foreach (Card card in hand.Cards) {
                if (card.Rank == 'A') {
                    hasAce = true;
                }
            }
            // NOTE: CalculateHandValue accounts for aces being worth 1 or 11, so if the hand value is <= 21,
            // it means the ace is currently being counted as 11, making it a soft hand
            return hasAce && CalculateHandValue(hand) <= 21;
        }

        /// <summary>
        /// Determines whether the specified hand is eligible for a split.
        /// </summary>
        /// <param name="hand">The hand to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the hand contains exactly two cards of the same rank; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSplit(Hand hand)
        {
            return hand.Cards.Count == 2 && hand.Cards[0].Rank == hand.Cards[1].Rank;
        }

        // Utilities
        /// <summary>
        /// Clears all cards from the specified hand.
        /// </summary>
        /// <param name="hand">The hand to clear.</param>
        public static void ClearHand(Hand hand)
        {
            hand.Cards.Clear();
        }

        /// <summary>
        /// Returns the number of cards currently in the specified hand.
        /// </summary>
        /// <param name="hand">The hand to count.</param>
        /// <returns>The number of cards in the hand.</returns>
        public static int CardCount(Hand hand)
        {
            return hand.Cards.Count();
        }

        /// <summary>
        /// Adds a card to the specified hand.
        /// </summary>
        /// <param name="hand">The hand to add the card to.</param>
        /// <param name="card">The card to add.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="card"/> is null.
        /// </exception>
        public static void AddCardToHand(Hand hand, Card card)
        {
            if (card == null) {
                throw new ArgumentNullException(nameof(card), "Card cannot be null");
            }
            hand.Cards.Add(card);
        }
    }
}