using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using Serilog;

namespace GoBack.Third
{
    public class ThirdViewModel : ViewModelBase
    {
        public ThirdViewModel(INavigationService navigationService, ILogger logger) : base(navigationService, logger)
        {
            BackCommand = ReactiveCommand.CreateFromTask(_ =>
            {
                logger.Information("Back: {Uri}", BackUrl);
                return navigationService.NavigateAsync(BackUrl);
                return navigationService.NavigateAsync(BackUrl, null, true, true); // NOTE: [rlittlesii: July 18, 2024] This has weird behavior.  It does the subsequent navigation as a Modal navigation.
            });

            string CreateUri(int backwards)
            {
                var result = Back;

                for (var i = 1; i < backwards; i++)
                {
                    result += Back;
                }

                return result;
            }

            this.WhenAnyValue(x => x.Backwards, x => x.Url, (back, url) => (back, url))
                .Where(x => x.back > 0)
                .DistinctUntilChanged()
                .Select(tuple => CreateUri(tuple.back) + tuple.url)
                .ToProperty(this, x => x.BackUrl, out _backUrl);
        }

        public ReactiveCommand<Unit, INavigationResult> BackCommand { get; set; }


        public int Backwards
        {
            get => _backwards;
            set => this.RaiseAndSetIfChanged(ref _backwards, value);
        }

        public string Url
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        public string BackUrl => _backUrl.Value;

        private int _backwards;
        private string _url;
        private const string Back = "../";
        private readonly ObservableAsPropertyHelper<string> _backUrl;
    }
}