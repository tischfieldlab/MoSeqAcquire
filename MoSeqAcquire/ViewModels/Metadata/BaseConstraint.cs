using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public abstract class BaseConstraint : BaseViewModel, IXmlSerializable
    {
        public BaseConstraint(MetadataItem Owner)
        {
            this.Owner = Owner;
        }
        public MetadataItem Owner { get; protected set; }
        public XmlSchema GetSchema()
        {
            return null;
        }
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}
