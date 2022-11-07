using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Metadata.Rules;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class DateDataType : BaseDataType
    {
        public DateDataType() : base(typeof(DateTime))
        {
        }
        public override string Name => "Date";

        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new DelegateRule("Date in Past", (mid) => ((DateTime)mid.Value).Date < DateTime.Now.Date ? RuleResult.Valid() : RuleResult.Invalid("Value must be in the past")),
                new RangeRule(this)
                {
                    MinValue = DateTime.Now.Subtract(TimeSpan.FromDays(365)).Date,
                    MaxValue = DateTime.Now.AddYears(1).Date
                },
                new ChoicesRule(this)
            };
        }

        public override object CoerceValue(object value)
        {
            try
            {
                return Convert.ToDateTime(value).Date;
            }
            catch (Exception e)
            {
                return DateTime.Now.Date;
            }
        }

        public override object Parse(string value)
        {
            return DateTime.Parse(value).Date;
        }

        public override string Serialize(object value)
        {
            return ((DateTime) value).Date.ToString();
        }
        public override object GetReasonableDefault()
        {
            return DateTime.Now.Date;
        }
    }
}
