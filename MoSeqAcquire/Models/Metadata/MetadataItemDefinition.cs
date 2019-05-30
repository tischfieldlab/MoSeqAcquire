using MoSeqAcquire.Models.Metadata.Rules;
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using MvvmValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MoSeqAcquire.Models.Metadata.DataTypes;

namespace MoSeqAcquire.Models.Metadata
{
    public enum ConstraintMode
    {
        None = 0,
        Choices,
        Range
    }
    public class MetadataItemDefinition : ValidatingBaseViewModel, IXmlSerializable
    {
        protected string name;
        protected BaseDataType valueType;
        protected object value;
        protected object defaultValue;
        protected string units;
        protected ConstraintMode constraint;
        protected BaseConstraint constraintImplementation;
        //protected ObservableCollection<BaseRule> validators;

        public MetadataItemDefinition(string Name, BaseDataType ValueType) : this()
        {
            this.name = Name;
            this.valueType = ValueType;
            this.defaultValue = this.valueType.CoerceValue(null);
            this.value = this.valueType.CoerceValue(null);
        }

        protected MetadataItemDefinition()
        {
            this.SetupValidationRules();
        }

        private void Validator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SetupValidationRules();
            this.Validator.ValidateAll();
            this.NotifyPropertyChanged(null);
        }
        private void Choice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Validator.ValidateAll();
        }

        protected void SetupValidationRules()
        {
            this.Validator.RemoveAllRules();
            //ensure property name is 
            this.Validator.AddRequiredRule(() => this.Name, "Name is required");
            //ensure value is of correct type
            this.Validator.AddRule(() => this.Value,
                                   () => RuleResult.Assert(this.ValueType.DataType.Equals(this.Value.GetType()), 
                                   "Value is not a valid " + this.ValueType.Name));

            if (this.Constraint != ConstraintMode.None)
            {
                this.Validator.AddRule(() => this.Value, () => this.ConstraintImplementation.Validate(this.value));
            }

            if (this.valueType != null)
            {
                foreach (var rule in this.valueType.Validators)
                {
                    if (rule.IsActive)
                    {
                        this.Validator.AddRule(() => this.Value, () => rule.IsActive ? rule.Validate(this) : RuleResult.Valid());
                    }
                }
            }
        }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        
        public BaseDataType ValueType
        {
            get => this.valueType;
            set
            {
                this.valueType?.Validators.ForEach(r => r.PropertyChanged -= Validator_PropertyChanged);
                value?.Validators.ForEach(r => r.PropertyChanged += Validator_PropertyChanged);
                this.SetField(ref this.valueType, value);
                this.CoerceAllValues();
            }
        }
        public object Value
        {
            get => this.value;
            set
            {
                this.SetField(ref this.value, this.valueType.CoerceValue(value));
                this.CoerceAllValues();
            }
        }
        public object DefaultValue
        {
            get => this.defaultValue;
            set
            {
                this.SetField(ref this.defaultValue, this.valueType.CoerceValue(value));
                this.CoerceAllValues();
            }
        }

        public string Units
        {
            get => this.units;
            set
            {
                this.SetField(ref this.units, value);
                this.NotifyPropertyChanged(nameof(this.HasUnits));
            }
        }

        public bool HasUnits => !string.IsNullOrWhiteSpace(this.Units);

        public List<ConstraintMode> ConstraintsAllowed => this.valueType.ValidTypeConstraints;
        
        public ConstraintMode Constraint
        {
            get => this.constraint;
            set
            {
                this.SetField(ref this.constraint, value);
                if (this.constraint == ConstraintMode.Choices)
                {
                    this.ConstraintImplementation = new ChoicesConstraint(this);
                }
                else if (this.constraint == ConstraintMode.Range)
                {
                    this.ConstraintImplementation = new RangeConstraint(this);
                }
                else
                {
                    this.ConstraintImplementation = null;
                }
                this.CoerceAllValues();
            }
        }

        public BaseConstraint ConstraintImplementation
        {
            get => this.constraintImplementation;
            protected set => this.SetField(ref this.constraintImplementation, value);
        }
        public List<BaseRule> Validators
        {
            get => this.valueType.Validators;
        }

        #region Value Coerceion
        public void ResetValue()
        {
            this.Value = this.DefaultValue;
        }
        protected void CoerceAllValues()
        {
            if (!this.ConstraintsAllowed.Contains(this.Constraint))
            {
                this.Constraint = ConstraintMode.None;
                return;
            }
            this.value = this.valueType.CoerceValue(this.value);
            this.defaultValue = this.valueType.CoerceValue(this.defaultValue);
            if (this.ConstraintImplementation is ChoicesConstraint)
            {
                foreach (var c in (this.constraintImplementation as ChoicesConstraint).Choices)
                {
                    c.Value = this.valueType.CoerceValue(c.Value);
                }
            }
            else if (this.ConstraintImplementation is RangeConstraint)
            {
                var rc = this.constraintImplementation as RangeConstraint;
                rc.MinValue = this.valueType.CoerceValue(rc.MinValue);
                rc.MaxValue = this.valueType.CoerceValue(rc.MaxValue);
            }
            this.SetupValidationRules();
            this.Validator.ValidateAll();
            this.NotifyPropertyChanged(null);
        }
#endregion

        

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Name = reader.GetAttribute("Name");

            

            reader.ReadToDescendant("DataType");
            var typeStr = reader.ReadElementContentAsString();
            this.ValueType = BaseDataType.FromType(System.Type.GetType(typeStr));
            if (this.ValueType == null)
            {
                throw new ArgumentException("Unable to find Type for " + typeStr);
            }

            //reader.ReadToFollowing("DefaultValue");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            else
            {
                this.DefaultValue = reader.ReadElementContentAs(this.ValueType.DataType, null);
            }

            //reader.ReadToFollowing("CurrentValue");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            else
            {
                this.Value = reader.ReadElementContentAs(this.ValueType.DataType, null);
            }

            //reader.ReadToFollowing("Units");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement(); 
            }
            else
            {
                this.Units = reader.ReadElementContentAsString();
            }

            //reader.ReadToFollowing("Constraint");
            this.Constraint = (ConstraintMode)Enum.Parse(typeof(ConstraintMode), reader.GetAttribute("Type"));
            if (this.constraint == ConstraintMode.None)
            {
                reader.ReadStartElement();
            }
            else
            { 
                this.constraintImplementation.ReadXml(reader);
            }
            reader.ReadEndElement();
            this.CoerceAllValues();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);

            writer.WriteElementString("DataType", this.ValueType.DataType.FullName);
            writer.WriteElementString("DefaultValue", this.DefaultValue != null ? this.defaultValue.ToString() : string.Empty);
            writer.WriteElementString("CurrentValue", this.Value != null ? this.value.ToString() : string.Empty);
            writer.WriteElementString("Units", this.units);

            writer.WriteStartElement("Constraint");
            writer.WriteAttributeString("Type", this.constraint.ToString());
            if (this.constraint != ConstraintMode.None)
            {
                this.constraintImplementation.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
        #endregion IXmlSerializable
        public override bool Equals(object obj)
        {

            if (!(obj is MetadataItemDefinition mdi))
                return false;

            if (!this.Name.Equals(mdi.Name))
                return false;
            if (!this.ValueType.Equals(mdi.ValueType))
                return false;
            if ((this.DefaultValue == null && mdi.DefaultValue != null) 
                || (this.DefaultValue != null && !this.DefaultValue.Equals(mdi.DefaultValue)))
                return false;
            if ((this.Value == null && mdi.Value != null)
                || (this.DefaultValue != null && !this.Value.Equals(mdi.Value)))
                return false;
            if ((this.Units == null && mdi.Units != null)
                || (this.Units != null && !this.Units.Equals(mdi.Units)))
                return false;
            if ((this.ConstraintImplementation == null && mdi.ConstraintImplementation != null)
                || (this.ConstraintImplementation != null && !this.ConstraintImplementation.Equals(mdi.ConstraintImplementation)))
                return false;

            return true;
        }

        

        public override int GetHashCode()
        {
            var hashCode = -1563108727;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ValueType.DataType);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(DefaultValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Units);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ConstraintMode>>.Default.GetHashCode(ConstraintsAllowed);
            hashCode = hashCode * -1521134295 + Constraint.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseConstraint>.Default.GetHashCode(ConstraintImplementation);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<BaseRule>>.Default.GetHashCode(Validators);
            return hashCode;
        }

        public ICommand AddChoice => new ActionCommand((p) =>
        {
            var constraint = (this.constraintImplementation as ChoicesConstraint);
            object val = null;
            if (this.ValueType.DataType == typeof(string))
            {
                val = "New Option "+constraint.Choices.Count;
            }
            else
            {
                val = Activator.CreateInstance(this.ValueType.DataType);
            }

            var choice = new ChoicesConstraintChoice(this) {Value = val};
            choice.PropertyChanged += Choice_PropertyChanged;
            constraint.Choices.Add(choice);
        });

        public ICommand RemoveChoice => new ActionCommand((p) =>
        {
            var choice = p as ChoicesConstraintChoice;
            choice.PropertyChanged -= Choice_PropertyChanged;
            (this.constraintImplementation as ChoicesConstraint).Choices.Remove(choice);
        });
    }
}
