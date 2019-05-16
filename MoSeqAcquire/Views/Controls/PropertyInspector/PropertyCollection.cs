using MoSeqAcquire.Models.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MoSeqAcquire.Models.Attributes;
using System.Windows.Data;
using System.ComponentModel;
using System;

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

        protected string filterString = string.Empty;
        public string FilterString
        {
            get => this.filterString;
            set {
                this.filterString = value;
                this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(this.FilterString)));
                this.View?.Refresh();
            }
        }
        public ICollectionView View
        {
            get; protected set;
        }
        protected void Initialize()
        {
            if(this.sourceObject == null)
            {
                return;
            }
            this.sourceObject
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => {
                    var ha = pi.GetCustomAttribute<HiddenAttribute>();
                    return (ha == null) || (ha != null && ha.IsHidden == false);
                })
                .Select<PropertyInfo, PropertyItem>((p) => {
                    if (typeof(IPropertyCapabilityProvider).IsAssignableFrom(this.sourceObject.GetType())
                    && (this.sourceObject as IPropertyCapabilityProvider).IsPropertyComplex(p.Name)) {

                        return new ComplexPropertyItem(this.sourceObject, p.Name, (this.sourceObject as IPropertyCapabilityProvider).GetComplexProperty(p.Name));
                    }
                    return new SimplePropertyItem(this.sourceObject, p.Name);
                })
                .ForEach(pi => this.Add(pi));

            this.View = CollectionViewSource.GetDefaultView(this);
            this.View.Filter = this.FilterItems;
        }
        protected virtual bool FilterItems(object item)
        {
            if (!(item is PropertyItem pi))
                return false;

            if (pi.PropertyName.IndexOf(this.filterString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (pi.DisplayName.IndexOf(this.filterString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
    }
}
