using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ResultScreenViewModel : BaseModel
    {
        private readonly Action _showBetting;

        public ResultScreenViewModel(Action ShowBetting)
        {
            _showBetting = ShowBetting;
        }

    }
}
