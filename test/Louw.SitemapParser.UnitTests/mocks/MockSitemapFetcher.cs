using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Louw.SitemapParser.UnitTests.mocks
{
    public class MockSitemapFetcher : ISitemapFetcher
    {
        public Task<string> Fetch(Uri sitemapLocation)
        {
            //Simply translate path to folder containing testdata
            string path = sitemapLocation.LocalPath;
            string filename = string.Concat("testdata", path);
            string sitemapContent = File.ReadAllText(filename, System.Text.Encoding.UTF8);

            return Task.FromResult(sitemapContent);
        }
    }
}
