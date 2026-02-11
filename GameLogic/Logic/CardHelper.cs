using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Logic
{
    public class CardHelper
    {
        public int GetValue(Card card)
        {
            return card.Rank switch {
                'A' => 11,

                'K' or 'Q' or 'J' or 'T' => 10,

                _ => card.Rank - '0' // For numbers, removing the char '0' gets the int value
            };
        }

        // Overriding ToString to simplify the display of the cards (For testing)
        public string ToString(Card card)
        {
            return $"{card.Rank}{card.Suit}";
        }
    }
}
