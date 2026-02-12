using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Models;
using SharedModels.Models;

namespace GameLogic.Core {
    public class Game 
    {
        // Game components
        public Dealer Dealer { get; set; }
        public List<Player> Players { get; set; }
        public Shoe Shoe { get; set; }
        public GameState GameState { get; set; }

        // Turn/Round tracking
        public int CurrentPlayerIndex { get; set; }
        public int RoundNumber { get; set; }

        // Game settings
        public double MinBet { get; set; }
        public int MaxPlayers { get; set; } = 5;
        public double PayoutRatio { get; set; } = 1.5;
        public double InsurancePayoutRatio { get; set; } = 2.0;

        public Game() : this(5, new Shoe(3)) { }

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

        public void AddPlayer(Player player) 
        {
            if(player == null) {
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

        public void RemovePlayer(Player player)
        {
            if (player == null) {
                throw new ArgumentNullException(nameof(player), "Player cannot be null");
            }

            Players.Remove(player);
        }

        public void StartGame() 
        {
            if (Players.Count == 0)
                throw new InvalidOperationException("Cannot start game without players.");

            GameState.TransitionTo(GameStateEnum.STARTINGGAME);

            foreach (Player player in Players) {
                player.Hand.Cards.Clear(); // TODO: This is a very large line so i might update the function later
            }
            Dealer.Hand.Cards.Clear();

            // TODO: Implement Shoe functionality
            // Shoe.Shuffle();

            CurrentPlayerIndex = 0;
            RoundNumber = 1;
            GameState.TransitionTo(GameStateEnum.PLAYING);
        }

        public void EndGame()
        {
            GameState.TransitionTo(GameStateEnum.ENDGAME);

            foreach (Player player in Players) {
                player.Hand.Cards.Clear();
            }
            Dealer.Hand.Cards.Clear();

            CurrentPlayerIndex = 0;
            GameState.TransitionTo(GameStateEnum.IDLE);
        }

        public void NextPlayer()
        {
            // Lets us use CurrentPlayerIndex to track both players and dealer (Player.Count + 1 is dealer))
            if (CurrentPlayerIndex > Players.Count) {
                CurrentPlayerIndex = 0;
            }

            CurrentPlayerIndex++;
        }

        public Player GetCurrentPlayer()
        {
            if (CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count)
                return Players[CurrentPlayerIndex];

            return null; // if we return null, it means it's (probably) dealer's turn
        }

        public void ResetGame()
        {
            GameState.TransitionTo(GameStateEnum.RESTARTING);

            foreach (Player player in Players) {
                player.Hand.Cards.Clear();
            }
            Dealer.Hand.Cards.Clear();

            // TODO: Implement Shoe functionality
            // if (Shoe.CardsRemanining < 52) {
            //    Shoe.ResetShoe(3);
            //    Shoe.Shuffle();
            // }

            CurrentPlayerIndex = 0;
            RoundNumber++;
            GameState.TransitionTo(GameStateEnum.PLAYING);
        }

        public bool IsPlayerTurn()
        {
            return CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Players.Count;
        }

        public bool IsDealerTurn()
        {
            return CurrentPlayerIndex >= Players.Count;
        }
    }
}
