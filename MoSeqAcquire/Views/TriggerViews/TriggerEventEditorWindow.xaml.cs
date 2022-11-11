using System;
using System.Windows;
using MoSeqAcquire.ViewModels.Triggers;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for RecorderEditor.xaml
    /// </summary>
    public partial class TriggerEventEditorWindow : Window
    {
        public TriggerEventEditorWindow()
        {
            InitializeComponent();
            this.DataContextChanged += TriggerEventEditor_DataContextChanged;
        }

        private void TriggerEventEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
            {
                (e.OldValue as TriggerEventEditorViewModel).CancelRequested -= Vm_CancelRequested;
                (e.OldValue as TriggerEventEditorViewModel).Completed -= Vm_Completed;
            }
            if(e.NewValue != null)
            {
                (e.NewValue as TriggerEventEditorViewModel).CancelRequested += Vm_CancelRequested;
                (e.NewValue as TriggerEventEditorViewModel).Completed += Vm_Completed;
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
