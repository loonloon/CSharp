using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICOCrawler.DataAccess.Sqlite;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Model;

namespace ICOCrawler.DataAccess
{
    public class InitialCoinOfferingDataAccess : IDataAccess<InitialCoinOffering>
    {
        public static InitialCoinOfferingDataAccess Instance = new InitialCoinOfferingDataAccess();

        private InitialCoinOfferingDataAccess()
        {
        }

        public IEnumerable<InitialCoinOffering> Get(InitialCoinOfferingSource initialCoinOfferingSource)
        {
            using (var context = new DatabaseContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                if (context.InitialCoinOfferingDbSet == null)
                {
                    return new List<InitialCoinOffering>();
                }

                return context.InitialCoinOfferingDbSet.Where(GetFilterExpression(initialCoinOfferingSource)).ToList();
            }
        }

        public void Update(InitialCoinOffering item)
        {
            using (var context = new DatabaseContext())
            {
                try
                {
                    var existingMedium = context.InitialCoinOfferingDbSet.SingleOrDefault(x => x.No == item.No);

                    if (existingMedium == null)
                    {
                        return;
                    }

                    existingMedium.IsVerified = true;
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void Save(IEnumerable<InitialCoinOffering> newItems)
        {
            using (var context = new DatabaseContext())
            {
                if (context.InitialCoinOfferingDbSet.Any())
                {
                    var distinctInitialCoinOfferings = newItems.GroupBy(x => x.Hyperlink).Select(x => x.First());
                    var filteredInitialCoinOfferings = distinctInitialCoinOfferings.Where(p =>
                        !context.InitialCoinOfferingDbSet.Any(p2 => p2.Hyperlink.Equals(p.Hyperlink)));
                    context.InitialCoinOfferingDbSet.AddRange(filteredInitialCoinOfferings);
                }
                else
                {
                    context.InitialCoinOfferingDbSet.AddRange(newItems);
                }

                if (context.InitialCoinOfferingDbSet.Local.Any())
                {
                    var resultSaveChanges = context.SaveChanges();
                    Console.WriteLine($@"Saved: {resultSaveChanges}");
                }
            }
        }

        private static Expression<Func<InitialCoinOffering, bool>> GetFilterExpression(InitialCoinOfferingSource initialCoinOfferingSource)
        {
            Expression<Func<InitialCoinOffering, bool>> filterExpression;

            //temporary search for medium and linked in
            if (initialCoinOfferingSource == InitialCoinOfferingSource.Medium)
            {
                filterExpression = x => !x.IsVerified && (x.InitialCoinOfferingSourceId == (int)InitialCoinOfferingSource.Medium ||
                                                          x.InitialCoinOfferingSourceId == (int)InitialCoinOfferingSource.LinkedIn);
            }
            else
            {
                filterExpression = x => !x.IsVerified && x.InitialCoinOfferingSourceId == (int)initialCoinOfferingSource;
            }

            return filterExpression;
        }
    }
}
