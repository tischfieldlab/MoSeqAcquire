using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : WaitingWindow
    {


        public Splash() : base()
        {
            this.DataContext = this;
            InitializeComponent();
            this.CurrentStatus = "Initializing.....";
        }
        
    }
}
