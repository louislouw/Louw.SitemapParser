using System;

namespace Louw.SitemapParser
{
    public interface ISitemapParser
    {
        Sitemap Parse(string sitemapData, Uri sitemapLocation = null);
    }
}
