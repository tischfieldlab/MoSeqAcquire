using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Views.MediaSources.Visualization
{
    /// <summary>
    /// Interaction logic for PolygonWaveFormControl.xaml
    /// </summary>
    public partial class VideoStreamControl : UserControl
    {
        

        public VideoStreamControl()
        {
            InitializeComponent();
        }

        public WriteableBitmap Stream
        {
            get => this.mainImage.Source as WriteableBitmap;
            set => this.mainImage.Source = value;
        }

    }
}
