using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MoSeqAcquire.Models.Metadata.DataTypes;
using MoSeqAcquire.Models.Metadata.Deprecated;
using MoSeqAcquire.Models.Metadata.Rules;
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata
{
    public class ChoicesRule : BaseRule
    {
        public ChoicesRule(BaseDataType dataType) : base("Choices")
        {
            this.DataType = dataType;
            this.Choices = new ObservableCollection<ChoicesRuleChoice>();
            this.Choices.CollectionChanged += Choices_CollectionChanged;
        }

        private void Choices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<ChoicesRuleChoice>().ForEach(ccc => ccc.PropertyChanged += Choice_PropertyChanged);
            e.OldItems?.Cast<ChoicesRuleChoice>().ForEach(ccc => ccc.PropertyChanged -= Choice_PropertyChanged);
        }

        private void Choice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(null);
        }

        public BaseDataType DataType
        {
            get; protected set;
        }

        public ObservableCollection<ChoicesRuleChoice> Choices { get; protected set; }

        public override void ReadXml(XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            string selfName = reader.Name;

            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                while (true)
                {
                    if (reader.Name == "Choice" && reader.NodeType == XmlNodeType.Element)
                    {
                        var choice = new ChoicesRuleChoice(this.DataType);
                        choice.ReadXml(reader);
                        this.Choices.Add(choice);
                        //reader.ReadEndElement();
                    }
                    else if (reader.Name.Equals(selfName) && reader.NodeType == XmlNodeType.EndElement)
                    {
                        //reader.ReadEndElement();
                        return;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            foreach (var choice in this.Choices)
            {
                writer.WriteStartElement("Choice");
                choice.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public override RuleResult Validate(MetadataItemDefinition Item)
        {
            return RuleResult.Assert(this.Choices.Any(c => c.Value.Equals(Item.Value)), "Value must be one of available choices");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChoicesRule cc))
                return false;

            if (!this.Choices.SequenceEqual(cc.Choices))
                return false;

            return true;
        }

        public void AddChoice(object value)
        {
            var choice = new ChoicesRuleChoice(this.DataType)
            {
                Value = this.DataType.CoerceValue(value)
            };
            choice.PropertyChanged += Choice_PropertyChanged;
            this.Choices.Add(choice);
        }

        public ICommand AddChoiceCommand => new ActionCommand((p) => { this.AddChoice(null); });

        public ICommand RemoveChoiceCommand => new ActionCommand((p) =>
        {
            var choice = p as ChoicesRuleChoice;
            choice.PropertyChanged -= Choice_PropertyChanged;
            this.Choices.Remove(choice);
        });
    }

    public class ChoicesRuleChoice : BaseViewModel, IXmlSerializable
    {
        public ChoicesRuleChoice(BaseDataType dataType)
        {
            this.DataType = dataType;
            this.value = dataType.CoerceValue(null);
        }
        protected object value;
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
        public BaseDataType DataType
        {
            get; protected set;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.value = this.DataType.Parse(reader.ReadElementContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(this.DataType.Serialize(this.Value));
        }

        public override bool Equals(object obj)
        {

            if (!(obj is ChoicesRuleChoice ccc))
                return false;

            if (!this.Value.Equals(ccc.Value))
                return false;

            return true;
        }
    }
}
