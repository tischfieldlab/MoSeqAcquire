using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Properties;
using MoSeqAcquire.ViewModels.MediaSources;
using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MoSeqAcquire.ViewModels.Triggers;

namespace MoSeqAcquire.ViewModels
{
    public class ProtocolManagerViewModel : BaseViewModel
    {
        protected bool isProtocolLocked;
        protected int forceProtocolLock;

        public ProtocolManagerViewModel()
        {
            this.RecentlyUsedProtocols = new ObservableCollection<string>(Settings.Default.RecentProtocols.Cast<string>());

        }

        public ObservableCollection<string> RecentlyUsedProtocols { get; protected set; }

        public void ForceProtocolLocked()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.forceProtocolLock++;
                this.NotifyPropertyChanged(nameof(this.IsProtocolLocked));
                this.NotifyPropertyChanged(nameof(this.IsProtocolForceLocked));
            });
        }
        public void UndoForceProtoclLocked()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.forceProtocolLock--;
                if (this.forceProtocolLock < 0)
                {
                    this.forceProtocolLock = 0;
                }
                this.NotifyPropertyChanged(nameof(this.IsProtocolLocked));

                this.NotifyPropertyChanged(nameof(this.IsProtocolForceLocked));
            });
        }

        public bool IsProtocolForceLocked
        {
            get => this.forceProtocolLock > 0;
        }
        public bool IsProtocolLocked
        {
            get => this.isProtocolLocked || this.forceProtocolLock > 0;
            set => this.SetField(ref this.isProtocolLocked, value);
        }

        public void PushRecentProtocol(string path)
        {
            if (this.RecentlyUsedProtocols.Contains(path))
            {
                this.RecentlyUsedProtocols.Move(this.RecentlyUsedProtocols.IndexOf(path), 0);
            }
            else
            {
                this.RecentlyUsedProtocols.Insert(0, path);
            }

            if (this.RecentlyUsedProtocols.Count > Settings.Default.MaxRecentProtocolsToRemember)
            {
                this.RecentlyUsedProtocols
                    .Skip(Settings.Default.MaxRecentProtocolsToRemember)
                    .ToList()
                    .ForEach(rup => this.RecentlyUsedProtocols.Remove(rup));
            }

            Settings.Default.RecentProtocols.Clear();
            Settings.Default.RecentProtocols.AddRange(this.RecentlyUsedProtocols.ToArray());
            Settings.Default.Save();
        }

        public Protocol CurrentProtocol { get; protected set; }


        public Protocol GenerateProtocol()
        {
            var mediaSources = App.Current.Services.GetService<MediaSourceCollectionViewModel>();
            var recorder = App.Current.Services.GetService<RecordingManagerViewModel>();
            var triggers = App.Current.Services.GetService<TriggerManagerViewModel>();

            this.ForceProtocolLocked();
            var pcol = new Protocol("basic");
            foreach (var ms in mediaSources.Items)
            {
                pcol.Sources.Add(ms.MediaSource.GetType(), ms.MediaSource.DeviceId, ms.Config.GetSnapshot());
            }
            foreach (var mw in recorder.Recorders)
            {
                pcol.Recordings.Recorders.Add(mw.GetRecorderDefinition());
            }
            foreach (var tvm in triggers.Triggers)
            {
                pcol.Triggers.Add(tvm.GetTriggerDefinition());
            }

            pcol.Metadata = recorder.RecordingMetadata.Items;
            pcol.Recordings.GeneralSettings = recorder.GeneralSettings.GetSnapshot();
            pcol.Locked = this.isProtocolLocked;
            this.CurrentProtocol = pcol;
            this.UndoForceProtoclLocked();
            return pcol;
        }
        public void UnloadProtocol()
        {
            var mediaSources = App.Current.Services.GetService<MediaSourceCollectionViewModel>();
            var recorder = App.Current.Services.GetService<RecordingManagerViewModel>();
            var triggers = App.Current.Services.GetService<TriggerManagerViewModel>();

            // force protocol to be locked
            this.ForceProtocolLocked();
            
            // remove all media sources
            mediaSources.Items.ForEach(s => s.MediaSource.Stop());
            mediaSources.Items.Clear();

            // clear all recoring items
            recorder.ClearRecorders();
            recorder.RecordingMetadata.Items.Clear();

            // remove all triggers
            triggers.RemoveTriggers();

            // unlock protocol
            this.isProtocolLocked = false;
            this.UndoForceProtoclLocked();
        }
        public void ApplyProtocol(Protocol protocol)
        {
            var mediaSources = App.Current.Services.GetService<MediaSourceCollectionViewModel>();
            var recorder = App.Current.Services.GetService<RecordingManagerViewModel>();
            var triggers = App.Current.Services.GetService<TriggerManagerViewModel>();

            this.ForceProtocolLocked();
            //prepare
            this.UnloadProtocol();

            this.CurrentProtocol = protocol;

            //add media sources
            var tasks = new List<Task>();
            foreach (var s in protocol.Sources)
            {
                var msvm = new MediaSourceViewModel(s);
                tasks.Add(msvm.InitTask);
                mediaSources.Items.Add(msvm);
            }

            //necessary to wait for all hardware to load prior to applying recorders
            //otherwise the channel does not exist!
            Task.WhenAll(tasks).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (protocol.Recordings != null)
                    {
                        recorder.GeneralSettings.ApplySnapshot(protocol.Recordings.GeneralSettings);
                        foreach (var r in protocol.Recordings.Recorders)
                        {
                            recorder.AddRecorder(new RecorderViewModel(r));
                        }
                    }
                    if (protocol.Triggers != null)
                    {
                        foreach (var trigger in protocol.Triggers)
                        {
                            triggers.AddTrigger(trigger);
                        }
                    }

                    if (protocol.Metadata != null)
                    {
                        foreach (var item in protocol.Metadata)
                        {
                            recorder.RecordingMetadata.Items.Add(item);
                        }
                    }
                    recorder.RecordingMetadata.Items.ResetValuesToDefaults();
                    this.isProtocolLocked = protocol.Locked;
                    this.NotifyPropertyChanged();
                    this.UndoForceProtoclLocked();
                });
            }, TaskScheduler.Default);
        }
    }
}
