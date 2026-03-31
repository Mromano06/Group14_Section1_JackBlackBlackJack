using GameLogic.Logic;
using Jables_Protocol.Serializers;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.DTOs
{
    public class PlayerDto
    {
        public string Name { get; set; }
        public int CardCount { get; set; }
        public List<CardDto>? Hand { get; set; }    // Made it nullable
        public double CurrentBet { get; set; }
        public bool HasDoubled { get; set; }
        public bool HasInsured { get; set; }
        public int ActionCount { get; set; }
        public double Balance { get; set; }

        public PlayerDto() { }

        public PlayerDto(Player player)
        {
            List<CardDto> cards = new List<CardDto>();

            foreach (Card card in player.Hand.Cards) {
                CardDto cardDto = new CardDto() {
                    Rank = card.Rank,
                    Suit = card.Suit,
                };

                cards.Add(cardDto);
            }

            Name = player.Name;
            CardCount = HandHelper.CardCount(player.Hand);
            Hand = cards;
            CurrentBet = player.CurrentBet;
            HasDoubled = player.HasDoubled;
            HasInsured = player.HasInsured;
            ActionCount = player.ActionCount;
            Balance = player.Balance;
        }
    }
}
