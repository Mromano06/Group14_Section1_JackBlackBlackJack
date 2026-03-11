using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BaseModel
    {
        public object CurrentViewModel { get; set; }

        // Client shared across all screens
        private readonly NetworkClient _client;

        public MainMenuModel MainMenuVM { get; }
        //public GameViewModel GameVM { get; }

        public MainWindowViewModel()
        {
            // create the client
            _client = new NetworkClient();

            MainMenuVM = new MainMenuModel(_client);
            // GameVM = new GameViewModel(this);

            CurrentViewModel = MainMenuVM;
        }

        public void ShowGame()
        {
            //CurrentViewModel = GameVM;
        }

        public void ShowMenu()
        {
            CurrentViewModel = MainMenuVM;
        }


    }
}
