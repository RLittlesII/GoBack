using Prism.Navigation;
using ReactiveUI;
using Serilog;

namespace GoBack
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected ViewModelBase(INavigationService navigationService, ILogger logger)
        {
            ForwardCommand = ReactiveCommand.CreateFromTask<string, INavigationResult>(uri =>
            {
                logger.Information("Forward: {Uri}", uri);
                return navigationService.NavigateAsync(uri);
            });
            ModalCommand = ReactiveCommand.CreateFromTask<string, INavigationResult>(uri =>
            {
                logger.Information("Modal: {Uri}", uri);
                return navigationService.NavigateAsync("NavigationPage/" + uri, null, true, true);
            });
        }

        public ReactiveCommand<string, INavigationResult> ForwardCommand { get; }
        public ReactiveCommand<string, INavigationResult> ModalCommand { get; }
    }
}