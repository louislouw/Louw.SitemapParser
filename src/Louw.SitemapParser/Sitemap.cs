using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Louw.SitemapParser
{
    public class Sitemap
    {
        public Uri SitemapLocation { get; }
        public SitemapType SitemapType { get; }
        public IEnumerable<Sitemap> Sitemaps { get; }
        public IEnumerable<SitemapItem> Items { get; }
        public DateTime? LastModified { get; }

        public Sitemap(Uri sitemapLocation, DateTime? lastModified = null)
        {
            if (sitemapLocation == null)
                throw new ArgumentNullException("sitemapUri");

            SitemapLocation = sitemapLocation;
            LastModified = lastModified;
            SitemapType = SitemapType.NotLoaded;
        }

        public Sitemap(IEnumerable<Sitemap> sitemaps, Uri sitemapLocation = null, DateTime? lastModified = null)
        {
            if (sitemaps == null)
                throw new ArgumentNullException("sitemaps");

            Sitemaps = sitemaps.ToList();
            SitemapLocation = sitemapLocation;
            LastModified = lastModified;

            if (sitemapLocation != null && sitemapLocation.LocalPath.StartsWith("robots.txt", StringComparison.OrdinalIgnoreCase))
                SitemapType = SitemapType.Robots;
            else
                SitemapType = SitemapType.Index;
        }

        public Sitemap(IEnumerable<SitemapItem> items, Uri sitemapLocation = null)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            Items = items.ToList();
            SitemapLocation = sitemapLocation;
            LastModified = Items.Where(x => x.LastModified.HasValue).Max(x => x.LastModified.Value);
            SitemapType = SitemapType.Items;
        }

        public async Task<Sitemap> LoadAsync()
        {
            //We are already loaded!
            if (SitemapType != SitemapType.NotLoaded)
                return this;

            if (SitemapLocation == null)
                throw new InvalidOperationException("Sitemap location not specified");

            return await Task.FromResult<Sitemap>(null);
        }
    }
}
