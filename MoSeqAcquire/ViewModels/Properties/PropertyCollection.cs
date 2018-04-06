using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Properties
{
    public class PropertyCollection : ObservableCollection<PropertyItem>
    {
        protected object sourceObject;

        public PropertyCollection(object data)
        {
            this.sourceObject = data;
            this.Initialize();
            
        }
        public new void Add(PropertyItem Item)
        {
            Item.PropertyChanged += this.Item_PropertyChanged;
            base.Add(Item);
        }
        public new void Remove(PropertyItem Item)
        {
            Item.PropertyChanged -= this.Item_PropertyChanged;
            base.Remove(Item);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.sourceObject
                .GetType()
                .GetProperty(e.PropertyName)
                .SetValue(this.sourceObject, (sender as PropertyItem).Value);
        }

        protected void Initialize()
        {
            this.sourceObject
                .GetType()
                .GetProperties()
                .Select(p => new PropertyItem(p.Name, p.GetValue(this.sourceObject)))
                .ForEach(pi => this.Add(pi));
        }
    }

    public class PropertyItem : BaseViewModel
    {
        protected string propertyName;
        protected object value;

        public PropertyItem(string PropertyName, object Value)
        {
            this.propertyName = PropertyName;
            this.value = Value;
        }
        public string PropertyName { get => this.propertyName; }
        public string DisplayName { get; set; }
        public object Value
        {
            get => this.value;
            set => this.SetField(ref this.value, value);
        }
    }
}
