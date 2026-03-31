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
        public List<CardDto> Hand { get; set; } = new();
        public double CurrentBet { get; set; }
        public bool HasDoubled { get; set; }
        public bool HasInsured { get; set; }
        public int ActionCount { get; set; }
        public double Balance { get; set; }

        public PlayerDto() { }

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
