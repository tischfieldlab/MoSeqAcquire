using System;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.PropertyInspector
{
    public class PropertyItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            PropertyItem pi = item as PropertyItem;
            Type pit = pi.ValueType;
            if (pit.Equals(typeof(bool)))
            {
                return elemnt.FindResource("CheckboxEditor") as DataTemplate;
            }
            else if (pit.IsEnum)
            {
                return elemnt.FindResource("EnumComboBoxEditor") as DataTemplate;
            }
            else if (pi.SupportsChoices)
            {
                return elemnt.FindResource("CollectionComboBoxEditor") as DataTemplate;
            }
            else if (pit.Equals(typeof(int)) || pit.Equals(typeof(float)) || pit.Equals(typeof(double)))
            {
                if (pi.SupportsRange)
                {
                    return elemnt.FindResource("RangeEditor") as DataTemplate;
                }
                return elemnt.FindResource("NumericEditor") as DataTemplate;
            }
            /*else if (pit.Equals(typeof(int)))
            {
                if (pi.SupportsRange)
                {
                    return elemnt.FindResource("RangeEditor") as DataTemplate;
                }
                return elemnt.FindResource("NumericEditor") as DataTemplate;
            }*/

            else
            {
                return elemnt.FindResource("TextBoxEditor") as DataTemplate;
            }
        }
    }
}
