using Prism.Navigation;
using ReactiveUI;

namespace GoBack
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected ViewModelBase(INavigationService navigationService)
        {
            ForwardCommand = ReactiveCommand.CreateFromTask<string, INavigationResult>(uri => navigationService.NavigateAsync(uri));
            ModalCommand = ReactiveCommand.CreateFromTask<string, INavigationResult>(uri => navigationService.NavigateAsync(uri, null, true, true));
        }

        public ReactiveCommand<string, INavigationResult> ForwardCommand { get; }
        public ReactiveCommand<string, INavigationResult> ModalCommand { get; }
    }
}