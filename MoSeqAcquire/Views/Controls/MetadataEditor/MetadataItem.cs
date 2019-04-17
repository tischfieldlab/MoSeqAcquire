using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using MvvmValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Serialization;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public enum ConstraintMode
    {
        None = 0,
        Choices,
        Range
    }
    public class MetadataItem : ValidatingBaseViewModel
    {
        protected string name;
        protected Type valueType;
        protected object value;
        protected string units;
        protected ConstraintMode constraint;
        protected BaseConstraint constraintImplementation;

        public MetadataItem(string Name, Type ValueType)
        {
            this.name = Name;
            this.valueType = ValueType;
            this.Validator.AddRule(nameof(this.Name), () => RuleResult.Assert(!string.IsNullOrEmpty(this.Name), "Name is required"));
            this.Validator.AddRule(nameof(this.Value), () => RuleResult.Assert(this.ValueType.IsAssignableFrom(this.Value.GetType()), "Value is not a valid " + this.ValueType.Name));
        }

        public virtual TypeConverter Converter
        {
            get => TypeDescriptor.GetConverter(this.ValueType);
        }

        [XmlAttribute]
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        [XmlAttribute]
        public Type ValueType
        {
            get => this.valueType;
            set
            {
                this.SetField(ref this.valueType, value);
                this.CoerceAllValues();
            }
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
            get => true;
        }
        public ConstraintMode Constraint
        {
            get => this.constraint;
            set
            {
                this.SetField(ref this.constraint, value);
                if (this.constraint == ConstraintMode.Choices)
                {
                    this.ConstraintImplementation = new ChoicesConstraint();
                }
                else if (this.constraint == ConstraintMode.Range)
                {
                    this.ConstraintImplementation = new RangeConstraint();
                }
                else
                {
                    this.ConstraintImplementation = null;
                }
            }
        }

        public BaseConstraint ConstraintImplementation
        {
            get => this.constraintImplementation;
            set => this.SetField(ref this.constraintImplementation, value);
        }





        protected void CoerceAllValues()
        {
            this.Value = this.CoerceValue(this.value, this.ValueType);
            if (this.ConstraintImplementation is ChoicesConstraint)
            {
                foreach (var c in (this.constraintImplementation as ChoicesConstraint).Choices)
                {
                    c.Value = this.CoerceValue(c.Value, this.ValueType);
                }
            }
            
        }
        protected object CoerceValue(object value, Type type)
        {
            try
            {
                return Convert.ChangeType(value, type);
            } 
            catch (FormatException e)
            {
                try
                {
                    return TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, type);
                }catch(NotSupportedException e2)
                {
                    return Activator.CreateInstance(type);
                }
            }
        }
        

       
        

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

            var constraint = (this.constraintImplementation as ChoicesConstraint);
            constraint.Choices.Add(new ChoicesConstraintChoice() { Value = val });
        });
        public ICommand RemoveChoice => new ActionCommand((p) =>
        {
            (this.constraintImplementation as ChoicesConstraint).Choices.Remove(p as ChoicesConstraintChoice);
        });
    }

    public abstract class BaseConstraint : BaseViewModel { }

    public class ChoicesConstraint : BaseConstraint
    {
        public ChoicesConstraint()
        {
            this.Choices = new ObservableCollection<ChoicesConstraintChoice>();
        }
        public ObservableCollection<ChoicesConstraintChoice> Choices { get; protected set; }
    }

    public class ChoicesConstraintChoice : BaseViewModel
    {
        protected object value;
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
    }

    public class RangeConstraint : BaseConstraint
    {
        protected object minValue;
        protected object maxValue;

        public object MinValue
        {
            get => this.minValue;
            set => this.SetField(ref this.minValue, value);
        }
        public object MaxValue
        {
            get => this.maxValue;
            set => this.SetField(ref this.maxValue, value);
        }
    }
}
