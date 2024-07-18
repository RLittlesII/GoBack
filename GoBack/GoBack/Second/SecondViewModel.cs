using Prism.Navigation;
using Serilog;

namespace GoBack.Second
{
    public class SecondViewModel : ViewModelBase
    {
        public SecondViewModel(INavigationService navigationService, ILogger logger) : base(navigationService, logger)
        {
        }
    }
}