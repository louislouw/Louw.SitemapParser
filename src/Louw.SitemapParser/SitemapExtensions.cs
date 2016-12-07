using System;
using System.Threading.Tasks;

namespace Louw.SitemapParser
{
    public static class SitemapExtensions
    {
        public static async Task<Sitemap> LoadAsync(this Sitemap sitemap, ISitemapFetcher fetcher = null, ISitemapParser parser = null)
        {
            if (sitemap == null)
                throw new ArgumentNullException("sitemap");

            //We are already loaded!
            if (sitemap.IsLoaded)
                return sitemap;

            if (sitemap.SitemapLocation == null)
                throw new InvalidOperationException("Sitemap location not specified");

            //Use default implementations
            if (fetcher == null)
                fetcher = new WebSitemapFetcher();
            if (parser == null)
                parser = new SitemapParser();

            string sitemapContent = await fetcher.Fetch(sitemap.SitemapLocation);
            var loadedSitemap = parser.Parse(sitemapContent, sitemap.SitemapLocation);
            return loadedSitemap;
        }
    }
}
