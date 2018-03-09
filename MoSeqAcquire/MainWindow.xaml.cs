using Hdf5DotNetTools;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Acquisition.Kinect;
using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoSeqAcquire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var buss = MediaBus.Instance;
            var kinect = new KinectManager();
            while (!kinect.Initalize())
            {
                Thread.Sleep(500);
            }
            buss.Publish(kinect);

            buss.Subscribe(c => c.Channel.Name == "Kinect Color Channel", this.GetBitmapUpdater<byte>(this.ColorImg));
            buss.Subscribe(c => c.Channel.Name == "Kinect Depth Channel", this.GetBitmapUpdater<short>(this.DepthImg));

            //depth.Feed.LinkTo(HDF5File.GetWriter<short>("testfile.h5", new ulong[] { 307200 }));


            kinect.Start();



        }

        protected ActionBlock<ChannelFrame<T>> GetBitmapUpdater<T>(Image image)
        {
            return new ActionBlock<ChannelFrame<T>>(frame =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (image.Source == null)
                    {
                        image.Source = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, PixelFormats.Gray16, null);
                    }

                    (image.Source as WriteableBitmap).WritePixels(
                        new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                        frame.FrameData,
                        frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                        0);
                }));
            });
        }
    }
}
