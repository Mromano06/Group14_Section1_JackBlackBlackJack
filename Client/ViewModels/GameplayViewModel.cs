using System;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class GameplayViewModel : BaseModel
    {
        private readonly int _wager;
        private readonly int _playerMoney;
        private readonly NetworkClient _client;

        public GameplayViewModel(NetworkClient client, int wager, int playerMoney) {
            _wager = wager;
            _client = client;
            _playerMoney = playerMoney;

        }

        // Readonly so no setters
        public int Wager
        {
            get => _wager;

        }

        public int PlayerMoney
        {
            get => _playerMoney;
        }
    }
}
