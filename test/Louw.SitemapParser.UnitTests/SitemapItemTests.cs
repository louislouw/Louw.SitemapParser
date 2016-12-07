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
    }
}
