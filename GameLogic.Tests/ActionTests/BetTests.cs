using GameLogic.Actions;
using GameLogic.Actions.ActionTypes;
using GameLogic.Core;
using GameLogic.Logic;
using GameLogic.Models;
using SharedModels.Models;
using System.Xml.Linq;

namespace GameLogic.Tests;



[TestClass]
public class BetTests
{
    private Game _game;
    private Player _player;

    [TestInitialize]
    public void Setup()
    {
        _game = new Game(10, new Shoe(3));

        _player = new Player("John", 100);
        _game.AddPlayer(_player);
    }

    [TestMethod]
    public void CheckIfThrowsError_NullPlayerNameReferenced()
    {
        // Arrange
        double betVal = 20;
        string name = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bet(name, betVal, _game));
    }

    [TestMethod]
    public void CheckIfThrowsError_NullGameReferenced()
    {
        // Arrange
        double betVal = 20;
        Game nullGame = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bet(_player.Name, betVal, nullGame));
    }


    [TestMethod]
    public void CheckIfThrowsError_BetLessThatMinAllowed()
    {
        // Arrange
        double betVal = 5;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bet(_player.Name, betVal, _game));
    }


    [TestMethod]
    public void IsExecutable_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Bet bet = new Bet("NonExistentPlayer", 20, _game);
        bool expected = false;

        // Act
        bool actual = bet.IsExecutable(_game);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsExecutable_NotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        double betVal = 20;
        Bet bet = new Bet(_player.Name, betVal, _game);

        Player player2 = new Player("notJohn", 100);
        _game.AddPlayer(player2);

        bool expected = false;

        _game.CurrentPlayerIndex = 1; // _player = 0, player2 = 1

        // Act
        bool actual = bet.IsExecutable(_game);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsExecutable_PlayerDoesNotHaveEnoughBalance_ReturnsFalse()
    {
        // Arrange
        double betVal = 1000;
        Bet bet = new Bet(_player.Name, betVal, _game);

        bool expected = false;

        // Act
        bool actual = bet.IsExecutable(_game);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsExecutable_PlayersBetEqualsBalance_ReturnsTrue()
    {
        // Arrange
        double betVal = 100;
        Bet bet = new Bet(_player.Name, betVal, _game);
        bool expected = true;

        // Act
        bool actual = bet.IsExecutable(_game);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void IsExecutable_PlayersBetEqualsMinBet_ReturnsTrue()
    {
        // Arrange
        double initialBalance = _player.Balance;
        double betVal = _game.MinBet;
        Bet bet = new Bet(_player.Name, betVal, _game);
        bool expected = true;

        // Act
        bool actual = bet.IsExecutable(_game);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Execute_ValidBet_DeductsCorrectAmount()
    {
        // Arrange
        double initialBalance = _player.Balance;
        double betVal = 25;
        Bet bet = new Bet(_player.Name, betVal, _game);

        double expected = initialBalance - betVal;

        // Act
        bet.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.Balance);
        Assert.AreEqual(betVal, _player.CurrentBet);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultFailed()
    {
        // Arrange
        double initialBalance = _player.Balance;
        int betVal = 1000;
        Bet bet = new Bet(_player.Name, betVal, _game);

        bool expected = false;

        // Act
        ActionResult actual = bet.Execute(_game);

        // Assert
        Assert.AreEqual(expected, actual.Success);
        Assert.AreEqual(initialBalance, _player.Balance);
        Assert.AreEqual(0, _player.CurrentBet);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultSuccess()
    {
        // Arrange
        int betVal = 10;
        Bet bet = new Bet(_player.Name, betVal, _game);

        bool expected = true;
        double expectedVal = _player.Balance - betVal;

        // Act
        ActionResult actual = bet.Execute(_game);

        // Assert
        Assert.AreEqual(expected, actual.Success);
        Assert.AreEqual(expectedVal, _player.Balance);
        Assert.AreEqual(betVal, _player.CurrentBet);
    }

    [TestMethod]
    public void Execute_MultiplePlayersExist_OnlyTargetPlayerBets()
    {
        // Arrange
        Player player2 = new Player("notJohn", 100);
        _game.AddPlayer(player2);

        double betVal = 75;
        double initialBalanceJohn = _player.Balance;
        double initialBalanceNotJohn = player2.Balance;

        Bet bet = new Bet(_player.Name, betVal, _game);

        double expectedJohn = _player.Balance - betVal;
        double expectednotJohn = player2.Balance;

        // Act
        ActionResult result = bet.Execute(_game);

        // Assert
        Assert.AreEqual(expectedJohn, _player.Balance);
        Assert.AreEqual(expectednotJohn, player2.Balance);
        
        Assert.AreEqual(betVal, _player.CurrentBet);
        Assert.AreEqual(0, player2.CurrentBet);
    }
}
