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
public class HitTests
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
        Assert.Throws<ArgumentException>(() => new Hit(name));
    }

    [TestMethod]
    public void IsExecutable_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Hit hit = new Hit("NonExistentPlayer");

        // Act
        bool actual = hit.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_NotPlayersTurn_ReturnsFalse()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        Player player2 = new Player("notJohn", 100);
        _game.Players.Add(player2);

        _game.CurrentPlayerIndex = 1; // _player = 0, player2 = 1

        // Act
        bool actual = hit.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void IsExecutable_HitAfterBusting_ReturnsFalse()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        _player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'J', Suit = 'D' });

        // Act
        bool actual = hit.IsExecutable(_game);

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Execute_ActionIsNotExectutable_ReturnsActionResultFailed()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        // This hand busts, therefore, action cannot execute
        _player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'J', Suit = 'D' });

        // Act
        ActionResult actual = hit.Execute(_game);

        // Assert
        Assert.IsFalse(actual.Success);
    }

    [TestMethod]
    public void Execute_ActionIsExectutable_ReturnsActionResultSuccess()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        // Act
        ActionResult actual = hit.Execute(_game);

        // Assert
        Assert.IsTrue(actual.Success);
    }

    [TestMethod]
    public void Execute_PlayerGainsOneCard()
    {
        // Arrange
        Hit doubleAction = new Hit(_player.Name);

        int expected = 1; // Player has 0 cards before hit

        // Act
       doubleAction.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.Hand.Cards.Count());
    }

    [TestMethod]
    public void Execute_IfBusted_NextPlayer()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        int expected = 1; // Current player == 0

        // This about to busts (21), therefore, action can execute but will bust
        _player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'Q', Suit = 'D' });
        _player.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'D' });

        // Act
        ActionResult actual = hit.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _game.CurrentPlayerIndex);
    }

    [TestMethod]
    public void Execute_PlayersActionCountIncreased()
    {
        // Arrange
        Hit hit = new Hit(_player.Name);

        int expected = 1; // Current action count == 0

        // Act
        hit.Execute(_game);

        // Assert
        Assert.AreEqual(expected, _player.ActionCount); // Since the player has no cards, they should just have 1 now
    }
}
