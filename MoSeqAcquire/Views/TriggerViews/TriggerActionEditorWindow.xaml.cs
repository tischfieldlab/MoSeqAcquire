using System;
using System.Windows;
using MoSeqAcquire.ViewModels.Triggers;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for TriggerActionEditorWindow.xaml
    /// </summary>
    public partial class TriggerActionEditorWindow : Window
    {
        public TriggerActionEditorWindow()
        {
            InitializeComponent();
            this.DataContextChanged += TriggerEditor_DataContextChanged;
        }

        private void TriggerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
            {
                (e.OldValue as TriggerActionEditorViewModel).CancelRequested -= Vm_CancelRequested;
                (e.OldValue as TriggerActionEditorViewModel).Completed -= Vm_Completed;
            }
            if(e.NewValue != null)
            {
                (e.NewValue as TriggerActionEditorViewModel).CancelRequested += Vm_CancelRequested;
                (e.NewValue as TriggerActionEditorViewModel).Completed += Vm_Completed;
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
