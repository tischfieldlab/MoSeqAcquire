using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models
{
    /// <summary>
    /// This class works as an intermediary between UI logic and program logic.
    /// It inherits from the INotifyPropertyChanged interface because it raises
    /// this event whenever the UI is changed and a reflective change needs to 
    /// occur in the code base.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// This is the event that gets raised every time a property is changed in the 
        /// UI that needs to be changed in the codebase.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field of the property that is changed to reflect the changes made
        /// in the UI.
        /// </summary>
        /// <param name="field">Field associated with updated values.</param>
        /// <param name="value">Value that has been changed.</param>
        /// <param name="propertyName">Property name that these changes will take place under.</param>
        /// <returns>True if the change is valid, false if it is not.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the field of the property that is changed to reflect the changes made
        /// in the UI, but also invokes a passed in task if a change is valid.
        /// </summary>
        /// <param name="field">Field associated with updated values.</param>
        /// <param name="value">Value that has been changed.</param>
        /// <param name="Task">Task to be invoked once change is vaildated.</param>
        /// <param name="propertyName">Property name that these changes will take place under.</param>
        /// <returns>True if the change is valid, false if it is not.</returns>
        protected bool SetField<T>(ref T field, T value, Action Task, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            Task.Invoke();
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Event handler that invokes the property changed event and forces the event to be handled.
        /// </summary>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
