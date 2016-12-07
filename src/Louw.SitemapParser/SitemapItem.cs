using System;
using System.Globalization;

namespace Louw.SitemapParser
{
    public class SitemapItem
    {
        public Uri Location { get; private set; }
        public DateTime? LastModified { get; private set; }
        public SitemapChangeFrequency? ChangeFrequency { get; private set; }
        public double? Priority { get; private set; }

        public SitemapItem(Uri location, DateTime? lastModified = null, SitemapChangeFrequency? changeFrequency = null, double? priority = null)
        {
            if (priority.HasValue && ((priority.Value < 0.0) || (priority.Value > 1.0)))
                throw new ArgumentOutOfRangeException("priority");

            Location = location;
            LastModified = lastModified;
            ChangeFrequency = changeFrequency;
            Priority = priority;
        }

        public SitemapItem(string location, string lastModified = null, string changeFrequency = null, string priority = null)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentException("location may not be null or empty", "location");

            Uri parsedLocation;
            if (!Uri.TryCreate(location, UriKind.Absolute, out parsedLocation))
                throw new ArgumentException("location not valid Uri");
            Location = parsedLocation;

            LastModified = SafeDateTimeParse(lastModified);

            SitemapChangeFrequency parsedChangeFrequency;
            if (SitemapChangeFrequency.TryParse(changeFrequency, true, out parsedChangeFrequency))
                ChangeFrequency = parsedChangeFrequency;

            double parsedPriority;
            if (double.TryParse(priority, out parsedPriority))
            {
                parsedPriority = Math.Max(0.0, parsedPriority);
                parsedPriority = Math.Min(1.0, parsedPriority);
                Priority = parsedPriority;
            }
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
    }
}
