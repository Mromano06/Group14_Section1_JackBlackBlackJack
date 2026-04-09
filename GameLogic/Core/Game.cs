using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using GameLogic.Logic;
using GameLogic.Models;
using SharedModels.Core;
using SharedModels.Models;

/// <summary>
/// Represents the possible outcomes of a round for a player.
/// </summary>
public enum ROUND_RESULT
{
    /// <summary>The player won the round.</summary>
    WIN,
    /// <summary>The player lost the round.</summary>
    LOSS,
    /// <summary>The round ended in a push (tie).</summary>
    PUSH,
    /// <summary>The round result could not be determined.</summary>
    DEFAULT
}

namespace GameLogic.Core
{
    /// <summary>
    /// Represents the core game instance managing state, players, and round logic.
    /// </summary>
    /// <remarks>
    /// Responsible for orchestrating all aspects of a blackjack game including:
    /// <list type="bullet">
    /// <item><description>Managing players and dealer.</description></item>
    /// <item><description>Tracking game and round state.</description></item>
    /// <item><description>Handling round results and player balance updates.</description></item>
    /// <item><description>Controlling turn progression.</description></item>
    /// </list>
    /// </remarks>
    /// <author>Evan Travis</author>
    public class Game
    {
        // Game components
        /// <summary>Gets or sets the dealer participating in the game.</summary>
        public Dealer Dealer { get; set; }

        /// <summary>Gets or sets the list of players participating in the game.</summary>
        public List<Player> Players { get; set; }

        /// <summary>Gets or sets the shoe used to draw cards.</summary>
        public Shoe Shoe { get; set; }

        /// <summary>Gets or sets the current state of the game.</summary>
        public GameState GameState { get; set; }

        // Turn/Round tracking
        /// <summary>Gets or sets the index of the current player taking their turn.</summary>
        public int CurrentPlayerIndex { get; set; }

        /// <summary>Gets or sets the current round number.</summary>
        public int RoundNumber { get; set; }

        // Game settings
        /// <summary>Gets or sets the minimum allowed bet for the game.</summary>
        public double MinBet { get; set; }

        /// <summary>Gets or sets the maximum number of players allowed in the game.</summary>
        public int MaxPlayers { get; set; } = 1;

        /// <summary>Gets or sets the payout ratio applied to winning bets.</summary>
        public double PayoutRatio { get; set; } = 2.0;

        /// <summary>Gets or sets the payout ratio applied to successful insurance bets.</summary>
        public double InsurancePayoutRatio { get; set; } = 1.5;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class with default settings.
        /// </summary>
        /// <remarks>
        /// Defaults to a minimum bet of 5 and a shoe containing 3 decks.
        /// </remarks>
        public Game() : this(5, new Shoe(3)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class with the specified settings.
        /// </summary>
        /// <param name="minBet">The minimum bet amount for the game.</param>
        /// <param name="shoe">The shoe to draw cards from.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="minBet"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="shoe"/> is null.
        /// </exception>
        public Game(double minBet, Shoe shoe)
        {
            if (minBet <= 0) {
                throw new ArgumentException("Minimum bet must be greater than zero");
            }
            MinBet = minBet;
            Shoe = shoe ?? throw new ArgumentNullException(nameof(shoe));
            Dealer = new Dealer();
            Players = new List<Player>();
            GameState = new GameState();

            CurrentPlayerIndex = 0;
            RoundNumber = 0;
        }

        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        /// <param name="player">The player to add.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="player"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the maximum number of players has been reached,
        /// or a player with the same name already exists.
        /// </exception>
        public void AddPlayer(Player player)
        {
            if (player == null) {
                throw new ArgumentNullException(nameof(player), "Player cannot be null");
            }

            if (Players.Count >= MaxPlayers) {
                throw new InvalidOperationException("Maximum number of players reached");
            }

            // I assume this is checked somewhere else before this function but i still will add it for safety
            if (Players.Exists(p => p.Name == player.Name)) {
                throw new InvalidOperationException("Player with the same name already exists");
            }

            Players.Add(player);
        }

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="player"/> is null.
        /// </exception>
        public void RemovePlayer(Player player)
        {
            if (player == null) {
                throw new ArgumentNullException(nameof(player), "Player cannot be null");
            }

            Players.Remove(player);
        }

        /// <summary>
        /// Starts a new game, initializing all players, the dealer, and the shoe.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no players have been added to the game.
        /// </exception>
        public void StartGame()
        {
            if (Players.Count == 0)
                throw new InvalidOperationException("Cannot start game without players.");

            GameState.TransitionTo(GameStateEnum.STARTINGGAME);

            foreach (Player player in Players) {
                player.Hand.Cards.Clear(); // TODO: This is a very large line so i might update the function later
            }
            Dealer.Hand.Cards.Clear();

            Shoe.Shuffle();

            CurrentPlayerIndex = 0;
            RoundNumber = 1;
            GameState.TransitionTo(GameStateEnum.PLAYING);
        }

        /// <summary>
        /// Processes a loss outcome for the specified player.
        /// </summary>
        /// <param name="player">The player who lost.</param>
        /// <param name="dealerValue">The dealer's final hand value.</param>
        /// <remarks>
        /// If the player had insured and the dealer hit 21, a partial balance
        /// recovery is applied based on the insurance payout ratio.
        /// </remarks>
        public void Loss(Player player, int dealerValue)
        {
            // Insurance detection
            if (player.HasInsured && dealerValue == 21) {
                player.Balance += player.CurrentBet / InsurancePayoutRatio; // Regain original bet - insurance
            }
        }

        /// <summary>
        /// Processes a win outcome for the specified player.
        /// </summary>
        /// <param name="player">The player who won.</param>
        /// <remarks>
        /// Adds the player's current bet multiplied by the payout ratio to their balance.
        /// </remarks>
        public void Win(Player player)
        {
            player.Balance += player.CurrentBet * PayoutRatio;
        }

        /// <summary>
        /// Processes a push outcome for the specified player.
        /// </summary>
        /// <param name="player">The player who pushed.</param>
        /// <remarks>
        /// Returns the player's current bet to their balance.
        /// </remarks>
        public void Push(Player player)
        {
            player.Balance += player.CurrentBet;
        }

        /// <summary>
        /// Determines the round result for the specified player.
        /// </summary>
        /// <param name="player">The player to evaluate.</param>
        /// <returns>
        /// A <see cref="ROUND_RESULT"/> indicating whether the player won, lost, pushed, or hit a default state.
        /// </returns>
        /// <remarks>
        /// Evaluates bust conditions for both the player and dealer before
        /// comparing hand values to determine the outcome.
        /// </remarks>
        public ROUND_RESULT RoundResult(Player player)
        {
            Hand dealerHand = Dealer.Hand;
            int dealerValue = HandHelper.CalculateHandValue(dealerHand);
            int playerValue = HandHelper.CalculateHandValue(player.Hand);

            // Player busted
            if (HandHelper.IsBust(player.Hand)) {
                return ROUND_RESULT.LOSS;
            }

            // Dealer busted
            else if (HandHelper.IsBust(dealerHand)) {
                return ROUND_RESULT.WIN;
            }

            // Neither busted
            else {
                if (playerValue < dealerValue) {
                    return ROUND_RESULT.LOSS;
                }

                else if (playerValue == dealerValue) {
                    return ROUND_RESULT.PUSH;
                }

                if (playerValue > dealerValue) {
                    return ROUND_RESULT.WIN;
                }
            }

            return ROUND_RESULT.DEFAULT;
        }

        /// <summary>
        /// Updates the specified player's balance based on their round result.
        /// </summary>
        /// <param name="player">The player to update.</param>
        /// <param name="roundResult">The outcome of the round for the player.</param>
        /// <param name="dealerValue">The dealer's final hand value.</param>
        public void RoundResultUpdatePlayer(Player player, ROUND_RESULT roundResult, int dealerValue)
        {
            switch (roundResult) {
                case ROUND_RESULT.LOSS:
                    Loss(player, dealerValue);
                    break;

                case ROUND_RESULT.WIN:
                    Win(player);
                    break;

                case ROUND_RESULT.PUSH:
                    Push(player);
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// Ends the current round, resolves all player outcomes, and resets the game state.
        /// </summary>
        /// <remarks>
        /// Calculates the dealer's hand value, resolves each player's result,
        /// clears all hands, resets player round state, and transitions to idle.
        /// </remarks>
        public void EndRound()
        {
            Hand dealerHand = Dealer.Hand;
            int dealerValue = HandHelper.CalculateHandValue(dealerHand);

            GameState.TransitionTo(GameStateEnum.ENDROUND);

            foreach (Player player in Players) {
                ROUND_RESULT roundResult = RoundResult(player);
                RoundResultUpdatePlayer(player, roundResult, dealerValue);
            }

            // Reset the round to a base state
            foreach (Player player in Players) {
                player.Hand.Cards.Clear();
                PlayerHelper.PlayerRoundReset(player);
            }
            Dealer.Hand.Cards.Clear();

            CurrentPlayerIndex = 0;
            RoundNumber++;
            GameState.TransitionTo(GameStateEnum.IDLE);
        }

        /// <summary>
        /// Advances the turn to the next player or dealer.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="CurrentPlayerIndex"/> to track both players and the dealer.
        /// When the index exceeds the player count, the dealer's turn is indicated.
        /// </remarks>
        public void NextPlayer()
        {
            // Lets us use CurrentPlayerIndex to track both players and dealer (Player.Count + 1 is dealer))
            if (CurrentPlayerIndex > Players.Count) {
                CurrentPlayerIndex = 0;
            }

            CurrentPlayerIndex++;
        }

        /// <summary>
        /// Retrieves a player from the game by their name.
        /// </summary>
        /// <param name="playerName">The unique name of the player to retrieve.</param>
        /// <returns>The <see cref="Player"/> with the specified name.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when no player with the given name exists in the game.
        /// </exception>
        // Since we're making everyone enter unique names we can just get a player by their name
        public Player GetPlayer(string playerName)
        {
            foreach (Player player in Players) {
                if (player.Name == playerName) return player;
            }

            throw new ArgumentException("Player cannot be found");
        }

        /// <summary>
        /// Gets the player whose turn it currently is.
        /// </summary>
        /// <returns>
        /// The current <see cref="Player"/>, or <c>null</c> if it is the dealer's turn.
        /// </returns>
        public Player GetCurrentPlayer()
        {
            if (CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count)
                return Players[CurrentPlayerIndex];

            return null; // if we return null, it means it's (probably) the dealer's turn
        }

        /// <summary>
        /// Resets the game to a fresh state for a new round.
        /// </summary>
        /// <remarks>
        /// Clears all player and dealer hands, reshuffles the shoe if needed,
        /// and transitions the game state back to playing.
        /// </remarks>
        public void ResetGame()
        {
            GameState.TransitionTo(GameStateEnum.RESTARTING);

            foreach (Player player in Players) {
                player.Hand.Cards.Clear();
            }
            Dealer.Hand.Cards.Clear();


            if (Shoe.CardsRemaining() < 52) {
                Shoe.ResetShoe(3);
                Shoe.Shuffle();
            }

            CurrentPlayerIndex = 0;
            RoundNumber++;
            GameState.TransitionTo(GameStateEnum.PLAYING);
        }

        /// <summary>
        /// Determines whether it is currently a player's turn.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current turn belongs to a player; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlayerTurn()
        {
            return CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count;
        }

        /// <summary>
        /// Determines whether it is currently the dealer's turn.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current turn belongs to the dealer; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDealerTurn()
        {
            return CurrentPlayerIndex >= Players.Count;
        }
    }
}