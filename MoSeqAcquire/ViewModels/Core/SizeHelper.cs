using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Core
{
    public class SizeHelper : BaseViewModel
    {
        protected double minWidth;
        protected double maxWidth;
        protected double minHeight;
        protected double maxHeight;
        protected double? ratio;

        protected double width;
        protected double height;

        protected double initWidth;
        protected double initHeight;


        public SizeHelper(double minWidth, double maxWidth, double minHeight, double maxHeight, double width, double height)
        {
            this.minWidth = minWidth;
            this.maxWidth = maxWidth;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;

            this.width = this.initWidth = width;
            this.height = this.initHeight = height;
        }

        #region Property Accessors
        public double MinWidth
        {
            get => this.minWidth;
            set
            {
                this.SetField(ref this.minWidth, value);
                this.Coerce();
            }
        }
        public double MaxWidth
        {
            get => this.maxWidth;
            set
            {
                this.SetField(ref this.maxWidth, value);
                this.Coerce();
            }
        }
        public double MinHeight
        {
            get => this.minHeight;
            set
            {
                this.SetField(ref this.minHeight, value);
                this.Coerce();
            }
        }
        public double MaxHeight
        {
            get => this.maxHeight;
            set
            {
                this.SetField(ref this.maxHeight, value);
                this.Coerce();
            }
        }
        public double? Ratio
        {
            get => this.ratio;
            set
            {
                this.SetField(ref this.ratio, value);
                this.Coerce();
            }
        }
        public double Width
        {
            get => this.width;
            set
            {
                this.SetField(ref this.width, value);
                this.Coerce();
            }
        }
        public double Height
        {
            get => this.height;
            set
            {
                this.SetField(ref this.height, value);
                this.Coerce();
            }
        }
        #endregion Property Accessors

        public void Reset()
        {
            this.width = this.initWidth;
            this.height = this.initHeight;
            this.Coerce();
        }

        protected void Coerce()
        {
            if (this.width > this.maxWidth)
            {
                this.width = this.maxWidth;
            }
            else if (this.width < this.minWidth)
            {
                this.width = this.minWidth;
            }

            if (this.height > this.maxHeight)
            {
                this.height = this.maxHeight;
            }
            else if (this.height < this.minHeight)
            {
                this.height = this.minHeight;
            }

            if (this.ratio != null)
            {
                this.height = (int)(this.width * this.ratio);
            }

            this.NotifyPropertyChanged(null);
        }
    }
}
