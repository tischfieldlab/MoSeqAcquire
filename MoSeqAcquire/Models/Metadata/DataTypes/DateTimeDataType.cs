using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Metadata.Rules;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class DateTimeDataType : BaseDataType
    {
        public DateTimeDataType() : base(typeof(DateTime))
        {
        }

        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new DelegateRule("Date in Past", (mid) => ((DateTime)mid.Value).Date < DateTime.Now.Date ? RuleResult.Valid() : RuleResult.Invalid("Value must be in the past"))
            };
        }

        public override object CoerceValue(object value)
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }
    }
}
