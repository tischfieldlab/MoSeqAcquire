﻿using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class StopRecordingCommand : BaseCommand
    {
        public StopRecordingCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
            this.ViewModel.Recorder.PropertyChanged += Recorder_PropertyChanged;
        }

        private void Recorder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || e.PropertyName.Equals(nameof(this.ViewModel.Recorder.State)))
            {
                this.RaiseCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return this.ViewModel.Recorder.State == RecordingManagerState.Recording;
        }

        public override void Execute(object parameter)
        {
            this.ViewModel.Recorder.StopRecording();
        }
    }
}
