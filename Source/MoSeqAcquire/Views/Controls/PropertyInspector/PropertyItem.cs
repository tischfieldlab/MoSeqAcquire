using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace MoSeqAcquire.Views.Controls.PropertyInspector
{
    abstract class PropertyItem : BaseViewModel
    {
        protected string propertyName;
        protected object sourceObject;
        protected PropertyInfo propertyInfo;

        public PropertyItem(object SourceObject, string PropertyName)
        {
            this.propertyName = PropertyName;
            this.sourceObject = SourceObject;
            this.propertyInfo = this.sourceObject.GetType().GetProperty(this.PropertyName);
        }
        public string PropertyName { get => this.propertyName; }

        public virtual TypeConverter Converter
        {
            get
            {
                return TypeDescriptor.GetConverter(this.ValueType);
            }
        }

        public string DisplayName
        {
            get
            {
                DisplayNameAttribute attr = this.propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                if (attr != null)
                {
                    return attr.DisplayName;
                }
                return this.propertyName;
            }
        }
        public string Category
        {
            get
            {
                CategoryAttribute attr = this.propertyInfo.GetCustomAttribute<CategoryAttribute>();
                if (attr != null)
                {
                    return attr.Category;
                }
                return null;
            }
        }


        public abstract Type ValueType { get; }
        public abstract object Value { get; set; }
        public abstract object DefaultValue { get; }
        public abstract bool IsEnabled { get; }

      
        public abstract bool SupportsChoices { get; }
        public abstract IEnumerable<object> Choices { get; }
        public abstract string ChoicesDisplayPath { get; }
        public abstract string ChoicesValuePath { get; }


        public abstract bool SupportsRange { get; }
        public abstract object MinValue { get; }
        public abstract object MaxValue { get; }
        public abstract object StepValue { get; }
        

        public abstract bool SupportsAutomatic { get; }
        public abstract bool IsAutomatic { get; set; }

        public virtual ICommand ResetCommand => new ActionCommand((p) =>
        {
            this.Value = this.DefaultValue;
        });

    }
    
}
