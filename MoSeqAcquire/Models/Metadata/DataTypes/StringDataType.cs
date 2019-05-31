using MoSeqAcquire.Models.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class StringDataType : BaseDataType
    {
        public StringDataType() : base(typeof(string))
        {
        }

        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new RequiredRule(),
                new ChoicesRule(this)
            };
        }

        public override object CoerceValue(object value)
        {
            return Convert.ToString(value);
        }

        public override object Parse(string value)
        {
            return value;
        }
    }
}
