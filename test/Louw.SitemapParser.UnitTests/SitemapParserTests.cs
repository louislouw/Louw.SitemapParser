using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Louw.SitemapParser.UnitTests
{
    public class SitemapParserTests
    {
        #region SitemapParser.ParseSitemapItemFields
        [Fact]
        public void TestParseItemCreate1()
        {
            var testUrl = "http://example.com/";
            var item =  SitemapParser.ParseSitemapItemFields(null, testUrl);
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.False(item.LastModified.HasValue);
            Assert.False(item.ChangeFrequency.HasValue);
            Assert.False(item.Priority.HasValue);
        }

        [Fact]
        public void TestParseItemCreate2()
        {
            var testUrl = "http://example.com/";
            var item = SitemapParser.ParseSitemapItemFields(null, testUrl, "2016-11-01", "hourly", "0.5");
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.True(item.LastModified.HasValue);
            Assert.True(item.ChangeFrequency.HasValue);
            Assert.True(item.Priority.HasValue);

            Assert.Equal(new DateTime(2016, 11, 01), item.LastModified.Value);
            Assert.Equal(SitemapChangeFrequency.Hourly, item.ChangeFrequency.Value);
            Assert.Equal(0.5, item.Priority.Value);
        }

        [Fact]
        public void TestParseItemCreate3()
        {
            var testUrl = "http://example.com/";
            var item = SitemapParser.ParseSitemapItemFields(null, testUrl, "2004-12-23T18:30:15+00:00", "Hourly", "0");
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.True(item.LastModified.HasValue);
            Assert.True(item.ChangeFrequency.HasValue);
            Assert.True(item.Priority.HasValue);

            Assert.Equal(DateTimeKind.Utc, item.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 12, 23, 18, 30, 15, DateTimeKind.Utc), item.LastModified.Value);
            Assert.Equal(SitemapChangeFrequency.Hourly, item.ChangeFrequency.Value);
            Assert.Equal(0.0, item.Priority.Value);
        }

        [Fact]
        public void TestParseItemCreate4()
        {
            var testUrl = "http://example.com/";
            var item = SitemapParser.ParseSitemapItemFields(null, testUrl, "baddate", "badfreq", "badnum");
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.False(item.LastModified.HasValue);
            Assert.False(item.ChangeFrequency.HasValue);
            Assert.False(item.Priority.HasValue);
        }

        [Fact]
        public void TestParseItemCreatePriorityRanges()
        {
            var testUrl = "http://example.com/";
            var item1 = SitemapParser.ParseSitemapItemFields(null, testUrl, null, null, "0.0");
            Assert.Equal(0.0, item1.Priority.Value);
            var item2 = SitemapParser.ParseSitemapItemFields(null, testUrl, null, null, "1.0");
            Assert.Equal(1.0, item2.Priority.Value);

            //If priority out of range, it is adjusted
            var item3 = SitemapParser.ParseSitemapItemFields(null, testUrl, null, null, "-0.5");
            Assert.Equal(0.0, item3.Priority.Value);
            var item4 = SitemapParser.ParseSitemapItemFields(null, testUrl, null, null, "1.5");
            Assert.Equal(1.0, item4.Priority.Value);
        }

        [Fact]
        public void TestParseItemCreateLocalDates()
        {
            var testUrl = "http://example.com/";
            var item = SitemapParser.ParseSitemapItemFields(null, testUrl, "2004-12-23T18:30:15+02:00", null, null);
            Assert.Equal(DateTimeKind.Utc, item.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 12, 23, 16, 30, 15, DateTimeKind.Utc), item.LastModified.Value);
        }

        [Fact]
        public void TestParseItemCreateOutOfSpecDates()
        {
            var testUrl = "http://example.com/";
            var item1 = SitemapParser.ParseSitemapItemFields(null, testUrl, "7 May, 2016", null, null);
            Assert.Equal(DateTimeKind.Utc, item1.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2016, 5, 7), item1.LastModified.Value);

            var item2 = SitemapParser.ParseSitemapItemFields(null, testUrl, "7 May, 2016 16:40", null, null);
            Assert.Equal(DateTimeKind.Utc, item2.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2016, 5, 7, 16, 40, 0, 0, DateTimeKind.Utc), item2.LastModified.Value);
        }

        [Fact]
        public void TestParseItemRelativePath()
        {
            //Note: Relative paths only supported if baseUri is supplied
            Uri baseUri = new Uri("http://example.com/subdir/sitemap.xml");

            var item1 = SitemapParser.ParseSitemapItemFields(baseUri, "/path/blog");
            Assert.NotNull(item1);
            Assert.Equal("http://example.com/path/blog", item1.Location.AbsoluteUri);

            var item2 = SitemapParser.ParseSitemapItemFields(baseUri, "path/abc");
            Assert.NotNull(item2);
            Assert.Equal("http://example.com/subdir/path/abc", item2.Location.AbsoluteUri);
        }

        [Fact]
        public void TestParseItemInvalidLocation()
        {
            //Not valid Uri
            var item1 = SitemapParser.ParseSitemapItemFields(null, "http://bad url.com/");
            Assert.Null(item1);

            //Relative paths only supported if baseUri is supplied
            var item2 = SitemapParser.ParseSitemapItemFields(null, "/path/blog");
            Assert.Null(item2);
        }
        #endregion

        #region SitemapParser.ParseSitemapFields
        [Fact]
        public void TestParseSitemapCreate1()
        {
            string location = "http://example.com/sitemap.xml";
            var sitemap = SitemapParser.ParseSitemapFields(null, location, null);
            Assert.NotNull(sitemap);
            Assert.Equal(location, sitemap.SitemapLocation.AbsoluteUri);
            Assert.False(sitemap.LastModified.HasValue);
            Assert.Equal(SitemapType.NotLoaded, sitemap.SitemapType);
            Assert.Empty(sitemap.Sitemaps);
            Assert.Empty(sitemap.Items);
            Assert.False(sitemap.IsLoaded);
        }

        [Fact]
        public void TestParseSitemapCreate2()
        {
            string location = "http://example.com/sitemap.xml";
            var sitemap = SitemapParser.ParseSitemapFields(null, location, "2016-09-11");
            Assert.NotNull(sitemap);
            Assert.Equal(location, sitemap.SitemapLocation.AbsoluteUri);
            Assert.True(sitemap.LastModified.HasValue);
            Assert.Equal(new DateTime(2016, 9, 11), sitemap.LastModified.Value);
            Assert.Equal(SitemapType.NotLoaded, sitemap.SitemapType);
            Assert.Empty(sitemap.Sitemaps);
            Assert.Empty(sitemap.Items);
            Assert.False(sitemap.IsLoaded);
        }

        [Fact]
        public void TestParseSitemapCreateDateFormats()
        {
            string location = "http://example.com/sitemap.xml";
            var sitemap1 = SitemapParser.ParseSitemapFields(null, location, "2004-10-01T18:23:17+00:00");
            Assert.Equal(DateTimeKind.Utc, sitemap1.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 10, 1,18,23,17, DateTimeKind.Utc), sitemap1.LastModified.Value);

            var sitemap2 = SitemapParser.ParseSitemapFields(null, location, "2004-10-01T18:23:17+02:00");
            Assert.Equal(DateTimeKind.Utc, sitemap2.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 10, 1, 16, 23, 17, DateTimeKind.Utc), sitemap2.LastModified.Value);
        }

        [Fact]
        public void TestParseSitemapCreateOutOfSpecDateFormats()
        {
            string location = "http://example.com/sitemap.xml";
            var sitemap1 = SitemapParser.ParseSitemapFields(null, location, "2004-10-01 18:23:17");
            Assert.Equal(DateTimeKind.Utc, sitemap1.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 10, 1, 18, 23, 17, DateTimeKind.Utc), sitemap1.LastModified.Value);

            var sitemap2 = SitemapParser.ParseSitemapFields(null, location, "7 May, 2016 18:23");
            Assert.Equal(DateTimeKind.Utc, sitemap2.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2016, 5, 7, 18, 23, 00, DateTimeKind.Utc), sitemap2.LastModified.Value);
        }

        [Fact]
        public void TestParseSitemapCreateRelativePaths()
        {
            Uri baseUri = new Uri("http://example.com/subdir/sitemap.xml");
            var sitemap1 = SitemapParser.ParseSitemapFields(baseUri, "/map1.xml", null);
            Assert.Equal("http://example.com/map1.xml", sitemap1.SitemapLocation.AbsoluteUri);

            var sitemap2 = SitemapParser.ParseSitemapFields(baseUri, "path/map2.xml", null);
            Assert.Equal("http://example.com/subdir/path/map2.xml", sitemap2.SitemapLocation.AbsoluteUri);
        }

        [Fact]
        public void TestParseSitemapBadLocation()
        {
            var sitemap1 = SitemapParser.ParseSitemapFields(null, "http://bad url/map1.xml", null);
            Assert.Null(sitemap1);

            //Only support relative paths if baseUri supplied
            var sitemap2 = SitemapParser.ParseSitemapFields(null, "map2.xml", null);
            Assert.Null(sitemap2);
        }
        #endregion

        #region Sitemap Parsing Tests
        [Fact]
        public async Task TestParseSitemapIndex()
        {
            DateTime expectedLastModifiedMax = new DateTime(2016, 11, 30, 20, 19, 52, 0, DateTimeKind.Utc);
            DateTime expectedLastModifiedMin = new DateTime(2016, 11, 29, 20, 19, 52, 0, DateTimeKind.Utc);
            Uri sitemapLocation = new Uri("http://example.com/sitemap_index.xml");
            var sitemap1 = new Sitemap(sitemapLocation);
            Assert.False(sitemap1.IsLoaded);

            var fetcher = new mocks.MockSitemapFetcher();
            var sitemap2 = await sitemap1.LoadAsync(fetcher);
            Assert.NotNull(sitemap2);
            Assert.True(sitemap2.IsLoaded);
            Assert.Equal(SitemapType.Index, sitemap2.SitemapType);
            Assert.Equal(sitemapLocation, sitemap2.SitemapLocation);
            Assert.True(sitemap2.LastModified.HasValue);
            Assert.Equal(expectedLastModifiedMax, sitemap2.LastModified.Value);
            Assert.Empty(sitemap2.Items);
            Assert.NotNull(sitemap2.Sitemaps);

            var sitemaps = sitemap2.Sitemaps.ToList();
            
            Assert.Equal(2, sitemaps.Count);
            Assert.Equal("http://example/post-sitemap.xml", sitemaps[0].SitemapLocation.AbsoluteUri);
            Assert.Equal(expectedLastModifiedMax, sitemaps[0].LastModified.Value);
            Assert.Equal(SitemapType.NotLoaded, sitemaps[0].SitemapType);
            Assert.Empty(sitemaps[0].Items);
            Assert.Empty(sitemaps[0].Sitemaps);

            Assert.Equal("http://example.com/category-sitemap.xml", sitemaps[1].SitemapLocation.AbsoluteUri);
            Assert.Equal(expectedLastModifiedMin, sitemaps[1].LastModified.Value);
            Assert.Equal(SitemapType.NotLoaded, sitemaps[1].SitemapType);
            Assert.Empty(sitemaps[1].Items);
            Assert.Empty(sitemaps[1].Sitemaps);
        }
        #endregion
    }
}
