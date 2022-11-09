using MoSeqAcquire.Models.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class BooleanDataType : BaseDataType
    {
        public BooleanDataType() : base(typeof(bool))
        {
            //this.Validators.Add(new DelegateRule("Required", (mid) => (bool)mid.Value ? RuleResult.Valid() : RuleResult.Invalid("Value must be true")));
        }

        public override object CoerceValue(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override object Parse(string value)
        {
            return bool.Parse(value);
        }

        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new DelegateRule("Required", (mid) => (bool)mid.Value ? RuleResult.Valid() : RuleResult.Invalid("Value must be true"))
            };
        }
    }
}
