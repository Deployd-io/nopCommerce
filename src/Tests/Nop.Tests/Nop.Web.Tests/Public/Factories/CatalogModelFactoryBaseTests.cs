using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CatalogModelFactoryBaseTests : WebTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _catalogSettings = GetService<CatalogSettings>();
        _categoryService = GetService<ICategoryService>();
		logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
		logger.LogDebug("SetUp(): _categoryService → {_categoryService}", _categoryService);
        _catalogModelFactory = GetService<ICatalogModelFactory>();
        _category = await _categoryService.GetCategoryByIdAsync(1);
		logger.LogDebug("SetUp(): _catalogModelFactory → {_catalogModelFactory}", _catalogModelFactory);
		logger.LogDebug("SetUp(): _category → {_category}", _category);
        _product = await GetService<IProductService>().GetProductByIdAsync(1);
        _manufacturer = await GetService<IManufacturerService>().GetManufacturerByIdAsync(1);
		logger.LogDebug("SetUp(): _product → {_product}", _product);
		logger.LogDebug("SetUp(): _manufacturer → {_manufacturer}", _manufacturer);
        _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
        _httpContextAccessor = GetService<IHttpContextAccessor>();
		logger.LogDebug("SetUp(): _vendor → {_vendor}", _vendor);
		logger.LogDebug("SetUp(): _httpContextAccessor → {_httpContextAccessor}", _httpContextAccessor);

        _productTag = await GetService<IProductTagService>().GetProductTagByIdAsync(1);
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _productTag → {_productTag}", _productTag);
    }

    [Test]
    public async Task CanPrepareSearchModel()
    {
		logger.LogDebug(">>> Entering CanPrepareSearchModel()");
        var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());
        model.AvailableCategories.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareSearchModel(): model → {Model}", model);
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
		logger.LogDebug("<<< Exiting CanPrepareSearchModel()");
    }

    [Test]
    public async Task CanPrepareCategoryModel()
    {
		logger.LogDebug(">>> Entering CanPrepareCategoryModel()");
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogProductsCommand());

		logger.LogDebug("CanPrepareCategoryModel(): model → {Model}", model);
        model.Id.Should().Be(_category.Id);
        model.Name.Should().Be(_category.Name);
        model.Description.Should().Be(_category.Description);
        model.MetaKeywords.Should().Be(_category.MetaKeywords);
        model.MetaDescription.Should().Be(_category.MetaDescription);
        model.MetaTitle.Should().Be(_category.MetaTitle);

        model.CategoryBreadcrumb.Any().Should().BeTrue();
        model.CategoryBreadcrumb.FirstOrDefault()?.Name.Should().Be("Computers");
        model.SubCategories.Count.Should().Be(3);
		logger.LogDebug("<<< Exiting CanPrepareCategoryModel()");
    }

    [Test]
    public void PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareCategoryModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareCategoryModelAsync(_category, null).Wait());
			logger.LogDebug("<<< Exiting PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareCategoryTemplateViewPath()
    {
		logger.LogDebug(">>> Entering CanPrepareCategoryTemplateViewPath()");
        var model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(1);
        model.Should().Be("CategoryTemplate.ProductsInGridOrLines");
		logger.LogDebug("CanPrepareCategoryTemplateViewPath(): model → {Model}", model);

        model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(int.MaxValue);
        model.Should().Be("CategoryTemplate.ProductsInGridOrLines");
		logger.LogDebug("<<< Exiting CanPrepareCategoryTemplateViewPath()");
    }

    [Test]
    public async Task CanPrepareCategoryNavigationModel()
    {
		logger.LogDebug(">>> Entering CanPrepareCategoryNavigationModel()");
        var model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, 0);

		logger.LogDebug("CanPrepareCategoryNavigationModel(): model → {Model}", model);
        model.Categories.Count.Should().Be(7);
        model.CurrentCategoryId.Should().Be(_category.Id);

        model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(0, _product.Id);
        model.Categories.Count.Should().Be(7);
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(_product.Id);
        model.CurrentCategoryId.Should().Be(productCategories.FirstOrDefault()?.CategoryId ?? 0);
		logger.LogDebug("CanPrepareCategoryNavigationModel(): productCategories → {ProductCategories}", productCategories);

        model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, _product.Id);

        model.Categories.Count.Should().Be(7);
        model.CurrentCategoryId.Should().Be(_category.Id);
		logger.LogDebug("<<< Exiting CanPrepareCategoryNavigationModel()");
    }

    [Test]
    public async Task CanPrepareHomepageCategoryModels()
    {
		logger.LogDebug(">>> Entering CanPrepareHomepageCategoryModels()");
        var model = await _catalogModelFactory.PrepareHomepageCategoryModelsAsync();

		logger.LogDebug("CanPrepareHomepageCategoryModels(): model → {Model}", model);
        model.Any().Should().BeTrue();
        model.Count.Should().Be(3);

        var categories = new[] { "Electronics", "Apparel", "Digital downloads" };

		logger.LogDebug("CanPrepareHomepageCategoryModels(): categories → {Categories}", categories);
        foreach (var categoryModel in model)
            categoryModel.Name.Should().BeOneOf(categories);
			logger.LogDebug("<<< Exiting CanPrepareHomepageCategoryModels()");
    }

    [Test]
    public async Task CanPrepareManufacturerModel()
    {
		logger.LogDebug(">>> Entering CanPrepareManufacturerModel()");
        var model = await _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, new CatalogProductsCommand());
        model.Id.Should().Be(_manufacturer.Id);
		logger.LogDebug("CanPrepareManufacturerModel(): model → {Model}", model);
        model.Name.Should().Be(_manufacturer.Name);
        model.Description.Should().Be(_manufacturer.Description);
        model.MetaKeywords.Should().Be(_manufacturer.MetaKeywords);
        model.MetaDescription.Should().Be(_manufacturer.MetaDescription);
        model.MetaTitle.Should().Be(_manufacturer.MetaTitle);
		logger.LogDebug("<<< Exiting CanPrepareManufacturerModel()");
    }

    [Test]
    public void PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareManufacturerModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, null).Wait());
			logger.LogDebug("<<< Exiting PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareManufacturerTemplateViewPath()
    {
		logger.LogDebug(">>> Entering CanPrepareManufacturerTemplateViewPath()");
        var model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(1);
        model.Should().Be("ManufacturerTemplate.ProductsInGridOrLines");
		logger.LogDebug("CanPrepareManufacturerTemplateViewPath(): model → {Model}", model);

        model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(int.MaxValue);
        model.Should().Be("ManufacturerTemplate.ProductsInGridOrLines");
		logger.LogDebug("<<< Exiting CanPrepareManufacturerTemplateViewPath()");
    }

    [Test]
    public async Task CanPrepareManufacturerAllModels()
    {
		logger.LogDebug(">>> Entering CanPrepareManufacturerAllModels()");
        var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();
        model.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareManufacturerAllModels(): model → {Model}", model);
        model.Count.Should().Be(3);
        var manufacturers = new[] { "Apple", "HP", "Nike" };

		logger.LogDebug("CanPrepareManufacturerAllModels(): manufacturers → {Manufacturers}", manufacturers);
        foreach (var manufacturerModel in model)
            manufacturerModel.Name.Should().BeOneOf(manufacturers);
			logger.LogDebug("<<< Exiting CanPrepareManufacturerAllModels()");
    }

    [Test]
    public async Task CanPrepareManufacturerNavigationModel()
    {
		logger.LogDebug(">>> Entering CanPrepareManufacturerNavigationModel()");
        var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(_manufacturer.Id);
        model.TotalManufacturers.Should().Be(3);
		logger.LogDebug("CanPrepareManufacturerNavigationModel(): model → {Model}", model);
        model.Manufacturers.Any().Should().BeTrue();
        model.Manufacturers.Count.Should().Be(2);

        var manufacturers = new[] { "Apple", "HP" };

		logger.LogDebug("CanPrepareManufacturerNavigationModel(): manufacturers → {Manufacturers}", manufacturers);
        foreach (var manufacturerModel in model.Manufacturers)
            manufacturerModel.Name.Should().BeOneOf(manufacturers);
			logger.LogDebug("<<< Exiting CanPrepareManufacturerNavigationModel()");
    }

    [Test]
    public async Task CanPrepareVendorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareVendorModel()");
        var model = await _catalogModelFactory.PrepareVendorModelAsync(_vendor, new CatalogProductsCommand());

		logger.LogDebug("CanPrepareVendorModel(): model → {Model}", model);
        model.Id.Should().Be(_vendor.Id);
        model.Name.Should().Be(_vendor.Name);
        model.Description.Should().Be(_vendor.Description);
        model.MetaKeywords.Should().Be(_vendor.MetaKeywords);
        model.MetaDescription.Should().Be(_vendor.MetaDescription);
        model.MetaTitle.Should().Be(_vendor.MetaTitle);
		logger.LogDebug("<<< Exiting CanPrepareVendorModel()");
    }

    [Test]
    public void PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareVendorModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareVendorModelAsync(_vendor, null).Wait());
			logger.LogDebug("<<< Exiting PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareVendorAllModels()
    {
		logger.LogDebug(">>> Entering CanPrepareVendorAllModels()");
        var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();

		logger.LogDebug("CanPrepareVendorAllModels(): model → {Model}", model);
        model.Any().Should().BeTrue();
        model.Count.Should().Be(2);
        var vendors = new[] { "Vendor 1", "Vendor 2" };

		logger.LogDebug("CanPrepareVendorAllModels(): vendors → {Vendors}", vendors);
        foreach (var manufacturerModel in model)
            manufacturerModel.Name.Should().BeOneOf(vendors);
			logger.LogDebug("<<< Exiting CanPrepareVendorAllModels()");
    }

    [Test]
    public async Task CanPrepareProductsByTagModel()
    {
		logger.LogDebug(">>> Entering CanPrepareProductsByTagModel()");
        var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(_productTag, new CatalogProductsCommand());

		logger.LogDebug("CanPrepareProductsByTagModel(): model → {Model}", model);
        model.Id.Should().Be(_productTag.Id);
        model.TagName.Should().Be(_productTag.Name);
        model.CatalogProductsModel.Products.Count.Should().Be(6);
		logger.LogDebug("<<< Exiting CanPrepareProductsByTagModel()");
    }

    [Test]
    public void PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareProductsByTagModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareProductsByTagModelAsync(_productTag, null).Wait());
			logger.LogDebug("<<< Exiting PrepareVendorModelShouldRaiseExceptionIfProductTagOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareProductTagsAllModel()
    {
		logger.LogDebug(">>> Entering CanPrepareProductTagsAllModel()");
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();
        model.Tags.Count.Should().Be(16);
		logger.LogDebug("<<< Exiting CanPrepareProductTagsAllModel()");
		logger.LogDebug("CanPrepareProductTagsAllModel(): model → {Model}", model);
    }

    [Test]
    public void PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSearchModelAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), null).Wait());
			logger.LogDebug("<<< Exiting PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareSortingOptions()
    {
		logger.LogDebug(">>> Entering CanPrepareSortingOptions()");
        var model = new CatalogProductsModel();
        var command = new CatalogProductsCommand();
		logger.LogDebug("CanPrepareSortingOptions(): model → {Model}", model);
		logger.LogDebug("CanPrepareSortingOptions(): command → {Command}", command);
        await _catalogModelFactory.PrepareSortingOptionsAsync(model, command);

        model.AllowProductSorting.Should().BeTrue();
        model.AvailableSortOptions.Count.Should().Be(6);
        command.OrderBy.Should().Be(0);
		logger.LogDebug("<<< Exiting CanPrepareSortingOptions()");
    }

    [Test]
    public void PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSortingOptionsAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareSortingOptionsAsync(new CatalogProductsModel(), null).Wait());
			logger.LogDebug("<<< Exiting PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareViewModes()
    {
		logger.LogDebug(">>> Entering CanPrepareViewModes()");
        var model = new CatalogProductsModel();
        await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogProductsCommand());
		logger.LogDebug("CanPrepareViewModes(): model → {Model}", model);

        model.AllowProductViewModeChanging.Should().BeTrue();
        model.AvailableViewModes.Count.Should().Be(2);
        model.ViewMode.Should().Be("grid");
		logger.LogDebug("<<< Exiting CanPrepareViewModes()");
    }

    [Test]
    public void PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareViewModesAsync(null, new CatalogProductsCommand()).Wait());

        Assert.Throws<AggregateException>(() =>
            _catalogModelFactory.PrepareViewModesAsync(new CatalogProductsModel(), null).Wait());
			logger.LogDebug("<<< Exiting PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPreparePageSizeOptions()
    {
		logger.LogDebug(">>> Entering CanPreparePageSizeOptions()");
        var pageSizes = "10, 20, 30";
        var model = new CatalogProductsModel();
		logger.LogDebug("CanPreparePageSizeOptions(): pageSizes → {PageSizes}", pageSizes);
		logger.LogDebug("CanPreparePageSizeOptions(): model → {Model}", model);
        var command = new CatalogProductsCommand();
        await _catalogModelFactory.PreparePageSizeOptionsAsync(model, command, true, pageSizes, 0);
		logger.LogDebug("CanPreparePageSizeOptions(): command → {Command}", command);

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
		logger.LogDebug("<<< Exiting CanPreparePageSizeOptions()");
    }

    [Test]
    public void PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
    {
		logger.LogDebug(">>> Entering PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
        Assert.Throws<NullReferenceException>(() =>
            _catalogModelFactory.PreparePageSizeOptionsAsync(null, new CatalogProductsCommand(), true, string.Empty, 15).Wait());

        Assert.Throws<NullReferenceException>(() =>
            _catalogModelFactory.PreparePageSizeOptionsAsync(new CatalogProductsModel(), null, false, "10, 15, 20", 0).Wait());
			logger.LogDebug("<<< Exiting PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareCategoryProductsModelAsync()
    {
		logger.LogDebug(">>> Entering CanPrepareCategoryProductsModelAsync()");
        var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync((await _categoryService.GetAllCategoriesAsync("Notebooks")).First(), new CatalogProductsCommand());
        model.UseAjaxLoading.Should().Be(_catalogSettings.UseAjaxCatalogProductsLoading);
		logger.LogDebug("CanPrepareCategoryProductsModelAsync(): model → {Model}", model);
        model.AvailableSortOptions.Should().NotBeEmpty();
        model.AvailableViewModes.Should().NotBeEmpty();
        model.Products.Should().NotBeEmpty();
        model.Products.Count.Should().Be(6);
        model.ManufacturerFilter.Manufacturers.Should().NotBeEmpty();
        model.ManufacturerFilter.Manufacturers.Count.Should().Be(2);
        model.SpecificationFilter.Attributes.Should().NotBeEmpty();
        model.SpecificationFilter.Attributes.Count.Should().Be(2);
        model.TotalItems.Should().Be(model.Products.Count);
		logger.LogDebug("<<< Exiting CanPrepareCategoryProductsModelAsync()");
    }

    [Test]
    public async Task CanPrepareNewProductsModelAsync()
    {
		logger.LogDebug(">>> Entering CanPrepareNewProductsModelAsync()");
        var model = await _catalogModelFactory.PrepareNewProductsModelAsync(new CatalogProductsCommand());
        model.Products.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareNewProductsModelAsync(): model → {Model}", model);
        model.Products.Count.Should().Be(6);
        model.UseAjaxLoading.Should().Be(_catalogSettings.UseAjaxCatalogProductsLoading);
		logger.LogDebug("<<< Exiting CanPrepareNewProductsModelAsync()");
    }
}