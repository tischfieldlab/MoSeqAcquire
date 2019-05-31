using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MoSeqAcquire.ViewModels;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.Deprecated
{
    public class ChoicesConstraint : BaseConstraint
    {
        public ChoicesConstraint(MetadataItemDefinition Owner) : base(Owner)
        {
            this.Name = "Choices";
            this.Choices = new ObservableCollection<ChoicesChoice>();
            this.Choices.CollectionChanged += Choices_CollectionChanged;
        }

        private void Choices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<ChoicesChoice>().ForEach(ccc => ccc.PropertyChanged += Choice_PropertyChanged);
            e.OldItems?.Cast<ChoicesChoice>().ForEach(ccc => ccc.PropertyChanged -= Choice_PropertyChanged);
        }

        private void Choice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(null);
        }

        public ObservableCollection<ChoicesChoice> Choices { get; protected set; }

        public  void ReadXml(XmlReader reader)
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
                        var choice = new ChoicesChoice(this.Owner);
                        choice.ReadXml(reader);
                        this.Choices.Add(choice);
                    }
                    else if (reader.Name.Equals(selfName) && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        return;
                    }
                }
            }
        }

        public  void WriteXml(XmlWriter writer)
        {
            foreach (var choice in this.Choices)
            {
                writer.WriteStartElement("Choice");
                choice.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public  bool Equals(object obj)
        {
            if (!(obj is ChoicesConstraint cc))
                return false;

            if (!this.Choices.SequenceEqual(cc.Choices))
                return false;

            return true;
        }

        public  RuleResult Validate(object value)
        {
            return RuleResult.Assert(this.Choices.Any(c => c.Value.Equals(value)), "Value must be one of available choices");
        }
    }

    public class ChoicesChoice : BaseViewModel, IXmlSerializable
    {
        public ChoicesChoice(MetadataItemDefinition Owner)
        {
            this.Owner = Owner;
        }
        public MetadataItemDefinition Owner { get; protected set; }
        protected object value;
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.value = reader.ReadElementContentAsString();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(this.Value.ToString());
        }

        public override bool Equals(object obj)
        {

            if (!(obj is ChoicesChoice ccc))
                return false;

            if (!this.Value.Equals(ccc.Value))
                return false;

            return true;
        }
    }
}
