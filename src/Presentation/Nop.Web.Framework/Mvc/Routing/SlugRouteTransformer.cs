using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents slug route transformer
/// </summary>
public partial class SlugRouteTransformer : DynamicRouteValueTransformer
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILanguageService _languageService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public SlugRouteTransformer(CatalogSettings catalogSettings,
        ICategoryService categoryService,
        IEventPublisher eventPublisher,
        ILanguageService languageService,
        IManufacturerService manufacturerService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        LocalizationSettings localizationSettings)
    {
		_logger.LogError(">>> Entering SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings})", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings);
        _catalogSettings = catalogSettings;
        _categoryService = categoryService;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _categoryService → {_categoryService}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _categoryService);
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _catalogSettings → {_catalogSettings}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _catalogSettings);
    _logger.LogWarning("<<< Exiting SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings})", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _eventPublisher → {_eventPublisher}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _eventPublisher);
        _languageService = languageService;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _languageService → {_languageService}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _languageService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _manufacturerService → {_manufacturerService}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _manufacturerService);
        _storeContext = storeContext;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _storeContext → {_storeContext}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _storeContext);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("SlugRouteTransformer(catalogSettings={CatalogSettings},categoryService={CategoryService},eventPublisher={EventPublisher},languageService={LanguageService},manufacturerService={ManufacturerService},storeContext={StoreContext},urlRecordService={UrlRecordService},localizationSettings={LocalizationSettings}): _urlRecordService → {_urlRecordService}", catalogSettings, categoryService, eventPublisher, languageService, manufacturerService, storeContext, urlRecordService, localizationSettings, _urlRecordService);
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Transform route values according to the passed URL record
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="urlRecord">Record found by the URL slug</param>
    /// <param name="catalogPath">URL catalog path</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SingleSlugRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
    {
		_logger.LogError(">>> Entering SingleSlugRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath})", httpContext, values, urlRecord, catalogPath);
        //if URL record is not active let's find the latest one
        var slug = urlRecord.IsActive
            ? urlRecord.Slug
            : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
        if (string.IsNullOrEmpty(slug))
            return;

        if (!urlRecord.IsActive || !string.IsNullOrEmpty(catalogPath))
        {
            //permanent redirect to new URL with active single slug
            InternalRedirect(httpContext, values, $"/{slug}", true);
            return;
        }

        //Ensure that the slug is the same for the current language, 
        //otherwise it can cause some issues when customers choose a new language but a slug stays the same
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("SingleSlugRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): store → {Store}", httpContext, values, urlRecord, catalogPath, store);
            var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
			_logger.LogError("SingleSlugRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): languages → {Languages}", httpContext, values, urlRecord, catalogPath, languages);
            var language = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ?? languages.FirstOrDefault();

            var slugLocalized = await _urlRecordService.GetSeNameAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id, true, false);
			_logger.LogError("SingleSlugRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): slugLocalized → {SlugLocalized}", httpContext, values, urlRecord, catalogPath, slugLocalized);
            if (!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //redirect to the page for current language
                InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{slugLocalized}", false);
                return;
            }
        }

        //since we are here, all is ok with the slug, so process URL
        switch (urlRecord.EntityName)
        {
            case var name when name.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Product", "ProductDetails", slug, (NopRoutingDefaults.RouteValue.ProductId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(ProductTag), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Catalog", "ProductsByTag", slug, (NopRoutingDefaults.RouteValue.ProductTagId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Catalog", "Category", slug, (NopRoutingDefaults.RouteValue.CategoryId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Catalog", "Manufacturer", slug, (NopRoutingDefaults.RouteValue.ManufacturerId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(Vendor), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Catalog", "Vendor", slug, (NopRoutingDefaults.RouteValue.VendorId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "News", "NewsItem", slug, (NopRoutingDefaults.RouteValue.NewsItemId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(BlogPost), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Blog", "BlogPost", slug, (NopRoutingDefaults.RouteValue.BlogPostId, urlRecord.EntityId));
                return;

            case var name when name.Equals(nameof(Topic), StringComparison.InvariantCultureIgnoreCase):
                RouteToAction(values, "Topic", "TopicDetails", slug, (NopRoutingDefaults.RouteValue.TopicId, urlRecord.EntityId));
				_logger.LogWarning("<<< Exiting SingleSlugRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath})", httpContext, values, urlRecord, catalogPath);
                return;
        }
    }

    /// <summary>
    /// Try transforming the route values, assuming the passed URL record is of a product type
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="urlRecord">Record found by the URL slug</param>
    /// <param name="catalogPath">URL catalog path</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value whether the route values were processed
    /// </returns>
    protected virtual async Task<bool> TryProductCatalogRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
    {
		_logger.LogError(">>> Entering TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath})", httpContext, values, urlRecord, catalogPath);
        //ensure it's a product URL record
        if (!urlRecord.EntityName.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase))
            return false;

        //if the product URL structure type is product seName only, it will be processed later by a single slug
        if (_catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.Product)
            return false;

        //get active slug for the product
        var slug = urlRecord.IsActive
            ? urlRecord.Slug
            : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
        if (string.IsNullOrEmpty(slug))
            return false;

        //try to get active catalog (e.g. category or manufacturer) seName for the product
        var catalogSeName = string.Empty;
		_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): catalogSeName → {CatalogSeName}", httpContext, values, urlRecord, catalogPath, catalogSeName);
        var isCategoryProductUrl = _catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.CategoryProduct;
		_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): isCategoryProductUrl → {IsCategoryProductUrl}", httpContext, values, urlRecord, catalogPath, isCategoryProductUrl);
        if (isCategoryProductUrl)
        {
            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): productCategory → {ProductCategory}", httpContext, values, urlRecord, catalogPath, productCategory);
            var category = await _categoryService.GetCategoryByIdAsync(productCategory?.CategoryId ?? 0);
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): category → {Category}", httpContext, values, urlRecord, catalogPath, category);
            catalogSeName = category is not null ? await _urlRecordService.GetSeNameAsync(category) : string.Empty;
        }
        var isManufacturerProductUrl = _catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.ManufacturerProduct;
		_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): isManufacturerProductUrl → {IsManufacturerProductUrl}", httpContext, values, urlRecord, catalogPath, isManufacturerProductUrl);
        if (isManufacturerProductUrl)
        {
            var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): productManufacturer → {ProductManufacturer}", httpContext, values, urlRecord, catalogPath, productManufacturer);
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): manufacturer → {Manufacturer}", httpContext, values, urlRecord, catalogPath, manufacturer);
            catalogSeName = manufacturer is not null ? await _urlRecordService.GetSeNameAsync(manufacturer) : string.Empty;
        }
        if (string.IsNullOrEmpty(catalogSeName))
            return false;

        //get URL record by the specified catalog path
        var catalogUrlRecord = await _urlRecordService.GetBySlugAsync(catalogPath);
		_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): catalogUrlRecord → {CatalogUrlRecord}", httpContext, values, urlRecord, catalogPath, catalogUrlRecord);
        if (catalogUrlRecord is null ||
            (isCategoryProductUrl && !catalogUrlRecord.EntityName.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase)) ||
            (isManufacturerProductUrl && !catalogUrlRecord.EntityName.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase)) ||
            !urlRecord.IsActive)
        {
            //permanent redirect to new URL with active catalog seName and active slug
            InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
            return true;
        }

        //ensure the catalog seName and slug are the same for the current language
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): store → {Store}", httpContext, values, urlRecord, catalogPath, store);
            var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): languages → {Languages}", httpContext, values, urlRecord, catalogPath, languages);
            var language = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ?? languages.FirstOrDefault();

            var slugLocalized = await _urlRecordService.GetSeNameAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id, true, false);
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): slugLocalized → {SlugLocalized}", httpContext, values, urlRecord, catalogPath, slugLocalized);
            var catalogSlugLocalized = await _urlRecordService.GetSeNameAsync(catalogUrlRecord.EntityId, catalogUrlRecord.EntityName, language.Id, true, false);
			_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): catalogSlugLocalized → {CatalogSlugLocalized}", httpContext, values, urlRecord, catalogPath, catalogSlugLocalized);
            if ((!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(catalogSlugLocalized) && !catalogSlugLocalized.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase)))
            {
                //redirect to localized URL for the current language
                var activeSlug = !string.IsNullOrEmpty(slugLocalized) ? slugLocalized : slug;
				_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): activeSlug → {ActiveSlug}", httpContext, values, urlRecord, catalogPath, activeSlug);
                var activeCatalogSlug = !string.IsNullOrEmpty(catalogSlugLocalized) ? catalogSlugLocalized : catalogUrlRecord.Slug;
				_logger.LogError("TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath}): activeCatalogSlug → {ActiveCatalogSlug}", httpContext, values, urlRecord, catalogPath, activeCatalogSlug);
                InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{activeCatalogSlug}/{activeSlug}", false);
                return true;
            }
        }

        //ensure the specified catalog path is equal to the active catalog seName
        //we do it here after localization check to avoid double redirect
        if (!catalogSeName.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase))
        {
            //permanent redirect to new URL with active catalog seName and active slug
            InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
            return true;
        }

        //all is ok, so select the appropriate action
        RouteToAction(values, "Product", "ProductDetails", slug,
            (NopRoutingDefaults.RouteValue.ProductId, urlRecord.EntityId), (NopRoutingDefaults.RouteValue.CatalogSeName, catalogSeName));
		_logger.LogWarning("<<< Exiting TryProductCatalogRoutingAsync(httpContext={HttpContext},values={Values},urlRecord={UrlRecord},catalogPath={CatalogPath})", httpContext, values, urlRecord, catalogPath);
        return true;
    }

    /// <summary>
    /// Transform route values to redirect the request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="path">Path</param>
    /// <param name="permanent">Whether the redirect should be permanent</param>
    protected virtual void InternalRedirect(HttpContext httpContext, RouteValueDictionary values, string path, bool permanent)
    {
		_logger.LogError(">>> Entering InternalRedirect(httpContext={HttpContext},values={Values},path={Path},permanent={Permanent})", httpContext, values, path, permanent);
        values[NopRoutingDefaults.RouteValue.Controller] = "Common";
        values[NopRoutingDefaults.RouteValue.Action] = "InternalRedirect";
        values[NopRoutingDefaults.RouteValue.Url] = $"{httpContext.Request.PathBase}{path}{httpContext.Request.QueryString}";
        values[NopRoutingDefaults.RouteValue.PermanentRedirect] = permanent;
        httpContext.Items[NopHttpDefaults.GenericRouteInternalRedirect] = true;
		_logger.LogWarning("<<< Exiting InternalRedirect(httpContext={HttpContext},values={Values},path={Path},permanent={Permanent})", httpContext, values, path, permanent);
    }

    /// <summary>
    /// Transform route values to set controller, action and action parameters
    /// </summary>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="controller">Controller name</param>
    /// <param name="action">Action name</param>
    /// <param name="slug">URL slug</param>
    /// <param name="parameters">Action parameters</param>
    protected virtual void RouteToAction(RouteValueDictionary values, string controller, string action, string slug, params (string Key, object Value)[] parameters)
    {
		_logger.LogError(">>> Entering RouteToAction(values={Values},controller={Controller},action={Action},slug={Slug},parameters={Parameters})", values, controller, action, slug, parameters);
        values[NopRoutingDefaults.RouteValue.Controller] = controller;
        values[NopRoutingDefaults.RouteValue.Action] = action;
        values[NopRoutingDefaults.RouteValue.SeName] = slug;
        foreach (var (key, value) in parameters)
        {
            values[key] = value;
        }
		_logger.LogWarning("<<< Exiting RouteToAction(values={Values},controller={Controller},action={Action},slug={Slug},parameters={Parameters})", values, controller, action, slug, parameters);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a set of transformed route values that will be used to select an action
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="routeValues">The route values associated with the current match</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the set of values
    /// </returns>
    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary routeValues)
    {
		_logger.LogError(">>> Entering TransformAsync(httpContext={HttpContext},routeValues={RouteValues})", httpContext, routeValues);
        //get values to transform for action selection
        var values = new RouteValueDictionary(routeValues);
		_logger.LogError("TransformAsync(httpContext={HttpContext},routeValues={RouteValues}): values → {Values}", httpContext, routeValues, values);

        if (!values.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug) || slug is null)
            return values;

        //find record by the URL slug
        if (await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
            return values;

        //allow third-party handlers to select an action by the found URL record
        var routingEvent = new GenericRoutingEvent(httpContext, values, urlRecord);
		_logger.LogError("TransformAsync(httpContext={HttpContext},routeValues={RouteValues}): routingEvent → {RoutingEvent}", httpContext, routeValues, routingEvent);
        await _eventPublisher.PublishAsync(routingEvent);
        if (routingEvent.StopProcessing)
            return values;

        //then try to select an action by the found URL record and the catalog path
        var catalogPath = values.TryGetValue(NopRoutingDefaults.RouteValue.CatalogSeName, out var catalogPathValue)
            ? catalogPathValue.ToString()
            : string.Empty;
        if (await TryProductCatalogRoutingAsync(httpContext, values, urlRecord, catalogPath))
            return values;

        //finally, select an action by the URL record only
        await SingleSlugRoutingAsync(httpContext, values, urlRecord, catalogPath);

		_logger.LogWarning("<<< Exiting TransformAsync(httpContext={HttpContext},routeValues={RouteValues})", httpContext, routeValues);
        return values;
    }

    #endregion
}