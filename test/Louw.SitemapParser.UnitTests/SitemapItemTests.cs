using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Louw.SitemapParser.UnitTests
{
    public class SitemapItemTests
    {
        [Fact]
        public void TestBasicCreate1()
        {
            var testUri = new Uri("http://example.com");
            var item = new SitemapItem(new Uri("http://example.com"));
            Assert.NotNull(item);
            Assert.Equal(testUri.AbsoluteUri, item.Location.AbsoluteUri);
            Assert.False(item.LastModified.HasValue);
            Assert.False(item.ChangeFrequency.HasValue);
            Assert.False(item.Priority.HasValue);
        }

        [Fact]
        public void TestBasicCreate2()
        {
            var testUri = new Uri("http://example.com");
            var lastModified = DateTime.UtcNow;
            var item = new SitemapItem(testUri, lastModified, SitemapChangeFrequency.Hourly, 0.5);
            Assert.NotNull(item);
            Assert.Equal(testUri.AbsoluteUri, item.Location.AbsoluteUri);
            Assert.True(item.LastModified.HasValue);
            Assert.True(item.ChangeFrequency.HasValue);
            Assert.True(item.Priority.HasValue);

            Assert.Equal(lastModified, item.LastModified.Value);
            Assert.Equal(SitemapChangeFrequency.Hourly, item.ChangeFrequency.Value);
            Assert.Equal(0.5, item.Priority.Value);
        }

        [Fact]
        public void TestBasicCreateBadPriority()
        {
            var testUri = new Uri("http://example.com");
            var lastModified = DateTime.UtcNow;
            Assert.Throws<ArgumentOutOfRangeException>(() => new SitemapItem(testUri, lastModified, SitemapChangeFrequency.Hourly, -0.5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new SitemapItem(testUri, lastModified, SitemapChangeFrequency.Hourly, 1.5));

            var item1 = new SitemapItem(testUri, null, null, 0.0);
            Assert.Equal(0.0, item1.Priority.Value);
        }

        [Fact]
        public void TestBasicCreatePriorityMinMax()
        {
            var testUri = new Uri("http://example.com");
            var item1 = new SitemapItem(testUri, null, null, 0.0);
            Assert.Equal(0.0, item1.Priority.Value);

            var item2 = new SitemapItem(testUri, null, null, 1.0);
            Assert.Equal(1.0, item2.Priority.Value);
        }

        [Fact]
        public void TestParseCreate1()
        {
            var testUrl = "http://example.com/";
            var item = new SitemapItem(testUrl);
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.False(item.LastModified.HasValue);
            Assert.False(item.ChangeFrequency.HasValue);
            Assert.False(item.Priority.HasValue);
        }

        [Fact]
        public void TestParseCreate2()
        {
            var testUrl = "http://example.com/";
            var item = new SitemapItem(testUrl, "2016-11-01", "hourly", "0.5");
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.True(item.LastModified.HasValue);
            Assert.True(item.ChangeFrequency.HasValue);
            Assert.True(item.Priority.HasValue);

            Assert.Equal(new DateTime(2016,11,01), item.LastModified.Value);
            Assert.Equal(SitemapChangeFrequency.Hourly, item.ChangeFrequency.Value);
            Assert.Equal(0.5, item.Priority.Value);
        }

        [Fact]
        public void TestParseCreate3()
        {
            var testUrl = "http://example.com/";
            var item = new SitemapItem(testUrl, "2004-12-23T18:30:15+00:00", "Hourly", "0");
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
        public void TestParseCreate4()
        {
            var testUrl = "http://example.com/";
            var item = new SitemapItem(testUrl, "baddate", "badfreq", "badnum");
            Assert.NotNull(item);
            Assert.Equal(testUrl, item.Location.AbsoluteUri);
            Assert.False(item.LastModified.HasValue);
            Assert.False(item.ChangeFrequency.HasValue);
            Assert.False(item.Priority.HasValue);
        }

        [Fact]
        public void TestParseCreatePriorityRanges()
        {
            var testUrl = "http://example.com/";
            var item1 = new SitemapItem(testUrl, null, null, "0.0");
            Assert.Equal(0.0, item1.Priority.Value);
            var item2 = new SitemapItem(testUrl, null, null, "1.0");
            Assert.Equal(1.0, item2.Priority.Value);

            //If priority out of range, it is adjusted
            var item3 = new SitemapItem(testUrl, null, null, "-0.5");
            Assert.Equal(0.0, item3.Priority.Value);
            var item4 = new SitemapItem(testUrl, null, null, "1.5");
            Assert.Equal(1.0, item4.Priority.Value);
        }

        [Fact]
        public void TestParseCreateLocalDates()
        {
            var testUrl = "http://example.com/";
            var item = new SitemapItem(testUrl, "2004-12-23T18:30:15+02:00", null, null);
            Assert.Equal(DateTimeKind.Utc, item.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2004, 12, 23, 16, 30, 15, DateTimeKind.Utc), item.LastModified.Value);
        }

        [Fact]
        public void TestParseCreateOutOfSpecDates()
        {
            var testUrl = "http://example.com/";
            var item1 = new SitemapItem(testUrl, "7 May, 2016", null, null);
            Assert.Equal(DateTimeKind.Utc, item1.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2016, 5, 7), item1.LastModified.Value);

            var item2 = new SitemapItem(testUrl, "7 May, 2016 16:40", null, null);
            Assert.Equal(DateTimeKind.Utc, item2.LastModified.Value.Kind);
            Assert.Equal(new DateTime(2016, 5, 7, 16, 40, 0, 0, DateTimeKind.Utc), item2.LastModified.Value);
        }
    }
}
