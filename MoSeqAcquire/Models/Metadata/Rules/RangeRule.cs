using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MoSeqAcquire.Models.Metadata.Rules;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata
{
    public class RangeRule : BaseRule
    {
        protected object minValue;
        protected object maxValue;

        public RangeRule() : base("Range")
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

        public override RuleResult Validate(object value)
        {
            return RuleResult.Assert((value as IComparable).CompareTo((this.MinValue as IComparable)) >= 0
                                  && (value as IComparable).CompareTo((this.MaxValue as IComparable)) <= 0,
                                    $"Value must be within range [{this.MinValue}, {this.MaxValue}]");
        }

        public override RuleResult Validate(MetadataItemDefinition Item)
        {
            throw new NotImplementedException();
        }
    }
}
