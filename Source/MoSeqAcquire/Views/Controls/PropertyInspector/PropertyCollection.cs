using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MoSeqAcquire.Views.Controls.PropertyInspector
{
    class PropertyCollection : ObservableCollection<PropertyItem>
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
            if(this.sourceObject == null)
            {
                return;
            }
            this.sourceObject
                .GetType()
                .GetProperties()
                .Select<PropertyInfo, PropertyItem>((p) => {
                    if (typeof(IPropertyCapabilityProvider).IsAssignableFrom(this.sourceObject.GetType())
                    && (this.sourceObject as IPropertyCapabilityProvider).IsPropertyComplex(p.Name)) {

                        return new ComplexPropertyItem(this.sourceObject, p.Name, (this.sourceObject as IPropertyCapabilityProvider).GetComplexProperty(p.Name));
                    }
                    return new SimplePropertyItem(this.sourceObject, p.Name);
                })
                .ForEach(pi => this.Add(pi));
        }
    }
}
