using GameLogic.Logic;
using SharedModels.Models;
using System.Net.Security;

namespace GameLogic.Tests;

[TestClass]
public class CardHelperTests
{
    [TestMethod]
    public void GetCardValue_ReturnsCorrectValue()
    {

        Card card = new Card() { Suit = 'H', Rank = 'A' };

        Assert.AreEqual(11, CardHelper.GetCardValue(card));
    }

    [TestMethod]
    public void GetCardValue_ReturnsCorrectValue_ForFaceCards()
    {
        Card card = new Card() { Suit = 'D', Rank = 'K' };
        Assert.AreEqual(10, CardHelper.GetCardValue(card));
    }

    [TestMethod]
    public void CreateStandardDeck_Returns52CorrectCards()
    {
        var deck = CardHelper.CreateStandardDeck();

        foreach (var card in deck)
        {
            StringAssert.Contains("HDSC", card.Suit.ToString());
            StringAssert.Contains("A23456789TJQK", card.Rank.ToString());

        }
    }

}
