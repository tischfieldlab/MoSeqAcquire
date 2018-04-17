using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Recording
{
    public class RecorderSpecification
    {
        public RecorderSpecification(Type Type)
        {
            this.RecorderType = Type;
        }
        public Type RecorderType { get; protected set; }
        public string TypeName { get => this.RecorderType.FullName; }
        public string DisplayName
        {
            get
            {
                var n = (DisplayNameAttribute)Attribute.GetCustomAttribute(this.RecorderType, typeof(DisplayNameAttribute));
                if (n != null)
                {
                    return n.DisplayName;
                }
                return String.Empty;
            }
        }
        public Type SettingsImplementation
        {
            get
            {
                var n = (SettingsImplementationAttribute)Attribute.GetCustomAttribute(this.RecorderType, typeof(SettingsImplementationAttribute));
                if (n != null)
                {
                    return n.SettingsImplementation;
                }
                return typeof(RecorderSettings);
            }
        }
        public List<Type> KnownTypes
        {
            get
            {
                var types = new List<Type>();
                Attribute[] attrs = Attribute.GetCustomAttributes(this.RecorderType, typeof(KnownTypeAttribute));
                foreach (Attribute attr in attrs)
                {
                    if (attr is KnownTypeAttribute)
                    {
                        KnownTypeAttribute a = (KnownTypeAttribute)attr;
                        if (!a.IsDefaultAttribute())
                        {
                            types.Add(a.KnownType);
                        }
                    }
                }
                return types;
            }
        }
        public List<SupportedChannelTypeAttribute> SupportedChannels
        {
            get
            {
                var sc = new List<SupportedChannelTypeAttribute>();
                Attribute[] attrs = Attribute.GetCustomAttributes(this.RecorderType, typeof(SupportedChannelTypeAttribute));
                foreach (Attribute attr in attrs)
                {
                    if (attr is SupportedChannelTypeAttribute)
                    {
                        SupportedChannelTypeAttribute a = (SupportedChannelTypeAttribute)attr;
                        if (!a.IsDefaultAttribute())
                        {
                            sc.Add(a);
                        }
                    }
                }
                return sc;
            }
        }
        public RecorderSettings SettingsFactory()
        {
            var sit = this.RecorderType.GetCustomAttribute<SettingsImplementationAttribute>();
            return Activator.CreateInstance(sit.SettingsImplementation) as RecorderSettings;
        }
        public MediaWriter Factory()
        {
            return (MediaWriter)Activator.CreateInstance(this.RecorderType);
        }
    }
}
