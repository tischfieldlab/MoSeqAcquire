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

namespace MoSeqAcquire.Models.Metadata
{
    public class ChoicesConstraint : BaseConstraint
    {
        public ChoicesConstraint(MetadataItemDefinition Owner) : base(Owner)
        {
            this.Choices = new ObservableCollection<ChoicesConstraintChoice>();
        }
        public ObservableCollection<ChoicesConstraintChoice> Choices { get; protected set; }

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
                        var choice = new ChoicesConstraintChoice(this.Owner);
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

        public override void WriteXml(XmlWriter writer)
        {
            foreach (var choice in this.Choices)
            {
                writer.WriteStartElement("Choice");
                choice.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public override bool Equals(object obj)
        {

            if (!(obj is ChoicesConstraint cc))
                return false;

            if (!this.Choices.SequenceEqual(cc.Choices))
                return false;

            return true;
        }
    }

    public class ChoicesConstraintChoice : BaseViewModel, IXmlSerializable
    {
        public ChoicesConstraintChoice(MetadataItemDefinition Owner)
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

            if (!(obj is ChoicesConstraintChoice ccc))
                return false;

            if (!this.Value.Equals(ccc.Value))
                return false;

            return true;
        }
    }
}
