﻿using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Protocol")]
    public class Protocol
    {
        public static readonly string Extension = "xml";
        public static readonly string TypeDesc = "Protocol";

        protected Protocol()
        {
            this.Sources = new ProtocolSourceCollection();
            this.Recordings = new ProtocolRecordingsSetup();
            this.Triggers = new ProtocolTriggerCollection();
            this.Metadata = new MetadataDefinitionCollection();
        }
        public Protocol(String Name) : this()
        {
            this.Name = Name;
        }
        [XmlAttribute]
        public string Name { get; set; }


        public bool Locked { get; set; }
        public ProtocolSourceCollection Sources { get; set; }
        public ProtocolRecordingsSetup Recordings { get; set; }
        public ProtocolTriggerCollection Triggers { get; set; }
        public MetadataDefinitionCollection Metadata { get; set; }

        public override bool Equals(object obj)
        {
            var pcol = obj as Protocol;

            //if (!this.Name.Equals(pcol.Name))
            //    return false;
            if (!this.Locked.Equals(pcol.Locked))
                return false;
            if (!this.Sources.SequenceEqual(pcol.Sources))
                return false;
            if (!this.Recordings.Equals(pcol.Recordings))
                return false;
            if (!this.Triggers.SequenceEqual(pcol.Triggers))
                return false;
            if (!this.Metadata.SequenceEqual(pcol.Metadata))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
