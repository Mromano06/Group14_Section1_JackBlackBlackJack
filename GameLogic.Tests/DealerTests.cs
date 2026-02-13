using GameLogic.Models;

namespace GameLogic.Tests;

[TestClass]
public class DealerTests
{
    [TestMethod]
    public void DealerNameEmpty_ThrowsException()
    {
        // Arrange
        Dealer dealer = new Dealer();
        // Act & Assert
        Assert.Throws<ArgumentException>(() => dealer.Name = "");
    }

}
