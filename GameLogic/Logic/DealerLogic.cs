using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Models;
using SharedModels.Models;
using GameLogic.Core;

namespace GameLogic.Logic {
    public class DealerLogic 
    {
        public static void DealInitialCards(Game game)
        {
            Shoe shoe = game.Shoe;
            Dealer dealer = game.Dealer;

            for (int i = 0; i < 2; i++) {
                foreach (Player player in game.Players) {
                    player.Hand.Cards.Add(shoe.DrawCard());
                }

                dealer.Hand.Cards.Add(shoe.DrawCard());
            }
        }

        public static void DealCardToPlayer(Shoe shoe, Player player)
        {
            Card card = shoe.DrawCard();
            player.Hand.Cards.Add(card);
        }

        public static void PlayTurn(Game game)
        {
            Dealer dealer = game.Dealer;
            Shoe shoe = game.Shoe;

            // Stands on 17
            while (HandHelper.CalculateHandValue(dealer.Hand) < 17) {
                Card card = shoe.DrawCard();
                dealer.Hand.Cards.Add(card);
            }
        }
    }
}
