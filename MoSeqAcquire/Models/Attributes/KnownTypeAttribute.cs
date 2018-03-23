using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class KnownTypeAttribute : Attribute
    {
        /// <summary>
        /// String field.
        /// </summary>
        Type _knownType;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public KnownTypeAttribute(Type KnownType)
        {
            this._knownType = KnownType;
        }

        /// <summary>
        /// Get and set.
        /// </summary>
        public Type KnownType
        {
            get { return this._knownType; }
            set { this._knownType = value; }
        }
    }
}
