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
        private readonly string _userAgent;

        public WebSitemapFetcher()
        {
            _userAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";
        }

        public WebSitemapFetcher(string userAgent)
        {
            _userAgent = userAgent;
        }

        public async Task<string> Fetch(Uri sitemapLocation)
        {
            //Automatically handle gzip compressed content
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate 
            };

            using (var client = new HttpClient(handler) )
            {
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                //client.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1");

                return await client.GetStringAsync(sitemapLocation);
            }
        }
    }
}
