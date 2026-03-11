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

        public ICommand PlayCommand { get; }
        public ICommand RulesCommand { get; }
        public ICommand ExitCommand { get; }

        public MainMenuModel(NetworkClient client)
        {
            this._client = client;

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

        
        }

        private void Rules()
        {
            // TODO: Implement rules screen
        }

        // Exit's the application from the main meny
        private void Exit()
        {
            // clean exiting
            Application.Current.Shutdown();
        }
    }
}
