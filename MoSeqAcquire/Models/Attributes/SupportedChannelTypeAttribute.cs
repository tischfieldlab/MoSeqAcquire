using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedChannelTypeAttribute : Attribute
    {
        MediaType mediaType;
        int maxCount;

        public SupportedChannelTypeAttribute(MediaType mediaType) : this(mediaType, -1) { }
        public SupportedChannelTypeAttribute(MediaType mediaType, int maxCount)
        {
            this.mediaType = mediaType;
            this.maxCount = maxCount;
        }

        public MediaType MediaType
        {
            get => this.mediaType;
        }
        public int MaxCount
        {
            get => this.maxCount;
        }
        public bool HasLimit
        {
            get => this.maxCount > 0;
        }
    }
}
