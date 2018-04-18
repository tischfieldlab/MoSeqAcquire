using MoSeqAcquire.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.PropertyManagement
{
    public class PropertyItem : BaseViewModel
    {
        protected string propertyName;
        protected object sourceObject;
        protected PropertyInfo propertyInfo;

        public PropertyItem(object SourceObject, string PropertyName)
        {
            this.propertyName = PropertyName;
            this.sourceObject = SourceObject;
            this.propertyInfo = this.sourceObject.GetType().GetProperty(this.PropertyName);
        }
        public Type ValueType { get => this.propertyInfo.PropertyType; }
        public string PropertyName { get => this.propertyName; }

        public object Value
        {
            get => this.propertyInfo.GetValue(this.sourceObject);
            set
            {
                this.propertyInfo.SetValue(this.sourceObject, Convert.ChangeType(value, this.ValueType));
                this.NotifyPropertyChanged("Value");
            }
        }
        public IEnumerable<object> Choices
        {
            get
            {
                if (typeof(Enum).IsAssignableFrom(this.ValueType))
                {
                    return Enum.GetValues(this.ValueType) as IEnumerable<object>;
                }
                ChoicesMethodAttribute cma = this.propertyInfo.GetCustomAttribute<ChoicesMethodAttribute>();
                if (cma != null)
                {
                    return this.sourceObject.GetType().GetMethod(cma.MethodName).Invoke(this.sourceObject, null) as IEnumerable<object>;
                }
                return null;
            }
        }
        public string ChoicesDisplayPath
        {
            get
            {
                ChoicesMethodAttribute attr = this.propertyInfo.GetCustomAttribute<ChoicesMethodAttribute>();
                if (attr != null)
                {
                    return attr.DisplayPath;
                }
                return null;
            }
        }
        public string ChoicesValuePath
        {
            get
            {
                ChoicesMethodAttribute attr = this.propertyInfo.GetCustomAttribute<ChoicesMethodAttribute>();
                if (attr != null)
                {
                    return attr.ValuePath;
                }
                return null;
            }
        }
        public string DisplayName
        {
            get
            {
                DisplayNameAttribute attr = this.propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                if (attr != null)
                {
                    return attr.DisplayName;
                }
                return this.propertyName;
            }
        }
        public string Category
        {
            get
            {
                CategoryAttribute attr = this.propertyInfo.GetCustomAttribute<CategoryAttribute>();
                if (attr != null)
                {
                    return attr.Category;
                }
                return null;
            }
        }
    }
}
