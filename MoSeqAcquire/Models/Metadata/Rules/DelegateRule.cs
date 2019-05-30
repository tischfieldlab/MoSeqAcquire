using MvvmValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata.Rules
{
    public class DelegateRule : BaseRule
    {
        private Func<MetadataItemDefinition, RuleResult> action;

        public DelegateRule(string Name, Func<MetadataItemDefinition, RuleResult> action) : base(Name)
        {
            this.action = action;
        }
        public override RuleResult Validate(MetadataItemDefinition Item)
        {
            return action(Item);
        }
    }
}
