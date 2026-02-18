using GameLogic.Core;
using GameLogic.Models;
using SharedModels.Models;

namespace GameLogic.Tests;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void NewGameInvalidMinBet_ThrowsException()
    { 
        Assert.Throws<ArgumentException>(() => new Game(-0.01, new Shoe(2)));

    }

    [TestMethod]
    public void AddPlayer_TooManyPlayer_ThrowsException()
    {
        Game game = new Game(10, new Shoe(2));
        for (int i = 0; i < game.MaxPlayers; i++)
        {
            game.AddPlayer(new Player("Player " + i, 100));
        }
        Assert.Throws<InvalidOperationException>(() => game.AddPlayer(new Player("Extra Player", 100)));
    }

    [TestMethod]
    public void AddPlayer_DuplicateName_ThrowsException()
    {
        Game game = new Game(10, new Shoe(2));
        game.AddPlayer(new Player("Player 1", 100));
        Assert.Throws<InvalidOperationException>(() => game.AddPlayer(new Player("Player 1", 100)));
    }
     [TestMethod]

     public void StartGame_NoPlayers_ThrowsException()
        {
            Game game = new Game(10, new Shoe(2));
            Assert.Throws<InvalidOperationException>(() => game.StartGame());
    }

    [TestMethod]
    public void StartGame_Success()
    {
        Game game = new Game(10, new Shoe(2));
        Player player = new Player("Player 1", 100);
        game.AddPlayer(player);
        game.StartGame();
        Assert.AreEqual(GameStateEnum.PLAYING, game.GameState.State);
    }
}
