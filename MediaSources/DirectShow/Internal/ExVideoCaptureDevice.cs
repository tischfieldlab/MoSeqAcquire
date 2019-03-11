using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow.Internal
{
    public class ExVideoCaptureDevice : VideoCaptureDevice
    {
        public ExVideoCaptureDevice(string deviceMoniker) : base(deviceMoniker)
        {
        }

        /// <summary>
        /// Sets a specified property on the video signal adjustments.
        /// </summary>
        /// 
        /// <param name="property">Specifies the property to set.</param>
        /// <param name="value">Specifies the new value of the property.</param>
        /// <param name="controlFlags">Specifies the desired control setting.</param>
        /// 
        /// <returns>Returns true on success or false otherwise.</returns>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        /// 
        public bool SetVideoProcAmpProperty(VideoProcAmpProperty property, int value, VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if (String.IsNullOrEmpty(Source))
                throw new ArgumentException("Video source is not specified.");

            lock (SyncLock)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(Source);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support video signal adjustments.");

                IAMVideoProcAmp pVideoProcAmp = (IAMVideoProcAmp)tempSourceObject;
                int hr = pVideoProcAmp.Set(property, value, controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        /// <summary>
        /// Gets the current setting of a video signal adjustment property.
        /// </summary>
        /// 
        /// <param name="property">Specifies the property to retrieve.</param>
        /// <param name="value">Receives the value of the property.</param>
        /// <param name="controlFlags">Receives the value indicating whether the setting is controlled manually or automatically</param>
        /// 
        /// <returns>Returns true on sucee or false otherwise.</returns>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        /// 
        public bool GetVideoProcAmpProperty(VideoProcAmpProperty property, out int value, out VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if (String.IsNullOrEmpty(Source))
                throw new ArgumentException("Video source is not specified.");

            lock (SyncLock)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(Source);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support video signal adjustments.");

                IAMVideoProcAmp pVideoProcAmp = (IAMVideoProcAmp)tempSourceObject;
                int hr = pVideoProcAmp.Get(property, out value, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        /// <summary>
        /// Gets the range and default value of a specified video stream property.
        /// </summary>
        /// 
        /// <param name="property">Specifies the property to query.</param>
        /// <param name="minValue">Receives the minimum value of the property.</param>
        /// <param name="maxValue">Receives the maximum value of the property.</param>
        /// <param name="stepSize">Receives the step size for the property.</param>
        /// <param name="defaultValue">Receives the default value of the property.</param>
        /// <param name="controlFlags">Receives a member of the <see cref="CameraControlFlags"/> enumeration, indicating whether the property is controlled automatically or manually.</param>
        /// 
        /// <returns>Returns true on sucee or false otherwise.</returns>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        /// 
        public bool GetVideoProcAmpRange(VideoProcAmpProperty property, out int minValue, out int maxValue, out int stepSize, out int defaultValue, out VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if (String.IsNullOrEmpty(Source))
                throw new ArgumentException("Video source is not specified.");

            lock (SyncLock)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(Source);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support video signal adjustments.");

                IAMVideoProcAmp pVideoProcAmp = (IAMVideoProcAmp)tempSourceObject;
                int hr = pVideoProcAmp.GetRange(property, out minValue, out maxValue, out stepSize, out defaultValue, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        protected object SyncLock
        {
            get
            {
                var field = typeof(ExVideoCaptureDevice).BaseType.GetField("sync", BindingFlags.Instance | BindingFlags.NonPublic);
                return field.GetValue(this);
            }
        }
    }
}
