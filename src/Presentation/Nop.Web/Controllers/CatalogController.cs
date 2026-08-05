using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CatalogController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICatalogModelFactory _catalogModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CatalogController(CatalogSettings catalogSettings,
        IAclService aclService,
        ICatalogModelFactory catalogModelFactory,
        ICategoryService categoryService,
        ICustomerActivityService customerActivityService,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IProductTagService productTagService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        FilterLevelSettings filterLevelSettings,
        MediaSettings mediaSettings,
        VendorSettings vendorSettings)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _catalogModelFactory = catalogModelFactory;
        _categoryService = categoryService;
        _customerActivityService = customerActivityService;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _productTagService = productTagService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _filterLevelSettings = filterLevelSettings;
        _mediaSettings = mediaSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Categories

    [SaveLastContinueShoppingPage]
    public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering Category(categoryId={CategoryId},command={Command})", categoryId, command);
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

		_logger.LogDebug("Category(categoryId={CategoryId},command={Command}): category → {Category}", categoryId, command, category);
        if (!await CheckCategoryAvailabilityAsync(category))
            return InvokeHttp404();
        
        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.CATEGORIES_VIEW))
            DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.ADMIN }));

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

        //model
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);

		_logger.LogDebug("Category(categoryId={CategoryId},command={Command}): model → {Model}", categoryId, command, model);
        //template
        var templateViewPath = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
		_logger.LogDebug("Category(categoryId={CategoryId},command={Command}): templateViewPath → {TemplateViewPath}", categoryId, command, templateViewPath);
		_logger.LogDebug("<<< Exiting Category(categoryId={CategoryId},command={Command})", categoryId, command);
        return View(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetCategoryProducts(categoryId={CategoryId},command={Command})", categoryId, command);
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

		_logger.LogDebug("GetCategoryProducts(categoryId={CategoryId},command={Command}): category → {Category}", categoryId, command, category);
        if (!await CheckCategoryAvailabilityAsync(category))
            return NotFound();

        var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

		_logger.LogDebug("GetCategoryProducts(categoryId={CategoryId},command={Command}): model → {Model}", categoryId, command, model);
		_logger.LogDebug("<<< Exiting GetCategoryProducts(categoryId={CategoryId},command={Command})", categoryId, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Manufacturers

    [SaveLastContinueShoppingPage]
    public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering Manufacturer(manufacturerId={ManufacturerId},command={Command})", manufacturerId, command);
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

		_logger.LogDebug("Manufacturer(manufacturerId={ManufacturerId},command={Command}): manufacturer → {Manufacturer}", manufacturerId, command, manufacturer);
        if (!await CheckManufacturerAvailabilityAsync(manufacturer))
            return InvokeHttp404();

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.MANUFACTURER_VIEW))
            DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.ADMIN }));

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewManufacturer",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

        //model
        var model = await _catalogModelFactory.PrepareManufacturerModelAsync(manufacturer, command);

		_logger.LogDebug("Manufacturer(manufacturerId={ManufacturerId},command={Command}): model → {Model}", manufacturerId, command, model);
        //template
        var templateViewPath = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(manufacturer.ManufacturerTemplateId);

		_logger.LogDebug("Manufacturer(manufacturerId={ManufacturerId},command={Command}): templateViewPath → {TemplateViewPath}", manufacturerId, command, templateViewPath);
		_logger.LogDebug("<<< Exiting Manufacturer(manufacturerId={ManufacturerId},command={Command})", manufacturerId, command);
        return View(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetManufacturerProducts(manufacturerId={ManufacturerId},command={Command})", manufacturerId, command);
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

		_logger.LogDebug("GetManufacturerProducts(manufacturerId={ManufacturerId},command={Command}): manufacturer → {Manufacturer}", manufacturerId, command, manufacturer);
        if (!await CheckManufacturerAvailabilityAsync(manufacturer))
            return NotFound();

        var model = await _catalogModelFactory.PrepareManufacturerProductsModelAsync(manufacturer, command);

		_logger.LogDebug("GetManufacturerProducts(manufacturerId={ManufacturerId},command={Command}): model → {Model}", manufacturerId, command, model);
		_logger.LogDebug("<<< Exiting GetManufacturerProducts(manufacturerId={ManufacturerId},command={Command})", manufacturerId, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual async Task<IActionResult> ManufacturerAll()
    {
		_logger.LogDebug(">>> Entering ManufacturerAll()");
        var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();

		_logger.LogDebug("ManufacturerAll(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting ManufacturerAll()");
        return View(model);
    }

    #endregion

    #region Vendors

    [SaveLastContinueShoppingPage]
    public virtual async Task<IActionResult> Vendor(int vendorId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering Vendor(vendorId={VendorId},command={Command})", vendorId, command);
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

		_logger.LogDebug("Vendor(vendorId={VendorId},command={Command}): vendor → {Vendor}", vendorId, command, vendor);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return InvokeHttp404();
        
        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_VIEW))
            DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.ADMIN }));

        //model
        var model = await _catalogModelFactory.PrepareVendorModelAsync(vendor, command);

		_logger.LogDebug("Vendor(vendorId={VendorId},command={Command}): model → {Model}", vendorId, command, model);
		_logger.LogDebug("<<< Exiting Vendor(vendorId={VendorId},command={Command})", vendorId, command);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetVendorProducts(int vendorId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetVendorProducts(vendorId={VendorId},command={Command})", vendorId, command);
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

		_logger.LogDebug("GetVendorProducts(vendorId={VendorId},command={Command}): vendor → {Vendor}", vendorId, command, vendor);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return NotFound();

        var model = await _catalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);

		_logger.LogDebug("GetVendorProducts(vendorId={VendorId},command={Command}): model → {Model}", vendorId, command, model);
		_logger.LogDebug("<<< Exiting GetVendorProducts(vendorId={VendorId},command={Command})", vendorId, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual async Task<IActionResult> VendorReviews(int vendorId, VendorReviewsPagingFilteringModel pagingModel)
    {
		_logger.LogDebug(">>> Entering VendorReviews(vendorId={VendorId},pagingModel={PagingModel})", vendorId, pagingModel);
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

		_logger.LogDebug("VendorReviews(vendorId={VendorId},pagingModel={PagingModel}): vendor → {Vendor}", vendorId, pagingModel, vendor);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return NotFound();

        var model = await _catalogModelFactory.PrepareVendorProductReviewsModelAsync(vendor, pagingModel);

		_logger.LogDebug("VendorReviews(vendorId={VendorId},pagingModel={PagingModel}): model → {Model}", vendorId, pagingModel, model);
		_logger.LogDebug("<<< Exiting VendorReviews(vendorId={VendorId},pagingModel={PagingModel})", vendorId, pagingModel);
        return View(model);
    }

    public virtual async Task<IActionResult> VendorAll()
    {
		_logger.LogDebug(">>> Entering VendorAll()");
        //we don't allow viewing of vendors if "vendors" block is hidden
        if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();
		_logger.LogDebug("VendorAll(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting VendorAll()");
        return View(model);
    }

    #endregion

    #region Product tags

    public virtual async Task<IActionResult> ProductsByTag(int productTagId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering ProductsByTag(productTagId={ProductTagId},command={Command})", productTagId, command);
        var productTag = await _productTagService.GetProductTagByIdAsync(productTagId);
        if (productTag == null)
			_logger.LogDebug("ProductsByTag(productTagId={ProductTagId},command={Command}): productTag → {ProductTag}", productTagId, command, productTag);
            return InvokeHttp404();

        var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(productTag, command);

		_logger.LogDebug("ProductsByTag(productTagId={ProductTagId},command={Command}): model → {Model}", productTagId, command, model);
		_logger.LogDebug("<<< Exiting ProductsByTag(productTagId={ProductTagId},command={Command})", productTagId, command);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetTagProducts(int tagId, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetTagProducts(tagId={TagId},command={Command})", tagId, command);
        var productTag = await _productTagService.GetProductTagByIdAsync(tagId);
        if (productTag == null)
			_logger.LogDebug("GetTagProducts(tagId={TagId},command={Command}): productTag → {ProductTag}", tagId, command, productTag);
            return NotFound();

        var model = await _catalogModelFactory.PrepareTagProductsModelAsync(productTag, command);

		_logger.LogDebug("GetTagProducts(tagId={TagId},command={Command}): model → {Model}", tagId, command, model);
		_logger.LogDebug("<<< Exiting GetTagProducts(tagId={TagId},command={Command})", tagId, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual async Task<IActionResult> ProductTagsAll()
    {
		_logger.LogDebug(">>> Entering ProductTagsAll()");
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

		_logger.LogDebug("ProductTagsAll(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting ProductTagsAll()");
        return View(model);
    }

    #endregion

    #region New (recently added) products page

    public virtual async Task<IActionResult> NewProducts(CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering NewProducts(command={Command})", command);
        if (!_catalogSettings.NewProductsEnabled)
            return InvokeHttp404();

        var model = new NewProductsModel
        {
            CatalogProductsModel = await _catalogModelFactory.PrepareNewProductsModelAsync(command)
        };

		_logger.LogDebug("<<< Exiting NewProducts(command={Command})", command);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetNewProducts(CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetNewProducts(command={Command})", command);
        if (!_catalogSettings.NewProductsEnabled)
            return NotFound();

        var model = await _catalogModelFactory.PrepareNewProductsModelAsync(command);

		_logger.LogDebug("GetNewProducts(command={Command}): model → {Model}", command, model);
		_logger.LogDebug("<<< Exiting GetNewProducts(command={Command})", command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> NewProductsRss()
    {
		_logger.LogDebug(">>> Entering NewProductsRss()");
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: New products",
            "Information about products",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);
			_logger.LogDebug("NewProductsRss(): store → {Store}", store);

        if (!_catalogSettings.NewProductsEnabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();

		_logger.LogDebug("NewProductsRss(): items → {Items}", items);
        var storeId = store.Id;
        var products = await _productService.GetProductsMarkedAsNewAsync(storeId: storeId);

		_logger.LogDebug("NewProductsRss(): products → {Products}", products);
        foreach (var product in products)
        {
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol());
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
			_logger.LogDebug("NewProductsRss(): productUrl → {ProductUrl}", productUrl);
			_logger.LogDebug("NewProductsRss(): productName → {ProductName}", productName);
            var productDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription);
            var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{store.Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
			_logger.LogDebug("NewProductsRss(): productDescription → {ProductDescription}", productDescription);
			_logger.LogDebug("NewProductsRss(): item → {Item}", item);
            items.Add(item);
            //uncomment below if you want to add RSS enclosure for pictures
            //var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
            //if (picture != null)
            //{
            //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductDetailsPictureSize);
            //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl), new XAttribute("length", picture.PictureBinary.Length)));
            //}

        }
        feed.Items = items;
		_logger.LogDebug("NewProductsRss(): feed.Items → {Arg0}", feed.Items);
		_logger.LogDebug("<<< Exiting NewProductsRss()");
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    #endregion

    #region Searching

    [SaveLastContinueShoppingPage]
    public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
    {
        if (model == null)
            model = new SearchModel();

		_logger.LogDebug("Search(model={Model},command={Command}): model → {Model1}", model, command, model);
        model = await _catalogModelFactory.PrepareSearchModelAsync(model, command);

		_logger.LogDebug("<<< Exiting Search(model={Model},command={Command})", model, command);
        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> SearchTermAutoComplete(string term, int categoryId)
    {
		_logger.LogDebug(">>> Entering SearchTermAutoComplete(term={Term},categoryId={CategoryId})", term, categoryId);
        if (string.IsNullOrWhiteSpace(term))
            return Content("");

        term = term.Trim();

		_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): term → {Term1}", term, categoryId, term);
        if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
            return Content("");

        //products
        var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
            _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;
        var store = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): store → {Store}", term, categoryId, store);
        var categoryIds = new List<int>();
        if (categoryId > 0)
			_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): categoryIds → {CategoryIds}", term, categoryId, categoryIds);
            categoryIds.AddRange([categoryId, .. await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)]);

        var products = await _productService.SearchProductsAsync(0,
            categoryIds: categoryIds,
            storeId: store.Id,
            keywords: term,
            languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
            visibleIndividuallyOnly: true,
            pageSize: productNumber);

        var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

		_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): showLinkToResultSearch → {ShowLinkToResultSearch}", term, categoryId, showLinkToResultSearch);
        var models = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
        var result = (from p in models
                select new
				_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): models → {Models}", term, categoryId, models);
                {
                    label = p.Name,
                    producturl = _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = p.SeName }).Result,
                    productpictureurl = p.PictureModels.FirstOrDefault()?.ImageUrl,
                    showlinktoresultsearch = showLinkToResultSearch
					_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): label → {Label}", term, categoryId, label);
					_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): productpictureurl → {Productpictureurl}", term, categoryId, productpictureurl);
					_logger.LogDebug("SearchTermAutoComplete(term={Term},categoryId={CategoryId}): showlinktoresultsearch → {Showlinktoresultsearch}", term, categoryId, showlinktoresultsearch);
                })
            .ToList();
		_logger.LogDebug("<<< Exiting SearchTermAutoComplete(term={Term},categoryId={CategoryId})", term, categoryId);
        return Json(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering SearchProducts(searchModel={SearchModel},command={Command})", searchModel, command);
        if (searchModel == null)
            searchModel = new SearchModel();

		_logger.LogDebug("SearchProducts(searchModel={SearchModel},command={Command}): searchModel → {SearchModel1}", searchModel, command, searchModel);
        var model = await _catalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

		_logger.LogDebug("SearchProducts(searchModel={SearchModel},command={Command}): model → {Model}", searchModel, command, model);
		_logger.LogDebug("<<< Exiting SearchProducts(searchModel={SearchModel},command={Command})", searchModel, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Filter level values

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetFilterLevelValues(string filterLevel1Value = "", string filterLevel2Value = "", string filterLevel3Value = "")
    {
		_logger.LogDebug(">>> Entering GetFilterLevelValues(filterLevel1Value={FilterLevel1Value},filterLevel2Value={FilterLevel2Value},filterLevel3Value={FilterLevel3Value})", filterLevel1Value, filterLevel2Value, filterLevel3Value);
        var values = await _filterLevelValueService.GetAllFilterLevelValuesAsync(
            filterLevel1Value, filterLevel2Value, filterLevel3Value);

        var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.Select");

		_logger.LogDebug("GetFilterLevelValues(filterLevel1Value={FilterLevel1Value},filterLevel2Value={FilterLevel2Value},filterLevel3Value={FilterLevel3Value}): defaultItemText → {DefaultItemText}", filterLevel1Value, filterLevel2Value, filterLevel3Value, defaultItemText);
        if (string.IsNullOrEmpty(filterLevel1Value))
        {
            var result = values
                .Select(f => new 
                { 
                    filterLevel1Value = f.FilterLevel1Value, 
                    defaultItemText = defaultItemText 
                })
                .Distinct();
            return Json(result);
        }

        if (string.IsNullOrEmpty(filterLevel2Value))
        {
            var result = values
                .Where(f => f.FilterLevel1Value == filterLevel1Value)
                .Select(f => new
                {
                    filterLevel1Value = f.FilterLevel1Value,
                    filterLevel2Value = f.FilterLevel2Value,
                    defaultItemText = defaultItemText
                })
                .Distinct();
            return Json(result);
        }

        if (string.IsNullOrEmpty(filterLevel3Value))
        {
            var result = values
                .Where(f => f.FilterLevel1Value == filterLevel1Value &&
                            f.FilterLevel2Value == filterLevel2Value)
                .Select(f => new
                {
                    filterLevel1Value = f.FilterLevel1Value,
                    filterLevel2Value = f.FilterLevel2Value,
                    filterLevel3Value = f.FilterLevel3Value,
                    defaultItemText = defaultItemText
                })
                .Distinct();
            return Json(result);
        }

        var finalResult = values
            .Where(f => f.FilterLevel1Value == filterLevel1Value &&
                        f.FilterLevel2Value == filterLevel2Value &&
                        f.FilterLevel3Value == filterLevel3Value)
            .Select(f => new
            {
                filterLevel1Value = f.FilterLevel1Value,
                filterLevel2Value = f.FilterLevel2Value,
                filterLevel3Value = f.FilterLevel3Value,
                defaultItemText = defaultItemText
            })
            .Distinct();

		_logger.LogDebug("<<< Exiting GetFilterLevelValues(filterLevel1Value={FilterLevel1Value},filterLevel2Value={FilterLevel2Value},filterLevel3Value={FilterLevel3Value})", filterLevel1Value, filterLevel2Value, filterLevel3Value);
        return Json(finalResult);
    }

    [SaveLastContinueShoppingPage]
    public virtual async Task<IActionResult> SearchByFilterLevelValues(SearchFilterLevelValueModel model, CatalogProductsCommand command)
    {
        if (!_filterLevelSettings.FilterLevelEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        
        if (model == null)
            model = new SearchFilterLevelValueModel();

		_logger.LogDebug("SearchByFilterLevelValues(model={Model},command={Command}): model → {Model1}", model, command, model);
        model = await _filterLevelValueModelFactory.PrepareSearchFilterLevelValueModelAsync(model, command);

		_logger.LogDebug("<<< Exiting SearchByFilterLevelValues(model={Model},command={Command})", model, command);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SearchProductsByFilterLevelValues(SearchFilterLevelValueModel searchModel, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering SearchProductsByFilterLevelValues(searchModel={SearchModel},command={Command})", searchModel, command);
        if (searchModel == null)
            searchModel = new SearchFilterLevelValueModel();

		_logger.LogDebug("SearchProductsByFilterLevelValues(searchModel={SearchModel},command={Command}): searchModel → {SearchModel1}", searchModel, command, searchModel);
        var model = await _catalogModelFactory.PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel, command);

		_logger.LogDebug("SearchProductsByFilterLevelValues(searchModel={SearchModel},command={Command}): model → {Model}", searchModel, command, model);
		_logger.LogDebug("<<< Exiting SearchProductsByFilterLevelValues(searchModel={SearchModel},command={Command})", searchModel, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Utilities

    protected virtual async Task<bool> CheckCategoryAvailabilityAsync(Category category)
    {
        if (category is null)
            return false;

        var isAvailable = true;

        if (category.Deleted)
            isAvailable = false;

        var notAvailable =
            //published?
            !category.Published ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(category) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(category);
        //Check whether the current user has a "Manage categories" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.CATEGORIES_VIEW);
        if (notAvailable && !hasAdminAccess)
            isAvailable = false;

        return isAvailable;
    }

    protected virtual async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
    {
        if (manufacturer == null)
            return false;

        var isAvailable = true;

		_logger.LogDebug("CheckManufacturerAvailabilityAsync(manufacturer={Manufacturer}): isAvailable → {IsAvailable}", manufacturer, isAvailable);
        if (manufacturer.Deleted)
            isAvailable = false;

        var notAvailable =
            //published?
            !manufacturer.Published ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(manufacturer) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(manufacturer);
        //Check whether the current user has a "Manage categories" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.MANUFACTURER_VIEW);
        if (notAvailable && !hasAdminAccess)
			_logger.LogDebug("CheckManufacturerAvailabilityAsync(manufacturer={Manufacturer}): hasAdminAccess → {HasAdminAccess}", manufacturer, hasAdminAccess);
            isAvailable = false;

		_logger.LogDebug("<<< Exiting CheckManufacturerAvailabilityAsync(manufacturer={Manufacturer})", manufacturer);
        return isAvailable;
    }

    protected virtual Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
    {
        var isAvailable = true;

        if (vendor == null || vendor.Deleted || !vendor.Active)
            isAvailable = false;

		_logger.LogDebug("CheckVendorAvailabilityAsync(vendor={Vendor}): isAvailable → {IsAvailable}", vendor, isAvailable);
		_logger.LogDebug("<<< Exiting CheckVendorAvailabilityAsync(vendor={Vendor})", vendor);
        return Task.FromResult(isAvailable);
    }

    #endregion
}