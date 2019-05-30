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
            this.ValidTypeConstraints.Add(ConstraintMode.Choices);

            this.Validators.Add(new RequiredRule());
        }
        public override object CoerceValue(object value)
        {
            return Convert.ToString(value);
        }
    }
}
