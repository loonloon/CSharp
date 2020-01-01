using System.Runtime.Serialization;
using ICOCrawler.Model;

namespace ICOCrawler.Common
{
    [DataContract]
    public class QueryItem : ViewModelBase
    {
        [DataMember]
        public string Keyword { get; set; }

        private SearchStatus _searchStatus = SearchStatus.None;
        public SearchStatus SearchStatus
        {
            get => _searchStatus;
            set
            {
                _searchStatus = value;
                OnPropertyChanged("SearchStatus");
            }
        }
    }
}
