using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;
using System.Linq;

namespace MoSeqAcquire.Models.Core
{
    public class ComponentSpecification
    {
        public ComponentSpecification(Type Type)
        {
            this.ComponentType = Type;
        }
        public Type ComponentType { get; protected set; }
        public string TypeName { get => this.ComponentType.AssemblyQualifiedName; }
        public bool IsHidden
        {
            get
            {
                var a = (HiddenAttribute)Attribute.GetCustomAttribute(this.ComponentType, typeof(HiddenAttribute));
                if (a != null)
                {
                    return a.IsHidden;
                }
                return false;
            }
        }
        public string DisplayName
        {
            get
            {
                var n = (DisplayNameAttribute)Attribute.GetCustomAttribute(this.ComponentType, typeof(DisplayNameAttribute));
                if (n != null)
                {
                    return n.DisplayName;
                }
                return this.TypeName;
            }
        }
        public Type SettingsImplementation
        {
            get
            {
                var n = (SettingsImplementationAttribute)Attribute.GetCustomAttribute(this.ComponentType, typeof(SettingsImplementationAttribute));
                if (n != null)
                {
                    return n.SettingsImplementation;
                }
                throw new InvalidOperationException("No settings implementation is defined for " + this.TypeName);
            }
        }
        public List<Type> KnownTypes
        {
            get
            {
                var types = new List<Type>();

                this.GetAttributes<KnownTypeAttribute>()
                    .Where(a => a is KnownTypeAttribute && !a.IsDefaultAttribute())
                    .Select(a => a.KnownType)
                    .ForEach(t => types.Add(t));
                
                types.AddRange(this.SettingsFactory().GetKnownTypes());
                return types;
            }
        }
        
        public virtual BaseConfiguration SettingsFactory()
        {
            return Activator.CreateInstance(this.SettingsImplementation) as BaseConfiguration;
        }
        public virtual Component Factory()
        {
            return (Component)Activator.CreateInstance(this.ComponentType);
        }


        protected IEnumerable<T> GetAttributes<T>() where T : Attribute
        {
            return Attribute.GetCustomAttributes(this.ComponentType, typeof(T)).Cast<T>();
        }
        protected T GetAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(this.ComponentType, typeof(T));
        }
    }
}
