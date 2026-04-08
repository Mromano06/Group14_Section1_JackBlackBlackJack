using Client.Commands;
using Client.Networking;
using Jables_Protocol;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using SharedModels.Core;
using SharedModels.Models;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel for the victory screen displayed when the player wins the game.
    /// </summary>
    /// <remarks>
    /// Manages the victory screen, including display an image and
    /// automatic navigation back to the main menu after a short delay.
    /// </remarks>
    public class VictoryScreenViewModel: BaseModel
    {
        /// <summary>
        /// Action used to navigate back to the main menu.
        /// </summary>
        private readonly Action _showMainMenu;

        /// <summary>
        /// Shared network client used by the application.
        /// 
        /// Unused but could be nice for future features such as displaying stats.
        /// </summary>
        private readonly NetworkClient _client;

        /// <summary>
        /// Stores the image path displayed on the victory screen.
        /// </summary>
        private string _victoryImagePath;


        /// <summary>
        /// Initializes a new instance of the <see cref="VictoryScreenViewModel"/> class.
        /// </summary>
        /// <param name="client">Shared network client used by the application.</param>
        /// <param name="showMainMenu">Action used to navigate back to the main menu.</param>
        /// <remarks>
        /// Sets the default victory image and starts a timer that automatically
        /// returns the user to the main menu.
        /// </remarks>
        public VictoryScreenViewModel(NetworkClient client, Action showMainMenu)
        {
            _client = client;
            _showMainMenu = showMainMenu;
            _victoryImagePath = "pack://application:,,,/Loser.jpg"; // Default winner image path

            StartAutoReturnTimer(); // navigates back to the main menu
        }

        /// <summary>
        /// Gets or sets the image path displayed on the victory screen.
        /// </summary>
        /// <remarks>
        /// Updating this property notifies the UI so that the displayed image
        /// refreshes automatically through data binding.
        /// </remarks>
        public string VictoryImagePath
        {
            get => _victoryImagePath;
            set
            {
                _victoryImagePath = value;
                OnPropertyChanged(nameof(VictoryImagePath));
            }
        }

        /// <summary>
        /// Starts the timer that automatically returns the user to the main menu.
        /// </summary>
        /// <remarks>
        /// Waits for a fixed delay before navigating back to the main menu.
        /// </remarks>
        private async void StartAutoReturnTimer()
        {
            await Task.Delay(12000); // 12 seconds

            Application.Current.Dispatcher.Invoke(() =>
            {
                _showMainMenu?.Invoke();
            });
        }
    }

}
