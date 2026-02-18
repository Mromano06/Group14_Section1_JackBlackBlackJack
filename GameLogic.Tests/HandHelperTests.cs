using GameLogic.Logic;
using SharedModels.Models;


namespace GameLogic.Tests;

[TestClass]
public class HandHelperTests
{

    [TestMethod]
    public void CalculateHandValue_ReturnsCorrectValue_AceAndKing()
    {
        // Arrange
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'A', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'D' });

        int handValue = HandHelper.CalculateHandValue(hand);

        Assert.AreEqual(21, handValue);
    }

    [TestMethod]
    public void CalculateHandValue_ReturnsCorrectValue_TwoAces()
    {
        // Arrange
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'A', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = 'A', Suit = 'D' });

        int handValue = HandHelper.CalculateHandValue(hand);

        Assert.AreEqual(12, handValue);

    }

    [TestMethod]
    public void CalculateHandValue_ReturnsCorrectValue_Bust()
    {
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'H' }); 
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'C'});
        hand.Cards.Add(new Card() { Rank = '4', Suit = 'H'});
        
        int handValue = HandHelper.CalculateHandValue(hand);

        Assert.AreEqual(24, handValue);

    }

    [TestMethod]
    public void IsBust_ReturnsTrue_HandValueOver21()
    {
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'C' });
        hand.Cards.Add(new Card() { Rank = 'Q', Suit = 'H' });

        Assert.IsTrue(HandHelper.IsBust(hand));
    }

    [TestMethod]
    public void IsBlackJack_ReturnsTrue_HandValueOf21()
    {
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'A', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'C' });

        Assert.IsTrue(HandHelper.IsBlackjack(hand));

    }

    [TestMethod]
    public void IsSoft_ReturnsTrue_HandContainsAceValuedAt11()
    {
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = 'A', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = '5', Suit = 'C' });
        Assert.IsTrue(HandHelper.IsSoft(hand));
    }

    [TestMethod]
    public void AddCardToHand_UpdatesHandValue()
    {
        Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = '5', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = '6', Suit = 'C' });
        int initialHandValue = HandHelper.CalculateHandValue(hand);
        Assert.AreEqual(11, initialHandValue);
        hand.Cards.Add(new Card() { Rank = 'K', Suit = 'D' });
        int updatedHandValue = HandHelper.CalculateHandValue(hand);
        Assert.AreEqual(21, updatedHandValue);
    }

    [TestMethod]
    public void CanSplit_ReturnsTrue_HandofSameRankCards()
    {
       Hand hand = new Hand();
        hand.Cards.Add(new Card() { Rank = '8', Suit = 'H' });
        hand.Cards.Add(new Card() { Rank = '8', Suit = 'C' });
        Assert.IsTrue(HandHelper.CanSplit(hand));
    }

}
