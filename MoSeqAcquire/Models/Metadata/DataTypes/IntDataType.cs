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
            this.ValidTypeConstraints.Add(ConstraintMode.Choices);
            this.ValidTypeConstraints.Add(ConstraintMode.Range);

            this.Validators.Add(new RequiredRule());
            
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
