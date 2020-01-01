using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using CefSharp;
using CefSharp.Wpf;
using ICOCrawler.Common;
using ICOCrawler.DataAccess;
using ICOCrawler.Model;

namespace ICOCrawler.LinkedIn.ViewModel
{
    public class LinkedInViewModel : ViewModelBase
    {
        private int _totalFound;
        private int _maxPaging;
        private int _pagingIndex = 1;
        private const string LinkedInDefaultAddress = "https://www.linkedin.com/";
        private readonly CancellationTokenSource _cancelSource;
        private string _currentWebBrowserContent;
        private OperationState _operationState = OperationState.Idle;

        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand CancelCommand { get; }
        public ChromiumWebBrowser WebBrowser { get; set; }
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

        public LinkedInViewModel()
        {
            _cancelSource = new CancellationTokenSource();
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            WebBrowser = new ChromiumWebBrowser();
            QueryItems = new ObservableCollection<QueryItem>();
            SetupQueryItems();
        }

        private Task<bool> RunWebBrowserAsync(string webAddress)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            const int maxPageNumber = 100;

            async void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
            {
                if (!e.IsLoading && OperationState != OperationState.Idle)
                {
                    _currentWebBrowserContent = await WebBrowser.GetSourceAsync();
                    _totalFound = LinkedInTransformer.ExtractTotalFoundNumber(_currentWebBrowserContent);
                    _maxPaging = ((_totalFound - 1) / 10) + 1;
                    Console.WriteLine($@"TotalFound {_totalFound} Max Paging {_maxPaging}");

                    if (_maxPaging > maxPageNumber)
                    {
                        _maxPaging = maxPageNumber;
                    }

                    WebBrowser.LoadingStateChanged -= LoadingStateChanged;

                    if (_totalFound != 0)
                    {
                        do
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            var htmlContent = await WebBrowser.GetSourceAsync();
                            var newLinkedInList = LinkedInTransformer.Extract(htmlContent, LinkedInDefaultAddress);

                            Console.WriteLine($@"newLinkedInList {newLinkedInList.Count()}");

                            if (newLinkedInList.Any())
                            {
                                Task.Run(() => InitialCoinOfferingDataAccess.Instance.Save(newLinkedInList));
                            }

                            _pagingIndex++;

                            if (_pagingIndex > _maxPaging)
                            {
                                continue;
                            }

                            await WebBrowser.EvaluateScriptAsync($"var buttons = document.getElementsByTagName(\"button\"); for(var i=0; i<buttons.length; i++) {{if(buttons[i].innerText == \"Next\") {{buttons[i].click();}}}}");
                            await Task.Delay(TimeSpan.FromSeconds(3));
                            await WebBrowser.EvaluateScriptAsync("window.parent.parent.scrollTo(0,10000000);");
                        } while (_pagingIndex <= _maxPaging);
                    }

                    _pagingIndex = 1;
                    taskCompletionSource.SetResult(true);
                }
            }

            WebBrowser = new ChromiumWebBrowser();
            OnPropertyChanged(nameof(WebBrowser));
            WebBrowser.LoadingStateChanged += LoadingStateChanged;
            WebBrowser.Address = webAddress;
            return taskCompletionSource.Task;
        }

        private void ExecuteRefreshCommand(object obj)
        {
            WebBrowser.Address = LinkedInDefaultAddress;
        }

        private async void ExecuteSearchCommand(object obj)
        {
            SetStatus(SearchStatus.Waiting, OperationState.Running);

            foreach (var queryItem in QueryItems)
            {
                queryItem.SearchStatus = SearchStatus.InProgress;
                var webAddress = $"https://www.linkedin.com/search/results/companies/?keywords={queryItem.Keyword}&origin=SWITCH_SEARCH_VERTICAL";
                await RunWebBrowserAsync(webAddress);
                queryItem.SearchStatus = SearchStatus.Done;
                Console.WriteLine($@"{queryItem.Keyword} done.");
            }

            ExecuteCancelCommand(this);
        }

        private void ExecuteCancelCommand(object obj)
        {
            _cancelSource.Cancel();
            SetStatus(SearchStatus.None, OperationState.Idle);
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

        private void SetStatus(SearchStatus searchStatus, OperationState operationState)
        {
            foreach (var queryItem in QueryItems)
            {
                queryItem.SearchStatus = searchStatus;
            }

            OperationState = operationState;
        }
    }
}
