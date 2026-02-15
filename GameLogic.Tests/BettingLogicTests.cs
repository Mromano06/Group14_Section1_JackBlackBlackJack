using GameLogic.Logic;
using GameLogic.Core;
using GameLogic.Models;
using SharedModels.Models;


namespace GameLogic.Tests;

[TestClass]
public class BettingLogicTests
{
    [TestMethod]
    public void PlaceBet_ThrowsException_BetLessThanGameMin()
    {
        Game game = new Game(10, new Shoe(2));
        Player player = new Player("Test Player", 100);
        
        Assert.Throws<ArgumentException>(() => BettingLogic.PlaceBet(player, game, 5));


    }

    ///TODO: Add more tests for betting logic
    /// Then continue on to Action Logic And then Core Game Logic
}
