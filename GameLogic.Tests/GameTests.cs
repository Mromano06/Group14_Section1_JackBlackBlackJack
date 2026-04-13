using GameLogic.Core;
using GameLogic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedModels.Core;
using SharedModels.Models;

namespace GameLogic.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="Game"/> class.
    /// </summary>
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void Constructor_ValidArguments_InitialisesGame()
        {
            // Arrange + Act
            var game = new Game(5, new Shoe(2));

            // Assert
            Assert.IsNotNull(game.Dealer);
            Assert.IsNotNull(game.Players);
            Assert.IsNotNull(game.Shoe);
            Assert.IsNotNull(game.GameState);
        }

        [TestMethod]
        public void Constructor_InvalidMinBet_ThrowsArgumentException()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentException>(() => new Game(-1, new Shoe(2)));
        }

        [TestMethod]
        public void Constructor_ZeroMinBet_ThrowsArgumentException()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentException>(() => new Game(0, new Shoe(2)));
        }

        [TestMethod]
        public void Constructor_NullShoe_ThrowsArgumentNullException()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentNullException>(() => new Game(5, null));
        }

        [TestMethod]
        public void DefaultConstructor_SetsMinBetFiveAndThreeDecks()
        {
            // Arrange + Act
            var game = new Game();

            // Assert
            Assert.AreEqual(5, game.MinBet);
            Assert.AreEqual(156, game.Shoe.CardsRemaining()); // 3 decks = 156 cards
        }

        [TestMethod]
        public void AddPlayer_ValidPlayer_AddsToList()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);

            // Act
            game.AddPlayer(player);

            // Assert
            Assert.AreEqual(1, game.Players.Count);
        }

        [TestMethod]
        public void AddPlayer_NullPlayer_ThrowsArgumentNullException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => game.AddPlayer(null));
        }

        [TestMethod]
        public void AddPlayer_ExceedsMaxPlayers_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            for (int i = 0; i < game.MaxPlayers; i++) {
                game.AddPlayer(new Player("Matt number " + i, 100));
            }

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(new Player("OneMore", 100)));
        }

        [TestMethod]
        public void AddPlayer_DuplicateName_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Sam", 100));

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(new Player("Sam", 200)));
        }

        [TestMethod]
        public void RemovePlayer_ValidPlayer_RemovesFromList()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 100);
            game.AddPlayer(player);

            // Act
            game.RemovePlayer(player);

            // Assert
            Assert.AreEqual(0, game.Players.Count);
        }

        [TestMethod]
        public void RemovePlayer_NullPlayer_ThrowsArgumentNullException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => game.RemovePlayer(null));
        }

        [TestMethod]
        public void StartGame_NoPlayers_ThrowsInvalidOperationException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => game.StartGame());
        }

        [TestMethod]
        public void StartGame_WithPlayer_SetsStateToPlaying()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Brodie", 100));

            // Act
            game.StartGame();

            // Assert
            Assert.AreEqual(GameStateEnum.PLAYING, game.GameState.State);
        }

        [TestMethod]
        public void StartGame_ClearsPlayerHands()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Matt", 100);
            player.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'H' });
            game.AddPlayer(player);

            // Act
            game.StartGame();

            // Assert
            Assert.AreEqual(0, player.Hand.Cards.Count);
        }

        [TestMethod]
        public void StartGame_SetRoundNumberToOne()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Sam", 100));

            // Act
            game.StartGame();

            // Assert
            Assert.AreEqual(1, game.RoundNumber);
        }

        [TestMethod]
        public void GetPlayer_ExistingName_ReturnsPlayer()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 100);
            game.AddPlayer(player);

            // Act
            var result = game.GetPlayer("Evan");

            // Assert
            Assert.AreEqual(player, result);
        }

        [TestMethod]
        public void GetPlayer_NonExistentName_ThrowsArgumentException()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));

            // Act + Assert
            Assert.Throws<ArgumentException>(() => game.GetPlayer("Nobody"));
        }

        [TestMethod]
        public void GetCurrentPlayer_AtIndexZero_ReturnsFirstPlayer()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);
            game.AddPlayer(player);

            // Act
            var result = game.GetCurrentPlayer();

            // Assert
            Assert.AreEqual(player, result);
        }

        [TestMethod]
        public void GetCurrentPlayer_IndexOutOfRange_ReturnsNull()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Sam", 100));
            game.CurrentPlayerIndex = 5; // past the player list

            // Act
            var result = game.GetCurrentPlayer();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IsPlayerTurn_WhenIndexInRange_ReturnsTrue()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Matt", 100));
            game.CurrentPlayerIndex = 0;

            // Act + Assert
            Assert.IsTrue(game.IsPlayerTurn());
        }

        [TestMethod]
        public void IsDealerTurn_WhenIndexAtPlayerCount_ReturnsTrue()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Evan", 100));
            game.CurrentPlayerIndex = 1; // past the one player

            // Act + Assert
            Assert.IsTrue(game.IsDealerTurn());
        }

        [TestMethod]
        public void NextPlayer_IncrementsIndex()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Jesus", 100));
            int before = game.CurrentPlayerIndex;

            // Act
            game.NextPlayer();

            // Assert
            Assert.AreEqual(before + 1, game.CurrentPlayerIndex);
        }

        [TestMethod]
        public void Win_AddsCurrentBetTimesPayoutRatioToBalance()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);
            player.CurrentBet = 50;
            game.AddPlayer(player);

            // Act
            game.Win(player);

            // Assert — balance was 100, CurrentBet(50) * PayoutRatio(2.0) = +100
            Assert.AreEqual(200, player.Balance);
        }

        [TestMethod]
        public void Push_ReturnsCurrentBetToBalance()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 50);
            player.CurrentBet = 50;
            game.AddPlayer(player);

            // Act
            game.Push(player);

            // Assert
            Assert.AreEqual(100, player.Balance);
        }

        [TestMethod]
        public void Loss_NoInsurance_BalanceUnchanged()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);
            player.CurrentBet = 50;
            player.HasInsured = false;
            game.AddPlayer(player);

            // Act
            game.Loss(player, 21);

            // Assert
            Assert.AreEqual(100, player.Balance);
        }

        [TestMethod]
        public void Loss_WithInsuranceAndDealerBlackjack_PartialRefund()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Sam", 100);
            player.CurrentBet = 60; // e.g. original 40 + 20 insurance
            player.HasInsured = true;
            game.AddPlayer(player);

            // Act
            game.Loss(player, 21);

            // Assert — gets back CurrentBet / InsurancePayoutRatio(1.5) = 40
            Assert.AreEqual(140, player.Balance);
        }

        [TestMethod]
        public void RoundResult_PlayerBust_ReturnsLoss()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Matt", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'D' });
            player.Hand.Cards.Add(new Card { Rank = '5', Suit = 'S' }); // 25 — bust
            game.AddPlayer(player);

            // Act
            var result = game.RoundResult(player);

            // Assert
            Assert.AreEqual(ROUND_RESULT.LOSS, result);
        }

        [TestMethod]
        public void RoundResult_DealerBust_ReturnsWin()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.Hand.Cards.Add(new Card { Rank = '8', Suit = 'D' }); // 18
            game.AddPlayer(player);

            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'S' });
            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'C' });
            game.Dealer.Hand.Cards.Add(new Card { Rank = '5', Suit = 'H' }); // 25 = bust

            // Act
            var result = game.RoundResult(player);

            // Assert
            Assert.AreEqual(ROUND_RESULT.WIN, result);
        }

        [TestMethod]
        public void RoundResult_PlayerHigher_ReturnsWin()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.Hand.Cards.Add(new Card { Rank = '9', Suit = 'D' }); // 19
            game.AddPlayer(player);

            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'S' });
            game.Dealer.Hand.Cards.Add(new Card { Rank = '7', Suit = 'C' }); // 17

            // Act
            var result = game.RoundResult(player);

            // Assert
            Assert.AreEqual(ROUND_RESULT.WIN, result);
        }

        [TestMethod]
        public void RoundResult_DealerHigher_ReturnsLoss()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Sam", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.Hand.Cards.Add(new Card { Rank = '7', Suit = 'D' }); // 17
            game.AddPlayer(player);

            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'S' });
            game.Dealer.Hand.Cards.Add(new Card { Rank = '9', Suit = 'C' }); // 19

            // Act
            var result = game.RoundResult(player);

            // Assert
            Assert.AreEqual(ROUND_RESULT.LOSS, result);
        }

        [TestMethod]
        public void RoundResult_EqualValues_ReturnsPush()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Matt", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.Hand.Cards.Add(new Card { Rank = '8', Suit = 'D' }); // 18
            game.AddPlayer(player);

            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'S' });
            game.Dealer.Hand.Cards.Add(new Card { Rank = '8', Suit = 'C' }); // 18

            // Act
            var result = game.RoundResult(player);

            // Assert
            Assert.AreEqual(ROUND_RESULT.PUSH, result);
        }

        [TestMethod]
        public void EndRound_ClearsPlayerHandAndResetsBet()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 100);
            player.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });
            player.CurrentBet = 50;
            game.AddPlayer(player);

            // Act
            game.EndRound();

            // Assert
            Assert.AreEqual(0, player.Hand.Cards.Count);
            Assert.AreEqual(0, player.CurrentBet);
        }

        [TestMethod]
        public void EndRound_ClearsDealerHand()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Brodie", 100);
            game.AddPlayer(player);
            game.Dealer.Hand.Cards.Add(new Card { Rank = 'K', Suit = 'H' });

            // Act
            game.EndRound();

            // Assert
            Assert.AreEqual(0, game.Dealer.Hand.Cards.Count);
        }

        [TestMethod]
        public void EndRound_IncrementsRoundNumber()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Sam", 100));
            int roundBefore = game.RoundNumber;

            // Act
            game.EndRound();

            // Assert
            Assert.AreEqual(roundBefore + 1, game.RoundNumber);
        }

        [TestMethod]
        public void EndRound_TransitionsStateToIdle()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Matt", 100));

            // Act
            game.EndRound();

            // Assert
            Assert.AreEqual(GameStateEnum.IDLE, game.GameState.State);
        }

        [TestMethod]
        public void ResetGame_ClearsHandsAndIncrementsRound()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            var player = new Player("Evan", 100);
            player.Hand.Cards.Add(new Card { Rank = 'A', Suit = 'H' });
            game.AddPlayer(player);
            int roundBefore = game.RoundNumber;

            // Act
            game.ResetGame();

            // Assert
            Assert.AreEqual(0, player.Hand.Cards.Count);
            Assert.AreEqual(roundBefore + 1, game.RoundNumber);
        }

        [TestMethod]
        public void ResetGame_TransitionsStateToPlaying()
        {
            // Arrange
            var game = new Game(5, new Shoe(2));
            game.AddPlayer(new Player("Evan", 100));

            // Act
            game.ResetGame();

            // Assert
            Assert.AreEqual(GameStateEnum.PLAYING, game.GameState.State);
        }
    }
}