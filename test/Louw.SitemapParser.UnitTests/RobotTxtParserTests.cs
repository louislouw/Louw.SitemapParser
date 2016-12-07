using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Louw.SitemapParser.UnitTests
{
    public class RobotTxtParserTests
    {
        [Fact]
        public void TestParse()
        {
            var robotsTxtUri = new Uri("http://example.com/robots.txt");
            var robotsData = File.ReadAllText(@"testdata\Robots.txt");
            Assert.False(string.IsNullOrEmpty(robotsData));

            IRobotsTxtParser parser = new RobotsTxtParser();
            var sitemaps = parser.Parse(robotsData, robotsTxtUri).ToList();

            int c = 0;
            Assert.Equal(7, sitemaps.Count);
            Assert.Equal("http://example.com/testmap.xml", sitemaps[c++].AbsoluteUri);
            Assert.Equal("http://anothersite.com/CaseSenSitive.Xml", sitemaps[c++].AbsoluteUri);
            Assert.Equal("http://blog.example.com/another-subdomain.xml", sitemaps[c++].AbsoluteUri);

            Assert.Equal("http://example.com/testmap.xml", sitemaps[c++].AbsoluteUri);
            Assert.Equal("http://example.com/CaseSenSitive.Xml", sitemaps[c++].AbsoluteUri);
            Assert.Equal("http://example.com/nospace.xml", sitemaps[c++].AbsoluteUri);

            Assert.Equal("http://example.com/map.xml", sitemaps[c++].AbsoluteUri); //Relative path resolves to base only
        }
    }
}
