using System;
using System.Collections.Generic;
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
            var loader = new SitemapLoader();
            Sitemap sitemap = await loader.LoadFromRobotsTxtAsync(new Uri("https://www.google.com"));
            Assert.Equal(SitemapType.RobotsTxt, sitemap.SitemapType);
            Assert.Empty(sitemap.Items); //Robots.txt can only link to Sitemaps 
            Assert.NotEmpty(sitemap.Sitemaps); //We expect at least some sitemaps to be in list

            Sitemap firstSitemap = sitemap.Sitemaps.First();
            Assert.False(firstSitemap.IsLoaded); //We only have link, not yet loaded
            Assert.Empty(sitemap.Items); //Items will still be empty

            var firstLoadedSitemap = await loader.LoadAsync(firstSitemap.SitemapLocation);
            Assert.True(firstLoadedSitemap.IsLoaded); //Now items are loaded!

            //We have to check type as we can either have links to other sitemaps
            //or links to actual sitemap items (i.e. links to content)
            switch(firstLoadedSitemap.SitemapType)
            {
                case SitemapType.Index: Assert.NotEmpty(firstLoadedSitemap.Sitemaps); break;
                case SitemapType.Items: Assert.NotEmpty(firstLoadedSitemap.Items); break;
                default: throw new NotSupportedException($"SitemapType {firstLoadedSitemap.SitemapType} not expected here");
            }
        }
    }
}
