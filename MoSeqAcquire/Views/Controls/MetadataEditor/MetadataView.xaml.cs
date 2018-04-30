using MoSeqAcquire.Views.Controls.MetadataEditor;
using MoSeqAcquire.Views.Controls.PropertyInspector;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls
{
    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class MetadataView : UserControl
    {
        protected MetadataViewModel innerViewModel;

        public MetadataView()
        {
            this.InnerViewModel = new MetadataViewModel();
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(MetadataView), new PropertyMetadata(null, new PropertyChangedCallback(selectedObjectChangedCallBack)));
        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }
        private static void selectedObjectChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PropertyView).ItemsGrid.ItemsSource = new PropertyCollection(e.NewValue);
        }

        public static readonly DependencyProperty IsTemplateEditableProperty = DependencyProperty.Register("IsTemplateEditable", typeof(bool), typeof(MetadataView), new PropertyMetadata(true));
        public bool IsTemplateEditable
        {
            get { return (bool)GetValue(IsTemplateEditableProperty); }
            set { SetValue(IsTemplateEditableProperty, value); }
        }
        public MetadataViewModel InnerViewModel
        {
            get; set;
        }

        private void add_item(object sender, RoutedEventArgs e)
        {
            this.InnerViewModel.Items.Add(new MetadataItem("New Item", typeof(string)) { Value = "Some Value" });
        }

        private void editItem(object sender, RoutedEventArgs e)
        {
            
        }
    }

    
}
