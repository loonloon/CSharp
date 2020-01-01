using System.Collections.Generic;
using ICOCrawler.Model;

namespace ICOCrawler.DataAccess
{
    public interface IDataAccess<T>
    {
        IEnumerable<T> Get(InitialCoinOfferingSource initialCoinOfferingSource);
        void Update(T item);
        void Save(IEnumerable<T> newItems);
    }
}
