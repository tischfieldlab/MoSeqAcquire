using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.Rules
{
    public class RequiredRule : BaseRule
    {
        public RequiredRule() : base("Required")
        {

        }

        public override RuleResult Validate(MetadataItemDefinition Item)
        {
            if (Item.ValueType.DataType.Equals(typeof(Boolean)))
                return (bool)Item.Value ? RuleResult.Valid() : RuleResult.Invalid("Value must be true");

            if (string.IsNullOrWhiteSpace(Item.Value as string))
                return RuleResult.Invalid("A value is required.");

            return RuleResult.Valid();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
