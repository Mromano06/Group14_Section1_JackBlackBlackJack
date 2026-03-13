using GameLogic.Actions;
using GameLogic.Actions.ActionTypes;
using GameLogic.Core;
using GameLogic.Logic;
using GameLogic.Models;
using SharedModels.Models;
using System.Data;
using System.Numerics;
using System.Xml.Linq;

namespace GameLogic.Tests;

[TestClass]
public class InsureTests
{
    private Game _game;
    private Player _player;

    [TestInitialize]
    public void Setup()
    {
        _game = new Game(10, new Shoe(3));

        _player = new Player("John", 100);
        _game.Players.Add(_player);
    }

    [TestMethod]
    public void CheckIfThrowsError_NullPlayerNameReferenced()
    {
        // Arrange
        string name = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Insure(name));
    }

    [TestMethod]
    public void IsExecutable_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Insure insure = new Insure("NonExistentPlayer");

        // Act
        bool actual = insure.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_NotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        Player player2 = new Player("notJohn", 100);
        _game.AddPlayer(player2);

        _game.CurrentPlayerIndex = 1; // _player = 0, player2 = 1

        // Act
        bool actual = insure.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_DealerIsNotShowingAnAce_ReturnsFalse()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _game.Dealer.Hand.Cards.Add(new Card { Rank = '2', Suit = 'D' });
        _game.Dealer.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'D' }); // Unshown card is an Ace

        // Act
        bool actual = insure.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_ActionCountGreaterThan0_ReturnsFalse()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _player.ActionCount = 1;

        // Act
        bool actual = insure.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }


    [TestMethod]
    public void IsExecutable_NotEnoughBalance_ReturnsFalse()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _player.CurrentBet = 80; // _player.Balance == 100

        // Act
        bool actual = insure.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultFailed()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        // This bet * 1.5 is greater than players balance, therefore, action cannot execute
        _player.CurrentBet = 80;

        // Act
        ActionResult actual = insure.Execute(_game);

        // Assert
        Assert.IsFalse(actual.Success);
    }

    [TestMethod]
    public void Execute_ActionIsExectutable_ReturnsActionResultSuccess()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _player.CurrentBet = 50;

        // Act
        ActionResult actual = insure.Execute(_game);

        // Assert
        Assert.IsTrue(actual.Success);
    }

    [TestMethod]
    public void Execute_PlayersCurrentBetIncreasedByThreeOverTwo()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _player.CurrentBet = 50;

        double expected = _player.CurrentBet * 1.5;

        // Act
        insure.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.CurrentBet);
    }

    [TestMethod]
    public void Execute_PlayersBalanceDecreasedByHalfBet()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _game.Dealer.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'D' }); // Dealer showing an ace
        _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });

        _player.CurrentBet = 50;

        _player.Balance -= _player.CurrentBet; // bet action would typically decrease balance by initial bet

        double expected = _player.Balance - (_player.CurrentBet * 0.5);

        // Act
        insure.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.Balance);
    }

    [TestMethod]
    public void Execute_PlayersHasInsuredSetToTrue()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        _game.Dealer.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'D' }); // Dealer showing an ace
        _game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });

        _player.CurrentBet = 50;

        _player.Balance -= _player.CurrentBet; // bet action would typically decrease balance by initial bet

        bool expected = true;

        // Act
        insure.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.HasInsured);
    }

    [TestMethod]
    public void Execute_PlayersActionCountStaysTheSame()
    {
        // Arrange
        Insure insure = new Insure(_player.Name);

        int expected = 0; // Current action count == 0

        // Act
        insure.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.ActionCount); // Since the player has no cards, they should just have 1 now
    }
}
