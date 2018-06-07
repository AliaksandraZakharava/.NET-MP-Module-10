using System.Collections.Generic;

namespace NETMP.Module10.Http.HttpManager.Interfaces
{
    public interface IHtmlCrawler
    {
        IEnumerable<string> FindHtmlPageLinks(string html);
    }
}
