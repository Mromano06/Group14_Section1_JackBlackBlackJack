using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BetPlacingViewModel : BaseModel
    {
        private readonly NetworkClient _client;

        public ICommand IncreaseBetCommand { get; }
        public ICommand DecreaseBetCommand { get; }
        public ICommand MaxBetCommand { get; }

        public ICommand ConfirmBetCommand { get; }

        public BetPlacingViewModel(NetworkClient client)
        {
            this._client = client;

            IncreaseBetCommand = new CommandRelay(IncBet);
            DecreaseBetCommand = new CommandRelay(DecBet);
            MaxBetCommand = new CommandRelay(MaxBet);
            ConfirmBetCommand = new CommandRelay(Confirm);
        }

        private void IncBet()
        {
        }

        private void DecBet()
        {

        }

        private void MaxBet()
        {

        }

        private void Confirm()
        {
        }
    }
}
