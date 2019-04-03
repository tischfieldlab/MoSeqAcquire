using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public abstract class ComplexProperty 
        : IRangeInfo, IDefaultInfo, IAutomaticInfo, ISupportInfo, IChoicesProvider
    {
        private PropertyCapability capability;
        public Type ValueType
        {
            get => this.Value.GetType();
        }
        public abstract object Value { get; set; }
        public abstract bool IsAutomatic { get; set; }
        public abstract void ResetValue();

        protected abstract void PushCurrentValue();
        protected abstract void ReadCurrentValue();
        protected abstract PropertyCapability ReadCapability();


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

        public bool Validate(object Value)
        {
            if (this.HasRange)
            {
                if ((Value as IComparable).CompareTo(this.Min as IComparable) < 0
                 && (Value as IComparable).CompareTo(this.Max as IComparable) > 0)
                    return false;
            }
            else if (this.HasChoices)
            {
                if (!this.Choices.Contains(Value)) return false;
            }
            return true;
        }
        

        public bool IsSupported
        {
            get => this.Capability.IsSupported;
        }
        public bool AllowsAuto
        {
            get => this.Capability.AllowsAuto;
        }

        #region IRangeInfo Implementation
        public bool HasRange
        {
            get => (this.Capability is IRangeInfo);
        }
        public object Min
        {
            get => (this.Capability as IRangeInfo).Min;
        }
        public object Max
        {
            get => (this.Capability as IRangeInfo).Max;
        }
        public object Step
        {
            get => (this.Capability as IRangeInfo).Step;
        }
        #endregion

        #region IChoicesProvider Implementation
        public bool HasChoices
        {
            get => (this.Capability is IChoicesProvider);
        }
        public IEnumerable<object> Choices
        {
            get => (this.Capability as IChoicesProvider).Choices;
        }
        public string DisplayPath
        {
            get => (this.Capability as IChoicesProvider).DisplayPath;
        }
        public string ValuePath
        {
            get => (this.Capability as IChoicesProvider).ValuePath;
        }
        #endregion

    }
}
