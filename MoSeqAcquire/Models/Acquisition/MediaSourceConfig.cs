using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Annotations;
using System.Linq;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSourceConfig : BaseConfiguration, IPropertyCapabilityProvider
    {
        protected Dictionary<string, ComplexProperty> backingProperties;

        public MediaSourceConfig() /*: base() */ //DO NOT CALL BASE CONSTRUCTOR !!
        {
            this.backingProperties = new Dictionary<string, ComplexProperty>();
        }
        public ComplexProperty GetComplexProperty(string PropertyName)
        {
            if (this.IsPropertyComplex(PropertyName))
            {
                return this.backingProperties[PropertyName];
            }
            return null;
        }
        public bool IsPropertyComplex(string PropertyName)
        {
            return this.backingProperties != null 
                && this.backingProperties.ContainsKey(PropertyName)
                && this.backingProperties[PropertyName] != null;
        }
        public void RegisterComplexProperty(string PropertyName, ComplexProperty BackingProperty)
        {
            this.backingProperties[PropertyName] = BackingProperty;
        }
        protected bool SetField<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (!this.IsPropertyComplex(propertyName))
            {
                return false;
                throw new ArgumentException(propertyName + " is not backed by a complex property!");
            }
            var bp = this.GetComplexProperty(propertyName);

            if (EqualityComparer<T>.Default.Equals((T)bp.Value, value))
                return false;

            if (!bp.Validate(value))
                return false;

            bp.Value = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        protected T GetField<T>([CallerMemberName] string propertyName = null)
        {
            if (!this.IsPropertyComplex(propertyName))
            {
                throw new ArgumentException(propertyName + " is not backed by a complex property!");
            }
            var bp = this.GetComplexProperty(propertyName);
            return (T)bp.Value;
        }
    }
}
