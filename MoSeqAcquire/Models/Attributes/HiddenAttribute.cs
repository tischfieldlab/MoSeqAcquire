using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property, AllowMultiple = true)]
    public class HiddenAttribute : Attribute
    {
        /// <summary>
        /// String field.
        /// </summary>
        bool isHidden;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public HiddenAttribute(bool IsHidden=true)
        {
            this.isHidden = IsHidden;
        }

        /// <summary>
        /// Get and set.
        /// </summary>
        public bool IsHidden
        {
            get { return this.isHidden; }
            set { this.isHidden = value; }
        }
    }
}
