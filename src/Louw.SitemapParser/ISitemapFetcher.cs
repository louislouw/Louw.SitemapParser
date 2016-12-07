using System;
using System.Threading.Tasks;

namespace Louw.SitemapParser
{
    public interface ISitemapFetcher
    {
        Task<string> Fetch(Uri sitemapLocation);
    }
}
