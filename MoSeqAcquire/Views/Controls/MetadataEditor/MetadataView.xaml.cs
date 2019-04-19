using MoSeqAcquire.Views.Controls.MetadataEditor;
using MoSeqAcquire.Views.Controls.PropertyInspector;
using System;
using System.Windows;
using System.Windows.Controls;
using MoSeqAcquire.ViewModels.Metadata;

namespace MoSeqAcquire.Views.Controls
{
    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class MetadataView : UserControl
    {

        public MetadataView()
        {
            this.InnerViewModel = new MetadataViewModel();
            InitializeComponent();
        }
        public MetadataViewModel InnerViewModel { get; protected set; }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(MetadataView), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourceChangedCallBack)));
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MetadataView).InnerViewModel.Items = new MetadataCollection();
        }



        public static readonly DependencyProperty IsTemplateEditableProperty = DependencyProperty.Register("IsTemplateEditable", typeof(bool), typeof(MetadataView), new PropertyMetadata(true, new PropertyChangedCallback(isTemplateEditableChangedCallback)));
        private static void isTemplateEditableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MetadataView).InnerViewModel.IsTemplateEditable = (bool)e.NewValue;
        }
        public bool IsTemplateEditable
        {
            get { return (bool)GetValue(IsTemplateEditableProperty); }
            set { SetValue(IsTemplateEditableProperty, value); }
        }
        
    }

    
}
