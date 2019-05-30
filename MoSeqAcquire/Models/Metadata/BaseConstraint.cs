using System;
using System.Collections.Generic;
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
    public abstract class BaseConstraint : BaseViewModel, IXmlSerializable
    {
        public BaseConstraint(MetadataItemDefinition Owner)
        {
            this.Owner = Owner;
        }
        public string Name { get; protected set; }
        public MetadataItemDefinition Owner { get; protected set; }
        public XmlSchema GetSchema()
        {
            return null;
        }
        public abstract RuleResult Validate(object value);
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
        
    }
}
