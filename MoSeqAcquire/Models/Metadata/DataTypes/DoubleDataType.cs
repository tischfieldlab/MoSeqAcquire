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
            this.ValidTypeConstraints.Add(ConstraintMode.Choices);
            this.ValidTypeConstraints.Add(ConstraintMode.Range);

            this.Validators.Add(new RequiredRule());
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
    }
}
