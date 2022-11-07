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
        }
        public Type DataType { get; private set; }
        public virtual string Name => this.DataType.Name;

        public abstract List<BaseRule> GetValidators();
        public abstract object CoerceValue(object value);
        public abstract object Parse(string value);

        public virtual string Serialize(object value)
        {
            return value.ToString();
        }

        public virtual object GetReasonableDefault()
        {
            if (this.DataType.IsValueType)
            {
                return Activator.CreateInstance(this.DataType);
            }
            return null;
        }
        

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
                return new DateDataType();

            throw new NotSupportedException($"Type {type.FullName} is not supported!");
        }
        public static BaseDataType FromString(string type)
        {
            switch (type.Replace("System.", "").ToLower())
            {
                case "string":
                    return new StringDataType();
                case "bool":
                case "boolean":
                    return new BooleanDataType();
                case "int":
                case "int32":
                case "integer":
                    return new IntDataType();
                case "double":
                case "decimal":
                    return new DoubleDataType();
                case "datetime":
                case "date":
                    return new DateDataType();
            }

            throw new NotSupportedException($"Type {type} is not supported!");
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
