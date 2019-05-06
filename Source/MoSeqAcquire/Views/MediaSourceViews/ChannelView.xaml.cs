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
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for ChannelView.xaml
    /// </summary>
    public partial class ChannelView : UserControl
    {
        public ChannelView()
        {
            InitializeComponent();
        }
        private void Thumb_OnDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            e.Handled = true;
        }

        private void Thumb_OnDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            e.Handled = true;
        }

        private void Thumb_OnDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cvm = this.DataContext as ChannelViewModel;
            
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = cvm.DisplaySize.Height + e.VerticalChange;
            double xadjust = cvm.DisplaySize.Width + e.HorizontalChange;

            if ((xadjust >= 0) && (yadjust >= 0))
            {
                cvm.DisplaySize.Width = (int)xadjust;
                cvm.DisplaySize.Height = (int)yadjust;
            }

            e.Handled = true;
        }
    }
}
