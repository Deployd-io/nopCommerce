using System.Xml;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Logging;

namespace Nop.Services.Plugins.Marketplace;

/// <summary>
/// Represents the official feed manager (plugins from nopcommerce marketplace)
/// </summary>
public partial class OfficialFeedManager
{
    #region Fields

    protected readonly ILogger _logger;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public OfficialFeedManager(ILogger logger,
        NopHttpClient nopHttpClient)
    {
        _logger = logger;
		_logger.LogDebug(">>> Entering OfficialFeedManager(logger={Logger},nopHttpClient={NopHttpClient})", logger, nopHttpClient);
		_logger.LogDebug("OfficialFeedManager(logger={Logger},nopHttpClient={NopHttpClient}): _logger → {_logger}", logger, nopHttpClient, _logger);
        _nopHttpClient = nopHttpClient;
		_logger.LogDebug("<<< Exiting OfficialFeedManager(logger={Logger},nopHttpClient={NopHttpClient})", logger, nopHttpClient);
		_logger.LogDebug("OfficialFeedManager(logger={Logger},nopHttpClient={NopHttpClient}): _nopHttpClient → {_nopHttpClient}", logger, nopHttpClient, _nopHttpClient);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get element value
    /// </summary>
    /// <param name="node">XML node</param>
    /// <param name="elementName">Element name</param>
    /// <returns>Value (text)</returns>
    protected virtual string GetElementValue(XmlNode node, string elementName)
    {
		_logger.LogDebug(">>> Entering GetElementValue(node={Node},elementName={ElementName})", node, elementName);
		_logger.LogDebug("<<< Exiting GetElementValue(node={Node},elementName={ElementName})", node, elementName);
        return node?.SelectSingleNode(elementName)?.InnerText;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get available categories of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<OfficialFeedCategory>> GetCategoriesAsync()
    {
		_logger.LogDebug(">>> Entering GetCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //load XML
        var xml = new XmlDocument();
		_logger.LogDebug("GetCategoriesAsync(): xml → {Xml}", xml);
        try
        {
            xml.LoadXml(await _nopHttpClient.GetExtensionsCategoriesAsync());
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.nopcommerce.com is not available.", ex);
        }

        //get list of categories from the XML
		_logger.LogInformation("GetCategoriesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCategoriesAsync()");
        return xml.SelectNodes(@"//categories/category").Cast<XmlNode>().Select(node => new OfficialFeedCategory
        {
            Id = int.Parse(GetElementValue(node, @"id")),
            ParentCategoryId = int.Parse(GetElementValue(node, @"parentCategoryId")),
            Name = GetElementValue(node, @"name")
        }).ToList();
    }

    /// <summary>
    /// Get available versions of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<OfficialFeedVersion>> GetVersionsAsync()
    {
		_logger.LogDebug(">>> Entering GetVersionsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //load XML
        var xml = new XmlDocument();
		_logger.LogDebug("GetVersionsAsync(): xml → {Xml}", xml);
        try
        {
            xml.LoadXml(await _nopHttpClient.GetExtensionsVersionsAsync());
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.nopcommerce.com is not available.", ex);
        }

        //get list of versions from the XML
		_logger.LogInformation("GetVersionsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetVersionsAsync()");
        return xml.SelectNodes(@"//versions/version").Cast<XmlNode>().Select(node => new OfficialFeedVersion
        {
            Id = int.Parse(GetElementValue(node, @"id")),
            Name = GetElementValue(node, @"name")
        }).ToList();
    }

    /// <summary>
    /// Get all plugins
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="versionId">Version identifier</param>
    /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugins
    /// </returns>
    public virtual async Task<IPagedList<OfficialFeedPlugin>> GetAllPluginsAsync(int categoryId = 0,
        int versionId = 0, int price = 0, string searchTerm = "",
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //load XML
        var xml = new XmlDocument();
		_logger.LogDebug("GetAllPluginsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize}): xml → {Xml}", categoryId, versionId, price, searchTerm, pageIndex, pageSize, xml);
        try
        {
            xml.LoadXml(await _nopHttpClient.GetExtensionsAsync(categoryId, versionId, price, searchTerm, pageIndex, pageSize));
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("No access to the list of plugins. Website www.nopcommerce.com is not available.", ex);
        }

        //get list of extensions from the XML
        var list = xml.SelectNodes(@"//extensions/extension").Cast<XmlNode>().Select(node => new OfficialFeedPlugin
        {
            Name = GetElementValue(node, @"name"),
            Url = GetElementValue(node, @"url"),
            PictureUrl = GetElementValue(node, @"picture"),
            Category = GetElementValue(node, @"category"),
            SupportedVersions = GetElementValue(node, @"versions"),
            Price = GetElementValue(node, @"price")
        }).ToList();

        _ = int.TryParse(GetElementValue(xml.SelectNodes(@"//totalRecords")?[0], @"value"), out var totalRecords);
		_logger.LogDebug("GetAllPluginsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize}): _ → {_}", categoryId, versionId, price, searchTerm, pageIndex, pageSize, _);

		_logger.LogDebug("<<< Exiting GetAllPluginsAsync(categoryId={CategoryId},versionId={VersionId},price={Price},searchTerm={SearchTerm},pageIndex={PageIndex},pageSize={PageSize})", categoryId, versionId, price, searchTerm, pageIndex, pageSize);
		_logger.LogInformation("GetAllPluginsAsync(categoryId,versionId,price,searchTerm,pageIndex,pageSize)={CategoryId},{VersionId},{Price},{SearchTerm},{PageIndex},{PageSize}: external service call took {Arg6} ms", categoryId, versionId, price, searchTerm, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
        return new PagedList<OfficialFeedPlugin>(list, pageIndex, pageSize, totalRecords);
    }
	_logger.LogInformation("GetAllPluginsAsync(categoryId,versionId,price,searchTerm,pageIndex,pageSize)={CategoryId},{VersionId},{Price},{SearchTerm},{PageIndex},{PageSize}: external service call took {Arg6} ms", categoryId, versionId, price, searchTerm, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

    #endregion
}