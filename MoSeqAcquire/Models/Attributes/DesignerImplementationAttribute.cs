using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DesignerImplementationAttribute : Attribute
    {
        /// <summary>
        /// String field.
        /// </summary>
        Type _designerImplementationType;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public DesignerImplementationAttribute(Type designerType)
        {
            this._designerImplementationType = designerType;
        }

        /// <summary>
        /// Get and set.
        /// </summary>
        public Type DesignerImplementation
        {
            get { return this._designerImplementationType; }
            set { this._designerImplementationType = value; }
        }
    }
}
