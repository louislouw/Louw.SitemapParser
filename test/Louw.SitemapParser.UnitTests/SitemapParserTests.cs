using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Louw.SitemapParser.UnitTests
{
    public class SitemapParserTests
    {
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
    }
}
