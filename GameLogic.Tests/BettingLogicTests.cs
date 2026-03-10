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

    [TestMethod]
    public void PlaceBet_ThrowsException_PlayerBalanceTooLow()
    {
        Game game = new Game(10, new Shoe(2));
        Player player = new Player("Test Player", 5);
        
        Assert.Throws<InvalidOperationException>(() => BettingLogic.PlaceBet(player, game, 20));
    }

    [TestMethod]
    public void PlaceBet_SuccessfulBet()
    {
        Game game = new Game(10, new Shoe(2));
        Player player = new Player("Test Player", 100);
        
        double betAmount = BettingLogic.PlaceBet(player, game, 20);
        
        Assert.AreEqual(20, betAmount);
        Assert.AreEqual(80, player.Balance);
    }

}
