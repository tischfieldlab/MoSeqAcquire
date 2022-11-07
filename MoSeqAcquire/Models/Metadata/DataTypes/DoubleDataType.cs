using MoSeqAcquire.Models.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class DoubleDataType : BaseDataType
    {
        public DoubleDataType() : base(typeof(double))
        {
        }
        public override string Name => "Decimal";
        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new RequiredRule(),
                new RangeRule(this)
                {
                    MinValue = 1.0,
                    MaxValue = 10.0
                },
                new ChoicesRule(this)
            };
        }

        public override object CoerceValue(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch(Exception e)
            {
                return default(double);
            }
        }

        public override object Parse(string value)
        {
            return double.Parse(value);
        }
    }
}
