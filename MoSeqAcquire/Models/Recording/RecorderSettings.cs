using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Views.Extensions;
using MvvmValidation;

namespace MoSeqAcquire.Models.Recording
{
    public enum RecordingMode
    {
        [Description("Recordes until stopped")]
        [EnumDisplayName("Until stopped")]
        Indeterminate,

        [Description("Recordes the specified amount of time")]
        [EnumDisplayName("A specific amount of time")]
        TimeCount
    }
    public abstract class RecorderSettings : BaseConfiguration
    {
    }
    public class GeneralRecordingSettings : BaseConfiguration, INotifyDataErrorInfo, IDataErrorInfo
    {
        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }
        protected ValidationHelper Validator { get; private set; }
        public GeneralRecordingSettings()
        {
            Validator = new ValidationHelper();
            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(this.Validator);

            this.PropertyChanged += (s, e) => { if (!"IsValid".Equals(e.PropertyName)){ this.NotifyPropertyChanged("IsValid"); } };

            Validator.AddRequiredRule(() => this.Directory, "Directory cannot be empty!");
        }

        protected string directory = "";
        protected string basename = "";
        protected RecordingMode recordingMode;
        protected TimeSpan recordingTime;

        public string Directory
        {
            get => this.directory;
            set => this.SetField(ref this.directory, value);
        }
        [Hidden(true)]
        public string Basename
        {
            get => this.basename;
            set => this.SetField(ref this.basename, value);
        }
        public string ComputedBasePath
        {
            get => Path.Combine(this.Directory, this.Basename);
        }
        public RecordingMode RecordingMode
        {
            get => this.recordingMode;
            set => this.SetField(ref this.recordingMode, value);
        }

        public TimeSpan RecordingTime
        {
            get => this.recordingTime;
            set => this.SetField(ref this.recordingTime, value);
        }
        [Hidden]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.directory)
                    //&& !string.IsNullOrWhiteSpace(this.basename)
                    && (this.recordingMode.Equals(RecordingMode.Indeterminate)
                        || (this.recordingMode.Equals(RecordingMode.TimeCount) && this.recordingTime.TotalSeconds > 0)
                       );
            }
        }




        public string this[string columnName]
        {
            get { return this.Validator.GetResult(columnName).ToString(); }
        }

        public string Error
        {
            get { return this.Validator.GetResult().ToString(); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                ValidationResult result = this.Validator.GetResult();
                if (result.IsValid)
                    return (IEnumerable)Enumerable.Empty<string>();
                return (IEnumerable)new string[1]
                {
                    result.ToString()
                };
            }
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return NotifyDataErrorInfoAdapter.HasErrors; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }
    }    
}
