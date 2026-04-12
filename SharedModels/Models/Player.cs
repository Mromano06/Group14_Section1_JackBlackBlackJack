using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    /// <remarks>
    /// A player has a name, a hand of cards, and gameplay-related properties
    /// like balance, current bet, and action states.
    /// </remarks>
    public class Player 
    {
        /// <summary>
        /// The player's name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The player's current hand of cards.
        /// </summary>
        private Hand _hand;

        /// <summary>
        /// The player's current bet amount.
        /// </summary>
        public double CurrentBet { get; set; }

        /// <summary>
        /// Indicates whether the player has chosen to double down.
        /// </summary>
        public bool HasDoubled { get; set; }

        /// <summary>
        /// Indicates whether the player has taken insurance.
        /// </summary>
        public bool HasInsured { get; set; }

        /// <summary>
        /// Tracks the number of actions taken by the player in the current round.
        /// </summary>
        public int ActionCount { get; set; }

        /// <summary>
        /// The player's available balance.
        /// </summary>
        private double _balance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class
        /// with default values.
        /// </summary>
        public Player()
        {
            _name = "Anonymous";
            _balance = 100;
            _hand = new Hand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class
        /// with a specified name and starting balance.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="startingBalance">The initial balance (default is 100.00).</param>
        public Player(string name, double startingBalance = 100.00)
        {
            Name = name;
            Balance = startingBalance;
            _hand = new Hand();
        }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the name is null, empty, or whitespace.
        public string Name
        {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Player name cannot be empty");
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's hand.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when attempting to assign a null hand.
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

        /// <summary>
        /// Gets or sets the player's balance.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when attempting to assign a negative balance.
        /// </exception>
        public double Balance
        {
            get => _balance;
            set {
                if (value < 0)
                    throw new ArgumentException("Balance cannot be negative");
                _balance = value;
            }
        }
    }
}
