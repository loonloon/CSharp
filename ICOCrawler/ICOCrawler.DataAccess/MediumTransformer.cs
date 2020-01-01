using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Model;

namespace ICOCrawler.DataAccess
{
    public static class MediumTransformer
    {
        public static IEnumerable<InitialCoinOffering> Extract(string html, MediumSearchCategory mediumSearchCategory)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNodes = htmlDocument.DocumentNode.SelectNodes($".//*[contains(@class,'{GetNodeClass(mediumSearchCategory)}')]//li//h3");

            if (htmlNodes == null)
            {
                return new List<InitialCoinOffering>();
            }

            return from htmlNode in htmlNodes
                   let href = htmlNode.ChildNodes.FirstOrDefault()?.Attributes["href"].Value
                   let hyperlink = href?.Split('?').Take(1).FirstOrDefault()
                   select new InitialCoinOffering
                   {
                       Label = htmlNode.InnerText,
                       Hyperlink = hyperlink,
                       CreatedDate = DateTime.Now.ToString("MM/dd/yyyy"),
                       InitialCoinOfferingSourceId = (int)InitialCoinOfferingSource.Medium
                   };
        }

        private static string GetNodeClass(MediumSearchCategory mediumSearchCategory)
        {
            switch (mediumSearchCategory)
            {
                case MediumSearchCategory.Stories:
                    throw new NotImplementedException();
                case MediumSearchCategory.Users:
                    return "js-userResultsList";
                case MediumSearchCategory.Publications:
                    return "js-collectionResultsList";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mediumSearchCategory), mediumSearchCategory, null);
            }
        }
    }
}
