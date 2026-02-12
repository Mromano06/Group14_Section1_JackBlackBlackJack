using GameLogic.Models;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Logic {
    public class DealerLogic 
    {


        public void DealInitialCards()
        {
            // Deal 2 cards to each player, then dealer
            for (int i = 0; i < 2; i++) {
                foreach (var player in Players) {
                    player.Hand.Cards.Add(Shoe.DrawCard());
                }
                Dealer.Hand.Cards.Add(Shoe.DrawCard());
            }
        }


    }
}
