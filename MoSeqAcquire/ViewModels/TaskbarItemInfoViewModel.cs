using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.ViewModels
{
    public class TaskbarItemInfoViewModel : BaseViewModel
    {
        protected string description;
        protected ImageSource overlay;
        protected TaskbarItemProgressState progressState;
        protected double progressValue;
        protected Thickness thumbnailClipMargin;

        protected RecordingManagerViewModel recorder;


        public TaskbarItemInfoViewModel(RecordingManagerViewModel recorder)
        {
            this.recorder = recorder;

            this.recorder.PropertyChanged += Recorder_PropertyChanged;
        }

        private void Recorder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName.Equals(nameof(this.recorder.State)))
            {
                switch (this.recorder.State)
                {
                    case RecordingManagerState.Idle:
                        this.ProgressState = TaskbarItemProgressState.None;
                        break;

                    case RecordingManagerState.Starting:
                    case RecordingManagerState.Completing:
                        this.ProgressState = TaskbarItemProgressState.Indeterminate;
                        break;

                    case RecordingManagerState.Recording:
                        this.ProgressState = TaskbarItemProgressState.Normal;
                        break;
                }
            }

            if (e.PropertyName == null || e.PropertyName.Equals(nameof(this.recorder.Progress)))
            {
                this.ProgressValue = this.recorder.Progress == null
                    ? 0
                    : (double)this.recorder.Progress;
            }
        }

        public string Description
        {
            get => this.description;
            set => this.SetField(ref this.description, value);
        }
        public ImageSource Overlay
        {
            get => this.overlay;
            set => this.SetField(ref this.overlay, value);
        }
        public TaskbarItemProgressState ProgressState
        {
            get => this.progressState;
            set => this.SetField(ref this.progressState, value);
        }
        public double ProgressValue
        {
            get => this.progressValue;
            set => this.SetField(ref this.progressValue, value);
        }
        public Thickness ThumbnailClipMargin
        {
            get => this.thumbnailClipMargin;
            set => this.SetField(ref this.thumbnailClipMargin, value);
        }
    }
}
