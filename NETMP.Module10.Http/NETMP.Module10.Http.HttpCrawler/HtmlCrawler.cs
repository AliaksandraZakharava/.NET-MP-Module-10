using System.Collections.Generic;
using NETMP.Module10.Http.HttpManager.Interfaces;
using CsQuery;

namespace NETMP.Module10.Http.HttpManager
{
    public class HtmlCrawler : IHtmlCrawler
    {
        public IEnumerable<string> FindHtmlPageLinks(string html)
        {
            var csQuery = CQ.Create(html);

            foreach (var domObject in csQuery.Find("a"))
            {
                yield return domObject.GetAttribute("href");
            }
        }
    }
}
