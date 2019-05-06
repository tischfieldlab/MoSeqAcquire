using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeMethodAttribute : Attribute
    {
        readonly string methodName;

        public RangeMethodAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public string MethodName
        {
            get => this.methodName;
        }
    }
}
