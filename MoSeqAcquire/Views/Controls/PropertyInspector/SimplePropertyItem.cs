using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MoSeqAcquire.Views.Controls.PropertyInspector
{
    class SimplePropertyItem : PropertyItem
    {

        public SimplePropertyItem(object SourceObject, string PropertyName) : base(SourceObject, PropertyName)
        {

        }
        public override Type ValueType { get => this.propertyInfo.PropertyType; }

        public override object Value
        {
            get => this.propertyInfo.GetValue(this.sourceObject);
            set
            {
                this.propertyInfo.SetValue(this.sourceObject, Convert.ChangeType(value, this.ValueType));
                this.NotifyPropertyChanged("Value");
            }
        }
        public override object DefaultValue
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
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IDefaultInfo).Default;
                }
                return null;
            }
        }
        public override bool IsEnabled => true;


        #region Choices
        public override bool SupportsChoices
        {
            get
            {
                if (typeof(Enum).IsAssignableFrom(this.ValueType))
                {
                    return true;
                }
                ChoicesMethodAttribute cma = this.propertyInfo.GetCustomAttribute<ChoicesMethodAttribute>();
                if (cma != null)
                {
                    return true;
                }
                return false;
            }
        }
        public override IEnumerable<object> Choices
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
        public override string ChoicesDisplayPath
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
        public override string ChoicesValuePath
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
        public override bool SupportsRange
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
        public override object MinValue
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
        public override object MaxValue
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
        public override object StepValue
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
        public override bool SupportsAutomatic
        {
            get
            {
                RangeMethodAttribute rma = this.propertyInfo.GetCustomAttribute<RangeMethodAttribute>();
                if (rma != null)
                {
                    return (this.sourceObject.GetType().GetMethod(rma.MethodName).Invoke(this.sourceObject, null) as IAutomaticInfo).AllowsAuto;
                }
                return false;
            }
        }
        public override bool IsAutomatic
        {
            get
            {
                AutomaticPropertyAttribute apa = this.propertyInfo.GetCustomAttribute<AutomaticPropertyAttribute>();
                if (apa != null)
                {
                    return (bool)this.sourceObject.GetType().GetProperty(apa.PropertyName).GetValue(this.sourceObject);
                }
                return false;
            }
            set
            {
                AutomaticPropertyAttribute apa = this.propertyInfo.GetCustomAttribute<AutomaticPropertyAttribute>();
                if (apa != null)
                {
                    this.sourceObject.GetType().GetProperty(apa.PropertyName).SetValue(this.sourceObject, value);
                }
            }
        }
        #endregion
    }
}
