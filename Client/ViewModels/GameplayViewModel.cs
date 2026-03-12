using Client.Commands;
using Client.Networking;
using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

// Matthew Romano - March 12th, 2026 - GamplayViewModel Implementation
// The actual gameplay loop/aspects

// TODO: Figure out the actual gameplay logic and loop
// TODO: Add functionality to each ICommand sister Function
namespace Client.ViewModels
{
    public class GameplayViewModel : BaseModel
    {
        private readonly int _wager;
        private readonly int _playerMoney;
        private readonly NetworkClient _client;

        public ICommand HitCommand { get; }
        public ICommand StandCommand { get; }
        public ICommand DoubleDownCommand { get; }


        public GameplayViewModel(NetworkClient client, int wager, int playerMoney) {
            _wager = wager;
            _client = client;
            _playerMoney = playerMoney;
            HitCommand = new CommandRelay(Hit);
            StandCommand = new CommandRelay(Stand);
            DoubleDownCommand = new CommandRelay(DoubleDown);
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

        private void Hit()
        {

        }

        private void Stand()
        {

        }

        private void DoubleDown()
        {

        }
    }
}
