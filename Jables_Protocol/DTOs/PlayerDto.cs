using GameLogic.Logic;
using Jables_Protocol.Serializers;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a player in the game.
    /// Contains player identity, hand data, betting state, and round-specific action flags
    /// for transmission between server and client.
    /// </summary>
    public class PlayerDto
    {
        /// <summary>
        /// Gets or sets the player's display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of cards currently in the player's hand.
        /// </summary>
        /// <remarks>
        /// This value should match the number of items in <see cref="Hand"/>.
        /// It is stored separately to support serialization and deserialization.
        /// </remarks>
        public int CardCount { get; set; }

        /// <summary>
        /// Gets or sets the collection of cards currently in the player's hand.
        /// </summary>
        public List<CardDto> Hand { get; set; } = new();

        /// <summary>
        /// Gets or sets the player's current wager for the round.
        /// </summary>
        public double CurrentBet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player has doubled down.
        /// </summary>
        public bool HasDoubled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player has purchased insurance.
        /// </summary>
        public bool HasInsured { get; set; }

        /// <summary>
        /// Gets or sets the number of actions the player has taken during the current round.
        /// </summary>
        public int ActionCount { get; set; }

        /// <summary>
        /// Gets or sets the player's current balance.
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDto"/> class.
        /// </summary>
        public PlayerDto() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDto"/> class
        /// from a domain <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">The domain player object to convert into a DTO.</param>
        /// <remarks>
        /// This constructor maps the player's hand, betting information, and round state
        /// into a transport-friendly format for network transmission.
        /// </remarks>
        public PlayerDto(Player player)
        {
            Name = player.Name;
            CardCount = HandHelper.CardCount(player.Hand);
            CurrentBet = player.CurrentBet;
            HasDoubled = player.HasDoubled;
            HasInsured = player.HasInsured;
            ActionCount = player.ActionCount;
            Balance = player.Balance;

            Hand = new List<CardDto>();

            if (player.Hand != null && HandHelper.CardCount(player.Hand) > 0) {
                foreach (Card card in player.Hand.Cards) {
                    CardDto cardDto = new CardDto() {
                        Rank = card.Rank,
                        Suit = card.Suit,
                    };

                    Hand.Add(cardDto);
                }
            }
        }
    }
}
