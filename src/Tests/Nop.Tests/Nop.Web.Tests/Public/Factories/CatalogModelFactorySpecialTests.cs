using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CatalogModelFactorySpecialTests : WebTest
{
    private ISettingService _settingsService;
    private CatalogSettings _catalogSettings;
    private Category _category;
    private VendorSettings _vendorSettings;
    private ICatalogModelFactory _catalogModelFactory;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _settingsService = GetService<ISettingService>();

		logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
		logger.LogDebug("SetUp(): _vendorSettings.VendorsBlockItemsToDisplay → {Arg0}", _vendorSettings.VendorsBlockItemsToDisplay);
		logger.LogDebug("SetUp(): _vendorSettings.AllowSearchByVendor → {Arg0}", _vendorSettings.AllowSearchByVendor);
        var categoryService = GetService<ICategoryService>();
        _category = await categoryService.GetCategoryByIdAsync(1);
		logger.LogDebug("SetUp(): categoryService → {CategoryService}", categoryService);
		logger.LogDebug("SetUp(): _category → {_category}", _category);

        _vendorSettings = GetService<VendorSettings>();
        _vendorSettings.VendorsBlockItemsToDisplay = 1;
		logger.LogDebug("SetUp(): _vendorSettings → {_vendorSettings}", _vendorSettings);
        _vendorSettings.AllowSearchByVendor = true;

        await _settingsService.SaveSettingAsync(_vendorSettings);

        _catalogSettings = GetService<CatalogSettings>();
		logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
		logger.LogDebug("SetUp(): _catalogSettings.AllowProductViewModeChanging → {Arg0}", _catalogSettings.AllowProductViewModeChanging);
		logger.LogDebug("SetUp(): _catalogSettings.CategoryBreadcrumbEnabled → {Arg0}", _catalogSettings.CategoryBreadcrumbEnabled);
		logger.LogDebug("SetUp(): _catalogSettings.ShowProductsFromSubcategories → {Arg0}", _catalogSettings.ShowProductsFromSubcategories);
		logger.LogDebug("SetUp(): _catalogSettings.ShowCategoryProductNumber → {Arg0}", _catalogSettings.ShowCategoryProductNumber);
		logger.LogDebug("SetUp(): _catalogSettings.ShowCategoryProductNumberIncludingSubcategories → {Arg0}", _catalogSettings.ShowCategoryProductNumberIncludingSubcategories);
		logger.LogDebug("SetUp(): _catalogSettings.NumberOfProductTags → {Arg0}", _catalogSettings.NumberOfProductTags);

        _catalogSettings.AllowProductViewModeChanging = false;
        _catalogSettings.CategoryBreadcrumbEnabled = false;
        _catalogSettings.ShowProductsFromSubcategories = true;
        _catalogSettings.ShowCategoryProductNumber = true;
        _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = true;
        _catalogSettings.NumberOfProductTags = 20;

        await _settingsService.SaveSettingAsync(_catalogSettings);

        _catalogModelFactory = GetService<ICatalogModelFactory>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _catalogModelFactory → {_catalogModelFactory}", _catalogModelFactory);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        _vendorSettings.VendorsBlockItemsToDisplay = 0;
        _vendorSettings.AllowSearchByVendor = false;
		logger.LogDebug("TearDown(): _vendorSettings.AllowSearchByVendor → {Arg0}", _vendorSettings.AllowSearchByVendor);
		logger.LogDebug("TearDown(): _vendorSettings.VendorsBlockItemsToDisplay → {Arg0}", _vendorSettings.VendorsBlockItemsToDisplay);
        await _settingsService.SaveSettingAsync(_vendorSettings);

        _catalogSettings.AllowProductViewModeChanging = true;
		logger.LogDebug("TearDown(): _catalogSettings.AllowProductViewModeChanging → {Arg0}", _catalogSettings.AllowProductViewModeChanging);
		logger.LogDebug("TearDown(): _catalogSettings.CategoryBreadcrumbEnabled → {Arg0}", _catalogSettings.CategoryBreadcrumbEnabled);
		logger.LogDebug("TearDown(): _catalogSettings.ShowProductsFromSubcategories → {Arg0}", _catalogSettings.ShowProductsFromSubcategories);
		logger.LogDebug("TearDown(): _catalogSettings.ShowCategoryProductNumber → {Arg0}", _catalogSettings.ShowCategoryProductNumber);
		logger.LogDebug("TearDown(): _catalogSettings.ShowCategoryProductNumberIncludingSubcategories → {Arg0}", _catalogSettings.ShowCategoryProductNumberIncludingSubcategories);
		logger.LogDebug("TearDown(): _catalogSettings.NumberOfProductTags → {Arg0}", _catalogSettings.NumberOfProductTags);
        _catalogSettings.CategoryBreadcrumbEnabled = true;
        _catalogSettings.ShowProductsFromSubcategories = false;
        _catalogSettings.ShowCategoryProductNumber = false;
        _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = false;
        _catalogSettings.NumberOfProductTags = 15;
        await _settingsService.SaveSettingAsync(_catalogSettings);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareVendorNavigationModel()
    {
		logger.LogDebug(">>> Entering CanPrepareVendorNavigationModel()");
        var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
        model.TotalVendors.Should().Be(2);
		logger.LogDebug("CanPrepareVendorNavigationModel(): model → {Model}", model);
        model.Vendors.Any().Should().BeTrue();
        model.Vendors.Count.Should().Be(1);
        model.Vendors[0].Name.Should().Be("Vendor 1");
		logger.LogDebug("<<< Exiting CanPrepareVendorNavigationModel()");
    }

    [Test]
    public async Task PrepareSearchModelShouldDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareSearchModelShouldDependOnSettings()");
        var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());

		logger.LogDebug("PrepareSearchModelShouldDependOnSettings(): model → {Model}", model);
        model.AvailableVendors.Any().Should().BeTrue();
        model.AvailableVendors.Count.Should().Be(3);
		logger.LogDebug("<<< Exiting PrepareSearchModelShouldDependOnSettings()");
    }

    [Test]
    public async Task PrepareCategoryModelShouldDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareCategoryModelShouldDependOnSettings()");
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogProductsCommand());

		logger.LogDebug("PrepareCategoryModelShouldDependOnSettings(): model → {Model}", model);
        model.CategoryBreadcrumb.Any().Should().BeFalse();
        model.SubCategories.Count.Should().Be(3);
        model.CatalogProductsModel.Products.Count.Should().Be(6);
		logger.LogDebug("<<< Exiting PrepareCategoryModelShouldDependOnSettings()");
    }

    [Test]
    public async Task CanPreparePopularProductTagsModel()
    {
		logger.LogDebug(">>> Entering CanPreparePopularProductTagsModel()");
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync(_catalogSettings.NumberOfProductTags);

		logger.LogDebug("CanPreparePopularProductTagsModel(): model → {Model}", model);
        model.Tags.Count.Should().Be(16);
        model.TotalTags.Should().Be(16);
		logger.LogDebug("<<< Exiting CanPreparePopularProductTagsModel()");
    }

    [Test]
    public async Task PrepareViewModesShouldDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareViewModesShouldDependOnSettings()");
        var model = new CatalogProductsModel();
        await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogProductsCommand
        {
		logger.LogDebug("PrepareViewModesShouldDependOnSettings(): model → {Model}", model);
            ViewMode = "list"
        });

        model.AllowProductViewModeChanging.Should().BeFalse();
        model.AvailableViewModes.Count.Should().Be(0);
        model.ViewMode.Should().Be("list");
		logger.LogDebug("<<< Exiting PrepareViewModesShouldDependOnSettings()");
    }
}