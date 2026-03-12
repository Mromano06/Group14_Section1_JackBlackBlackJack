using System;
using System.Collections.Generic;
using System.Text;
using Client.Networking;
using Client.ViewModels;
using Client;

namespace Client.ViewModels
{
    internal class GameplayViewModel : BaseModel
    {
        private readonly int _wager;
        private readonly NetworkClient _client;

        public GameplayViewModel(NetworkClient client, int wager) {
            _wager = wager;
            _client = client;
        }

        // Readonly so no setter
        public int Wager
        {
            get => _wager;
        }

    }
}
