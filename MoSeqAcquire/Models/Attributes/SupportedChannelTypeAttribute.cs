using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
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
        readonly MediaType mediaType;
        readonly ChannelCapacity capacity;

        public SupportedChannelTypeAttribute(MediaType mediaType) : this(mediaType, ChannelCapacity.Single) { }
        public SupportedChannelTypeAttribute(MediaType mediaType, ChannelCapacity Capacity)
        {
            this.mediaType = mediaType;
            this.capacity = Capacity;
        }

        public MediaType MediaType
        {
            get => this.mediaType;
        }
        public ChannelCapacity Capacity
        {
            get => this.capacity;
        }
        /*public bool HasCapacity
        {
            get => this.capacity > 0;
        }*/
    }
}
