using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Helpers;

/// <summary>
/// Helper class for working with XML file of Browser Capabilities Project (http://browscap.org/)
/// </summary>
public partial class BrowscapXmlHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly INopFileProvider _fileProvider;
    protected Regex _crawlerUserAgentsRegexp;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="userAgentStringsPath">User agent file path</param>
    /// <param name="crawlerOnlyUserAgentStringsPath">User agent with crawlers only file path</param>
    /// <param name="additionalCrawlersFilePath">Additional crawlers user agent file path</param>
    /// <param name="fileProvider">File provider</param>
    public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, string additionalCrawlersFilePath, INopFileProvider fileProvider)
    {
		_logger.LogError(">>> Entering BrowscapXmlHelper(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath},fileProvider={FileProvider})", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, fileProvider);
        _fileProvider = fileProvider;

		_logger.LogDebug("BrowscapXmlHelper(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath},fileProvider={FileProvider}): _fileProvider → {_fileProvider}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, fileProvider, _fileProvider);
        Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath);
		_logger.LogWarning("<<< Exiting BrowscapXmlHelper(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath},fileProvider={FileProvider})", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, fileProvider);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Indicates whether the browscap item is a crawler
    /// </summary>
    /// <param name="browscapItem">Browscap item</param>
    /// <returns>True - if the item is a crawler, false otherwise</returns>
    protected virtual bool IsBrowscapItemIsCrawler(XElement browscapItem)
    {
		_logger.LogError(">>> Entering IsBrowscapItemIsCrawler(browscapItem={BrowscapItem})", browscapItem);
        var el = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name")?.Value == "Crawler");

		_logger.LogError("IsBrowscapItemIsCrawler(browscapItem={BrowscapItem}): el → {El}", browscapItem, el);
		_logger.LogWarning("<<< Exiting IsBrowscapItemIsCrawler(browscapItem={BrowscapItem})", browscapItem);
        return el != null && el.Attribute("value")?.Value.ToLowerInvariant() == "true";
    }

    /// <summary>
    /// Gets the additional crawler list
    /// </summary>
    /// <returns>List of crawlers</returns>
    protected virtual IEnumerable<XElement> GetAdditionalCrawlerItems(string additionalCrawlersFilePath)
    {
		_logger.LogError(">>> Entering GetAdditionalCrawlerItems(additionalCrawlersFilePath={AdditionalCrawlersFilePath})", additionalCrawlersFilePath);
        try
        {
            using var sr = new StreamReader(additionalCrawlersFilePath);
			_logger.LogError("GetAdditionalCrawlerItems(additionalCrawlersFilePath={AdditionalCrawlersFilePath}): sr → {Sr}", additionalCrawlersFilePath, sr);

            var crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();
			_logger.LogError("GetAdditionalCrawlerItems(additionalCrawlersFilePath={AdditionalCrawlersFilePath}): crawlerItems → {CrawlerItems}", additionalCrawlersFilePath, crawlerItems);

            return crawlerItems;
        }
        catch
        {
			_logger.LogError("Exception in GetAdditionalCrawlerItems(additionalCrawlersFilePath={AdditionalCrawlersFilePath}): {Arg1}", additionalCrawlersFilePath, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting GetAdditionalCrawlerItems(additionalCrawlersFilePath={AdditionalCrawlersFilePath})", additionalCrawlersFilePath);
        return Enumerable.Empty<XElement>();
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="userAgentStringsPath">User agent file path</param>
    /// <param name="crawlerOnlyUserAgentStringsPath">User agent with crawlers only file path</param>
    /// <param name="additionalCrawlersFilePath">Additional crawlers user agent file path</param>
    /// <exception cref="Exception"></exception>
    protected virtual void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, string additionalCrawlersFilePath)
    {
		_logger.LogError(">>> Entering Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath})", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath);
        List<XElement> crawlerItems = null;
        var comments = new XElement("comments");
		_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): comments → {Comments}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, comments);
        var needSaveCrawlerOnly = false;
		_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): needSaveCrawlerOnly → {NeedSaveCrawlerOnly}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, needSaveCrawlerOnly);

        if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath))
        {
            //try to load crawler list from crawlers only file
            using var sr = new StreamReader(crawlerOnlyUserAgentStringsPath);
			_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): sr → {Sr}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, sr);
            crawlerItems = XDocument.Load(sr).Root?.Elements("browscapitem").ToList();
			_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): crawlerItems → {CrawlerItems}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, crawlerItems);
        }

        if (crawlerItems == null || !crawlerItems.Any())
        {
            //try to load crawler list from full user agents file
            using var sr = new StreamReader(userAgentStringsPath);
            var rootElement = XDocument.Load(sr).Root;
			_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): rootElement → {RootElement}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, rootElement);
            crawlerItems = rootElement?.Element("browsercapitems")?.Elements("browscapitem")
                //only crawlers
                .Where(IsBrowscapItemIsCrawler).ToList();
            needSaveCrawlerOnly = true;
            comments = rootElement?.Element("comments");
        }

			_logger.LogError("Throwing Exception from Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath})", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath);
        if (crawlerItems == null || !crawlerItems.Any()) {
            throw new Exception("Incorrect file format");
        }

        if (_fileProvider.FileExists(additionalCrawlersFilePath))
            crawlerItems.AddRange(GetAdditionalCrawlerItems(additionalCrawlersFilePath));

        var crawlerRegexpPattern = string.Join("|", crawlerItems
            //get only user agent names
            .Select(e => e.Attribute("name"))
            .Where(e => !string.IsNullOrEmpty(e?.Value))
            .Select(e => e.Value)
            .Select(attributeValue =>
            {
                var sb = new StringBuilder(Regex.Escape(attributeValue));
				_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): sb → {Sb}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, sb);
                sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");

                return $"^{sb}$";
            }));

        _crawlerUserAgentsRegexp = new Regex(crawlerRegexpPattern);
		_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): _crawlerUserAgentsRegexp → {_crawlerUserAgentsRegexp}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, _crawlerUserAgentsRegexp);
		_logger.LogDebug("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): _crawlerUserAgentsRegexp → {_crawlerUserAgentsRegexp}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, _crawlerUserAgentsRegexp);

        if ((string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) || _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath)) && !needSaveCrawlerOnly)
            return;

        //try to write crawlers file
        using var sw = new StreamWriter(crawlerOnlyUserAgentStringsPath);
		_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): sw → {Sw}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, sw);
        var root = new XElement("browsercapitems");
		_logger.LogError("Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath}): root → {Root}", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath, root);

        comments?.AddFirst(new XElement("comment", new XCData("nopCommerce uses a short version of the \"browscap.xml\" file. This short version contains crawlers only. If you want to keep the crawlers list up to date, please download the full version of the original file from the official browscap site (http://browscap.org/). Please save it in the \\App_Data folder (The file name should be \"browscap.xml\"), delete \"browscap.crawlersonly.xml\", and restart the website.")));
        root.Add(comments);

        foreach (var crawler in crawlerItems)
        {
            foreach (var element in crawler.Elements().ToList())
            {
                if ((element.Attribute("name")?.Value.ToLowerInvariant() ?? string.Empty) == "crawler")
                    continue;
                element.Remove();
            }

            root.Add(crawler);
        }

        root.Save(sw);
		_logger.LogWarning("<<< Exiting Initialize(userAgentStringsPath={UserAgentStringsPath},crawlerOnlyUserAgentStringsPath={CrawlerOnlyUserAgentStringsPath},additionalCrawlersFilePath={AdditionalCrawlersFilePath})", userAgentStringsPath, crawlerOnlyUserAgentStringsPath, additionalCrawlersFilePath);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether a user agent is a crawler
    /// </summary>
    /// <param name="userAgent">User agent string</param>
    /// <returns>True if user agent is a crawler, otherwise - false</returns>
    public virtual bool IsCrawler(string userAgent)
    {
        return _crawlerUserAgentsRegexp.IsMatch(userAgent);
    }

    #endregion
}