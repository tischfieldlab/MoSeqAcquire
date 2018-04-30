using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public enum ConstraintMode
    {
        None = 0,
        Choices,
        Range
    }
    public class MetadataItem : BaseViewModel
    {
        protected string name;
        protected Type valueType;
        protected object value;
        protected string units;
        protected ConstraintMode constraint;

        public MetadataItem(string Name, Type ValueType)
        {
            this.name = Name;
            this.valueType = ValueType;
            this.Choices = new ObservableCollection<ChoiceOption>();
        }

        public virtual TypeConverter Converter
        {
            get
            {
                return TypeDescriptor.GetConverter(this.ValueType);
            }
        }

        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public Type ValueType
        {
            get => this.valueType;
            set => this.SetField(ref this.valueType, value);
        }
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
        public object DefaultValue { get; }
        public string Units
        {
            get => this.units;
            set => this.SetField(ref this.units, value);
        }

       
        public bool ConstraintsAllowed
        {
            get
            {
                return true;
            }
        }
        public ConstraintMode Constraint
        {
            get => this.constraint;
            set => this.SetField(ref this.constraint, value);
        }
        public ObservableCollection<ChoiceOption> Choices { get; protected set; }

        public object MinValue { get; set; }
        public object MaxValue { get; set; }

        public ICommand AddChoice => new ActionCommand((p) =>
        {
            object val = null;
            if (this.ValueType == typeof(string))
            {
                val = "New Option";
            }
            else
            {
                val = Activator.CreateInstance(this.ValueType);
            }
            this.Choices.Add(new ChoiceOption() { Value = val });
        });
        public ICommand RemoveChoice => new ActionCommand((p) =>
        {
            this.Choices.Remove(p as ChoiceOption);
        });
    }
    public class ChoiceOption : BaseViewModel
    {
        protected object value;
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
    }
}
