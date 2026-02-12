using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Logic
{
    public static class CardHelper
    {

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

        public static List<Card> CreateStandardDeck()
        {
            List<Card> cards = new List<Card>();

            char[] suits = { 'H', 'D', 'C', 'S' };
            char[] ranks = { 'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K' };

            for (int i = 0; i < suits.Length; i++)
            {
                for (int j = 0; j < ranks.Length; j++)
                {
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
