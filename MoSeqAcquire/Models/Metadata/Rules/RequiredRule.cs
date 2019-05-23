using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (Item.ValueType.Equals(typeof(Boolean)))
                return (bool)Item.Value ? RuleResult.Valid() : RuleResult.Invalid("Value must be true");

            if (string.IsNullOrWhiteSpace(Item.Value as string))
                return RuleResult.Invalid("A value is required.");

            return RuleResult.Valid();
        }
    }
}
