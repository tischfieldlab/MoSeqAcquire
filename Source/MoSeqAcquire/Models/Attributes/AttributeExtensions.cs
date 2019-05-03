using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            if (type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static List<TValue> GetAttributeValues<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            List<TValue> values = new List<TValue>();
            foreach (TAttribute att in type.GetCustomAttributes(typeof(TAttribute), true))
            {
                if (att != null)
                {
                    values.Add(valueSelector(att));
                }
            }
            return values;
        }
    }
}
