using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CatalogModelFactoryBaseTests : WebTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICatalogModelFactory _catalogModelFactory;
    private Category _category;
    private Product _product;
    private ICategoryService _categoryService;
    private Manufacturer _manufacturer;
    private Vendor _vendor;
    private IHttpContextAccessor _httpContextAccessor;
    private ProductTag _productTag;
    private CatalogSettings _catalogSettings;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _catalogSettings = GetService<CatalogSettings>();
        _categoryService = GetService<ICategoryService>();
		_logger.LogDebug("SetUp(): _categoryService → {_categoryService}", _categoryService);
		_logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
        _catalogModelFactory = GetService<ICatalogModelFactory>();
		_logger.LogDebug("SetUp(): _catalogModelFactory → {_catalogModelFactory}", _catalogModelFactory);
        _category = await _categoryService.GetCategoryByIdAsync(1);
		_logger.LogDebug("SetUp(): _category → {_category}", _category);
        _product = await GetService<IProductService>().GetProductByIdAsync(1);
		_logger.LogDebug("SetUp(): _product → {_product}", _product);
        _manufacturer = await GetService<IManufacturerService>().GetManufacturerByIdAsync(1);
		_logger.LogDebug("SetUp(): _manufacturer → {_manufacturer}", _manufacturer);
        _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
		_logger.LogDebug("SetUp(): _vendor → {_vendor}", _vendor);
        _httpContextAccessor = GetService<IHttpContextAccessor>();
		_logger.LogDebug("SetUp(): _httpContextAccessor → {_httpContextAccessor}", _httpContextAccessor);

        _productTag = await GetService<IProductTagService>().GetProductTagByIdAsync(1);
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _productTag → {_productTag}", _productTag);
    }

    [Test]
    public async Task CanPrepareSearchModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareSearchModel()");
        var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());
        model.AvailableCategories.Any().Should().BeTrue();
		_logger.LogDebug("CanPrepareSearchModel(): model → {Model}", model);
        model.AvailableCategories.Count.Should().Be(17);

        model.AvailableManufacturers.Any().Should().BeTrue();
        model.AvailableManufacturers.Count.Should().Be(4);

        model.AvailableVendors.Any().Should().BeFalse();

        var queryString = _httpContextAccessor.HttpContext.Request.QueryString;
        _httpContextAccessor.HttpContext.Request.Method = HttpMethods.Get;
        _httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?q=t");

        model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());

        _httpContextAccessor.HttpContext.Request.QueryString = queryString;

        model.CatalogProductsModel.WarningMessage.Should()
            .Be($"Search term minimum length is {_catalogSettings.ProductSearchTermMinimumLength} characters");
        model.CatalogProductsModel.Products.Count.Should().Be(0);

        _httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?q=Lenovo");

        model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel
        {
            q = "Lenovo"
        }, new CatalogProductsCommand());
        _httpContextAccessor.HttpContext.Request.QueryString = queryString;

        model.CatalogProductsModel.Products.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanPrepareSearchModel()");
    }

    [Test]
    public async Task CanPrepareCategoryModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCategoryModel()");
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogProductsCommand());

		_logger.LogDebug("CanPrepareCategoryModel(): model → {Model}", model);
        model.Id.Should().Be(_category.Id);
        model.Name.Should().Be(_category.Name);
        model.Description.Should().Be(_category.Description);
        model.MetaKeywords.Should().Be(_category.MetaKeywords);
        model.MetaDescription.Should().Be(_category.MetaDescription);
        model.MetaTitle.Should().Be(_category.MetaTitle);

        model.CategoryBreadcrumb.Any().Should().BeTrue();
        model.CategoryBreadcrumb.FirstOrDefault()?.Name.Should().Be("Computers");
        model.SubCategories.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanPrepareCategoryModel()");
    }

    [Test]
    public void PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareCategoryModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareCategoryModelAsync(_category, null).Wait());
			_logger.LogDebug("<<< Exiting PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareCategoryTemplateViewPath()
    {
		_logger.LogDebug(">>> Entering CanPrepareCategoryTemplateViewPath()");
        var model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(1);
        model.Should().Be("CategoryTemplate.ProductsInGridOrLines");
		_logger.LogDebug("CanPrepareCategoryTemplateViewPath(): model → {Model}", model);

        model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(int.MaxValue);
        model.Should().Be("CategoryTemplate.ProductsInGridOrLines");
		_logger.LogDebug("<<< Exiting CanPrepareCategoryTemplateViewPath()");
    }

    [Test]
    public async Task CanPrepareCategoryNavigationModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCategoryNavigationModel()");
        var model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, 0);

		_logger.LogDebug("CanPrepareCategoryNavigationModel(): model → {Model}", model);
        model.Categories.Count.Should().Be(7);
        model.CurrentCategoryId.Should().Be(_category.Id);

        model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(0, _product.Id);
        model.Categories.Count.Should().Be(7);
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(_product.Id);
		_logger.LogDebug("CanPrepareCategoryNavigationModel(): productCategories → {ProductCategories}", productCategories);
        model.CurrentCategoryId.Should().Be(productCategories.FirstOrDefault()?.CategoryId ?? 0);

        model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, _product.Id);

        model.Categories.Count.Should().Be(7);
        model.CurrentCategoryId.Should().Be(_category.Id);
		_logger.LogDebug("<<< Exiting CanPrepareCategoryNavigationModel()");
    }

    [Test]
    public async Task CanPrepareHomepageCategoryModels()
    {
		_logger.LogDebug(">>> Entering CanPrepareHomepageCategoryModels()");
        var model = await _catalogModelFactory.PrepareHomepageCategoryModelsAsync();

		_logger.LogDebug("CanPrepareHomepageCategoryModels(): model → {Model}", model);
        model.Any().Should().BeTrue();
        model.Count.Should().Be(3);

        var categories = new[] { "Electronics", "Apparel", "Digital downloads" };
		_logger.LogDebug("CanPrepareHomepageCategoryModels(): categories → {Categories}", categories);

        foreach (var categoryModel in model)
            categoryModel.Name.Should().BeOneOf(categories);
			_logger.LogDebug("<<< Exiting CanPrepareHomepageCategoryModels()");
    }

    [Test]
    public async Task CanPrepareManufacturerModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareManufacturerModel()");
        var model = await _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, new CatalogProductsCommand());
        model.Id.Should().Be(_manufacturer.Id);
		_logger.LogDebug("CanPrepareManufacturerModel(): model → {Model}", model);
        model.Name.Should().Be(_manufacturer.Name);
        model.Description.Should().Be(_manufacturer.Description);
        model.MetaKeywords.Should().Be(_manufacturer.MetaKeywords);
        model.MetaDescription.Should().Be(_manufacturer.MetaDescription);
        model.MetaTitle.Should().Be(_manufacturer.MetaTitle);
		_logger.LogDebug("<<< Exiting CanPrepareManufacturerModel()");
    }

    [Test]
    public void PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareManufacturerModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, null).Wait());
			_logger.LogDebug("<<< Exiting PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareManufacturerTemplateViewPath()
    {
		_logger.LogDebug(">>> Entering CanPrepareManufacturerTemplateViewPath()");
        var model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(1);
        model.Should().Be("ManufacturerTemplate.ProductsInGridOrLines");
		_logger.LogDebug("CanPrepareManufacturerTemplateViewPath(): model → {Model}", model);

        model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(int.MaxValue);
        model.Should().Be("ManufacturerTemplate.ProductsInGridOrLines");
		_logger.LogDebug("<<< Exiting CanPrepareManufacturerTemplateViewPath()");
    }

    [Test]
    public async Task CanPrepareManufacturerAllModels()
    {
		_logger.LogDebug(">>> Entering CanPrepareManufacturerAllModels()");
        var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();
        model.Any().Should().BeTrue();
		_logger.LogDebug("CanPrepareManufacturerAllModels(): model → {Model}", model);
        model.Count.Should().Be(3);
        var manufacturers = new[] { "Apple", "HP", "Nike" };
		_logger.LogDebug("CanPrepareManufacturerAllModels(): manufacturers → {Manufacturers}", manufacturers);

        foreach (var manufacturerModel in model)
            manufacturerModel.Name.Should().BeOneOf(manufacturers);
			_logger.LogDebug("<<< Exiting CanPrepareManufacturerAllModels()");
    }

    [Test]
    public async Task CanPrepareManufacturerNavigationModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareManufacturerNavigationModel()");
        var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(_manufacturer.Id);
        model.TotalManufacturers.Should().Be(3);
		_logger.LogDebug("CanPrepareManufacturerNavigationModel(): model → {Model}", model);
        model.Manufacturers.Any().Should().BeTrue();
        model.Manufacturers.Count.Should().Be(2);

        var manufacturers = new[] { "Apple", "HP" };
		_logger.LogDebug("CanPrepareManufacturerNavigationModel(): manufacturers → {Manufacturers}", manufacturers);

        foreach (var manufacturerModel in model.Manufacturers)
            manufacturerModel.Name.Should().BeOneOf(manufacturers);
			_logger.LogDebug("<<< Exiting CanPrepareManufacturerNavigationModel()");
    }

    [Test]
    public async Task CanPrepareVendorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareVendorModel()");
        var model = await _catalogModelFactory.PrepareVendorModelAsync(_vendor, new CatalogProductsCommand());

		_logger.LogDebug("CanPrepareVendorModel(): model → {Model}", model);
        model.Id.Should().Be(_vendor.Id);
        model.Name.Should().Be(_vendor.Name);
        model.Description.Should().Be(_vendor.Description);
        model.MetaKeywords.Should().Be(_vendor.MetaKeywords);
        model.MetaDescription.Should().Be(_vendor.MetaDescription);
        model.MetaTitle.Should().Be(_vendor.MetaTitle);
		_logger.LogDebug("<<< Exiting CanPrepareVendorModel()");
    }

    [Test]
    public void PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareVendorModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareVendorModelAsync(_vendor, null).Wait());
			_logger.LogDebug("<<< Exiting PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareVendorAllModels()
    {
		_logger.LogDebug(">>> Entering CanPrepareVendorAllModels()");
        var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();

		_logger.LogDebug("CanPrepareVendorAllModels(): model → {Model}", model);
        model.Any().Should().BeTrue();
        model.Count.Should().Be(2);
        var vendors = new[] { "Vendor 1", "Vendor 2" };
		_logger.LogDebug("CanPrepareVendorAllModels(): vendors → {Vendors}", vendors);

        foreach (var manufacturerModel in model)
            manufacturerModel.Name.Should().BeOneOf(vendors);
			_logger.LogDebug("<<< Exiting CanPrepareVendorAllModels()");
    }

    [Test]
    public async Task CanPrepareProductsByTagModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductsByTagModel()");
        var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(_productTag, new CatalogProductsCommand());

		_logger.LogDebug("CanPrepareProductsByTagModel(): model → {Model}", model);
        model.Id.Should().Be(_productTag.Id);
        model.TagName.Should().Be(_productTag.Name);
        model.CatalogProductsModel.Products.Count.Should().Be(6);
		_logger.LogDebug("<<< Exiting CanPrepareProductsByTagModel()");
    }

    [Test]
    public void PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareProductsByTagModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareProductsByTagModelAsync(_productTag, null).Wait());
			_logger.LogDebug("<<< Exiting PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareProductTagsAllModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductTagsAllModel()");
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();
        model.Tags.Count.Should().Be(16);
		_logger.LogDebug("<<< Exiting CanPrepareProductTagsAllModel()");
		_logger.LogDebug("CanPrepareProductTagsAllModel(): model → {Model}", model);
    }

    [Test]
    public void PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSearchModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), null).Wait());
			_logger.LogDebug("<<< Exiting PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareSortingOptions()
    {
		_logger.LogDebug(">>> Entering CanPrepareSortingOptions()");
        var model = new CatalogProductsModel();
        var command = new CatalogProductsCommand();
		_logger.LogDebug("CanPrepareSortingOptions(): command → {Command}", command);
		_logger.LogDebug("CanPrepareSortingOptions(): model → {Model}", model);
        await _catalogModelFactory.PrepareSortingOptionsAsync(model, command);

        model.AllowProductSorting.Should().BeTrue();
        model.AvailableSortOptions.Count.Should().Be(6);
        command.OrderBy.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanPrepareSortingOptions()");
    }

    [Test]
    public void PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSortingOptionsAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSortingOptionsAsync(new CatalogProductsModel(), null).Wait());
			_logger.LogDebug("<<< Exiting PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareViewModes()
    {
		_logger.LogDebug(">>> Entering CanPrepareViewModes()");
        var model = new CatalogProductsModel();
        await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogProductsCommand());
		_logger.LogDebug("CanPrepareViewModes(): model → {Model}", model);

        model.AllowProductViewModeChanging.Should().BeTrue();
        model.AvailableViewModes.Count.Should().Be(2);
        model.ViewMode.Should().Be("grid");
		_logger.LogDebug("<<< Exiting CanPrepareViewModes()");
    }

    [Test]
    public void PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareViewModesAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareViewModesAsync(new CatalogProductsModel(), null).Wait());
			_logger.LogDebug("<<< Exiting PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPreparePageSizeOptions()
    {
		_logger.LogDebug(">>> Entering CanPreparePageSizeOptions()");
        var pageSizes = "10, 20, 30";
        var model = new CatalogProductsModel();
		_logger.LogDebug("CanPreparePageSizeOptions(): model → {Model}", model);
        var command = new CatalogProductsCommand();
		_logger.LogDebug("CanPreparePageSizeOptions(): command → {Command}", command);
		_logger.LogDebug("CanPreparePageSizeOptions(): pageSizes → {PageSizes}", pageSizes);
        await _catalogModelFactory.PreparePageSizeOptionsAsync(model, command, true, pageSizes, 0);

        model.AllowCustomersToSelectPageSize.Should().BeTrue();
        model.PageSizeOptions.Count.Should().Be(3);

        foreach (var modelPageSizeOption in model.PageSizeOptions)
        {
            int.TryParse(modelPageSizeOption.Text, out _).Should().BeTrue();
            pageSizes.Contains(modelPageSizeOption.Text).Should().BeTrue();

            modelPageSizeOption.Value.Should().Be(modelPageSizeOption.Text);
        }

        command.PageSize.Should().Be(10);

        await _catalogModelFactory.PreparePageSizeOptionsAsync(model, command, false, "10, 20, 30", 15);

        model.AllowCustomersToSelectPageSize.Should().BeFalse();
        command.PageSize.Should().Be(15);
        model.PageSizeOptions.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanPreparePageSizeOptions()");
    }

    [Test]
    public void PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<NullReferenceException>(() =>
            _catalogModelFactory.PreparePageSizeOptionsAsync(null, new CatalogProductsCommand(), true, string.Empty, 15).Wait());

        Assert.Throws<NullReferenceException>(() =>
            _catalogModelFactory.PreparePageSizeOptionsAsync(new CatalogProductsModel(), null, false, "10, 15, 20", 0).Wait());
			_logger.LogDebug("<<< Exiting PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareCategoryProductsModelAsync()
    {
		_logger.LogDebug(">>> Entering CanPrepareCategoryProductsModelAsync()");
        var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync((await _categoryService.GetAllCategoriesAsync("Notebooks")).First(), new CatalogProductsCommand());
        model.UseAjaxLoading.Should().Be(_catalogSettings.UseAjaxCatalogProductsLoading);
		_logger.LogDebug("CanPrepareCategoryProductsModelAsync(): model → {Model}", model);
        model.AvailableSortOptions.Should().NotBeEmpty();
        model.AvailableViewModes.Should().NotBeEmpty();
        model.Products.Should().NotBeEmpty();
        model.Products.Count.Should().Be(6);
        model.ManufacturerFilter.Manufacturers.Should().NotBeEmpty();
        model.ManufacturerFilter.Manufacturers.Count.Should().Be(2);
        model.SpecificationFilter.Attributes.Should().NotBeEmpty();
        model.SpecificationFilter.Attributes.Count.Should().Be(2);
        model.TotalItems.Should().Be(model.Products.Count);
		_logger.LogDebug("<<< Exiting CanPrepareCategoryProductsModelAsync()");
    }

    [Test]
    public async Task CanPrepareNewProductsModelAsync()
    {
		_logger.LogDebug(">>> Entering CanPrepareNewProductsModelAsync()");
        var model = await _catalogModelFactory.PrepareNewProductsModelAsync(new CatalogProductsCommand());
        model.Products.Any().Should().BeTrue();
		_logger.LogDebug("CanPrepareNewProductsModelAsync(): model → {Model}", model);
        model.Products.Count.Should().Be(6);
        model.UseAjaxLoading.Should().Be(_catalogSettings.UseAjaxCatalogProductsLoading);
		_logger.LogDebug("<<< Exiting CanPrepareNewProductsModelAsync()");
    }
}