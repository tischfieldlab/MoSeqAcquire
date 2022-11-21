using Microsoft.Azure.Kinect.Sensor;
using System.ComponentModel;

namespace MoSeqAcquire.Models.Acquisition.KinectAzure
{
    public class KinectConfig : MediaSourceConfig
    {
        [DisplayName("Color Format")]
        [DefaultValue(ImageFormat.ColorBGRA32)]
        public ImageFormat ColorFormat
        {
            get => this._colorFormat;
            set => this.SetField(ref this._colorFormat, value);
        }
        private ImageFormat _colorFormat;

        [DisplayName("Color Resolution")]
        [DefaultValue(ColorResolution.R1080p)]
        public ColorResolution ColorResolution
        {
            get => this._colorResolution;
            set => this.SetField(ref this._colorResolution, value);
        }
        private ColorResolution _colorResolution;

        [DisplayName("Depth Mode")]
        [DefaultValue(DepthMode.NFOV_Unbinned)]
        public DepthMode DepthMode
        {
            get => this._depthMode;
            set => this.SetField(ref this._depthMode, value);
        }
        private DepthMode _depthMode;

        [DisplayName("Camera FPS")]
        [DefaultValue(FPS.FPS30)]
        public FPS CameraFPS
        {
            get => this._cameraFPS;
            set => this.SetField(ref this._cameraFPS, value);
        }
        private FPS _cameraFPS;

        [DisplayName("Synchronized Images Only")]
        [DefaultValue(false)]
        public bool SynchronizedImagesOnly
        {
            get => this._synchronizedImagesOnly;
            set => this.SetField(ref this._synchronizedImagesOnly, value);
        }
        private bool _synchronizedImagesOnly;

        [DisplayName("Depth Delay Off Color")]
        [DefaultValue(0)]
        public double DepthDelayOffColor
        {
            get => this._depthDelayOffColor;
            set => this.SetField(ref this._depthDelayOffColor, value);
        }
        private double _depthDelayOffColor;

        [DisplayName("Wired Sync Mode")]
        [DefaultValue(WiredSyncMode.Standalone)]
        public WiredSyncMode WiredSyncMode
        {
            get => this._wiredSyncMode;
            set => this.SetField(ref this._wiredSyncMode, value);
        }
        private WiredSyncMode _wiredSyncMode;

        [DisplayName("Suboridinate Delay Off Master")]
        [DefaultValue(0)]
        public double SuboridinateDelayOffMaster
        {
            get => this._suboridinateDelayOffMaster;
            set => this.SetField(ref this._suboridinateDelayOffMaster, value);
        }
        private double _suboridinateDelayOffMaster;


        [DisplayName("Disable Streaming Indicator")]
        [DefaultValue(false)]
        public bool DisableStreamingIndicator
        {
            get => this._disableStreamingIndicator;
            set => this.SetField(ref this._disableStreamingIndicator, value);
        }
        private bool _disableStreamingIndicator;

    }
}