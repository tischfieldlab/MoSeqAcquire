using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.PropertyManagement
{
    public class PropertyCollection : ObservableCollection<PropertyItem>
    {
        protected object sourceObject;

        public PropertyCollection(object data)
        {
            this.sourceObject = data;
            this.Initialize();  
        }
        public object SourceObject
        {
            get => this.sourceObject;
        }
        public new void Add(PropertyItem Item)
        {
            base.Add(Item);
        }
        public new void Remove(PropertyItem Item)
        {
            base.Remove(Item);
        }

        protected void Initialize()
        {
            this.sourceObject
                .GetType()
                .GetProperties()
                .Select<PropertyInfo, PropertyItem>((p) => {
                    if (typeof(ComplexProperty).IsAssignableFrom(p.PropertyType)) {
                        return new ComplexPropertyItem(this.sourceObject, p.Name);
                    }
                    return new SimplePropertyItem(this.sourceObject, p.Name);
                })
                .ForEach(pi => this.Add(pi));
        }
    }
}
