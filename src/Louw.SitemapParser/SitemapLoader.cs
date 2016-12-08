using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Louw.SitemapParser
{
    public class SitemapLoader
    {
        private readonly ISitemapFetcher _fetcher;
        private readonly ISitemapParser _sitemapParser;
        private readonly IRobotsTxtParser _robotsParser;

        public SitemapLoader(ISitemapFetcher fetcher = null, ISitemapParser sitemapParser = null, IRobotsTxtParser robotsParser = null)
        {
            _fetcher = fetcher != null ? fetcher : new WebSitemapFetcher();
            _sitemapParser = sitemapParser != null ? sitemapParser : new SitemapParser();
            _robotsParser = robotsParser != null ? robotsParser : new RobotsTxtParser();
        }

        public async Task<Sitemap> LoadFromRobotsTxtAsync(Uri websiteLocation)
        {
            Uri robotsTxtLocation = new Uri(websiteLocation, "/robots.txt");
            var robotsTxtContent = await _fetcher.Fetch(robotsTxtLocation);
            var sitemapLocations = _robotsParser.Parse(robotsTxtContent, robotsTxtLocation);
            var sitemaps = sitemapLocations.Select(x => new Sitemap(x));
            return new Sitemap(sitemaps, robotsTxtLocation);
        }

        public async Task<Sitemap> LoadAsync(Uri sitemapLocation)
        {
            var sitemapContent = await _fetcher.Fetch(sitemapLocation);
            var sitemap = _sitemapParser.Parse(sitemapContent, sitemapLocation);
            return sitemap;
        }

        public async Task<Sitemap> LoadAsync(Sitemap sitemap)
        {
            if (sitemap == null)
                throw new ArgumentNullException("sitemap");
            return await LoadAsync(sitemap.SitemapLocation);
        }
    }
}
