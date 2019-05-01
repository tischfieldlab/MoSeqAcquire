using MoSeqAcquire.Models;
using MoSeqAcquire.Models.Core;
using MvvmValidation;

namespace MoSeqAcquire.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {
            Validator = new ValidationHelper(new ValidationSettings()
            {
                DefaultRuleSettings = new ValidationRuleSettings()
                {
                    ExecuteOnAlreadyInvalidTarget = true
                }
            });
        }
        protected ValidationHelper Validator { get; private set; }
    }
}
