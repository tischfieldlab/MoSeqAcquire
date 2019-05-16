using MoSeqAcquire.Views.Controls.PropertyInspector;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls
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
            var propView = d as PropertyView;
            propView.InnerContainer.DataContext = new PropertyCollection(e.NewValue);

            foreach (var col in (propView.ItemsGrid.View as GridView).Columns)
            {
                if (double.IsNaN(col.Width)) col.Width = col.ActualWidth;
                col.Width = double.NaN;
            }
        }

    }

    
}
