using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MoSeqAcquire.Views.Controls
{
    public class WaitingWindowManager
    {
        private readonly Type _windowType;
        private Thread _thread;
        private WaitingWindow _waitingWindow;
        private EventHandler _closedHandler;

        public WaitingWindowManager(Type windowType)
        {
            this._windowType = windowType;
        }

        public void BeginWaiting()
        {
            this._thread = new Thread(this.RunThread)
            {
                Name = "WaitingWindowThread",
                IsBackground = true
            };
            this._thread.SetApartmentState(ApartmentState.STA);
            this._thread.Start();
        }
        public void SetCurrentStatus(string statusText)
        {
            _waitingWindow?.Dispatcher.BeginInvoke(new Action(() =>
            {
                this._waitingWindow.CurrentStatus = statusText;
            }));
        }
        public void EndWaiting()
        {
            _waitingWindow?.Dispatcher.BeginInvoke(new Action(() => { this._waitingWindow.Close(); }));
        }

        public void RunThread()
        {
            this._waitingWindow = (WaitingWindow)this._windowType.GetConstructor(new Type[] { })?.Invoke(null);
            this._closedHandler = new EventHandler(WaitingWindow_Closed);
            this._waitingWindow.Closed += this._closedHandler;
            this._waitingWindow.Closed += (sender2, e2) => this._waitingWindow.Dispatcher.InvokeShutdown();
            this._waitingWindow.Show();
            Dispatcher.Run();
        }

        private void WaitingWindow_Closed(object sender, EventArgs e)
        {
            this._waitingWindow.Closed -= this._closedHandler;
        }
    }


    public class WaitingWindow : Window, INotifyPropertyChanged
    {
        protected string currentStatus;

        public event PropertyChangedEventHandler PropertyChanged;


        public string CurrentStatus
        {
            get  => this.currentStatus;
            set
            {
                this.currentStatus = value;
                this.NotifyPropertyChanged(nameof(this.CurrentStatus));
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
