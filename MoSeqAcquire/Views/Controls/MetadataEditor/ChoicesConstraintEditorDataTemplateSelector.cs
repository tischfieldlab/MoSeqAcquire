using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public class ChoicesConstraintEditorDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ChoicesConstraintChoice)
            {
                FrameworkElement elemnt = container as FrameworkElement;
                ChoicesConstraintChoice ccc = item as ChoicesConstraintChoice;
                MetadataItem pi = ccc.Owner; 
                Type pit = pi.ValueType;

                PropertyChangedEventHandler lambda = null;
                lambda = (o, args) =>
                {
                    if (args.PropertyName == nameof(pi.ValueType))
                    {
                        pi.PropertyChanged -= lambda;
                        var cp = (ContentPresenter)container;
                        cp.ContentTemplateSelector = null;
                        cp.ContentTemplateSelector = this;
                    }
                };
                pi.PropertyChanged += lambda;


                if (pit.Equals(typeof(bool)))
                {
                    return elemnt.FindResource(elemnt.Name+"CheckboxEditor") as DataTemplate;
                }
                else if (pit.IsEnum)
                {
                    return elemnt.FindResource(elemnt.Name + "EnumComboBoxEditor") as DataTemplate;
                }
                else if (pit.Equals(typeof(int)) || pit.Equals(typeof(float)) || pit.Equals(typeof(double)))
                {
                    return elemnt.FindResource(elemnt.Name + "NumericEditor") as DataTemplate;
                }
                else
                {
                    return elemnt.FindResource(elemnt.Name + "TextBoxEditor") as DataTemplate;
                }
            }
            return null;
        }
    }
}
