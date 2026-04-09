using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

// Matthew Romano & Brodie Arkell - March 12th, 2026 - MainWindowViewModel implementation
// Handles the logic of the main view model

namespace Client.ViewModels
{
    /// <summary>
    /// Main ViewModel responsible for managing application navigation and shared state.
    /// </summary>
    /// <remarks>
    /// This Class is the central controller for switching between different views in the app.
    /// Maintains a reference to the currently active ViewModel and a shared <see cref="NetworkClient"/>
    /// used across all screens.
    /// 
    /// Also stores global player data such as current money and latest bet.
    /// </remarks>
    public class MainWindowViewModel : BaseModel
    {
        /// <summary>
        /// The currently active ViewModel displayed in the main window.
        /// </summary>
        private BaseModel _currentViewModel;

        /// <summary>
        /// Network client object shared across all screens.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// Stores the player's current balance.
        /// </summary>
        private double _playerMoney;

        /// <summary>
        /// Stores the most recent bet placed by the player.
        /// </summary>
        private double _latestBet;

        /// <summary>
        /// Gets and sets for the currently active ViewModel.
        /// </summary>
        /// <remarks>
        /// When switching views, this property ensures that any previous ViewModel
        /// properly cleans up its resources to prevent duplicate event handling.
        /// </remarks>
        public BaseModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            { 
                // before changing screens check if was on gamescreen
                if (_currentViewModel is GameplayViewModel previousScreen)
                {
                    previousScreen.Cleanup(); // unsubscribes to events when switching screens
                }

                if (_currentViewModel is BetPlacingViewModel previousBetPlacing)
                {
                    previousBetPlacing.Cleanup(); 
                }

                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <remarks>
        /// Creates the shared <see cref="NetworkClient"/> and sets the initial screen
        /// to the main menu, passes in action items ShowBetting and ShowRules to 
        /// navigate from there.
        /// </remarks>
        public MainWindowViewModel()
        {
            _client = new NetworkClient();
            _currentViewModel = new MainMenuModel(_client, ShowBetting, ShowRules);
        }

        /// <summary>
        /// Gets or sets the player's current money.
        /// </summary>
        /// <remarks>
        /// Value must be non-negative.
        /// </remarks>
        public double PlayerMoney
        {
            get => _playerMoney;
            set
            {
                if (value >= 0)
                    _playerMoney = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's lastest bet.
        /// </summary>
        /// <remarks>
        /// Check the bet amount isn't greater than the player's current 
        /// balance in the set.
        /// </remarks>
        public double LatestBet
        {
            get => _latestBet;
            set
            {
                if (value <= this.PlayerMoney)
                    _latestBet = value;
            }
        }

        /// <summary>
        /// Navigates to the gameplay screen.
        ///<summary>
        public void ShowGame()
        {
            CurrentViewModel = new GameplayViewModel(_client, ShowResults, ShowMenu, ShowLoss, ShowVictory);
        }

        /// <summary>
        /// Navigates to the betting screen.
        ///<summary>
        public void ShowBetting()
        {
            CurrentViewModel = new BetPlacingViewModel(_client, ShowGame, ShowMenu);
        }

        /// <summary>
        /// Navigates to the main menu screen.
        ///<summary>
        public void ShowMenu()
        {
            CurrentViewModel = new MainMenuModel(_client, ShowBetting, ShowRules);
        }
        /// <summary>
        /// Navigates to the rules screen.
        ///<summary>
        public void ShowRules()
        {
            CurrentViewModel = new RulesViewModel(ShowMenu);
        }

        /// <summary>
        /// Navigates to the round result screen.
        ///<summary>
        public void ShowResults(String resultMessage)
        {
            CurrentViewModel = new ResultScreenViewModel(_client, ShowBetting, ShowMenu, resultMessage);
        }

        /// <summary>
        /// Navigates to the victory image screen.
        ///<summary>
        public void ShowVictory()
        {
            CurrentViewModel = new VictoryScreenViewModel(_client, ShowMenu);
        }

        /// <summary>
        /// Navigates to the loss image screen.
        ///<summary>
        public void ShowLoss()
        {
            CurrentViewModel = new LossScreenViewModel(_client, ShowMenu);
        }
    }
}
