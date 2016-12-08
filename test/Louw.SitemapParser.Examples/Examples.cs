using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Louw.SitemapParser.Examples
{
    public class Examples
    {
        [Fact]
        public async Task BasicExample()
        {
            var sitemapLink = new Sitemap(new Uri("https://www.google.com/sitemap.xml"));
            var loadedSitemap = await sitemapLink.LoadAsync();

            if (loadedSitemap.SitemapType == SitemapType.Index)
                Debug.WriteLine($"Sitemap Index contains {loadedSitemap.Sitemaps.Count()} entries");
            else if (loadedSitemap.SitemapType == SitemapType.Items)
                Debug.WriteLine($"Sitemap contains {loadedSitemap.Items.Count()} content locations");
        }

        [Fact]
        public async Task RobotsTxtExample()
        {
            var loader = new SitemapLoader();
            Sitemap robotSitemap = await loader.LoadFromRobotsTxtAsync(new Uri("https://www.google.com"));
            Assert.Equal(SitemapType.RobotsTxt, robotSitemap.SitemapType);
            Assert.NotEmpty(robotSitemap.Sitemaps); //We expect at least some Sitemaps to be in list
            Assert.Empty(robotSitemap.Items); //Robots.txt can only link to Sitemaps  (Not content items)

            Sitemap firstSitemap = robotSitemap.Sitemaps.First();
            Assert.False(firstSitemap.IsLoaded); //We only have sitemap location. Contents not yet loaded nor parsed

            var firstLoadedSitemap = await loader.LoadAsync(firstSitemap);
            Assert.True(firstLoadedSitemap.IsLoaded); //Now items are loaded!

            //We have to check type as we can either have links to other sitemaps (i.e. index sitemaps) 
            //-or- links to actual sitemap items (i.e. links to content)
            switch (firstLoadedSitemap.SitemapType)
            {
                case SitemapType.Index: Assert.NotEmpty(firstLoadedSitemap.Sitemaps); break;
                case SitemapType.Items: Assert.NotEmpty(firstLoadedSitemap.Items); break;
                default: throw new NotSupportedException($"SitemapType {firstLoadedSitemap.SitemapType} not expected here");
            }
        }
    }
}
