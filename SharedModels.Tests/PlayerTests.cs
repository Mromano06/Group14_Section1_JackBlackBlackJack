using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace SharedModels.Tests;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void CheckSetNameThrow_CantBeNull()
    {
        Player player = new Player();


        Assert.IsNull(player.Name);
    }

    [TestMethod]
    public void CheckSetNameThrow_CantBeEmpty()
    {
        Player player = new Player();
        Assert.Throws<ArgumentException>(() => player.Name = "");
    }

    [TestMethod]
    public void CheckThrow_NegativeBalance()
    {
        Player player = new Player();
        Assert.Throws<ArgumentException>(() => player.Balance = -0.01);
    }
}
