using MoSeqAcquire.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public object DefaultValue
        {
            get
            {
                DefaultValueAttribute attr = this.propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    return attr.Value;
                }
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IRangeInfo).Default;
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


        #region Choices
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
        #endregion

        #region Range
        public bool SupportsRange
        {
            get
            {
                RangeAttribute ra = this.propertyInfo.GetCustomAttribute<RangeAttribute>();
                if (ra != null)
                {
                    return true;
                }
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return true;
                }
                return false;
            }
        }
        public object MinValue
        {
            get
            {
                RangeAttribute ra = this.propertyInfo.GetCustomAttribute<RangeAttribute>();
                if (ra != null)
                {
                    return ra.Minimum;
                }
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IRangeInfo).Min;
                }
                return null;
            }
        }
        public object MaxValue
        {
            get
            {
                RangeAttribute ra = this.propertyInfo.GetCustomAttribute<RangeAttribute>();
                if (ra != null)
                {
                    return ra.Maximum;
                }
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IRangeInfo).Max;
                }
                return null;
            }
        }
        public object StepValue
        {
            get
            {
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IRangeInfo).Step;
                }
                return null;
            }
        }
        #endregion

        #region Automatic
        public bool SupportsAutomatic
        {
            get
            {
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IRangeInfo).AllowsAuto;
                }
                return false;
            }
        }
        public bool IsAutomatic
        {
            get
            {
               
                return false;
            }
        }
        #endregion
    }
}
