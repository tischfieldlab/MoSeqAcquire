using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Configuration
{
    [XmlRoot("Configuration")]
    public class ConfigSnapshot : Dictionary<string, ConfigSnapshotSetting>, IXmlSerializable
    {
        public ConfigSnapshot() { }

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            var knownTypes = ProtocolHelpers.GetKnownTypes();

            bool wasEmpty = reader.IsEmptyElement;
            string openName = reader.Name;

            if (wasEmpty)
                return;
            reader.Read();
            while (true)
            {
                if (reader.Name == "setting" && reader.NodeType == XmlNodeType.Element)
                {
                    var setting = new ConfigSnapshotSetting();
                    setting.ReadXml(reader);
                    this.Add(setting.Name, setting);
                }
                else
                {
                    return;
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (string key in this.Keys)
            {
                this[key].WriteXml(writer);
            }
        }
        #endregion
    }

    public class ConfigSnapshotSetting : IXmlSerializable
    {
        public string Name { get; set; }
        public bool? Automatic { get; set; }
        public object Value { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var knownTypes = ProtocolHelpers.GetKnownTypes();

           
            this.Name = reader.GetAttribute("name");
            string valueTypeString = reader.GetAttribute("type");
                
            var auto = reader.GetAttribute("automatic");
            if(auto != null)
            {
                this.Automatic = bool.Parse(auto);
            }
            reader.Read();
            string valueString = reader.Value;

            Type valueType = knownTypes.FirstOrDefault(t => t.FullName.Equals(valueTypeString)); //try to find in known types
            if (valueType == null)
            {
                valueType = System.Type.GetType(valueTypeString); //fall back to this method
            }

            if (valueType != null)
            {
                if (typeof(Enum).IsAssignableFrom(valueType))
                {
                    this.Value = Enum.Parse(valueType, valueString);
                }
                else if (typeof(IConvertible).IsAssignableFrom(valueType))
                {
                    this.Value = Convert.ChangeType(valueString, valueType);
                }
                else
                {
                    this.Value = valueString;
                }
            }
            if(!(reader.Name == "setting" && reader.NodeType == XmlNodeType.EndElement))
            {
                reader.Read();
            }
            reader.ReadEndElement();
            return;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("setting");
            writer.WriteAttributeString("name", this.Name);
            if (this.Automatic != null)
            {
                writer.WriteAttributeString("automatic", this.Automatic.ToString());
            }
            if (this.Value != null)
            {
                writer.WriteAttributeString("type", this.Value.GetType().FullName);
                writer.WriteRaw(this.Value.ToString());
            }
            else
            {
                writer.WriteAttributeString("type", "");
                writer.WriteRaw("");
            }
            writer.WriteEndElement();
        }
    }
}
