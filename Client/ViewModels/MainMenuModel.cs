using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MainMenuModel
    {
        private readonly NetworkClient _client;

        public ICommand PlayCommand { get; }

        public MainMenuModel(NetworkClient client)
        {
            this._client = client;

            PlayCommand = new CommandRelay(Play);

        }

        private void Play()
        {
            // call start game
            /// TODO: create command objects so we can enqueue them here
            // var playCommand = new PlayCommand();
            //_client.EnqueueCommand(playCommand);
        
        }
    }
}
