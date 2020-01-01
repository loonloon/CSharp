using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using CefSharp;
using CefSharp.Wpf;
using ICOCrawler.Common;
using ICOCrawler.DataAccess;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Model;

namespace ICOCrawler.Medium.ViewModel
{
    public class MediumViewModel : ViewModelBase
    {
        private DispatcherTimer _timer;
        private MediumSearchCategory _selectedSearchCategory = MediumSearchCategory.Publications;
        private OperationState _operationState = OperationState.Idle;

        public ICommand SearchCommand { get; }
        public ICommand CancelCommand { get; }
        public ChromiumWebBrowser WebBrowser { get; set; }
        public ObservableCollection<QueryItem> QueryItems { get;} 

        public MediumSearchCategory SelectedSearchCategory
        {
            get => _selectedSearchCategory;
            set
            {
                _selectedSearchCategory = value;
                OnPropertyChanged("SelectedSearchCategory");
            }
        }

        public OperationState OperationState
        {
            get => _operationState;
            set
            {
                _operationState = value;
                OnPropertyChanged("OperationState");
            }
        }

        public MediumViewModel()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            QueryItems = new ObservableCollection<QueryItem>();
            SetupQueryItems();
        }

        private async void ExecuteSearchCommand(object obj)
        {
            SetStatus(SearchStatus.Waiting, OperationState.Running);

            foreach (var queryItem in QueryItems)
            {
                queryItem.SearchStatus = SearchStatus.InProgress;
                var webAddress = $"https://medium.com/search/{SelectedSearchCategory.ToString().ToLower()}?q={queryItem.Keyword}";
                var newMediums = await RunWebBrowserAsync(webAddress);

                if (newMediums.Any())
                {
                    Task.Run(() => InitialCoinOfferingDataAccess.Instance.Save(newMediums));
                }

                WebBrowser.Dispose();
                queryItem.SearchStatus = SearchStatus.Done;
                _timer = null;
            }

            ExecuteCancelCommand(this);
        }

        private Task<IEnumerable<InitialCoinOffering>> RunWebBrowserAsync(string webAddress)
        {
            var taskCompletionSource = new TaskCompletionSource<IEnumerable<InitialCoinOffering>>();
            void LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
            {
                if (!e.IsLoading && OperationState != OperationState.Idle)
                {
                    var equalContentAttempt = 0;
                    const int maxContentAttemptNo = 5;
                    var currentWebBrowserContent = string.Empty;

                    _timer = new DispatcherTimer();
                    _timer.Tick += async delegate
                    {
                        try
                        {
                            var htmlContent = await WebBrowser.GetSourceAsync();

                            if (!currentWebBrowserContent.Equals(htmlContent))
                            {
                                equalContentAttempt = 0;
                                currentWebBrowserContent = await WebBrowser.GetSourceAsync();
                                await WebBrowser.EvaluateScriptAsync("window.parent.parent.scrollTo(0,10000000);");
                            }
                            else
                            {
                                equalContentAttempt++;
                            }

                            if (equalContentAttempt >= maxContentAttemptNo)
                            {
                                _timer?.Stop();
                                WebBrowser.LoadingStateChanged -= LoadingStateChanged;
                                var newMediums = MediumTransformer.Extract(currentWebBrowserContent, SelectedSearchCategory);
                                taskCompletionSource.TrySetResult(newMediums);
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            taskCompletionSource.TrySetResult(new List<InitialCoinOffering>());
                        }
                    };

                    _timer.Interval = TimeSpan.FromSeconds(5);
                    _timer.Start();
                }
            }

            WebBrowser = new ChromiumWebBrowser();
            OnPropertyChanged(nameof(WebBrowser));
            WebBrowser.LoadingStateChanged += LoadingStateChanged;
            WebBrowser.Address = webAddress;
            return taskCompletionSource.Task;
        }

        private void ExecuteCancelCommand(object obj)
        {
            _timer?.Stop();
            _timer = null;
            WebBrowser.Address = "about:blank";

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
