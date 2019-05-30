using MoSeqAcquire.Models.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MoSeqAcquire.Models.Metadata.DataTypes
{
    public abstract class BaseDataType
    {
        protected BaseDataType(Type type)
        {
            this.DataType = type;
            this.ValidTypeConstraints = new List<ConstraintMode>() { ConstraintMode.None };
            this.Validators = new List<BaseRule>();
        }
        public Type DataType { get; private set; }
        public virtual string Name => this.DataType.Name;
        public List<ConstraintMode> ValidTypeConstraints { get; private set; }
        public List<BaseRule> Validators { get; private set; }

        public abstract object CoerceValue(object value);

        public abstract BaseConstraint ProvideConstraintImplementation(ConstraintMode constraint);

        #region Operators
        public static implicit operator Type(BaseDataType dataType)
        {
            return dataType.DataType;
        }
        public static implicit operator BaseDataType(Type type)
        {
            return FromType(type);
        }
        #endregion

        #region Factory
        public static BaseDataType FromType(Type type)
        {
            if (typeof(BaseDataType).IsAssignableFrom(type))
                return (BaseDataType)Activator.CreateInstance(type);

            if (type == typeof(string))
                return new StringDataType();
            if (type == typeof(bool))
                return new BooleanDataType();
            if (type == typeof(int))
                return new IntDataType();
            if (type == typeof(double))
                return new DoubleDataType();
            if (type == typeof(DateTime))
                return new DateTimeDataType();

            throw new NotSupportedException($"Type {type.FullName} is not supported!");
        }
        public static IEnumerable<Type> AvailableTypes()
        {
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(t => typeof(BaseDataType).IsAssignableFrom(t))
                           .Where(t => !t.IsAbstract);
        }
        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            var type = obj as BaseDataType;
            return type != null &&
                   EqualityComparer<Type>.Default.Equals(DataType, type.DataType);
        }

        public override int GetHashCode()
        {
            return -598241675 + EqualityComparer<Type>.Default.GetHashCode(DataType);
        }
        #endregion
    }
}
