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
public class StandTests
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
        Assert.Throws<ArgumentException>(() => new Stand(name));
    }

    [TestMethod]
    public void IsExecutable_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Stand stand = new Stand("NonExistentPlayer");

        // Act
        bool actual = stand.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_NotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        Stand stand = new Stand(_player.Name);

        Player player2 = new Player("notJohn", 100);
        _game.Players.Add(player2);

        _game.CurrentPlayerIndex = 1; // _player = 0, player2 = 1

        // Act
        bool actual = stand.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_StandAfterBusting_ReturnsFalse()
    {
        // Arrange
        Stand stand = new Stand(_player.Name);

        _player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'J', Suit = 'D' });

        // Act
        bool actual = stand.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultFailed()
    {
        // Arrange
        Stand stand = new Stand(_player.Name);

        // This hand busts, therefore, action cannot execute
        _player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'J', Suit = 'D' });

        // Act
        ActionResult actual = stand.Execute(_game);

        // Assert
        Assert.IsFalse(actual.Success);
    }

    [TestMethod]
    public void Execute_ActionIsExectutable_ReturnsActionResultSuccess()
    {
        // Arrange
        Stand stand = new Stand(_player.Name);

        // Act
        ActionResult actual = stand.Execute(_game);

        // Assert
        Assert.IsTrue(actual.Success);
    }

    [TestMethod]
    public void Execute_IndexIncreasedToNextPlayer()
    {
        // Arrange
        Stand doubleAction = new Stand(_player.Name);

        Player player2 = new Player("notJohn", 100);
        _game.Players.Add(player2);

        int expected = 1;

        // Act
        doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _game.CurrentPlayerIndex);
    }
}
