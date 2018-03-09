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
            var depth = (BusChannel<short>)buss.__sources[0];
            var color = (BusChannel<byte>)buss.__sources[1];


            color.Feed.LinkTo(new ActionBlock<ChannelFrame<byte>>(frame => {
                //Console.WriteLine("got color frame");
                Dispatcher.Invoke(new Action(() =>
                {
                    if (this.ColorImg.Source == null)
                    {
                        this.ColorImg.Source = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, PixelFormats.Bgr32, null);
                    }

                    (this.ColorImg.Source as WriteableBitmap).WritePixels(
                        new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height), 
                        frame.FrameData, 
                        frame.Metadata.Width * frame.Metadata.BytesPerPixel, 
                        0);
                }));
            }));
            depth.Feed.LinkTo(new ActionBlock<ChannelFrame<short>>(frame => {
                //Console.WriteLine("got depth frame");
                Dispatcher.Invoke(new Action(() =>
                {
                    if (this.DepthImg.Source == null)
                    {
                        this.DepthImg.Source = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, PixelFormats.Gray16, null);
                    }

                    (this.DepthImg.Source as WriteableBitmap).WritePixels(
                        new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                        frame.FrameData,
                        frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                        0);
                }));
            }));


            color.Feed.LinkTo(new ActionBlock<ChannelFrame<byte>>(frame => {
                //Console.WriteLine("got color frame");
                Dispatcher.Invoke(new Action(() =>
                {
                    if (this.ColorImg2.Source == null)
                    {
                        this.ColorImg2.Source = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, PixelFormats.Bgr32, null);
                    }

                    (this.ColorImg2.Source as WriteableBitmap).WritePixels(
                        new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                        frame.FrameData,
                        frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                        0);
                }));
            }));
            depth.Feed.LinkTo(new ActionBlock<ChannelFrame<short>>(frame => {
                //Console.WriteLine("got depth frame");
                Dispatcher.Invoke(new Action(() =>
                {
                    if (this.DepthImg2.Source == null)
                    {
                        this.DepthImg2.Source = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, PixelFormats.Gray16, null);
                    }

                    (this.DepthImg2.Source as WriteableBitmap).WritePixels(
                        new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                        frame.FrameData,
                        frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                        0);
                }));
            }));

            //depth.Feed.LinkTo(HDF5File.GetWriter<short>("testfile.h5", new ulong[] { 307200 }));


            kinect.Start();



        }
    }
}
