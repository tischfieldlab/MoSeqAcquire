﻿using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class AddMediaSourceCommand : BaseCommand
    {
        public AddMediaSourceCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Protocol.IsProtocolLocked)
                return false;
            return true;
        }

        public override void Execute(object parameter)
        {
            var dialog = new NewMediaSourceDialog();
            var result = dialog.ShowDialog();
            var info = dialog.GetDialogResult();
            if(result.HasValue && result.Value)
            {
                var ps = new ProtocolSource()
                {
                    Provider = info.Provider.AssemblyQualifiedName,
                    DeviceId = info.DeviceId
                };

                var msvm = new MediaSourceViewModel(ps);
                this.ViewModel.MediaSources.Items.Add(msvm);
            }
        }
    }
}
