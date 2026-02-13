using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace SharedModels.Tests;

[TestClass]
public class HandTests
{
    [TestMethod]
    public void CheckToString_ReturnCorrect()
    {
        Hand hand = new Hand();
        hand.Cards = new List<Card>()
        {
            new Card() { Rank = 'A', Suit = 'H' },
            new Card() { Rank = '2', Suit = 'C' }
        };

        String HandString = hand.ToString();

        Assert.AreEqual("AH, 2C", HandString);
    }
}
