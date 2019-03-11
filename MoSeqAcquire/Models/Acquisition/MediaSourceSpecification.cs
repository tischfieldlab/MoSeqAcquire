using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Acquisition
{
    public class MediaSourceSpecification
    {
        public MediaSourceSpecification(Type Type)
        {
            this.SourceType = Type;
        }
        public Type SourceType { get; protected set; }
        public string TypeName { get => this.SourceType.AssemblyQualifiedName; }
        public string DisplayName
        {
            get
            {
                var n = (DisplayNameAttribute)Attribute.GetCustomAttribute(this.SourceType, typeof(DisplayNameAttribute));
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
                var n = (SettingsImplementationAttribute)Attribute.GetCustomAttribute(this.SourceType, typeof(SettingsImplementationAttribute));
                if (n != null)
                {
                    return n.SettingsImplementation;
                }
                return typeof(MediaSourceConfig);
            }
        }
        public List<Type> KnownTypes
        {
            get
            {
                var types = new List<Type>();
                Attribute[] attrs = Attribute.GetCustomAttributes(this.SourceType, typeof(KnownTypeAttribute));
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
                types.AddRange(this.SettingsFactory(this.Factory()).GetKnownTypes());
                return types;
            }
        }
        public MediaSourceConfig SettingsFactory(MediaSource Owner)
        {
            var sit = this.SourceType.GetCustomAttribute<SettingsImplementationAttribute>();
            return Activator.CreateInstance(sit.SettingsImplementation, Owner) as MediaSourceConfig;
        }
        public MediaSource Factory()
        {
            return (MediaSource)Activator.CreateInstance(this.SourceType);
        }
    }
}
