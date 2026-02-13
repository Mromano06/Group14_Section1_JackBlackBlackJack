using GameLogic.Models;

namespace GameLogic.Tests;

[TestClass]
public class ShoeTests
{
    [TestMethod]
    public void CheckIfThrowsError_ShoeWithZeroDecks()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Shoe(0));
    }

    [TestMethod]
    public void CheckCardsRemaining_AfterInitialization()
    {
        // Arrange
        int numberOfDecks = 3;
        Shoe shoe = new Shoe(numberOfDecks);
        // Act
        int cardsRemaining = shoe.CardsRemaining();
        // Assert
        Assert.AreEqual(52 * numberOfDecks, cardsRemaining);
    }
}
