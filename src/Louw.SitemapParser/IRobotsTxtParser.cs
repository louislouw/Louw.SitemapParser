using System;
using System.Collections.Generic;

namespace Louw.SitemapParser
{
    public interface IRobotsTxtParser
    {
        /// <summary>
        /// Parse Robots.txt content and extracts Sitemap locations.
        /// </summary>
        /// <param name="robotsTxt">Contents of Robots.txt file</param>
        /// <param name="baseUri">Uri to robots.txt file. This used to resolve relative paths.
        /// Relative paths is technically not allowed, but some sites do use them.
        /// </param>
        /// <returns>Returns valid list of sitemap locations</returns>
        IEnumerable<Uri> Parse(string robotsTxt, Uri baseUri);
    }
}
