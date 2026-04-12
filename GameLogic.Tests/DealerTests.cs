using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameLogic.Logic;
using GameLogic.Core;
using GameLogic.Models;
using SharedModels.Models;
using SharedModels.Core;
using System.Linq;

namespace GameLogic.Tests
{
    [TestClass]
    public class DealerTests
    {
        private Game _game;
        private Player _player1;
        private Player _player2;

        [TestInitialize]
        public void Setup()
        {
            _game = new Game(minBet: 5, new Shoe(6));

            _game.MaxPlayers = 2;

            _player1 = new Player("Alice", 1000);
            _player2 = new Player("Bob", 1000);
            
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
        }

        [TestMethod]
        public void DealInitialCards_EachPlayerGets2Cards()
        {
            // Act
            DealerLogic.DealInitialCards(_game);

            // Assert
            Assert.AreEqual(2, _player1.Hand.Cards.Count);
            Assert.AreEqual(2, _player2.Hand.Cards.Count);
        }

        [TestMethod]
        public void DealInitialCards_DealerGets2Cards()
        {
            // Act
            DealerLogic.DealInitialCards(_game);

            // Assert
            Assert.AreEqual(2, _game.Dealer.Hand.Cards.Count);
        }

        [TestMethod]
        public void DealCardToPlayer_AddsOneCard()
        {
            // Arrange
            int initialCount = _player1.Hand.Cards.Count;

            // Act
            DealerLogic.DealCardToPlayer(_game.Shoe, _player1);

            // Assert
            Assert.AreEqual(initialCount + 1, _player1.Hand.Cards.Count);
        }

        [TestMethod]
        public void DealCardToPlayer_MultipleCalls_AddsMultipleCards()
        {
            // Act
            DealerLogic.DealCardToPlayer(_game.Shoe, _player1);
            DealerLogic.DealCardToPlayer(_game.Shoe, _player1);
            DealerLogic.DealCardToPlayer(_game.Shoe, _player1);

            // Assert
            Assert.AreEqual(3, _player1.Hand.Cards.Count);
        }

        [TestMethod]
        public void DealCardToPlayer_OnlyAffectsTargetPlayer()
        {
            // Act
            DealerLogic.DealCardToPlayer(_game.Shoe, _player1);

            // Assert
            Assert.AreEqual(1, _player1.Hand.Cards.Count);
            Assert.AreEqual(0, _player2.Hand.Cards.Count);
        }


        [TestMethod]
        public void PlayTurn_DealerStandsOn17_OrHigher()
        {
            // Arrange - Give dealer cards totaling 17
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });  // 10
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '7', Suit = 'S' });  // 7

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.AreEqual(initialCount, _game.Dealer.Hand.Cards.Count);  // No cards drawn
        }

        [TestMethod]
        public void PlayTurn_DealerHitsBelow17()
        {
            // Arrange - Give dealer cards totaling less than 17
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '5', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '6', Suit = 'S' });
            // Total = 11

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.IsTrue(_game.Dealer.Hand.Cards.Count > initialCount);  // Dealer drew cards
        }

        [TestMethod]
        public void PlayTurn_DealerFinalValueIs17OrHigher()
        {
            // Arrange
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '5', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '6', Suit = 'S' });

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            int finalValue = HandHelper.CalculateHandValue(_game.Dealer.Hand);
            Assert.IsTrue(finalValue >= 17 || finalValue > 21);  // Either stands on 17+ or busts
        }

        [TestMethod]
        public void PlayTurn_DealerWith18_DoesNotDraw()
        {
            // Arrange
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '8', Suit = 'S' });
            // Total = 18

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.AreEqual(initialCount, _game.Dealer.Hand.Cards.Count);
        }

        [TestMethod]
        public void PlayTurn_DealerWith16_DrawsCards()
        {
            // Arrange
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '6', Suit = 'S' });
            // Total = 16

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.IsTrue(_game.Dealer.Hand.Cards.Count > initialCount);
        }

        [TestMethod]
        public void PlayTurn_DealerSoftAce_HandlesCorrectly()
        {
            // Arrange 
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '5', Suit = 'S' });
            // ^^^ Soft 16 (Ace + 5)

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.IsTrue(_game.Dealer.Hand.Cards.Count > initialCount);  // Dealer should hit on soft 16
            int finalValue = HandHelper.CalculateHandValue(_game.Dealer.Hand);
            Assert.IsTrue(finalValue >= 17 || finalValue > 21);
        }

        [TestMethod]
        public void PlayTurn_DealerCanBust()
        {
            // Arrange - Give dealer 16, likely to bust
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = '6', Suit = 'S' });

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            int finalValue = HandHelper.CalculateHandValue(_game.Dealer.Hand);
            // Dealer either reached 17-21 or busted (>21) just want to know if it went up
            Assert.IsTrue(finalValue >= 17);
        }

        [TestMethod]
        public void PlayTurn_Dealer21_DoesNotDraw()
        {
            // Arrange
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'S' });
            _game.Dealer.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'D' });
            // Total = 21

            int initialCount = _game.Dealer.Hand.Cards.Count;

            // Act
            DealerLogic.PlayTurn(_game);

            // Assert
            Assert.AreEqual(initialCount, _game.Dealer.Hand.Cards.Count);
        }
    }
}