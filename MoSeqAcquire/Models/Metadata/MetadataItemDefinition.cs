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
        protected Type valueType;
        protected object value;
        protected object defaultValue;
        protected string units;
        protected ConstraintMode constraint;
        protected BaseConstraint constraintImplementation;
        protected ObservableCollection<BaseRule> validators;

        protected readonly Dictionary<Type, List<ConstraintMode>> validTypeConstraints = new Dictionary<Type, List<ConstraintMode>>()
        {
            { typeof(bool), new List<ConstraintMode>(){ConstraintMode.None } },
            { typeof(string), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices } },
            { typeof(int), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range } },
            { typeof(double), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range } }
        };

        public MetadataItemDefinition(string Name, Type ValueType) : this()
        {
            this.name = Name;
            this.valueType = ValueType;
            this.defaultValue = this.CoerceValue(null, ValueType);
            this.value = this.CoerceValue(null, ValueType);
        }

        protected MetadataItemDefinition()
        {
            this.validators = new ObservableCollection<BaseRule>()
            {
                new RequiredRule()
            };
            this.validators.ForEach(v => v.PropertyChanged += Validator_PropertyChanged);
            this.SetupValidationRules();
        }

        private void Validator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SetupValidationRules();
            this.Validator.ValidateAll();
            this.NotifyPropertyChanged(null);
        }

        protected void SetupValidationRules()
        {
            this.Validator.RemoveAllRules();
            //ensure property name is 
            this.Validator.AddRequiredRule(() => this.Name, "Name is required");
            //ensure value is of correct type
            this.Validator.AddRule(() => this.Value,
                                   () => RuleResult.Assert(this.ValueType.IsAssignableFrom(this.Value.GetType()), 
                                   "Value is not a valid " + this.ValueType.Name));

            if (this.Constraint != ConstraintMode.None)
            {
                switch (this.constraint)
                {
                    case ConstraintMode.Choices:
                        
                        this.Validator.AddRule(() => this.Value,
                                               () =>
                                               {
                                                   var ccon = this.ConstraintImplementation as ChoicesConstraint;
                                                   return RuleResult.Assert(ccon.Choices.Any(c => c.Value.Equals(this.value)), "Value must be one of available choices");
                                               });
                        break;

                    case ConstraintMode.Range:
                        
                        this.Validator.AddRule(() => this.Value,
                                               () =>
                                               {
                                                   var rcon = this.ConstraintImplementation as RangeConstraint;
                                                   return RuleResult.Assert(
                                                       (this.Value as IComparable).CompareTo(
                                                           (rcon.MinValue as IComparable)) >= 0
                                                       && (this.Value as IComparable).CompareTo(
                                                           (rcon.MaxValue as IComparable)) <= 0,
                                                       "Value must be within range");
                                               });
                        break;
                }
            }

            foreach(var rule in this.validators)
            {
                if (rule.IsActive)
                {
                    this.Validator.AddRule(() => this.Value, () => rule.Validate(this));
                }
            }
        }

        public virtual TypeConverter Converter
        {
            get => TypeDescriptor.GetConverter(this.ValueType);
        }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        
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
            set
            {
                this.SetField(ref this.value, value);
                this.CoerceAllValues();
            }
        }
        public object DefaultValue
        {
            get => this.defaultValue;
            set
            {
                this.SetField(ref this.defaultValue, value);
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

        public bool HasUnits
        {
            get => !string.IsNullOrWhiteSpace(this.Units);
        }
        public List<ConstraintMode> ConstraintsAllowed
        {
            get => this.validTypeConstraints[this.ValueType];
        }
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
            set => this.SetField(ref this.constraintImplementation, value);
        }
        public ObservableCollection<BaseRule> Validators
        {
            get => this.validators;
        }

        #region Value Coerceion
        protected void CoerceAllValues()
        {
            if (!this.ConstraintsAllowed.Contains(this.Constraint))
            {
                this.Constraint = ConstraintMode.None;
                return;
            }
            this.value = this.CoerceValue(this.value, this.ValueType);
            this.defaultValue = this.CoerceValue(this.defaultValue, this.ValueType);
            if (this.ConstraintImplementation is ChoicesConstraint)
            {
                foreach (var c in (this.constraintImplementation as ChoicesConstraint).Choices)
                {
                    c.Value = this.CoerceValue(c.Value, this.ValueType);
                }
            }
            else if (this.ConstraintImplementation is RangeConstraint)
            {
                var rc = this.constraintImplementation as RangeConstraint;
                rc.MinValue = this.CoerceValue(rc.MinValue, this.ValueType);
                rc.MaxValue = this.CoerceValue(rc.MaxValue, this.ValueType);
            }
            this.SetupValidationRules();
            this.Validator.ValidateAll();
            this.NotifyPropertyChanged(null);
        }
        protected object CoerceValue(object value, Type type)
        {
            if (value == null)
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                return Activator.CreateInstance(type);
            }
            try
            {
                return Convert.ChangeType(value, type);
            } 
            catch (Exception e)
            {
                try
                {
                    return TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, type);
                }catch(Exception e2)
                {
                    return Activator.CreateInstance(type);
                }
            }
        }
#endregion

        public void ResetValue()
        {
            this.Value = this.DefaultValue;
        }

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
            this.ValueType = System.Type.GetType(typeStr);
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
                this.DefaultValue = reader.ReadElementContentAs(this.ValueType, null);
            }

            //reader.ReadToFollowing("CurrentValue");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            else
            {
                this.Value = reader.ReadElementContentAs(this.ValueType, null);
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

            writer.WriteElementString("DataType", this.ValueType.FullName);
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
            var mdi = obj as MetadataItemDefinition;

            if (mdi == null)
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

        private void Choice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Validator.ValidateAll();
        }

        public ICommand AddChoice => new ActionCommand((p) =>
        {
            var constraint = (this.constraintImplementation as ChoicesConstraint);
            object val = null;
            if (this.ValueType == typeof(string))
            {
                val = "New Option "+constraint.Choices.Count;
            }
            else
            {
                val = Activator.CreateInstance(this.ValueType);
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
