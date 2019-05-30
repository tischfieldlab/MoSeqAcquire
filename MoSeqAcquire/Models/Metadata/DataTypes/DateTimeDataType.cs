using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public class DateTimeDataType : BaseDataType
    {
        public DateTimeDataType() : base(typeof(DateTime))
        {
            this.ValidTypeConstraints.Add(ConstraintMode.Choices);
            this.ValidTypeConstraints.Add(ConstraintMode.Range);
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
