using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Configuration
{
    [XmlRoot("Configuration")]
    public class ConfigSnapshot : Dictionary<string, object>, IXmlSerializable
    {
        public ConfigSnapshot() { }

        //public static ConfigSnapshot GetDefault() { throw new NotImplementedException(); }

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

            while (reader.Read())
            {
                if(reader.Name == openName && reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                    return;
                }
                if (reader.Name == "setting" && reader.NodeType == XmlNodeType.Element)
                {
                    string key = reader.GetAttribute("name");
                    string typeName = reader.GetAttribute("type");
                    reader.Read();
                    string valueString = reader.Value;

                    Type valueType = knownTypes.FirstOrDefault(t => t.FullName.Equals(typeName)); //try to find in known types
                    if (valueType == null)
                    {
                        valueType = Type.GetType(typeName); //fall back to this method
                    }

                    object value = null;
                    if (valueType != null)
                    {
                        if (typeof(Enum).IsAssignableFrom(valueType))
                        {
                            value = Enum.Parse(valueType, valueString);
                        }
                        else if (typeof(IConvertible).IsAssignableFrom(valueType))
                        {
                            value = Convert.ChangeType(valueString, valueType);
                        }
                        else
                        {
                            value = valueString;
                        }
                    }
                    this.Add(key, value);
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (string key in this.Keys)
            {
                Type valueType = this[key].GetType();
                writer.WriteStartElement("setting");
                writer.WriteAttributeString("name", key);
                writer.WriteAttributeString("type", valueType.FullName);
                writer.WriteRaw(this[key].ToString());
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
