using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Metadata
{
    public class RecordingMetadataSnapshot : List<RecordingMetadataSnapshotItem>, IXmlSerializable
    {

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            string selfName = reader.Name;

            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                while (true)
                {
                    if (reader.Name == "Item" && reader.NodeType == XmlNodeType.Element)
                    {
                        var item = new RecordingMetadataSnapshotItem();
                        item.ReadXml(reader);
                        this.Add(item);
                    }
                    else if (reader.Name.Equals(selfName) && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        return;
                    }
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (var item in this)
            {
                writer.WriteStartElement("Item");
                item.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
    }

    public class RecordingMetadataSnapshotItem : IXmlSerializable
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Units { get; set; }
        public object Value { get; set; }

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Name = reader.GetAttribute("Name");
            this.Units = reader.GetAttribute("Units");

            var typeStr = reader.GetAttribute("Type");
            this.Type = System.Type.GetType(typeStr);
            if (this.Type == null)
            {
                throw new ArgumentException("Unable to find Type for " + typeStr);
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                string valueString = reader.ReadContentAsString();
                reader.ReadEndElement();

                if (this.Type != null)
                {
                    if (typeof(Enum).IsAssignableFrom(this.Type))
                    {
                        this.Value = Enum.Parse(this.Type, valueString);
                    }
                    else if (typeof(TimeSpan).IsAssignableFrom(this.Type))
                    {
                        this.Value = TimeSpan.Parse(valueString);
                    }
                    else if (typeof(IConvertible).IsAssignableFrom(this.Type))
                    {
                        this.Value = Convert.ChangeType(valueString, this.Type);
                    }
                    else
                    {
                        this.Value = valueString;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("Units", this.Units);

            writer.WriteAttributeString("Type", this.Type.FullName);
            if (this.Value != null)
            {
                writer.WriteRaw(this.Value.ToString());
            }
            else
            {
                writer.WriteRaw(string.Empty);
            }
        }
        #endregion
    }
}
