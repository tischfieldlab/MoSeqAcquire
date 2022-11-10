using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Sources")]
    public class ProtocolTriggerCollection : Collection<ProtocolTrigger>
    {
        /*public void Add(string Name, Type EventType, Type ActionType, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolTrigger()
            {
                Name = Name,
                Event = EventType.AssemblyQualifiedName,
                Action = ActionType.AssemblyQualifiedName,
                Config = Settings
            });
        }*/
    }

    public class ProtocolTriggerEvent
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public string Event { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }
        public Type GetEventType()
        {
            return Type.GetType(this.Event);
        }
    }

    public class ProtocolTriggerAction
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public bool Critical { get; set; }
        [XmlAttribute]
        public int Priority { get; set; }

        [XmlElement]
        public string Action { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }
        public Type GetActionType()
        {
            return Type.GetType(this.Action);
        }

        public override bool Equals(object obj)
        {
            var source = obj as ProtocolTriggerAction;

            if (!this.Name.Equals(source.Name))
                return false;
            if (!this.Critical.Equals(source.Critical))
                return false;
            if (!this.Priority.Equals(source.Priority))
                return false;
            if (!this.Action.Equals(source.Action))
                return false;
            if (!this.Config.Equals(source.Config))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 845239365;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Critical.GetHashCode();
            hashCode = hashCode * -1521134295 + Priority.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Action);
            hashCode = hashCode * -1521134295 + EqualityComparer<ConfigSnapshot>.Default.GetHashCode(Config);
            return hashCode;
        }
    }

    [XmlType("Trigger")]
    public class ProtocolTrigger
    {
        public ProtocolTriggerEvent Event { get; set; }
        public Collection<ProtocolTriggerAction> Actions { get; set; }
    }
}
