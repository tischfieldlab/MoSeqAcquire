using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MvvmValidation;

namespace MoSeqAcquire.ViewModels
{
    public class ValidatingBaseViewModel : BaseViewModel, INotifyDataErrorInfo, IDataErrorInfo
    {
        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }

        public ValidatingBaseViewModel() : base()
        {
            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(this.Validator);
            
        }
        public string this[string columnName]
        {
            get { return this.Validator.GetResult(columnName).ToString(); }
        }

        public string Error
        {
            get { return this.Validator.GetResult().ToString(); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                ValidationResult result = this.Validator.GetResult();
                if (result.IsValid)
                    return (IEnumerable)Enumerable.Empty<string>();
                return (IEnumerable)new string[1]
                {
                    result.ToString()
                };
            }
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return NotifyDataErrorInfoAdapter.HasErrors; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }

        protected bool SetField<T>(ref T field, T value, bool validate = true, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            if (validate)
            {
                Validator.Validate(propertyName);
            }
            return true;
        }
        protected bool SetField<T>(ref T field, T value, Action Task, bool validate = true, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            Task.Invoke();
            field = value;
            NotifyPropertyChanged(propertyName);
            if (validate)
            {
                Validator.Validate(propertyName);
            }
            return true;
        }
    }
}
