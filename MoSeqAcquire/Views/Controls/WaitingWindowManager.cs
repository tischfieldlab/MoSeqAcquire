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
        private Type windowType;
        private Thread thread;
        private bool canAbortThread = false;
        private WaitingWindow waitingWindow;
        private EventHandler closedHandler;

        public WaitingWindowManager(Type windowType)
        {
            this.windowType = windowType;
        }

        public void BeginWaiting()
        {
            this.thread = new Thread(this.RunThread);
            this.thread.Name = "LoadingThread";
            this.thread.IsBackground = true;
            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.Start();
        }
        public void SetCurrentStatus(String StatusText)
        {
            if (this.waitingWindow != null)
            {
                this.waitingWindow.Dispatcher.BeginInvoke(new setStatusText(this.doSetStatusText), DispatcherPriority.Normal, StatusText);
            }
        }
        protected delegate void setStatusText(String StatusText);
        protected void doSetStatusText(String StatusText)
        {
            this.waitingWindow.CurrentStatus = StatusText;
        }
        public void EndWaiting()
        {
            if (this.waitingWindow != null)
            {
                this.waitingWindow.Dispatcher.BeginInvoke(new closeWindow(this.doCloseWindow), DispatcherPriority.Normal, null);
            }
        }
        protected delegate void closeWindow();
        protected void doCloseWindow()
        {
            //MessageBox.Show("about to close window");
            this.waitingWindow.Close();
        }

        public void RunThread()
        {
            this.waitingWindow = (WaitingWindow)this.windowType.GetConstructor(new Type[] { }).Invoke(null);
            this.closedHandler = new EventHandler(waitingWindow_Closed);
            this.waitingWindow.Closed += this.closedHandler;
            this.waitingWindow.Closed += (sender2, e2) => this.waitingWindow.Dispatcher.InvokeShutdown();
            this.waitingWindow.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        void waitingWindow_Closed(object sender, EventArgs e)
        {
            //MessageBox.Show("in handler");
            this.waitingWindow.Closed -= this.closedHandler;
            this.canAbortThread = true;
        }
    }


    public class WaitingWindow : Window, INotifyPropertyChanged
    {
        protected String currentStatus;

        public event PropertyChangedEventHandler PropertyChanged;


        public String CurrentStatus
        {
            get { return this.currentStatus; }
            set { this.currentStatus = value; this.NotifyPropertyChanged("CurrentStatus"); }
        }

        protected void NotifyPropertyChanged(String PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
