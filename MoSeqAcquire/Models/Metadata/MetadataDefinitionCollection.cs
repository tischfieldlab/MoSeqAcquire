using System;
using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Metadata
{
    public class MetadataDefinitionCollection : ObservableCollection<MetadataItemDefinition>, IXmlSerializable
    {

        public MetadataDefinitionCollection() : base()
        {
            this.Initialize();  
        }

        public new void Add(MetadataItemDefinition Item)
        {
            base.Add(Item);
        }
        public new void Remove(MetadataItemDefinition Item)
        {
            base.Remove(Item);
        }

        protected void Initialize()
        {
        }

        public override bool Equals(object obj)
        {
            return this.SequenceEqual(obj as MetadataDefinitionCollection);
        }

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
                    if (reader.Name == "MetadataItem" && reader.NodeType == XmlNodeType.Element)
                    {
                        MetadataItemDefinition item = (MetadataItemDefinition)Activator.CreateInstance(typeof(MetadataItemDefinition), true);
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
            foreach (MetadataItemDefinition item in this)
            {
                writer.WriteStartElement("MetadataItem");
                item.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
