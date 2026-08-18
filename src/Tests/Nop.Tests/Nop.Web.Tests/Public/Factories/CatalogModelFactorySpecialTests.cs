using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CatalogModelFactorySpecialTests : WebTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ISettingService _settingsService;
    private CatalogSettings _catalogSettings;
    private Category _category;
    private VendorSettings _vendorSettings;
    private ICatalogModelFactory _catalogModelFactory;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _settingsService = GetService<ISettingService>();

		_logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
        var categoryService = GetService<ICategoryService>();
		_logger.LogDebug("SetUp(): categoryService → {CategoryService}", categoryService);
        _category = await categoryService.GetCategoryByIdAsync(1);

		_logger.LogDebug("SetUp(): _category → {_category}", _category);
        _vendorSettings = GetService<VendorSettings>();
        _vendorSettings.VendorsBlockItemsToDisplay = 1;
		_logger.LogDebug("SetUp(): _vendorSettings → {_vendorSettings}", _vendorSettings);
		_logger.LogDebug("SetUp(): _vendorSettings.VendorsBlockItemsToDisplay → {Arg0}", _vendorSettings.VendorsBlockItemsToDisplay);
		_logger.LogDebug("SetUp(): _vendorSettings.AllowSearchByVendor → {Arg0}", _vendorSettings.AllowSearchByVendor);
        _vendorSettings.AllowSearchByVendor = true;

        await _settingsService.SaveSettingAsync(_vendorSettings);

        _catalogSettings = GetService<CatalogSettings>();

		_logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
		_logger.LogDebug("SetUp(): _catalogSettings.AllowProductViewModeChanging → {Arg0}", _catalogSettings.AllowProductViewModeChanging);
		_logger.LogDebug("SetUp(): _catalogSettings.CategoryBreadcrumbEnabled → {Arg0}", _catalogSettings.CategoryBreadcrumbEnabled);
		_logger.LogDebug("SetUp(): _catalogSettings.ShowProductsFromSubcategories → {Arg0}", _catalogSettings.ShowProductsFromSubcategories);
		_logger.LogDebug("SetUp(): _catalogSettings.ShowCategoryProductNumber → {Arg0}", _catalogSettings.ShowCategoryProductNumber);
		_logger.LogDebug("SetUp(): _catalogSettings.ShowCategoryProductNumberIncludingSubcategories → {Arg0}", _catalogSettings.ShowCategoryProductNumberIncludingSubcategories);
		_logger.LogDebug("SetUp(): _catalogSettings.NumberOfProductTags → {Arg0}", _catalogSettings.NumberOfProductTags);
        _catalogSettings.AllowProductViewModeChanging = false;
        _catalogSettings.CategoryBreadcrumbEnabled = false;
        _catalogSettings.ShowProductsFromSubcategories = true;
        _catalogSettings.ShowCategoryProductNumber = true;
        _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = true;
        _catalogSettings.NumberOfProductTags = 20;

        await _settingsService.SaveSettingAsync(_catalogSettings);

        _catalogModelFactory = GetService<ICatalogModelFactory>();
		_logger.LogDebug("SetUp(): _catalogModelFactory → {_catalogModelFactory}", _catalogModelFactory);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _vendorSettings.VendorsBlockItemsToDisplay = 0;
        _vendorSettings.AllowSearchByVendor = false;
		_logger.LogDebug("TearDown(): _vendorSettings.AllowSearchByVendor → {Arg0}", _vendorSettings.AllowSearchByVendor);
		_logger.LogDebug("TearDown(): _vendorSettings.VendorsBlockItemsToDisplay → {Arg0}", _vendorSettings.VendorsBlockItemsToDisplay);
        await _settingsService.SaveSettingAsync(_vendorSettings);

        _catalogSettings.AllowProductViewModeChanging = true;
		_logger.LogDebug("TearDown(): _catalogSettings.AllowProductViewModeChanging → {Arg0}", _catalogSettings.AllowProductViewModeChanging);
		_logger.LogDebug("TearDown(): _catalogSettings.CategoryBreadcrumbEnabled → {Arg0}", _catalogSettings.CategoryBreadcrumbEnabled);
		_logger.LogDebug("TearDown(): _catalogSettings.ShowProductsFromSubcategories → {Arg0}", _catalogSettings.ShowProductsFromSubcategories);
		_logger.LogDebug("TearDown(): _catalogSettings.ShowCategoryProductNumber → {Arg0}", _catalogSettings.ShowCategoryProductNumber);
		_logger.LogDebug("TearDown(): _catalogSettings.ShowCategoryProductNumberIncludingSubcategories → {Arg0}", _catalogSettings.ShowCategoryProductNumberIncludingSubcategories);
		_logger.LogDebug("TearDown(): _catalogSettings.NumberOfProductTags → {Arg0}", _catalogSettings.NumberOfProductTags);
        _catalogSettings.CategoryBreadcrumbEnabled = true;
        _catalogSettings.ShowProductsFromSubcategories = false;
        _catalogSettings.ShowCategoryProductNumber = false;
        _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = false;
        _catalogSettings.NumberOfProductTags = 15;
        await _settingsService.SaveSettingAsync(_catalogSettings);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareVendorNavigationModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareVendorNavigationModel()");
        var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
        model.TotalVendors.Should().Be(2);
		_logger.LogDebug("CanPrepareVendorNavigationModel(): model → {Model}", model);
        model.Vendors.Any().Should().BeTrue();
        model.Vendors.Count.Should().Be(1);
        model.Vendors[0].Name.Should().Be("Vendor 1");
		_logger.LogDebug("<<< Exiting CanPrepareVendorNavigationModel()");
    }

    [Test]
    public async Task PrepareSearchModelShouldDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareSearchModelShouldDependOnSettings()");
        var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());

		_logger.LogDebug("PrepareSearchModelShouldDependOnSettings(): model → {Model}", model);
        model.AvailableVendors.Any().Should().BeTrue();
        model.AvailableVendors.Count.Should().Be(3);
		_logger.LogDebug("<<< Exiting PrepareSearchModelShouldDependOnSettings()");
    }

    [Test]
    public async Task PrepareCategoryModelShouldDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareCategoryModelShouldDependOnSettings()");
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogProductsCommand());

		_logger.LogDebug("PrepareCategoryModelShouldDependOnSettings(): model → {Model}", model);
        model.CategoryBreadcrumb.Any().Should().BeFalse();
        model.SubCategories.Count.Should().Be(3);
        model.CatalogProductsModel.Products.Count.Should().Be(6);
		_logger.LogDebug("<<< Exiting PrepareCategoryModelShouldDependOnSettings()");
    }

    [Test]
    public async Task CanPreparePopularProductTagsModel()
    {
		_logger.LogDebug(">>> Entering CanPreparePopularProductTagsModel()");
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync(_catalogSettings.NumberOfProductTags);

		_logger.LogDebug("CanPreparePopularProductTagsModel(): model → {Model}", model);
        model.Tags.Count.Should().Be(16);
        model.TotalTags.Should().Be(16);
		_logger.LogDebug("<<< Exiting CanPreparePopularProductTagsModel()");
    }

    [Test]
    public async Task PrepareViewModesShouldDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareViewModesShouldDependOnSettings()");
        var model = new CatalogProductsModel();
        await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogProductsCommand
        {
			_logger.LogDebug("PrepareViewModesShouldDependOnSettings(): model → {Model}", model);
            ViewMode = "list"
        });

        model.AllowProductViewModeChanging.Should().BeFalse();
        model.AvailableViewModes.Count.Should().Be(0);
        model.ViewMode.Should().Be("list");
		_logger.LogDebug("<<< Exiting PrepareViewModesShouldDependOnSettings()");
    }
}