using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Annotations;
using System.Linq;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSourceConfig : BaseConfiguration
    {
        protected Dictionary<string, ComplexProperty> backingProperties;
        //protected Dictionary<string, Action<object>> pushers;
        //protected Dictionary<string, Func<object>> pullers;
        //protected Dictionary<string, Func<Tuple<IComparable, IComparable>>> ranges;

        public MediaSourceConfig()
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
            return this.backingProperties.ContainsKey(PropertyName)
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
        /*public void Register<T>(string PropertyName, Action<T> push, Func<T> pull)
        {
            this.RegisterPull(PropertyName, pull);
            this.RegisterPush(PropertyName, push);
        }
        public void RegisterPush<T>(string PropertyName, Action<T> action)
        {
            this.pushers[PropertyName] = action as Action<object>;
        }
        public void RegisterPull<T>(string PropertyName, Func<T> action)
        {
            this.pullers[PropertyName] = action as Func<object>;
        }
        public void RegisterRange<T>(string PropertyName, Func<Tuple<IComparable, IComparable>> action) where T : IComparable
        {
            this.ranges[PropertyName] = action;
        }*/
        protected bool CheckRange(object value, object min, object max)
        {
            if ((value as IComparable).CompareTo(min as IComparable) >= 0 
             && (value as IComparable).CompareTo(max as IComparable) <= 0)
            {
                return true;
            }
            return false;
        }

        public abstract void ReadState();
    }
}
