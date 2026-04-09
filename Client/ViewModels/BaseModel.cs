using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// Matthew Romano - March 10th, 2026 - BaseModel implementation
// Base Model for all view models to inherit from, implements INotifyPropertyChanged for data binding

namespace Client.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="INotifyPropertyChanged"/> to support data binding between
    /// the Views and ViewModels in the MVVM architecture.
    /// 
    /// Any property in a derived ViewModel should call <see cref="OnPropertyChanged(string)"/>
    /// to notify the UI of updates.
    /// </remarks>
    public class BaseModel : INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property value/visual element changes.
    /// </summary>
    /// <remarks>
    /// This event is used by the UI to update bound elements when
    /// a property in the ViewModel changes.
    /// </remarks>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed.
        /// Automatically populated by the compiler if not explicitly stated.
        /// </param>
        /// <remarks>
        /// This method should be called in property setters to notify the UI of property changes.
        /// </remarks>
        protected void OnPropertyChanged([CallerMemberNameAttribute] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Actually updates the UI when a property changes
        }
    }
}
