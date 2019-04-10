using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;

namespace MoSeqAcquire.Models.Acquisition.KinectXboxOne
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