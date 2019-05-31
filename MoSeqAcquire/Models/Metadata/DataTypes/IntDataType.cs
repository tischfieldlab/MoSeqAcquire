using MoSeqAcquire.Models.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class IntDataType : BaseDataType
    {
        public IntDataType() : base(typeof(int))
        {
        }

        public override List<BaseRule> GetValidators()
        {
            return new List<BaseRule>()
            {
                new RequiredRule(),
                new RangeRule()
            };
        }

        public override object CoerceValue(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception e)
            {
                return default(int);
            }
        }
    }
}
