using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Configuration
{
    [XmlRoot("Configuration")]
    public class ConfigSnapshot : Dictionary<string, ConfigSnapshotSetting>, IXmlSerializable
    {
        public ConfigSnapshot() { }

        public void Add(string Name, Type ValueType, object Value, bool? Automatic)
        {
            this.Add(Name, new ConfigSnapshotSetting()
            {
                Name = Name,
                Automatic = Automatic,
                Value = Value,
                ValueType = ValueType
            });
        }
        public void Add(string Name, object Value, bool? Automatic)
        {
            this.Add(Name, new ConfigSnapshotSetting()
            {
                Name = Name,
                Automatic = Automatic,
                Value = Value,
                ValueType = Value.GetType()
            });
        }

        public override bool Equals(object obj)
        {
            return this.SequenceEqual(obj as ConfigSnapshot);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
                    if (reader.Name == "Setting" && reader.NodeType == XmlNodeType.Element)
                    {
                        var setting = new ConfigSnapshotSetting();
                        setting.ReadXml(reader);
                        this.Add(setting.Name, setting);
                    }
                    else if(reader.Name.Equals(selfName) && reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.ReadEndElement();
                        return;
                    }
                }
            }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            /*var list = new List<ConfigSnapshotSetting>(this.Values);
            XmlSerializer serializer = new XmlSerializer(list.GetType());
            serializer.Serialize(writer, list);*/

            foreach (string key in this.Keys)
            {
                writer.WriteStartElement("Setting");
                this[key].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
    }

    public class ConfigSnapshotSetting : IXmlSerializable
    {
        public ConfigSnapshotSetting() { }

        public string Name { get; set; }
        public bool? Automatic { get; set; }
        public Type ValueType { get; set; }
        public object Value { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public override bool Equals(object obj)
        {
            var css = obj as ConfigSnapshotSetting;

            if (!this.Name.Equals(css.Name))
                return false;
            if (!this.Automatic.Equals(css.Automatic))
                return false;
            if (!this.Value.Equals(css.Value))
                return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void ReadXml(XmlReader reader)
        {
            var knownTypes = ProtocolHelpers.GetKnownTypes();
            reader.MoveToContent();

            //Read setting name
            this.Name = reader.GetAttribute("Name");

            //Read setting value type
            string valueTypeString = reader.GetAttribute("Type");
            this.ValueType = knownTypes.FirstOrDefault(t => t.FullName.Equals(valueTypeString)); //try to find in known types
            if (this.ValueType == null)
            {
                this.ValueType = System.Type.GetType(valueTypeString); //fall back to this method
            }

            if(this.ValueType == null)
            {
                throw new ArgumentException("Unable to find Type for "+valueTypeString);
            }

            //Read setting automatic-ness
            string auto = reader.GetAttribute("Automatic");
            if(auto != null)
            {
                this.Automatic = bool.Parse(auto);
            }

            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmptyElement)
            {
                string valueString = reader.ReadContentAsString();
                reader.ReadEndElement();


                if (this.ValueType != null)
                {
                    if (typeof(Enum).IsAssignableFrom(this.ValueType))
                    {
                        this.Value = Enum.Parse(this.ValueType, valueString);
                    }
                    else if (typeof(IConvertible).IsAssignableFrom(this.ValueType))
                    {
                        this.Value = Convert.ChangeType(valueString, this.ValueType);
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
            if (this.Automatic != null)
            {
                writer.WriteAttributeString("Automatic", this.Automatic.ToString());
            }
            writer.WriteAttributeString("Type", this.ValueType.FullName);
            if (this.Value != null)
            {
                writer.WriteRaw(this.Value.ToString());
            }
            else
            {
                writer.WriteRaw(string.Empty);
            }
        }
    }
}
