using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameLogic.Logic;
using SharedModels.Models;

namespace GameLogic.Tests
{
    [TestClass]
    public class PlayerHelperTests
    {
        [TestMethod]
        public void PlayerRoundReset_ResetsAllRoundSpecificFields()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);
            player.CurrentBet = 100;
            player.HasDoubled = true;
            player.HasInsured = true;
            player.ActionCount = 5;

            // Act
            PlayerHelper.PlayerRoundReset(player);

            // Assert
            Assert.AreEqual(0, player.CurrentBet);
            Assert.IsFalse(player.HasDoubled);
            Assert.IsFalse(player.HasInsured);
            Assert.AreEqual(0, player.ActionCount);
        }

        [TestMethod]
        public void PlayerRoundReset_DoesNotModifyBalanceOrHand()
        {
            // Arrange
            var player = new Player("TestPlayer", 850);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.CurrentBet = 50;

            // Act
            PlayerHelper.PlayerRoundReset(player);

            // Assert
            Assert.AreEqual(850, player.Balance);
            Assert.AreEqual(1, player.Hand.Cards.Count);
        }
    }
}