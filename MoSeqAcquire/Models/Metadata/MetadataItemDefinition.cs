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

    public class MetadataItemDefinition : ValidatingBaseViewModel, IXmlSerializable
    {
        protected string name;
        protected BaseDataType valueType;
        protected object value;
        protected object defaultValue;
        protected string units;
        protected ObservableCollection<BaseRule> validators;

        public MetadataItemDefinition(string Name, BaseDataType ValueType) : this()
        {
            this.Name = Name;
            this.ValueType = ValueType;
        }

        protected MetadataItemDefinition()
        {
            this.validators = new ObservableCollection<BaseRule>();
            this.validators.CollectionChanged += Validators_CollectionChanged;
            this.SetupValidationRules();
        }

        private void Validators_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.OldItems?.Cast<BaseRule>().ForEach(r => r.PropertyChanged -= Validator_PropertyChanged);
            e.NewItems?.Cast<BaseRule>().ForEach(r => r.PropertyChanged += Validator_PropertyChanged);
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


            if (this.valueType != null)
            {
                foreach (var rule in this.Validators)
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
                this.validators.Clear();
                value.GetValidators().ForEach(r => this.validators.Add(r));
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
                this.ResetValue();
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

        public ObservableCollection<ChoicesRuleChoice> AvailableChoices
        {
            get => this.GetValidator<ChoicesRule>()?.Choices;
        }
        public object Minimum
        {
            get
            {
                var v = this.GetValidator<RangeRule>();
                if (v != null && v.IsActive)
                    return v.MinValue;
                return null;
            }
        }
        public object Maximum
        {
            get
            {
                var v = this.GetValidator<RangeRule>();
                if (v != null && v.IsActive)
                    return v.MaxValue;
                return null;
            }
        }

        public bool HasUnits => !string.IsNullOrWhiteSpace(this.Units);

       
        public ObservableCollection<BaseRule> Validators
        {
            get => this.validators;
        }

        public T GetValidator<T>()
        {
            return this.validators.OfType<T>().FirstOrDefault();
        }

        #region Value Coerceion
        public void ResetValue()
        {
            this.Value = this.DefaultValue;
        }
        protected void CoerceAllValues()
        {

            this.value = this.valueType.CoerceValue(this.value);
            this.defaultValue = this.valueType.CoerceValue(this.defaultValue);

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
                this.DefaultValue = this.ValueType.Parse(reader.ReadElementContentAsString());
            }

            //reader.ReadToFollowing("CurrentValue");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            else
            {
                this.Value = this.ValueType.Parse(reader.ReadElementContentAsString());
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


            //reader.ReadToFollowing("Validators");
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
            }
            else
            {
                
                while (true)
                {
                    if (reader.Name.Equals("Validators") && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        break;
                    }

                    reader.ReadStartElement();
                    var isEmpty = reader.IsEmptyElement;
                    if (reader.Name == "Validator" && reader.NodeType == XmlNodeType.Element)
                    {
                        var v = this.validators.Where(r => r.Name.Equals(reader.GetAttribute("Name"))).FirstOrDefault();
                        if (v != null)
                        {
                            v.IsActive = bool.Parse(reader.GetAttribute("Active"));
                        }

                        v.ReadXml(reader);

                        if (!isEmpty)
                        {
                            reader.ReadEndElement();
                        }
                    }
                }
            }
            reader.ReadEndElement();
            this.CoerceAllValues();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);

            writer.WriteElementString("DataType", this.ValueType.DataType.FullName);
            writer.WriteElementString("DefaultValue", this.ValueType.Serialize(this.DefaultValue));
            writer.WriteElementString("CurrentValue", this.ValueType.Serialize(this.Value));
            writer.WriteElementString("Units", this.units);

            writer.WriteStartElement("Validators");
            foreach (var v in this.Validators)
            {
                writer.WriteStartElement("Validator");
                writer.WriteAttributeString("Name", v.Name);
                writer.WriteAttributeString("Active", v.IsActive.ToString());

                v.WriteXml(writer);

                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            /* writer.WriteStartElement("Constraint");
             writer.WriteAttributeString("Type", this.constraint.ToString());
             if (this.constraint != ConstraintMode.None)
             {
                 this.constraintImplementation.WriteXml(writer);
             }
             writer.WriteEndElement();*/
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
            /*if ((this.ConstraintImplementation == null && mdi.ConstraintImplementation != null)
                || (this.ConstraintImplementation != null && !this.ConstraintImplementation.Equals(mdi.ConstraintImplementation)))
                return false;*/

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
           // hashCode = hashCode * -1521134295 + EqualityComparer<List<ConstraintMode>>.Default.GetHashCode(ConstraintsAllowed);
            //hashCode = hashCode * -1521134295 + Constraint.GetHashCode();
            //hashCode = hashCode * -1521134295 + EqualityComparer<BaseConstraint>.Default.GetHashCode(ConstraintImplementation);
            //hashCode = hashCode * -1521134295 + EqualityComparer<List<BaseRule>>.Default.GetHashCode(Validators);
            return hashCode;
        }

        /*public ICommand AddChoice => new ActionCommand((p) =>
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

            var choice = new ChoicesRuleChoice(this) {Value = val};
            choice.PropertyChanged += Choice_PropertyChanged;
            constraint.Choices.Add(choice);
        });

        public ICommand RemoveChoice => new ActionCommand((p) =>
        {
            var choice = p as ChoicesRuleChoice;
            choice.PropertyChanged -= Choice_PropertyChanged;
            (this.constraintImplementation as ChoicesConstraint).Choices.Remove(choice);
        });*/
    }
}
