using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using Prism.Plugin.Popups;
using Rg.Plugins.Popup.Contracts;
using Serilog;
using Xamarin.Forms;

namespace GoBack
{
    public class CustomNavigationService : PopupPageNavigationService
    {
        public CustomNavigationService(IPopupNavigation popupNavigation,
            IContainerExtension container,
            IApplicationProvider applicationProvider,
            IPageBehaviorFactory pageBehaviorFactor,
            ILoggerFacade logger) : base(popupNavigation,
            container,
            applicationProvider,
            pageBehaviorFactor,
            logger)
        {
        }

        protected override Task ProcessNavigationForRemovePageSegments(Page currentPage,
            string nextSegment,
            Queue<string> segments,
            INavigationParameters parameters,
            bool? useModalNavigation,
            bool animated)
        {
            if (!HasDirectNavigationPageParent(currentPage))
                throw new NavigationException(NavigationException.RelativeNavigationRequiresNavigationPage,
                    currentPage);

            if (CanRemoveAndPush(segments))
                return RemoveAndPush(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            else
                return RemoveAndGoBack(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }

        internal static bool HasDirectNavigationPageParent(Page page)
        {
            return page?.Parent != null && page?.Parent is NavigationPage;
        }

        private static bool CanRemoveAndPush(Queue<string> segments)
        {
            if (segments.All(segment => segment == RemovePageSegment))
                return false;
            else
                return true;
        }

        private Task RemoveAndGoBack(Page currentPage,
            string nextSegment,
            Queue<string> segments,
            INavigationParameters parameters,
            bool? useModalNavigation,
            bool animated)
        {
            List<Page> pagesToRemove = new List<Page>();

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            var currentModalIndex = currentPage.Navigation.ModalStack.Count;
            var currentPopupIndex = _popupNavigation.PopupStack.Count;

            Log.Logger.Information("Current Page Stack Count {PageStackCount}", currentPageIndex);
            Log.Logger.Information("Current Modal Stack Count {ModalStackCount}", currentModalIndex);
            Log.Logger.Information("Current Modal Stack Count {PopupStackCount}", currentPopupIndex);

            currentPage.Navigation.NavigationStack.Join(currentPage.Navigation.ModalStack,
                    page => page,
                    page => page is not NavigationPage navigationPage ? page : navigationPage.CurrentPage,
                    (page, modal) =>
                    {
                        Log.Logger.Warning("Page {Page} is on BOTH stacks", modal);
                        return page;
                    })
                .ToList();

            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Count != 0)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);

            return
                GoBackAsync(
                    parameters); // QUESTION: [rlittlesii: July 18, 2024] should this be using the useModalNavigation and animated, would that even fix my problem?
            return
                this.GoBackAsync(parameters, useModalNavigation,
                    animated); // NOTE: [rlittlesii: July 18, 2024] calling the PlatformNavigationService breaks the backwards navigation.
        }

        private async Task RemoveAndPush(Page currentPage,
            string nextSegment,
            Queue<string> segments,
            INavigationParameters parameters,
            bool? useModalNavigation,
            bool animated)
        {
            var pagesToRemove = new List<Page>
            {
                currentPage
            };

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Peek() == RemovePageSegment)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            await ProcessNavigation(currentPage, segments, parameters, useModalNavigation, animated);

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);
        }

        private static void RemovePagesFromNavigationPage(Page currentPage, List<Page> pagesToRemove)
        {
            var navigationPage = (NavigationPage) currentPage.Parent;
            foreach (var page in pagesToRemove)
            {
                navigationPage.Navigation.RemovePage(page);
                PageUtilities.DestroyPage(page);
            }
        }

        private const string RemovePageRelativePath = "../";
        private const string RemovePageInstruction = "__RemovePage/";
        private const string RemovePageSegment = "__RemovePage";
    }
}