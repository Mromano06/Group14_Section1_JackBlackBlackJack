using GameLogic.Actions;
using GameLogic.Actions.ActionTypes;
using GameLogic.Core;
using GameLogic.Logic;
using GameLogic.Models;
using SharedModels.Models;
using System.Data;
using System.Numerics;
using System.Xml.Linq;

// Must be strictly defined as it can be "ambigous" with the primitive double 
using Double = GameLogic.Actions.ActionTypes.Double;

namespace GameLogic.Tests;



[TestClass]
public class DoubleTests
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
        Assert.Throws<ArgumentException>(() => new Double(name));
    }

    [TestMethod]
    public void IsExecutable_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Double doubleAction = new Double("NonExistentPlayer");

        // Act
        bool actual = doubleAction.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_NotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _game.MaxPlayers = 2;

        Player player2 = new Player("notJohn", 100);
        _game.AddPlayer(player2);

        _game.CurrentPlayerIndex = 1; // _player = 0, player2 = 1

        // Act
        bool actual = doubleAction.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_ActionCountGreaterThan0_ReturnsFalse()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

       _player.ActionCount = 1;

        // Act
        bool actual = doubleAction.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_NotEnoughBalance_ReturnsFalse()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 75; // _player.Balance == 100

        // Act
        bool actual = doubleAction.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultFailed()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 75; // _player.Balance == 100

        // Act
        ActionResult actual = doubleAction.Execute(_game);

        // Assert
        Assert.IsFalse(actual.Success);
    }

    [TestMethod]
    public void Execute_ActionIsExectutable_ReturnsActionResultSuccess()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        // Act
        ActionResult actual = doubleAction.Execute(_game);

        // Assert
        Assert.IsTrue(actual.Success);
    }

    [TestMethod]
    public void Execute_CurrentBetIsDoubled()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        double expectedBet = _player.CurrentBet * 2;

        // Act
       doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expectedBet, _player.CurrentBet);
    }

    [TestMethod]
    public void Execute_PlayerBalanceIsDecreasedByDoubleBet()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        double expectedVal = _player.Balance - (_player.CurrentBet * 2);

        _player.Balance -= _player.CurrentBet; // Deduct the value of placing the initial bet

        // Act
        doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expectedVal, _player.Balance);
    }

    [TestMethod]
    public void Execute_PlayerHasDoubled_ReturnsTrue()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        // Act
        ActionResult actual = doubleAction.Execute(_game);

        // Assert
        Assert.IsTrue(_player.HasDoubled);
    }

    [TestMethod]
    public void Execute_PlayerGainedANewCard_CardCountIs1()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        int expected = 1; // Current card count == 0

        // Act
        doubleAction.Execute(_game);
        
        // Assert
        Assert.AreEqual(expected, _player.Hand.Cards.Count()); // Since the player has no cards, they should just have 1 now
    }

    [TestMethod]
    public void Execute_PlayersActionCountIncreased()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        int expected = 1; // Current action count == 0

        // Act
        doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.ActionCount); // Since the player has no cards, they should just have 1 now
    }

    [TestMethod]
    public void Execute_GameWentToNextPlayer()
    {
        // Arrange
        Double doubleAction = new Double(_player.Name);

        _player.CurrentBet = 15; // _player.Balance == 100

        int expected = 1; // Current player == 0

        // Act
        doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _game.CurrentPlayerIndex); // Since the player has no cards, they should just have 1 now
    }
}
