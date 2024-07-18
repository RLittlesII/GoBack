using GoBack.First;
using GoBack.Main;
using GoBack.Second;
using GoBack.Third;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace GoBack
{
    public partial class App : PrismApplication
    {
        public App() => InitializeComponent();

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>(nameof(NavigationPage));
            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(nameof(MainPage));
            containerRegistry.RegisterForNavigation<FirstPage, FirstViewModel>(nameof(FirstPage));
            containerRegistry.RegisterForNavigation<SecondPage, SecondViewModel>(nameof(SecondPage));
            containerRegistry.RegisterForNavigation<ThirdPage, ThirdViewModel>(nameof(ThirdPage));
        }

        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("/NavigationPage/MainPage")
                .ContinueWith(t => HandleFailedNavigationResult(t.Result));
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        private static void HandleFailedNavigationResult(INavigationResult result)
        {
            if (!result.Success)
            {
            }
        }
    }
}