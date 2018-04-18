using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChoicesMethodAttribute : Attribute
    {
        string methodName;
        
        public ChoicesMethodAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public string MethodName
        {
            get => this.methodName;
        }
        public string DisplayPath { get; set; }
        public string ValuePath { get; set; }
    }
}
