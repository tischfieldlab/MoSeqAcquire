using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public abstract class ComplexProperty : IRangeInfo, IDefaultInfo, IAutomaticInfo, ISupportInfo
    {
        private PropertyCapability capability;

        public PropertyCapability Capability
        {
            get
            {
                if (this.capability == null)
                {
                    this.capability = this.ReadCapability();
                }
                return this.capability;
            }
        }
        public object Default
        {
            get => (int)this.Capability.Default;
        }
        public object Min
        {
            get => (int)this.Capability.Min;
        }
        public object Max
        {
            get => (int)this.Capability.Max;
        }
        public object Step
        {
            get => (int)this.Capability.Step;
        }
        public bool IsSupported
        {
            get => this.Capability.IsSupported;
        }
        public bool AllowsAuto
        {
            get => this.Capability.AllowsAuto;
        }


        public abstract object Value{ get; set; }
        public abstract bool IsAutomatic { get; set; }
        public abstract void ResetValue();

        protected abstract void PushCurrentValue();
        protected abstract void ReadCurrentValue();
        protected abstract PropertyCapability ReadCapability();
    }


    public class DelegateComplexProperty : ComplexProperty
    {
        private Action push;
        private Action pull;
        private Func<PropertyCapability> query;
    
        public DelegateComplexProperty(Action push, Action pull, Func<PropertyCapability> query)
        {
            this.push = push;
            this.pull = pull;
            this.query = query;
        }

        public override object Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool IsAutomatic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void ResetValue()
        {
            throw new NotImplementedException();
        }

        protected override void PushCurrentValue()
        {
            throw new NotImplementedException();
        }

        protected override PropertyCapability ReadCapability()
        {
            return this.query.Invoke();
        }

        protected override void ReadCurrentValue()
        {
            throw new NotImplementedException();
        }
    }
}
