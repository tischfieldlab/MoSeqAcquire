using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MoSeqAcquire.Views.Extensions
{
    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;
        private BindingBase _binding;

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(Type), typeof(EnumerationExtension));
    

        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            EnumType = enumType;
        }
        public EnumerationExtension(BindingBase binding)
        {
            this._binding = binding;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType
        {
            get { return _enumType; }
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_binding != null)
            {
                var pvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                var target = pvt.TargetObject as DependencyObject;

                // if we are inside a template, WPF will call us again when it is applied
                if (target == null)
                    return this;

                BindingOperations.SetBinding(target, ValueProperty, _binding);
                EnumType = (Type)target.GetValue(ValueProperty);
                BindingOperations.ClearBinding(target, ValueProperty);
            }
            var enumValues = Enum.GetValues(EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember
              {
                  Value = enumValue,
                  Description = this.GetDisplayName(enumValue),
                  Help = this.GetHelpText(enumValue)
              }).ToArray();
        }


        private string GetDisplayName(object enumValue)
        {
            var attribute = this.GetAttribute<EnumDisplayNameAttribute>(enumValue);
            return (attribute != null) ? attribute.Name : this.GetDescription(enumValue);
        }
        private string GetDescription(object enumValue)
        {
            var attribute = this.GetAttribute<DescriptionAttribute>(enumValue);
            return (attribute != null) ? attribute.Description : enumValue.ToString();
        }
        private string GetHelpText(object enumValue)
        {
            var attribute = this.GetAttribute<EnumHelpTextAttribute>(enumValue);
            return (attribute != null) ? attribute.HelpText : "";
        }
        private TAttribute GetAttribute<TAttribute>(object enumValue) where TAttribute : Attribute
        {
            return EnumType.GetField(enumValue.ToString()).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
        }

        public class EnumerationMember
        {
            public object Value { get; set; }
            public string Description { get; set; }
            public string Help { get; set; }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumHelpTextAttribute : Attribute
    {
        protected String helptext;

        public EnumHelpTextAttribute(String HelpText)
        {
            this.helptext = HelpText;
        }
        public String HelpText
        {
            get
            {
                return this.helptext;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumDisplayNameAttribute : Attribute
    {
        protected String name;

        public EnumDisplayNameAttribute(String Name)
        {
            this.name = Name;
        }
        public String Name
        {
            get
            {
                return this.name;
            }
        }
    }

    public abstract class EnumFedCombobox<TEnum, TConverter> : ComboBox where TConverter : IValueConverter, new()
    {

        public EnumFedCombobox() : base()
        {
            this.DisplayMemberPath = "Description";
            BindingOperations.SetBinding(this, ComboBox.ItemsSourceProperty, new Binding() { Source = new EnumerationExtension(typeof(TEnum)) });
            BindingOperations.SetBinding(this, ComboBox.SelectedValueProperty, new Binding(this.GetSelectedValuePath())
            {
                Mode = BindingMode.TwoWay,
                Converter = new TConverter()
            });
            BindingOperations.SetBinding(this, ComboBox.ToolTipProperty, new Binding("SelectedItem.Help") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
        }
        protected abstract string GetSelectedValuePath();
    }
}
