using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ICOCrawler.Common;
using ICOCrawler.DataAccess;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Model;
using ResourceString = ICOCrawler.Common.Properties.Resources;

namespace ICOCrawler.Medium.ViewModel
{
    public class CheckListViewModel : ViewModelBase
    {
        private Visibility _busyIndicatorVisibility = Visibility.Collapsed;

        public ICommand SearchCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand OpenHyperlinkCommand { get; }
        public ObservableCollection<InitialCoinOffering> IcoCollection { get; set; }
        public string TotalIcoCount => string.Format(ResourceString.TotalNumber, IcoCollection.Count);
        public Visibility BusyIndicatorVisibility
        {
            get => _busyIndicatorVisibility;
            set
            {
                _busyIndicatorVisibility = value;
                OnPropertyChanged("BusyIndicatorVisibility");
            }
        }

        public CheckListViewModel()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            UpdateCommand = new RelayCommand(ExecuteUpdateCommand);
            OpenHyperlinkCommand = new RelayCommand(ExecuteOpenHyperlinkCommand);
            IcoCollection = new ObservableCollection<InitialCoinOffering>();
        }

        public virtual async void ExecuteSearchCommand(object obj)
        {
            if (BusyIndicatorVisibility == Visibility.Visible)
            {
                return;
            }

            IcoCollection.Clear();
            BusyIndicatorVisibility = Visibility.Visible;
            var initialCoinOfferings = await Task.Run(() => InitialCoinOfferingDataAccess.Instance.Get((InitialCoinOfferingSource)obj));

            foreach (var initialCoinOffering in initialCoinOfferings)
            {
                IcoCollection.Add(initialCoinOffering);
            }

            BusyIndicatorVisibility = Visibility.Collapsed;
            OnPropertyChanged("TotalIcoCount");
        }

        public virtual void ExecuteUpdateCommand(object obj)
        {
            var messageBoxResult = MessageBox.Show(ResourceString.UpdateICOMessage, ResourceString.ICOResearchTool, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            var icoNo = (int)obj;
            var targetIco = IcoCollection.SingleOrDefault(x => x.No == icoNo);

            Task.Run(() => InitialCoinOfferingDataAccess.Instance.Update(targetIco));
            IcoCollection.RemoveAt(IcoCollection.IndexOf(targetIco));
            OnPropertyChanged("TotalIcoCount");
        }

        public virtual void ExecuteOpenHyperlinkCommand(object obj)
        {
            try
            {
                if (obj is InitialCoinOffering selectedIco)
                {
                    Task.Run(() => Process.Start(new ProcessStartInfo(selectedIco.Hyperlink)));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
