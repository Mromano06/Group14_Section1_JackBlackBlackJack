using Client.Commands;
using Client.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

// Matthew Romano - March 12th, 2026 - RulesgViewModel implementation
// Hnadles the logic of the rules view model

namespace Client.ViewModels
{
    public class RulesViewModel : BaseModel
    {
        private readonly Action _showMainMenu;

        public ICommand BackCommand { get; }

        public RulesViewModel(Action showMainMenu) {
            _showMainMenu = showMainMenu;
            BackCommand = new CommandRelay(Back);
        }

        void Back()
        {
            _showMainMenu?.Invoke();
        }
    }
}
