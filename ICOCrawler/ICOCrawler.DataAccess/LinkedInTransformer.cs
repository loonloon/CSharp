using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ICOCrawler.DataAccess.Sqlite.Model;
using ICOCrawler.Model;

namespace ICOCrawler.DataAccess
{
    public static class LinkedInTransformer
    {
        public static IEnumerable<InitialCoinOffering> Extract(string html, string linkedInDefaultAddress)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNodes = htmlDocument.DocumentNode.SelectNodes(".//*[contains(@class,'search-result__wrapper')]");

            if (htmlNodes == null)
            {
                return new List<InitialCoinOffering>();
            }

            var list = from htmlNode in htmlNodes
                       let href = htmlNode.ChildNodes[1].ChildNodes[1].Attributes["href"].Value
                       let labelWithSpaces = htmlNode.ChildNodes[3].InnerText.Replace("\n", " ")
                       let regex = new Regex("[ ]{2,}", RegexOptions.None)
                       let label = regex.Replace(labelWithSpaces, " ").Trim()
                       let hyperlink = $"{string.Join("", linkedInDefaultAddress.Take(linkedInDefaultAddress.Length - 1))}{href}"
                       select new InitialCoinOffering { Label = label, Hyperlink = hyperlink, CreatedDate = DateTime.Now.ToString("MM/dd/yyyy"), InitialCoinOfferingSourceId = (int)InitialCoinOfferingSource.LinkedIn };

            return list;
        }

        public static int ExtractTotalFoundNumber(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//h3[contains(@class, 'search-results__total')]");

            if (htmlNode == null)
            {
                return 0;
            }

            var splitWords = htmlNode.InnerText.Trim().Replace(",", "").Split(' ');
            return Convert.ToInt32(splitWords[1]);
        }

        public static int ExtractYear(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//p[contains(@class, 'org-about-company-module__founded')]");
            return htmlNode == null ? 0 : Convert.ToInt16(htmlNode.InnerText);
        }
    }
}
