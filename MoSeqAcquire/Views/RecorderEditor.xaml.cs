using System;
using System.Windows;
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for RecorderEditor.xaml
    /// </summary>
    public partial class RecorderEditor : Window
    {
        public RecorderEditor()
        {
            InitializeComponent();
            this.DataContextChanged += RecorderEditor_DataContextChanged;
        }

        private void RecorderEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
            {
                (e.OldValue as RecorderEditorViewModel).CancelRequested -= Vm_CancelRequested;
                (e.OldValue as RecorderEditorViewModel).Completed -= Vm_Completed;
            }
            if(e.NewValue != null)
            {
                (e.NewValue as RecorderEditorViewModel).CancelRequested += Vm_CancelRequested;
                (e.NewValue as RecorderEditorViewModel).Completed += Vm_Completed;
            }
        }

        private void Vm_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Vm_CancelRequested(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
