using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// Matthew Romano - March 10th, 2026 - BaseModel implementation
// Base Model for all view models to inherit from, implements INotifyPropertyChanged for data binding

namespace Client.ViewModels
{
    public class BaseModel : INotifyPropertyChanged // Will allow databinding with the UI elements
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberNameAttribute] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Actually updates the UI when a property changes
        }
    }
}
