using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public abstract class BaseDataType
    {
        protected BaseDataType(Type type)
        {
            this.DataType = type;
        }
        public Type DataType { get; private set; }
        public virtual string Name => this.DataType.Name;
        public List<ConstraintMode> ValidTypeConstraints { get; protected set; }

        public abstract object CoerceValue(object value);


    }
    public class BooleanDataType : BaseDataType
    {
        public BooleanDataType() : base(typeof(bool))
        {
            this.ValidTypeConstraints = new List<ConstraintMode>() {ConstraintMode.None};
        }

        public override object CoerceValue(object value)
        {
            return Convert.ToBoolean(value);
        }
    }
    public class StringDataType : BaseDataType
    {
        public StringDataType() : base(typeof(string))
        {
            this.ValidTypeConstraints = new List<ConstraintMode>() { ConstraintMode.None, ConstraintMode.Choices };
        }
        public override object CoerceValue(object value)
        {
            return Convert.ToString(value);
        }
    }
    public class IntDataType : BaseDataType
    {
        public IntDataType() : base(typeof(int))
        {
            this.ValidTypeConstraints = new List<ConstraintMode>() { ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range };
        }
        public override object CoerceValue(object value)
        {
            return Convert.ToInt32(value);
        }
    }
    public class DoubleDataType : BaseDataType
    {
        public DoubleDataType() : base(typeof(int))
        {
            this.ValidTypeConstraints = new List<ConstraintMode>() { ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range };
        }
        public override object CoerceValue(object value)
        {
            return Convert.ToDouble(value);
        }
    }
    public class DateTimeDataType : BaseDataType
    {
        public DateTimeDataType() : base(typeof(DateTime))
        {
            this.ValidTypeConstraints = new List<ConstraintMode>() { ConstraintMode.None, ConstraintMode.Choices, ConstraintMode.Range };
        }
        public override object CoerceValue(object value)
        {
            return Convert.ToDateTime(value);
        }
    }
}
