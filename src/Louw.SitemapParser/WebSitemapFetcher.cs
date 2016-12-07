using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Louw.SitemapParser
{
    public class WebSitemapFetcher : ISitemapFetcher
    {
        public Task<string> Fetch(Uri sitemapLocation)
        {
            //Automatically handle gzip compressed content
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };

            using (var client = new HttpClient(handler))
            {
                return client.GetStringAsync(sitemapLocation);
            }
        }
    }
}
