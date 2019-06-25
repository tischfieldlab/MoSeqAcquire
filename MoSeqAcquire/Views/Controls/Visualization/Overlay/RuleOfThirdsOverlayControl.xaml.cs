using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoSeqAcquire.Views.Controls.Visualization.Overlay
{
    /// <summary>
    /// Interaction logic for RuleOfThirdsOverlayControl.xaml
    /// </summary>
    public partial class RuleOfThirdsOverlayControl : UserControl
    {
        public RuleOfThirdsOverlayControl()
        {
            InitializeComponent();
            this.SizeChanged += RuleOfThirdsOverlayControl_SizeChanged;
        }

        private void RuleOfThirdsOverlayControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.v1.X1 = e.NewSize.Width / 3;
            this.v1.X2 = e.NewSize.Width / 3;
            this.v1.Y1 = 0;
            this.v1.Y2 = e.NewSize.Height;

            this.v2.X1 = e.NewSize.Width / 3 * 2;
            this.v2.X2 = e.NewSize.Width / 3 * 2;
            this.v2.Y1 = 0;
            this.v2.Y2 = e.NewSize.Height;

            this.h1.X1 = 0;
            this.h1.X2 = e.NewSize.Width;
            this.h1.Y1 = e.NewSize.Height / 3;
            this.h1.Y2 = e.NewSize.Height / 3;

            this.h2.X1 = 0;
            this.h2.X2 = e.NewSize.Width;
            this.h2.Y1 = e.NewSize.Height / 3 * 2;
            this.h2.Y2 = e.NewSize.Height / 3 * 2;

            Canvas.SetLeft(this.cTL, (e.NewSize.Width / 3) - (this.cTL.ActualWidth / 2));
            Canvas.SetTop(this.cTL, (e.NewSize.Height / 3) - (this.cTL.ActualHeight / 2));

            Canvas.SetLeft(this.cTR, (e.NewSize.Width / 3 * 2) - (this.cTR.ActualWidth / 2));
            Canvas.SetTop(this.cTR, (e.NewSize.Height / 3) - (this.cTR.ActualHeight / 2));

            Canvas.SetLeft(this.cBL, (e.NewSize.Width / 3) - (this.cBL.ActualWidth / 2));
            Canvas.SetTop(this.cBL, (e.NewSize.Height / 3 * 2) - (this.cBL.ActualHeight / 2));

            Canvas.SetLeft(this.cBR, (e.NewSize.Width / 3 * 2) - (this.cBR.ActualWidth / 2));
            Canvas.SetTop(this.cBR, (e.NewSize.Height / 3 * 2) - (this.cBR.ActualHeight / 2));
        }
    }
}
