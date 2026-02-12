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
    }
}
