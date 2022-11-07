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
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.Views.RecorderViews
{
    /// <summary>
    /// Interaction logic for RecordingError.xaml
    /// </summary>
    public partial class RecordingError : UserControl
    {
        private RecordingErrorData error;
        public RecordingError(RecordingErrorData error)
        {
            this.error = error;
            InitializeComponent();
            this.Message.Text = error.Exception.ToString();
        }
    }
}
