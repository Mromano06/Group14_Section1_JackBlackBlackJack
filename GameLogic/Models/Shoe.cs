using System;
using System.Collections.Generic;
using System.Text;
using static System.Random;
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

            Shuffle();
        }

        public List<Card> Cards
        {
            get => _cards;
            private set {
                if (value == null || value.Count < 52) {
                    throw new ArgumentException("Shoe must contain at least one deck of cards (52)");
                }

                _cards = value;
            }
        }

        public int CardsRemaining()
        {
            return _cards.Count; 
        }

        public void ResetShoe(int numberOfDecks)
        {
            if (numberOfDecks == 0) {
                throw new ArgumentException("Shoe must contain at least one deck of cards");
            }

            _cards.Clear();

            for (int i = 0; i < numberOfDecks; i++) {
                _cards.AddRange(CardHelper.CreateStandardDeck());
            }

            Shuffle();
        }

        // Fisher-Yates shuffle algorithm
        public void Shuffle()
        {
            Random random = new Random();
            int n = _cards.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                Card value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Cannot draw card, shoe is empty");

            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }
    }
}
