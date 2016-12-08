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

            var loader = new SitemapLoader(fetcher, parser);
            return await loader.LoadAsync(sitemap);
        }
    }
}
