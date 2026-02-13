using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedModels.Tests;

[TestClass]
public class CardTests
{
    [TestMethod]
    public void CheckIfThrowsError_SetRank()
    {
        // Arrange
        Card card = new Card();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => card.Rank = '1');
    }

    [TestMethod]
    public void CheckSetThrowError_SetSuit()
    {
        Card card = new Card();

        Assert.Throws<ArgumentException>(() => card.Suit = 'X');
    }

    [TestMethod]
    public void CheckToString_ReturnsCorrectFormat()
    {
        Card card = new Card { Rank = 'A', Suit = 'S' };
        string result = card.ToString(card);
        Assert.AreEqual("AS", result);
    }
}
