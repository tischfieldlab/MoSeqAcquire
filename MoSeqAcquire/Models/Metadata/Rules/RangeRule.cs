using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MoSeqAcquire.Models.Metadata.DataTypes;
using MoSeqAcquire.Models.Metadata.Rules;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata
{
    public class RangeRule : BaseRule
    {
        protected object minValue;
        protected object maxValue;

        public RangeRule(BaseDataType dataType) : base("Range")
        {
            this.DataType = dataType;
        }

        public BaseDataType DataType
        {
            get;
            protected set;
        }
        public object MinValue
        {
            get => this.minValue;
            set => this.SetField(ref this.minValue, this.DataType.CoerceValue(value));
        }
        public object MaxValue
        {
            get => this.maxValue;
            set => this.SetField(ref this.maxValue, this.DataType.CoerceValue(value));
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            this.MinValue = this.DataType.Parse(reader.ReadElementContentAsString("Minimum", ""));
            this.MaxValue = this.DataType.Parse(reader.ReadElementContentAsString("Maximum", ""));
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Minimum", this.DataType.Serialize(this.minValue));
            writer.WriteElementString("Maximum", this.DataType.Serialize(this.maxValue));
        }
        public override bool Equals(object obj)
        {

            if (!(obj is RangeRule rc))
                return false;

            if (!this.MinValue.Equals(rc.MinValue))
                return false;
            if (!this.MaxValue.Equals(rc.MaxValue))
                return false;

            return true;
        }

        public override RuleResult Validate(MetadataItemDefinition Item)
        {
            return RuleResult.Assert((Item.Value as IComparable).CompareTo((this.MinValue as IComparable)) >= 0
                                  && (Item.Value as IComparable).CompareTo((this.MaxValue as IComparable)) <= 0,
                $"Value must be within range [{this.MinValue}, {this.MaxValue}]");
        }
    }
}
