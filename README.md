# Louw.SitemapParser
.NET libary to parse Sitemap files.
See official specification: https://www.sitemaps.org/protocol.html

Support for various sitemap types:
* Parse Robots.txt to detect sitemaps
* Index Sitemaps
* Normal Sitemaps

FUTURE DEVLOPMENT ROADMAP:
* Image Sitemaps (https://support.google.com/webmasters/answer/178636?hl=en&ref_topic=6080646)
* Video Sitemaps (https://developers.google.com/webmasters/videosearch/sitemaps)
* Alternate languages (https://support.google.com/webmasters/answer/2620865?hl=en&ref_topic=6080646)
* Plain text sitemaps (https://www.sitemaps.org/protocol.html#otherformats)
* RSS feed sitemaps (https://www.sitemaps.org/protocol.html#otherformats)
* Utilities that validates "cross domain" sitemaps (https://www.sitemaps.org/protocol.html#location)

#####nuget
The package is available on nuget
https://www.nuget.org/packages/Louw.SitemapParser

```
install-package Louw.SitemapParser
```


#####Basic Example
```cs
	var loader = new SitemapLoader();
    Sitemap sitemap = await loader.LoadFromRobotsTxtAsync(new Uri("https://www.google.com"));
    Assert.Equal(SitemapType.RobotsTxt, sitemap.SitemapType);
    Assert.NotEmpty(sitemap.Sitemaps); //We expect at least some Sitemaps to be in list
    Assert.Empty(sitemap.Items); //Robots.txt can only link to Sitemaps  (Not content items)

    Sitemap firstSitemap = sitemap.Sitemaps.First();
    Assert.False(firstSitemap.IsLoaded); //We only have sitemap location. Contents not yet loaded nor parsed

    var firstLoadedSitemap = await loader.LoadAsync(firstSitemap.SitemapLocation);
    Assert.True(firstLoadedSitemap.IsLoaded); //Now items are loaded!

    //We have to check type as we can either have links to other sitemaps (i.e. index sitemaps) 
    //-or- links to actual sitemap items (i.e. links to content)
    switch (firstLoadedSitemap.SitemapType)
    {
        case SitemapType.Index: Assert.NotEmpty(firstLoadedSitemap.Sitemaps); break;
        case SitemapType.Items: Assert.NotEmpty(firstLoadedSitemap.Items); break;
        default: throw new NotSupportedException($"SitemapType {firstLoadedSitemap.SitemapType} not expected here");
    }
```

#####More Examples

More examples can be found here:
https://github.com/louislouw/Louw.SitemapParser/blob/master/test/Louw.SitemapParser.Examples/Examples.cs

