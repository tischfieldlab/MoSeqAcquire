using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutomaticPropertyAttribute : Attribute
    {
        string propertyName;
        
        public AutomaticPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName
        {
            get => this.propertyName;
        }
    }
}
