using System;
using System.Windows;
using MoSeqAcquire.ViewModels.Triggers;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for RecorderEditor.xaml
    /// </summary>
    public partial class TriggerEditorWindow : Window
    {
        public TriggerEditorWindow()
        {
            InitializeComponent();
            this.DataContextChanged += TriggerEditor_DataContextChanged;
        }

        private void TriggerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
            {
                (e.OldValue as TriggerEditorViewModel).CancelRequested -= Vm_CancelRequested;
                (e.OldValue as TriggerEditorViewModel).Completed -= Vm_Completed;
            }
            if(e.NewValue != null)
            {
                (e.NewValue as TriggerEditorViewModel).CancelRequested += Vm_CancelRequested;
                (e.NewValue as TriggerEditorViewModel).Completed += Vm_Completed;
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
