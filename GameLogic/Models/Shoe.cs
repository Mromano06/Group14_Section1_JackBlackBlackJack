using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using GameLogic.Logic;

namespace GameLogic.Models 
{

    [Serializable]
    public class Shoe 
    {
        private List<Card> _cards;

        Shoe()
        {
            _cards = new List<Card>();
        }

        public Shoe(int numberOfDecks) : this()
        {
            if (numberOfDecks < 1) {
                throw new ArgumentException("Shoe must contain at least one deck of cards");
            }

            for (int i = 0; i < numberOfDecks; i++) {
                _cards.AddRange(CardHelper.CreateStandardDeck());
            }
        }

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
