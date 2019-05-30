using System;
using System.ComponentModel;
using System.Data.Metadata.Edm;
using System.Windows;
using System.Windows.Controls;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.Views.Metadata
{
    public class ChoicesConstraintEditorDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ChoicesChoice)
            {
                FrameworkElement elemnt = container as FrameworkElement;
                ChoicesChoice ccc = item as ChoicesChoice;
                MetadataItemDefinition pi = ccc.Owner; 
                Type pit = pi.ValueType;

                void lambda(object o, PropertyChangedEventArgs args)
                {
                    if (args.PropertyName == nameof(pi.ValueType))
                    {
                        pi.PropertyChanged -= lambda;
                        var cp = (ContentPresenter)container;
                        cp.ContentTemplateSelector = null;
                        cp.ContentTemplateSelector = this;
                    }
                }

                pi.PropertyChanged += lambda;


                if (pit.Equals(typeof(bool)))
                {
                    return elemnt.FindResource(elemnt.Name+"CheckboxEditor") as DataTemplate;
                }
                else if (pit.Equals(typeof(int)) || pit.Equals(typeof(float)) || pit.Equals(typeof(double)))
                {
                    return elemnt.FindResource(elemnt.Name + "NumericEditor") as DataTemplate;
                }
                else if (pit.Equals(typeof(DateTime)))
                {
                    return elemnt.FindResource(elemnt.Name + "DateTimeEditor") as DataTemplate;
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
