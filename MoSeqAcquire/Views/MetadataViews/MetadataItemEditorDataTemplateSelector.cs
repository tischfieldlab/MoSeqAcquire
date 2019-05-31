using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.Views.Metadata
{
    public class MetadataItemEditorDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MetadataItemDefinition)
            {
                FrameworkElement elemnt = container as FrameworkElement;
                MetadataItemDefinition pi = item as MetadataItemDefinition;
                Type pit = pi.ValueType;

                void lambda(object o, PropertyChangedEventArgs args)
                {
                    pi.PropertyChanged -= lambda;
                    var cp = (ContentPresenter)container;
                    cp.ContentTemplateSelector = null;
                    cp.ContentTemplateSelector = this;
                }
                pi.PropertyChanged += lambda;


                if (pit.Equals(typeof(bool)))
                {
                    return elemnt.FindResource(elemnt.Name+"CheckboxEditor") as DataTemplate;
                }
                else if (pi.GetValidator<ChoicesRule>() is ChoicesRule cr && cr.IsActive)
                {
                    return elemnt.FindResource(elemnt.Name + "CollectionComboBoxEditor") as DataTemplate;
                }
                else if (pit.Equals(typeof(int)) || pit.Equals(typeof(float)) || pit.Equals(typeof(double)))
                {
                    if (pi.GetValidator<RangeRule>() is RangeRule rr && rr.IsActive)
                    {
                        return elemnt.FindResource(elemnt.Name + "RangeEditor") as DataTemplate;
                    }
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
