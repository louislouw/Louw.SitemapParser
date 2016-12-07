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
        public bool IsLoaded { get { return this.SitemapType != SitemapType.NotLoaded; } }

        public Sitemap(Uri sitemapLocation, DateTime? lastModified = null)
        {
            if (sitemapLocation == null)
                throw new ArgumentNullException("sitemapUri");

            SitemapLocation = sitemapLocation;
            LastModified = lastModified;
            SitemapType = SitemapType.NotLoaded;
            Sitemaps = Enumerable.Empty<Sitemap>();
            Items = Enumerable.Empty<SitemapItem>();
        }

        public Sitemap(IEnumerable<Sitemap> sitemaps, Uri sitemapLocation = null, DateTime? lastModified = null)
        {
            if (sitemaps == null)
                throw new ArgumentNullException("sitemaps");

            Sitemaps = sitemaps.ToList();
            SitemapLocation = sitemapLocation;
            LastModified = lastModified;
            Items = Enumerable.Empty<SitemapItem>();

            if ((sitemapLocation != null) && sitemapLocation.LocalPath.StartsWith("/robots.txt", StringComparison.OrdinalIgnoreCase))
                SitemapType = SitemapType.Robots;
            else
                SitemapType = SitemapType.Index;
        }

        public Sitemap(IEnumerable<SitemapItem> items, Uri sitemapLocation = null, DateTime? lastModified = null)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            Items = items.ToList();
            SitemapLocation = sitemapLocation;
            LastModified = lastModified;
            SitemapType = SitemapType.Items;
            Sitemaps = Enumerable.Empty<Sitemap>();
        }
    }
}
