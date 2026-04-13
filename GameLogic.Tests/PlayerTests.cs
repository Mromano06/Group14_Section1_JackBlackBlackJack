using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedModels.Models;

namespace GameLogic.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="Player"/> class.
    /// </summary>
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void DefaultConstructor_SetsDefaultName()
        {
            // Arrange + Act
            var player = new Player();

            // Assert
            Assert.AreEqual("Anonymous", player.Name);
        }

        [TestMethod]
        public void DefaultConstructor_SetsDefaultBalance()
        {
            // Arrange + Act
            var player = new Player();

            // Assert
            Assert.AreEqual(100, player.Balance);
        }

        [TestMethod]
        public void DefaultConstructor_InitialisesEmptyHand()
        {
            // Arrange + Act
            var player = new Player();

            // Assert
            Assert.IsNotNull(player.Hand);
            Assert.AreEqual(0, player.Hand.Cards.Count);
        }

        [TestMethod]
        public void ParameterisedConstructor_SetsNameAndBalance()
        {
            // Arrange + Act
            var player = new Player("Brodie", 500);

            // Assert
            Assert.AreEqual("Brodie", player.Name);
            Assert.AreEqual(500, player.Balance);
        }

        [TestMethod]
        public void ParameterisedConstructor_DefaultBalanceIs100()
        {
            // Arrange + Act
            var player = new Player("Sam");

            // Assert
            Assert.AreEqual(100, player.Balance);
        }

        [TestMethod]
        public void Name_SetValidName_UpdatesName()
        {
            // Arrange
            var player = new Player();

            // Act
            player.Name = "Matt";

            // Assert
            Assert.AreEqual("Matt", player.Name);
        }

        [TestMethod]
        public void Name_SetNullName_ThrowsArgumentException()
        {
            // Arrange
            var player = new Player();

            // Act + Assert
            Assert.Throws<ArgumentException>(() => player.Name = null);
        }

        [TestMethod]
        public void Balance_SetPositiveValue_UpdatesBalance()
        {
            // Arrange
            var player = new Player("Brodie", 100);

            // Act
            player.Balance = 250;

            // Assert
            Assert.AreEqual(250, player.Balance);
        }

        [TestMethod]
        public void Balance_SetZero_IsAllowed()
        {
            // Arrange
            var player = new Player("Sam", 100);

            // Act
            player.Balance = 0;

            // Assert
            Assert.AreEqual(0, player.Balance);
        }

        [TestMethod]
        public void Balance_SetNegativeValue_ThrowsArgumentException()
        {
            // Arrange
            var player = new Player("Matt", 100);

            // Act + Assert
            Assert.Throws<ArgumentException>(() => player.Balance = -1);
        }

        [TestMethod]
        public void Hand_SetValidHand_UpdatesHand()
        {
            // Arrange
            var player = new Player("Evan", 100);
            var newHand = new Hand();
            newHand.Cards.Add(new Card { Rank = 'A', Suit = 'H' });

            // Act
            player.Hand = newHand;

            // Assert
            Assert.AreEqual(1, player.Hand.Cards.Count);
        }

        [TestMethod]
        public void Hand_SetNullHand_ThrowsArgumentNullException()
        {
            // Arrange
            var player = new Player("Brodie", 100);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => player.Hand = null);
        }

        [TestMethod]
        public void CurrentBet_DefaultsToZero()
        {
            // Arrange + Act
            var player = new Player("Sam", 100);

            // Assert
            Assert.AreEqual(0, player.CurrentBet);
        }

        [TestMethod]
        public void HasDoubled_DefaultsToFalse()
        {
            // Arrange + Act
            var player = new Player("Matt", 100);

            // Assert
            Assert.IsFalse(player.HasDoubled);
        }

        [TestMethod]
        public void HasInsured_DefaultsToFalse()
        {
            // Arrange + Act
            var player = new Player("Brodie", 100);

            // Assert
            Assert.IsFalse(player.HasInsured);
        }

        [TestMethod]
        public void ActionCount_DefaultsToZero()
        {
            // Arrange + Act
            var player = new Player("Sam", 100);

            // Assert
            Assert.AreEqual(0, player.ActionCount);
        }

        [TestMethod]
        public void RoundStateProperties_CanBeSetAndRead()
        {
            // Arrange
            var player = new Player("Matt", 100);

            // Act
            player.CurrentBet = 50;
            player.HasDoubled = true;
            player.HasInsured = true;
            player.ActionCount = 3;

            // Assert
            Assert.AreEqual(50, player.CurrentBet);
            Assert.IsTrue(player.HasDoubled);
            Assert.IsTrue(player.HasInsured);
            Assert.AreEqual(3, player.ActionCount);
        }
    }
}