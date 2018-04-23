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
        string methodName;

        public RangeMethodAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public string MethodName
        {
            get => this.methodName;
        }
    }

    public interface IRangeInfo
    {
        object Min { get; }
        object Max { get; }
        object Step { get; }
        object Default { get; }
        //true => auto; false =>manual; null => none;
        bool AllowsAuto { get; }
        bool IsSupported { get; }
    }
}
