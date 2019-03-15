using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    class EnumKinectPropertyItem : KinectPropertyItem
    {
        public EnumKinectPropertyItem(object Source, string valueProperty)
        {
            this.source = Source;
            this.valueProperty = valueProperty;
        }
        public override bool IsAutomatic
        {
            get => false;
            set { return; }
        }

        protected override PropertyCapability ReadCapability()
        {
            var capability = new ChoicesPropertyCapability()
            {
                IsSupported = true,
                Choices = Enum.GetValues(this.ValueType) as IEnumerable<object>
            };
            return capability;
        }
    }
}
