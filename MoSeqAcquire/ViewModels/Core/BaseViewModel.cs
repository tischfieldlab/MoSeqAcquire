using MoSeqAcquire.Models;
using MoSeqAcquire.Models.Core;
using MvvmValidation;

namespace MoSeqAcquire.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {
            Validator = new ValidationHelper();
        }
        protected ValidationHelper Validator { get; private set; }
    }
}
