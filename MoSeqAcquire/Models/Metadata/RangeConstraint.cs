using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata
{
    public class RangeConstraint : BaseConstraint
    {
        protected object minValue;
        protected object maxValue;

        public RangeConstraint(MetadataItemDefinition Owner) : base(Owner)
        {
            
        }

        public object MinValue
        {
            get => this.minValue;
            set => this.SetField(ref this.minValue, value);
        }
        public object MaxValue
        {
            get => this.maxValue;
            set => this.SetField(ref this.maxValue, value);
        }

        public override void ReadXml(XmlReader reader)
        {
            this.MinValue = reader.ReadElementContentAsString("Minimum", null);
            this.MaxValue = reader.ReadElementContentAsString("Maximum", null);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Minimum", this.minValue.ToString());
            writer.WriteElementString("Maximum", this.maxValue.ToString());
        }
        public override bool Equals(object obj)
        {

            if (!(obj is RangeConstraint rc))
                return false;

            if (!this.MinValue.Equals(rc.MinValue))
                return false;
            if (!this.MaxValue.Equals(rc.MaxValue))
                return false;

            return true;
        }
    }
}
