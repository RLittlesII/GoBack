using System;
using System.Diagnostics.CodeAnalysis;
using DryIoc;
using GoBack.First;
using GoBack.Main;
using GoBack.Second;
using GoBack.Third;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using Prism.Plugin.Popups;
using Serilog;
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
            containerRegistry.RegisterPopupNavigationService<CustomNavigationService>();

            var container = Container.GetContainer();
            container.Register<INavigationService, CustomNavigationService>(setup: SetupWith.ParentReuse,
                ifAlreadyRegistered: IfAlreadyRegistered.Replace);
            container.Register<INavigationService, CustomNavigationService>(setup: SetupWith.ParentReuse,
                ifAlreadyRegistered: IfAlreadyRegistered.Replace, serviceKey: "PageNavigationService");
            container.RegisterInstance<ILogger>(Log.Logger);
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

    public static class SetupWith
    {
        /// <summary>
        /// Ensures dependency resolution matches the parent reuse. Equivalent to <code>Setup.With(useParentReuse: true)</code>
        /// </summary>
        public static Setup ParentReuse => Setup.With(useParentReuse: true);

        /// <summary>
        /// Enables tracking of a disposable transient. Equivalent to <code> Setup.With(trackDisposableTransient: true)</code>
        /// </summary>
        public static Setup TrackDisposableTransient => Setup.With(trackDisposableTransient: true);

        /// <summary>
        /// Allows disposable transient registration. Equivalent to <code>Setup.With(allowDisposableTransient: true)</code>
        /// </summary>
        public static Setup AllowDisposableTransient => Setup.With(allowDisposableTransient: true);
    }
}