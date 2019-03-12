using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Annotations;

namespace MoSeqAcquire.Models.Acquisition
{
    public abstract class MediaSourceConfig : BaseConfiguration
    {
        protected Dictionary<string, Action<object>> pushers;
        protected Dictionary<string, Func<object>> pullers;
        protected Dictionary<string, Func<Tuple<IComparable, IComparable>>> ranges;

        public MediaSourceConfig()
        {
        }
        protected new bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            if (this.ranges.ContainsKey(propertyName))
            {
                var min_max = (Tuple<IComparable, IComparable>)this.ranges[propertyName].DynamicInvoke();
                if (!this.CheckRange((IComparable)value, min_max.Item1, min_max.Item2)) return false;
            }
            if (this.pushers.ContainsKey(propertyName))
            {
                //to do: check return value for success, else rollback
                this.pushers[propertyName].Invoke(value);
            }
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        public void Register<T>(string PropertyName, Action<T> push, Func<T> pull)
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
        public void RegisterRange<T>(string PropertyName, Func<Tuple<T,T>> action) where T : IComparable
        {
            this.ranges[PropertyName] = action;
        }
        protected bool CheckRange<T>(T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            {
                return true;
            }
            return false;
        }

        public abstract void ReadState();
    }
}
