using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MainMenuModel : BaseModel
    {
        private readonly NetworkClient _client;
        private readonly Action _showGame;
        private readonly Action _showRules;

        public ICommand PlayCommand { get; }
        public ICommand RulesCommand { get; }
        public ICommand ExitCommand { get; }

        public MainMenuModel(NetworkClient client, Action ShowGame, Action ShowRules)
        {
            this._client = client;
            _showGame = ShowGame;
            _showRules = ShowRules;

            PlayCommand = new CommandRelay(Play);
            ExitCommand = new CommandRelay(Exit);
            RulesCommand = new CommandRelay(Rules);
        }

        private void Play()
        {

            // call start game
            /// TODO: create command objects so we can enqueue them here
            // var playCommand = new PlayCommand();
            //_client.EnqueueCommand(playCommand);

            // FOR TESTING!!!
            _showGame?.Invoke();
    
        }

        private void Rules()
        {
            _showRules();
        }

        // Exit's the application from the main meny
        private void Exit()
        {
            // clean exiting
            Application.Current.Shutdown();
        }
    }
}
