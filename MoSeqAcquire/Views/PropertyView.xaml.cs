using MoSeqAcquire.ViewModels.PropertyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class PropertyView : UserControl
    {
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyView), new PropertyMetadata(null, new PropertyChangedCallback(selectedObjectChangedCallBack)));

        

        public PropertyView()
        {
            InitializeComponent();
        }
        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }
        private static void selectedObjectChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }

    public class PropertyItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            PropertyItem pi = item as PropertyItem;
            Type pit = pi.Value.GetType();
            if (pit.Equals(typeof(bool)))
            {
                return elemnt.FindResource("CheckboxEditor") as DataTemplate;
            }
            else if (pit.IsEnum)
            {
                return elemnt.FindResource("ComboBoxEditor") as DataTemplate;
            }
            else
            {
                return elemnt.FindResource("TextBoxEditor") as DataTemplate;
            }
        }
    }
}
