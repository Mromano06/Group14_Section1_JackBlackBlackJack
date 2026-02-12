using SharedModels.Models;

namespace GameLogic.Logic
{
    public static class HandHelper
    {
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
        public static bool IsBust(Hand hand)
        {
            return CalculateHandValue(hand) > 21;
        }

        public static bool IsBlackjack(Hand hand)
        {
            return hand.Cards.Count == 2 && CalculateHandValue(hand) == 21;
        }

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

        public static bool CanSplit(Hand hand)
        {
            return hand.Cards.Count == 2 && hand.Cards[0].Rank == hand.Cards[1].Rank;
        }

        // Utilities
        public static void ClearHand(Hand hand)
        {
            hand.Cards.Clear();
        }

        public static void AddCardToHand(Hand hand, Card card)
        {
            if (card == null) {
                throw new ArgumentNullException(nameof(card), "Card cannot be null");
            }

            hand.Cards.Add(card);
        }

    }
}
