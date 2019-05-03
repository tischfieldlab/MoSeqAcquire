using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SettingsImplementationAttribute : Attribute
    {
        /// <summary>
        /// String field.
        /// </summary>
        Type _knownType;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public SettingsImplementationAttribute(Type KnownType)
        {
            this._knownType = KnownType;
        }

        /// <summary>
        /// Get and set.
        /// </summary>
        public Type SettingsImplementation
        {
            get { return this._knownType; }
            set { this._knownType = value; }
        }
    }
}
