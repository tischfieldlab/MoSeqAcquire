using MoSeqAcquire.Models.Utility;
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

namespace MoSeqAcquire.Views.Controls.Visualization.Data
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : BaseChart
    {
        public BarChart() : base()
        {
            InitializeComponent();
            this.TargetCanvas = this.MainCanvas;
            this.Measure(_infinateSize);
        }



        private List<Rectangle> _bars = new List<Rectangle>();
        public void Update(float[] data)
        {
            if (this.ActualHeight == 0 && this.ActualWidth == 0)
                return;            

            this.XAxis.UpdateExtents(data);
            this.YAxis.UpdateExtents(data);

            if (this._bars.Count > data.Length)
                this.ResetBars();

            for (int i = 0; i < data.Length; i++)
            {
                if (this._bars.Count <= i)
                {
                    this._bars.Add(new Rectangle());
                    this.MainCanvas.Children.Add(this._bars[i]);
                }

                try
                {
                    var ypos = this.YAxis.GetDataPosition(data[i]);
                    var xpos = this.XAxis.GetDataPosition(i);
                    this._bars[i].Height = ypos.Item1;
                    this._bars[i].Width = xpos.Item1;
                    Canvas.SetLeft(this._bars[i], xpos.Item2);
                    Canvas.SetTop(this._bars[i], data[i] < 0 ? ypos.Item2 - ypos.Item1 : ypos.Item2);
                }
                catch(ArgumentException)
                {
                    //nothing to do!
                }
            }
        }

        private void ResetBars()
        {
            this._bars
                .ToList()
                .ForEach(b => {
                    this._bars.Remove(b);
                    this.MainCanvas.Children.Remove(b);
                });
        }
    }
}
