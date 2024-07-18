using Prism.Navigation;
using Serilog;

namespace GoBack.Main
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService, ILogger logger) : base(navigationService, logger)
        {
        }
    }
}