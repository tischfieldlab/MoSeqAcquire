using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.Rules
{
    public abstract class BaseRule : BaseViewModel, IXmlSerializable
    {
        protected bool isActive;

        protected BaseRule(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
            protected set;
        }
        public bool IsActive
        {
            get => this.isActive;
            set => this.SetField(ref this.isActive, value);
        }

        public Type DesignerImplementation
        {
            get;
            set;
        }
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
        }

        public virtual void WriteXml(XmlWriter writer)
        {
        }


        public abstract RuleResult Validate(MetadataItemDefinition Item);
    }
}
