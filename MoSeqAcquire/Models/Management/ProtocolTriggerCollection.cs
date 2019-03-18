using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Sources")]
    public class ProtocolTriggerCollection : Collection<ProtocolTrigger>
    {
        public void Add(Type EventType, Type ActionType, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolTrigger()
            {
                Event = EventType.AssemblyQualifiedName,
                Action = ActionType.AssemblyQualifiedName,
                Config = Settings
            });
        }
    }

    [XmlType("Trigger")]
    public class ProtocolTrigger
    {
        [XmlAttribute]
        public string Event { get; set; }
        [XmlElement]
        public string Action { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }
        

        public Type GetEventType()
        {
            return Type.GetType(this.Event);
        }
        public Type GetActionType()
        {
            return Type.GetType(this.Action);
        }
        public override bool Equals(object obj)
        {
            var source = obj as ProtocolTrigger;

            if (!this.Event.Equals(source.Event))
                return false;
            if (!this.Action.Equals(source.Action))
                return false;
            if (!this.Config.Equals(source.Config))
                return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
