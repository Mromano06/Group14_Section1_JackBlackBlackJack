using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Models 
{

    [Serializable]
    public class Shoe 
    {
        private List<Card> _cards;

        public List<Card> Cards
        {
            get => _cards;
            set {
                if (value == null || value.Count < 52) {
                    throw new ArgumentException("Shoe must contain at least one deck of cards (52)");
                }
            }
        }

    }
}
