using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Vendors;
using Nop.Services.Configuration;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CommonModelFactoryTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICommonModelFactory _commonModelFactory;
    private LocalizationSettings _localizationSettings;
    private IWorkContext _workContext;
    private CustomerSettings _customerSettings;
    private ForumSettings _forumSettings;
    private StoreInformationSettings _storeInformationSettings;
    private NewsSettings _newsSettings;
    private CommonSettings _commonSettings;
    private Vendor _vendor;
    private ISettingService _settingsService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _settingsService = GetService<ISettingService>();
        _localizationSettings = GetService<LocalizationSettings>();
        _forumSettings = GetService<ForumSettings>();

		_logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
		_logger.LogDebug("SetUp(): _localizationSettings → {_localizationSettings}", _localizationSettings);
		_logger.LogDebug("SetUp(): _forumSettings → {_forumSettings}", _forumSettings);
		_logger.LogDebug("SetUp(): _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled → {Arg0}", _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled);
		_logger.LogDebug("SetUp(): _forumSettings.AllowPrivateMessages → {Arg0}", _forumSettings.AllowPrivateMessages);
        _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = true;
        await _settingsService.SaveSettingAsync(_localizationSettings);
        _forumSettings.AllowPrivateMessages = true;
        await _settingsService.SaveSettingAsync(_forumSettings);

        _commonModelFactory = GetService<ICommonModelFactory>();

		_logger.LogDebug("SetUp(): _commonModelFactory → {_commonModelFactory}", _commonModelFactory);
        _workContext = GetService<IWorkContext>();
        _customerSettings = GetService<CustomerSettings>();

		_logger.LogDebug("SetUp(): _workContext → {_workContext}", _workContext);
        _storeInformationSettings = GetService<StoreInformationSettings>();
        _newsSettings = GetService<NewsSettings>();
		_logger.LogDebug("SetUp(): _newsSettings → {_newsSettings}", _newsSettings);
		_logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
		_logger.LogDebug("SetUp(): _storeInformationSettings → {_storeInformationSettings}", _storeInformationSettings);
        _commonSettings = GetService<CommonSettings>();
		_logger.LogDebug("SetUp(): _commonSettings → {_commonSettings}", _commonSettings);

        _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
		_logger.LogDebug("SetUp(): _vendor → {_vendor}", _vendor);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = false;
        await _settingsService.SaveSettingAsync(_localizationSettings);
		_logger.LogDebug("TearDown(): _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled → {Arg0}", _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled);
		_logger.LogDebug("TearDown(): _forumSettings.AllowPrivateMessages → {Arg0}", _forumSettings.AllowPrivateMessages);
        _forumSettings.AllowPrivateMessages = false;
        await _settingsService.SaveSettingAsync(_forumSettings);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareLogoModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareLogoModel()");
        var model = await _commonModelFactory.PrepareLogoModelAsync();
        model.StoreName.Should().NotBeNullOrEmpty();
		_logger.LogDebug("CanPrepareLogoModel(): model → {Model}", model);
        model.StoreName.Should().Be("Your store name");
        model.LogoPath.Should().NotBeNullOrEmpty();
        model.LogoPath.Should()
            .Be($"http://{NopTestsDefaults.HostIpAddress}/Themes/DefaultClean/Content/images/logo.png");
			_logger.LogDebug("<<< Exiting CanPrepareLogoModel()");
    }

    [Test]
    public async Task CanPrepareLanguageSelectorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareLanguageSelectorModel()");
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		_logger.LogDebug("CanPrepareLanguageSelectorModel(): model → {Model}", model);
        model.CurrentLanguageId.Should().Be(1);
        model.UseImages.Should().Be(_localizationSettings.UseImagesForLanguageSelection);

        model.AvailableLanguages.Should().NotBeNullOrEmpty();
        var lang = model.AvailableLanguages.FirstOrDefault();
		_logger.LogDebug("CanPrepareLanguageSelectorModel(): lang → {Lang}", lang);
        lang.Should().NotBeNull();
        lang?.Name.Should().Be("English");
        lang?.FlagImageFileName.Should().Be("us.png");
		_logger.LogDebug("<<< Exiting CanPrepareLanguageSelectorModel()");
    }

    [Test]
    public async Task CanPrepareCurrencySelectorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCurrencySelectorModel()");
        var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
        model.CurrentCurrencyId.Should().Be(1);
		_logger.LogDebug("CanPrepareCurrencySelectorModel(): model → {Model}", model);
        model.AvailableCurrencies.Should().NotBeNullOrEmpty();
        model.AvailableCurrencies.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanPrepareCurrencySelectorModel()");
    }

    [Test]
    public async Task CanPrepareTaxTypeSelectorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareTaxTypeSelectorModel()");
        var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
        model.CurrentTaxType.Should().Be(await _workContext.GetTaxDisplayTypeAsync());
		_logger.LogDebug("CanPrepareTaxTypeSelectorModel(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CanPrepareTaxTypeSelectorModel()");
    }

    [Test]
    public async Task CanPrepareHeaderLinksModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareHeaderLinksModel()");
        var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

		_logger.LogDebug("CanPrepareHeaderLinksModel(): model → {Model}", model);
        model.RegistrationType.Should().Be(_customerSettings.UserRegistrationType);
        model.IsAuthenticated.Should().BeTrue();
        model.CustomerName.Should().Be("John");
        model.ShoppingCartEnabled.Should().BeTrue();
        model.WishlistEnabled.Should().BeTrue();
        model.AllowPrivateMessages.Should().Be(_forumSettings.AllowPrivateMessages);
        model.UnreadPrivateMessages.Should().BeEmpty();
        model.AlertMessage.Should().BeEmpty();
        model.ShoppingCartItems.Should().Be(0);
        model.WishlistItems.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanPrepareHeaderLinksModel()");
    }

    [Test]
    public async Task CanPrepareAdminHeaderLinksModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareAdminHeaderLinksModel()");
        var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
        model.ImpersonatedCustomerName.Should().Be("John");
		_logger.LogDebug("CanPrepareAdminHeaderLinksModel(): model → {Model}", model);
        model.IsCustomerImpersonated.Should().BeFalse();
        model.DisplayAdminLink.Should().BeTrue();
        model.EditPageUrl.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanPrepareAdminHeaderLinksModel()");
    }

    [Test]
    public async Task CanPrepareSocialModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareSocialModel()");
        var model = await _commonModelFactory.PrepareSocialModelAsync();

		_logger.LogDebug("CanPrepareSocialModel(): model → {Model}", model);
        model.FacebookLink.Should().Be(_storeInformationSettings.FacebookLink);
        model.TwitterLink.Should().Be(_storeInformationSettings.TwitterLink);
        model.YoutubeLink.Should().Be(_storeInformationSettings.YoutubeLink);
        model.InstagramLink.Should().Be(_storeInformationSettings.InstagramLink);
        model.WorkingLanguageId.Should().Be(1);
        model.NewsEnabled.Should().Be(_newsSettings.Enabled);
		_logger.LogDebug("<<< Exiting CanPrepareSocialModel()");
    }

    [Test]
    public async Task CanPrepareFooterModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareFooterModel()");
        var model = await _commonModelFactory.PrepareFooterModelAsync();

		_logger.LogDebug("CanPrepareFooterModel(): model → {Model}", model);
        model.StoreName.Should().Be("Your store name");
        model.HidePoweredByNopCommerce.Should().Be(_storeInformationSettings.HidePoweredByNopCommerce);
		_logger.LogDebug("<<< Exiting CanPrepareFooterModel()");
    }

    [Test]
    public async Task CanPrepareContactUsModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareContactUsModel()");
        var model = new ContactUsModel();
        model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);
		_logger.LogDebug("CanPrepareContactUsModel(): model → {Model}", model);

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
		_logger.LogDebug("CanPrepareContactUsModel(): model.SubjectEnabled → {Arg0}", model.SubjectEnabled);
        model.DisplayCaptcha.Should().BeFalse();
        model.Email.Should().BeNullOrEmpty();
        model.FullName.Should().BeNullOrEmpty();

        model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha.Should().BeFalse();
        model.Email.Should().Be(NopTestsDefaults.AdminEmail);
        model.FullName.Should().Be("John Smith");
		_logger.LogDebug("<<< Exiting CanPrepareContactUsModel()");
    }

    [Test]
    public void PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()");
        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactUsModelAsync(null, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactUsModelAsync(null, false).Wait());
			_logger.LogDebug("<<< Exiting PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()");
    }

    [Test]
    public async Task CanPrepareContactVendorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareContactVendorModel()");
        var model = new ContactVendorModel();
        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, true);
		_logger.LogDebug("CanPrepareContactVendorModel(): model → {Model}", model);
        model.Email.Should().BeNullOrEmpty();
        model.FullName.Should().BeNullOrEmpty();

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
		_logger.LogDebug("CanPrepareContactVendorModel(): model.SubjectEnabled → {Arg0}", model.SubjectEnabled);
        model.DisplayCaptcha.Should().BeFalse();
        model.VendorId.Should().Be(_vendor.Id);
        model.VendorName.Should().Be(_vendor.Name);

        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, false);

        model.Email.Should().Be(NopTestsDefaults.AdminEmail);
        model.FullName.Should().Be("John Smith");

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha.Should().BeFalse();
        model.VendorId.Should().Be(_vendor.Id);
        model.VendorName.Should().Be(_vendor.Name);
		_logger.LogDebug("<<< Exiting CanPrepareContactVendorModel()");
    }

    [Test]
    public void PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()");
        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, false).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, false).Wait());
			_logger.LogDebug("<<< Exiting PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()");
    }

    [Test]
    public async Task CanPrepareStoreThemeSelectorModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareStoreThemeSelectorModel()");
        var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
        model.CurrentStoreTheme.Should().NotBeNull();
		_logger.LogDebug("CanPrepareStoreThemeSelectorModel(): model → {Model}", model);
        model.CurrentStoreTheme.Name.Should().Be("DefaultClean");
        model.CurrentStoreTheme.Title.Should().Be("Default clean");
        model.AvailableStoreThemes.Should().NotBeNull();
        model.AvailableStoreThemes.Count.Should().BeGreaterThan(0);
		_logger.LogDebug("<<< Exiting CanPrepareStoreThemeSelectorModel()");
    }

    [Test]
    public async Task CanPrepareFaviconAndAppIconsModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareFaviconAndAppIconsModel()");
        var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
        model.HeadCode.Should().Be(_commonSettings.FaviconAndAppIconsHeadCode);
		_logger.LogDebug("CanPrepareFaviconAndAppIconsModel(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CanPrepareFaviconAndAppIconsModel()");
    }

    [Test]
    public async Task CanPrepareRobotsTextFile()
    {
		_logger.LogDebug(">>> Entering CanPrepareRobotsTextFile()");
        var model = await _commonModelFactory.PrepareRobotsTextFileAsync();
        model.Should().NotBeNullOrEmpty();
		_logger.LogDebug("CanPrepareRobotsTextFile(): model → {Model}", model);

        model.Trim().Split(Environment.NewLine).Length.Should().Be(165);
		_logger.LogDebug("<<< Exiting CanPrepareRobotsTextFile()");
    }
}