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

        //TODO: Image
        //TODO: Video

        public SitemapItem(Uri location, DateTime? lastModified = null, SitemapChangeFrequency? changeFrequency = null, double? priority = null)
        {
            if (priority.HasValue && ((priority.Value < 0.0) || (priority.Value > 1.0)))
                throw new ArgumentOutOfRangeException("priority");

            Location = location;
            LastModified = lastModified;
            ChangeFrequency = changeFrequency;
            Priority = priority;
        }
    }
}
