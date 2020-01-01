using System.Windows.Input;
using ICOCrawler.Common;
using ICOCrawler.GitHub.View;
using ICOCrawler.GitHub.ViewModel;
using ICOCrawler.LinkedIn.View;
using ICOCrawler.LinkedIn.ViewModel;
using ICOCrawler.Medium.View;
using ICOCrawler.Medium.ViewModel;
using ICOCrawler.Model;

namespace ICOCrawler.ViewModel
{
    public class MainWindowViewModel
    {
        private readonly GitHubViewModel _gitHubViewModel;
        private readonly LinkedInViewModel _linkedInViewModel;
        private readonly CheckListViewModel _checkListViewModel;

        public GitHubView GitHubViewUserControl { get; }
        public MediumView MediumViewUserControl { get; }
        public LinkedInView LinkedInViewUserControl { get; }
        public CheckListView CheckListViewUserControl { get; }
        public ICommand SearchCommand { get; }

        public MainWindowViewModel()
        {
            var mediumViewModel = new MediumViewModel();
            MediumViewUserControl = new MediumView { DataContext = mediumViewModel };

            _gitHubViewModel = new GitHubViewModel();
            GitHubViewUserControl = new GitHubView { DataContext = _gitHubViewModel };

            _linkedInViewModel = new LinkedInViewModel();
            LinkedInViewUserControl = new LinkedInView { DataContext = _linkedInViewModel };

            _checkListViewModel = new CheckListViewModel();
            CheckListViewUserControl = new CheckListView { DataContext = _checkListViewModel };

            SearchCommand = new RelayCommand(ExecuteSearchCommand);
        }

        private void ExecuteSearchCommand(object obj)
        {
            var selectedSource = (InitialCoinOfferingSource)obj;

            switch (selectedSource)
            {
                case InitialCoinOfferingSource.Medium:
                    _checkListViewModel?.SearchCommand.Execute(obj);
                    break;
                case InitialCoinOfferingSource.GitHub:
                    _gitHubViewModel?.SearchCommand.Execute(obj);
                    break;
                case InitialCoinOfferingSource.LinkedIn:
                    _linkedInViewModel?.RefreshCommand.Execute(obj);
                    break;
            }
        }
    }
}
