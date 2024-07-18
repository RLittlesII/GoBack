using Prism.Navigation;
using Serilog;

namespace GoBack.First
{
    public class FirstViewModel : ViewModelBase
    {
        public FirstViewModel(INavigationService navigationService, ILogger logger) : base(navigationService, logger)
        {
        }
    }
}