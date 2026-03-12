using System;
using System.Collections.Generic;
using System.Text;

namespace Client.ViewModels
{
    public class RulesViewModel : BaseModel
    {
        private readonly Action _showMainMenu;
        public RulesViewModel(Action showMainMenu) {
            _showMainMenu = showMainMenu;
        }
    }
}
