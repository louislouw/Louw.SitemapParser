using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Louw.SitemapParser
{
    public interface ISitemapParser
    {

    }


    public class SitemapParser : ISitemapParser
    {
        #region Element Names
        private const string SitemapSchema = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly XName SitemapIndexName = XName.Get("sitemapindex", SitemapSchema);
        private readonly XName UrlSetName = XName.Get("urlset", SitemapSchema);
        private readonly XName UrlName = XName.Get("url", SitemapSchema);
        private readonly XName LocationName = XName.Get("loc", SitemapSchema);
        private readonly XName LastModifiedName = XName.Get("lastmod", SitemapSchema);
        private readonly XName ChangeFrequencyName = XName.Get("changefreq", SitemapSchema);
        private readonly XName PriorityName = XName.Get("priority", SitemapSchema);
        #endregion

        public Sitemap Parse(string sitemapData, Uri sitemapUri = null)
        {
            if (string.IsNullOrWhiteSpace(sitemapData))
                return null;

            try
            {
                XElement sitemapXElement = XElement.Parse(sitemapData);

                //Check if this is Index Sitemap
                if (sitemapXElement.Element(SitemapIndexName) != null)
                {
                    return ParseIndexSitemap(sitemapXElement, sitemapUri);
                }

                //Check if this is Normal Sitemap with items
                if(sitemapXElement.Element(UrlSetName) != null)
                {
                    return ParseSitemapItems(sitemapXElement, sitemapUri);
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return null;
        }

        public static Sitemap ParseSitemapFields(Uri baseUri, string location, string lastModified)
        {
            if (string.IsNullOrEmpty(location))
                return null;

            Uri parsedLocation = SafeUriParse(baseUri, location);
            if (parsedLocation == null)
                return null;

            DateTime? parsedLastModified = SafeDateTimeParse(lastModified);

            return new Sitemap(parsedLocation, parsedLastModified);
        }

        public static SitemapItem ParseSitemapItemFields(Uri baseUri, string location, string lastModified = null, string changeFrequency = null, string priority = null)
        {
            if (string.IsNullOrEmpty(location))
                return null;

            Uri parsedLocation = SafeUriParse(baseUri, location);
            if (parsedLocation == null)
                return null;

            DateTime? parsedLastModified = SafeDateTimeParse(lastModified);

            SitemapChangeFrequency? parsedChangeFrequency = null;
            SitemapChangeFrequency triedChangeFrequency;
            if (SitemapChangeFrequency.TryParse(changeFrequency, true, out triedChangeFrequency))
                parsedChangeFrequency = triedChangeFrequency;

            double? parsedPriority = null;
            double triedPriority;
            if (double.TryParse(priority, out triedPriority))
            {
                triedPriority = Math.Max(0.0, triedPriority);
                triedPriority = Math.Min(1.0, triedPriority);
                parsedPriority = triedPriority;
            }

            return new SitemapItem(parsedLocation, parsedLastModified, parsedChangeFrequency, parsedPriority);
        }

        #region Private methods
        private Sitemap ParseIndexSitemap(XElement sitemapXElement, Uri sitemapLocation)
        {
            var sitemaps = new List<Sitemap>();
            foreach (var urlElement in sitemapXElement.Elements(UrlName))
            {
                var locElement = urlElement.Element(LocationName);
                if (locElement == null || string.IsNullOrWhiteSpace(locElement.Value))
                    continue;

                var lastmodElement = urlElement.Element(LastModifiedName);

                Sitemap sitemap = ParseSitemapFields(sitemapLocation, locElement?.Value, lastmodElement?.Value);
                if(sitemap!=null)
                    sitemaps.Add(sitemap);
            }

            return new Sitemap(sitemaps, sitemapLocation);
        }

        private Sitemap ParseSitemapItems(XElement sitemapXElement, Uri sitemapLocation)
        {
            var sitemapItems = new List<SitemapItem>();
            foreach (var urlElement in sitemapXElement.Elements(UrlName))
            {
                var locElement = urlElement.Element(LocationName);
                if (locElement == null || string.IsNullOrWhiteSpace(locElement.Value))
                    continue;

                var lastmodElement = urlElement.Element(LastModifiedName);
                var changefreqElement = urlElement.Element(ChangeFrequencyName);
                var priorityElement = urlElement.Element(PriorityName);
                var sitemapItem = ParseSitemapItemFields(sitemapLocation, locElement?.Value, lastmodElement?.Value, changefreqElement?.Value, priorityElement?.Value);
                sitemapItems.Add(sitemapItem);
            }

            return new Sitemap(sitemapItems, sitemapLocation);
        }

        private static Uri SafeUriParse(Uri baseUri, string location)
        {
            Uri parsedLocation;
            if (baseUri != null)
            {
                if (!Uri.TryCreate(baseUri, location, out parsedLocation))
                    return null;
            }
            else
            {
                if (!Uri.TryCreate(location, UriKind.Absolute, out parsedLocation))
                    return null;
            }

            return parsedLocation;
        }

        private static DateTime? SafeDateTimeParse(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
                return null;

            DateTime result;
            if (DateTime.TryParseExact(dateTimeString,
                new string[] {
                    "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
                    "yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
                    "yyyy-MM-dd'T'HH:mm:sszzz",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd"
                },
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
                return result;

            //Do our best to also parse "out of spec" dates
            if (DateTime.TryParse(dateTimeString, out result))
            {
                if (result.Kind == DateTimeKind.Unspecified)
                    result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                return result;
            }

            return null;
        }
        #endregion
    }
}
