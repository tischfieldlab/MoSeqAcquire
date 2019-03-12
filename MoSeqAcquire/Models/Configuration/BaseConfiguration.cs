using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Configuration
{
    public abstract class BaseConfiguration : ObservableObject, IConfigSnapshotProvider
    {
        protected IEnumerable<PropertyInfo> GetConfigurationProperties()
        {
            return this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.CanWrite || typeof(ComplexProperty).IsAssignableFrom(pi.PropertyType));
        }
        public List<Type> GetKnownTypes()
        {
            return this.GetConfigurationProperties()
                .Select((pi) =>
                {
                    if (typeof(ComplexProperty).IsAssignableFrom(pi.PropertyType))
                    {
                        ComplexProperty prop = (ComplexProperty)pi.GetValue(this);
                        return prop.ValueType;
                    }
                    else
                    {
                        return pi.PropertyType;
                    }
                }).Distinct().ToList();
        }
        public void ApplyDefaults()
        {
            this.GetConfigurationProperties()
                .ForEach((pi) =>
                {
                    if (typeof(ComplexProperty).IsAssignableFrom(pi.PropertyType))
                    {
                        ComplexProperty prop = (ComplexProperty)pi.GetValue(this);
                        prop.Value = prop.Default;
                    }
                    else
                    {
                        DefaultValueAttribute attr = pi.GetCustomAttribute<DefaultValueAttribute>();
                        if (attr != null)
                        {
                            pi.SetValue(this, attr.Value);
                            return;
                        }
                        RangeMethodAttribute rma = pi.GetCustomAttribute<RangeMethodAttribute>();
                        if (rma != null)
                        {
                            pi.SetValue(this, (this.GetType().GetMethod(rma.MethodName).Invoke(this, null) as IDefaultInfo).Default);
                            return;
                        }
                        if (pi.PropertyType.IsValueType)
                        {
                            pi.SetValue(this, Activator.CreateInstance(pi.PropertyType));
                            return;
                        }
                    }
                });
        }
        public ConfigSnapshot GetSnapshot()
        {
            var state = new ConfigSnapshot();
            this.GetConfigurationProperties()
                .ForEach((pi) => {
                    ConfigSnapshotSetting setting;
                    if (typeof(ComplexProperty).IsAssignableFrom(pi.PropertyType))
                    {
                        ComplexProperty prop = (ComplexProperty)pi.GetValue(this);
                        setting = new ConfigSnapshotSetting()
                        {
                            Name = pi.Name,
                            Value = prop.Value,
                            ValueType = prop.ValueType,
                            Automatic = prop.AllowsAuto ? (bool?)prop.IsAutomatic : null
                        };
                    }
                    else
                    {
                        setting = new ConfigSnapshotSetting()
                        {
                            Name = pi.Name,
                            Value = pi.GetValue(this),
                            ValueType = pi.PropertyType,
                            Automatic = null
                        };
                    }
                    state.Add(setting.Name, setting);
                });
            return state;
        }
        public void ApplySnapshot(ConfigSnapshot snapshot)
        {
            if (snapshot == null) { return; }

            foreach (string key in snapshot.Keys)
            {
                var prop = this.GetType().GetProperty(key);
                if (prop != null)
                {
                    if (typeof(ComplexProperty).IsAssignableFrom(prop.PropertyType))
                    {
                        ComplexProperty complexProp = (ComplexProperty)prop.GetValue(this);
                        complexProp.Value = snapshot[key].Value;
                        if (snapshot[key].Automatic.HasValue)
                        {
                            complexProp.IsAutomatic = snapshot[key].Automatic.Value;
                        }
                    }
                    else
                    {
                        prop.SetValue(this, snapshot[key].Value);
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Configuration has no property " + key);
                }
            }
            this.NotifyPropertyChanged(null);
        }
    }
}
