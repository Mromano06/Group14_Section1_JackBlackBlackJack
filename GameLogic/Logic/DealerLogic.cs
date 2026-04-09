using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Models;
using SharedModels.Models;
using GameLogic.Core;

namespace GameLogic.Logic
{
    /// <summary>
    /// Provides static utility methods for managing dealer actions during a game.
    /// </summary>
    /// <remarks>
    /// Encapsulates logic for dealing initial cards, drawing cards for players,
    /// and executing the dealer's turn according to standard blackjack rules.
    /// </remarks>
    /// <author>Evan Travis</author>
    public class DealerLogic
    {
        /// <summary>
        /// Deals the initial two cards to each player and the dealer at the start of a round.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <remarks>
        /// Cards are dealt in two passes, alternating between all players and the dealer,
        /// simulating standard blackjack dealing procedure.
        /// </remarks>
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

        /// <summary>
        /// Draws a single card from the shoe and adds it to the specified player's hand.
        /// </summary>
        /// <param name="shoe">The shoe to draw the card from.</param>
        /// <param name="player">The player to receive the card.</param>
        public static void DealCardToPlayer(Shoe shoe, Player player)
        {
            Card card = shoe.DrawCard();
            player.Hand.Cards.Add(card);
        }

        /// <summary>
        /// Executes the dealer's turn by drawing cards until the hand value reaches 17 or higher.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <remarks>
        /// Follows the standard blackjack rule where the dealer must stand on 17 or above.
        /// </remarks>
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