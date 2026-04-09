using System;
using System.Collections.Generic;
using System.Text;
using static System.Random;
using SharedModels.Models;
using GameLogic.Logic;

namespace GameLogic.Models
{
    /// <summary>
    /// Represents a shoe containing one or more standard decks of cards.
    /// </summary>
    /// <remarks>
    /// Encapsulates card storage, shuffling, and drawing logic used during a blackjack game.
    /// Supports multi-deck shoes and uses the Fisher-Yates algorithm for shuffling.
    /// </remarks>
    /// <author>Evan Travis</author>
    [Serializable]
    public class Shoe
    {
        /// <summary>
        /// Backing field for the collection of cards in the shoe.
        /// </summary>
        private List<Card> _cards;

        /// <summary>
        /// Initializes a new empty instance of the <see cref="Shoe"/> class.
        /// </summary>
        Shoe()
        {
            _cards = new List<Card>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shoe"/> class with the specified number of decks.
        /// </summary>
        /// <param name="numberOfDecks">The number of standard decks to include in the shoe.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="numberOfDecks"/> is less than 1.
        /// </exception>
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

        /// <summary>
        /// Gets the current collection of cards remaining in the shoe.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the value is null or contains fewer than 52 cards.
        /// </exception>
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

        /// <summary>
        /// Returns the number of cards remaining in the shoe.
        /// </summary>
        /// <returns>The count of remaining cards.</returns>
        public int CardsRemaining()
        {
            return _cards.Count;
        }

        /// <summary>
        /// Resets the shoe with the specified number of decks and shuffles it.
        /// </summary>
        /// <param name="numberOfDecks">The number of standard decks to repopulate the shoe with.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="numberOfDecks"/> is zero.
        /// </exception>
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

        /// <summary>
        /// Shuffles the cards in the shoe using the Fisher-Yates algorithm.
        /// </summary>
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

        /// <summary>
        /// Draws the top card from the shoe and removes it.
        /// </summary>
        /// <returns>The drawn <see cref="Card"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the shoe is empty.
        /// </exception>
        public Card DrawCard()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Cannot draw card, shoe is empty");
            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        /// <summary>
        /// Determines whether the shoe has no remaining cards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the shoe is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }
    }
}