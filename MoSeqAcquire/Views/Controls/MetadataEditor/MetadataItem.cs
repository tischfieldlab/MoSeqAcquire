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

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public enum ConstraintMode
    {
        None = 0,
        Choices,
        Range
    }
    public class MetadataItem : ValidatingBaseViewModel, IXmlSerializable
    {
        protected string name;
        protected Type valueType;
        protected object value;
        protected object defaultValue;
        protected string units;
        protected ConstraintMode constraint;
        protected BaseConstraint constraintImplementation;

        protected readonly Dictionary<Type, List<ConstraintMode>> validTypeConstraints = new Dictionary<Type, List<ConstraintMode>>()
        {
            { typeof(bool), new List<ConstraintMode>(){ConstraintMode.None } },
            { typeof(string), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices } },
            { typeof(int), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range } },
            { typeof(double), new List<ConstraintMode>(){ ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range } }
        };

        public MetadataItem(string Name, Type ValueType) : this()
        {
            this.name = Name;
            this.valueType = ValueType;
        }

        protected MetadataItem()
        {
            this.Validator.AddRule(nameof(this.Name), 
                                () => RuleResult.Assert(!string.IsNullOrEmpty(this.Name), "Name is required"));
            this.Validator.AddRule(nameof(this.Value), 
                () => RuleResult.Assert(this.ValueType.IsAssignableFrom(this.Value.GetType()), "Value is not a valid " + this.ValueType.Name));
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
                this.NotifyPropertyChanged(nameof(this.ConstraintsAllowed));
                this.CoerceAllValues();
            }
        }
        [XmlAttribute]
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
        [XmlElement]
        public object DefaultValue
        {
            get => this.defaultValue;
            set => this.SetField(ref this.defaultValue, value);
        }
        [XmlAttribute]
        public string Units
        {
            get => this.units;
            set => this.SetField(ref this.units, value);
        }
        [XmlIgnore]
        public List<ConstraintMode> ConstraintsAllowed
        {
            get => this.validTypeConstraints[this.ValueType];
        }
        [XmlElement]
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




        protected void ValidateConstraintForType()
        {
            if (!this.ConstraintsAllowed.Contains(this.Constraint))
            {
                this.Constraint = ConstraintMode.None;
            }
        }
        protected void CoerceAllValues()
        {
            this.ValidateConstraintForType();
            this.Value = this.CoerceValue(this.value, this.ValueType);
            this.DefaultValue = this.CoerceValue(this.defaultValue, this.ValueType);
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
            
        }
        protected object CoerceValue(object value, Type type)
        {
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
            if (this.constraint != ConstraintMode.None)
            {
                this.constraintImplementation.ReadXml(reader);
            }
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
            var mdi = obj as MetadataItem;

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
            constraint.Choices.Add(new ChoicesConstraintChoice(this) { Value = val });
        });
        public ICommand RemoveChoice => new ActionCommand((p) =>
        {
            (this.constraintImplementation as ChoicesConstraint).Choices.Remove(p as ChoicesConstraintChoice);
        });
    }

    public abstract class BaseConstraint : BaseViewModel, IXmlSerializable
    {
        public BaseConstraint(MetadataItem Owner)
        {
            this.Owner = Owner;
        }
        public MetadataItem Owner { get; protected set; }
        public XmlSchema GetSchema()
        {
            return null;
        }
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }

    

    
}
