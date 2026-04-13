using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace GameLogic.Models
{
    /// <summary>
    /// Represents the dealer in a blackjack game.
    /// </summary>
    /// <remarks>
    /// Encapsulates the dealer's name and hand, providing validation
    /// to ensure neither is null or invalid.
    /// </remarks>
    /// <author>Evan Travis</author>
    [Serializable]


    [ExcludeFromCodeCoverage]
    public class Dealer
    {
        /// <summary>
        /// Backing field for the dealer's name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Backing field for the dealer's hand.
        /// </summary>
        private Hand _hand;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dealer"/> class with the default name.
        /// </summary>
        /// <remarks>
        /// Sets the dealer's name to "Dealer" and initializes an empty hand.
        /// </remarks>
        public Dealer()
        {
            _name = "Dealer";
            _hand = new Hand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dealer"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name to assign to the dealer.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="name"/> is null, empty, or whitespace.
        /// </exception>
        public Dealer(string name)
        {
            Name = name;
            _hand = new Hand();
        }

        /// <summary>
        /// Gets or sets the dealer's name.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the value is null, empty, or whitespace.
        /// </exception>
        public string Name
        {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Dealer name cannot be empty");
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the dealer's current hand.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the value is null.
        /// </exception>
        public Hand Hand
        {
            get => _hand;
            set {
                if (value == null)
                    throw new ArgumentNullException("Hand cannot be null");
                _hand = value;
            }
        }
    }
}