using System;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public class MetadataItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MetadataItem)
            {
                FrameworkElement elemnt = container as FrameworkElement;
                MetadataItem pi = item as MetadataItem;
                Type pit = pi.ValueType;
                if (pit.Equals(typeof(bool)))
                {
                    return elemnt.FindResource("CheckboxEditor") as DataTemplate;
                }
                else if (pit.IsEnum)
                {
                    return elemnt.FindResource("EnumComboBoxEditor") as DataTemplate;
                }
                else if (pi.Constraint == ConstraintMode.Choices)
                {
                    return elemnt.FindResource("CollectionComboBoxEditor") as DataTemplate;
                }
                else if (pit.Equals(typeof(int)) || pit.Equals(typeof(float)) || pit.Equals(typeof(double)))
                {
                    if (pi.Constraint == ConstraintMode.Range)
                    {
                        return elemnt.FindResource("RangeEditor") as DataTemplate;
                    }
                    return elemnt.FindResource("NumericEditor") as DataTemplate;
                }

                else
                {
                    return elemnt.FindResource("TextBoxEditor") as DataTemplate;
                }
            }
            return null;
        }
    }
}
