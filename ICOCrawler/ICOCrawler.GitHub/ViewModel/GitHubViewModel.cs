using ICOCrawler.Common;
using ICOCrawler.DataAccess;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Medium.ViewModel;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using ICOCrawler.Model;

namespace ICOCrawler.GitHub.ViewModel
{
    public class GitHubViewModel : CheckListViewModel
    {
        private GitHubClient _gitHubClient;
        private readonly CancellationTokenSource _cancelSource;
        private OperationState _operationState = OperationState.Idle;

        public ICommand SearchGitHubCommand { get; }
        public ICommand CancelSearchGitHubCommand { get; }
        public ObservableCollection<QueryItem> QueryItems { get; }
        public OperationState OperationState
        {
            get => _operationState;
            set
            {
                _operationState = value;
                OnPropertyChanged("OperationState");
            }
        }

        public GitHubViewModel()
        {
            SetupGithubClient();

            _cancelSource = new CancellationTokenSource();
            SearchGitHubCommand = new RelayCommand(ExecuteSearchGitHubCommand);
            CancelSearchGitHubCommand = new RelayCommand(ExecuteCancelSearchGitHubCommand);

            QueryItems = new ObservableCollection<QueryItem>();
            SetupQueryItems();
        }

        private void ExecuteSearchGitHubCommand(object obj)
        {
            SetStatus(SearchStatus.Waiting, OperationState.Running, Visibility.Visible);
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                foreach (var queryItem in QueryItems)
                {
                    if (_cancelSource.IsCancellationRequested)
                    {
                        continue;
                    }

                    var dateTimeStart = new DateTime(2018, 1, 1);
                    var dateTimeEnd = new DateTime(2019, 10, 1);
                    SearchByDateTime(dateTimeStart, dateTimeEnd, queryItem);

                    Task.Factory.StartNew(() => queryItem.SearchStatus = SearchStatus.Done, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ContinueWith(task =>
            {
                ExecuteCancelSearchGitHubCommand(this);
            }, uiScheduler).ConfigureAwait(false);
        }

        private void ExecuteCancelSearchGitHubCommand(object obj)
        {
            _cancelSource.Cancel();
            SetStatus(SearchStatus.None, OperationState.Idle, Visibility.Collapsed);
        }

        private void SetupGithubClient()
        {
            _gitHubClient = new GitHubClient(new ProductHeaderValue("octokit"))
            {
                //configure
                Credentials = new Credentials("", "")
            };
        }

        private void SetupQueryItems()
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(ConstantUtil.QueryItemConfigFilePath);

                var queryItemConfiguration = SerializationUtil.DeserializeDataContract<QueryItemConfiguration>(xmlDocument.InnerXml);

                foreach (var queryItem in queryItemConfiguration.QueryItems)
                {
                    QueryItems.Add(queryItem);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SearchByDateTime(DateTime dateTimeStart, DateTime dateTimeEnd, QueryItem queryItem)
        {
            var newGitHubItems = new List<InitialCoinOffering>();

            for (; dateTimeStart.Date <= dateTimeEnd.Date; dateTimeStart = dateTimeStart.AddMonths(1))
            {
                var page = 1;
                Task<SearchUsersResult> searchUsersResult;

                do
                {
                    searchUsersResult = SearchUser(queryItem, page, dateTimeStart);
                    Console.WriteLine($@"dateTimeStart: {dateTimeStart} dateTimeStart :{dateTimeStart.AddMonths(1)}");

                    var initialCoinOfferings = searchUsersResult.Result.Items.Select(item => new InitialCoinOffering
                    {
                        Label = item.Login,
                        Hyperlink = item.HtmlUrl.ToLower(),
                        InitialCoinOfferingSourceId = (int)InitialCoinOfferingSource.GitHub
                    });

                    newGitHubItems.AddRange(initialCoinOfferings);
                    page++;

                    Console.WriteLine($@"keyword: {queryItem.Keyword} page :{page} _newGitHubItems.Count: {newGitHubItems.Count}");

                    if (page <= 10)
                    {
                        continue;
                    }

                    Console.WriteLine($@"Current query item more than 1000 records, keyword: {queryItem.Keyword} total count: {searchUsersResult.Result.TotalCount}");
                    break;
                }
                while (newGitHubItems.Count < searchUsersResult.Result.TotalCount);

                InitialCoinOfferingDataAccess.Instance.Save(newGitHubItems);
            }
        }

        private async Task<SearchUsersResult> SearchUser(QueryItem queryItem, int page, DateTime dateTimeStart)
        {
            var searchUsersRequest = new SearchUsersRequest(queryItem.Keyword)
            {
                Page = page,
                Created = DateRange.Between(dateTimeStart, dateTimeStart.AddMonths(1))
            };

            try
            {
                return await _gitHubClient.Search.SearchUsers(searchUsersRequest);
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                return await _gitHubClient.Search.SearchUsers(searchUsersRequest);
            }
        }

        private void SetStatus(SearchStatus searchStatus, OperationState operationState, Visibility busyIndicatorVisibility)
        {
            foreach (var queryItem in QueryItems)
            {
                queryItem.SearchStatus = searchStatus;
            }

            OperationState = operationState;
            BusyIndicatorVisibility = busyIndicatorVisibility;
        }
    }
}
