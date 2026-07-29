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

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CommonModelFactoryTests : BaseNopTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _settingsService = GetService<ISettingService>();
        _localizationSettings = GetService<LocalizationSettings>();
		logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
		logger.LogDebug("SetUp(): _localizationSettings → {_localizationSettings}", _localizationSettings);
		logger.LogDebug("SetUp(): _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled → {Arg0}", _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled);
		logger.LogDebug("SetUp(): _forumSettings.AllowPrivateMessages → {Arg0}", _forumSettings.AllowPrivateMessages);
        _forumSettings = GetService<ForumSettings>();

		logger.LogDebug("SetUp(): _forumSettings → {_forumSettings}", _forumSettings);
        _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = true;
        await _settingsService.SaveSettingAsync(_localizationSettings);
        _forumSettings.AllowPrivateMessages = true;
        await _settingsService.SaveSettingAsync(_forumSettings);

        _commonModelFactory = GetService<ICommonModelFactory>();
		logger.LogDebug("SetUp(): _commonModelFactory → {_commonModelFactory}", _commonModelFactory);

        _workContext = GetService<IWorkContext>();
		logger.LogDebug("SetUp(): _workContext → {_workContext}", _workContext);
        _customerSettings = GetService<CustomerSettings>();

		logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
        _storeInformationSettings = GetService<StoreInformationSettings>();
        _newsSettings = GetService<NewsSettings>();
		logger.LogDebug("SetUp(): _storeInformationSettings → {_storeInformationSettings}", _storeInformationSettings);
		logger.LogDebug("SetUp(): _newsSettings → {_newsSettings}", _newsSettings);
        _commonSettings = GetService<CommonSettings>();

		logger.LogDebug("SetUp(): _commonSettings → {_commonSettings}", _commonSettings);
        _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _vendor → {_vendor}", _vendor);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = false;
        await _settingsService.SaveSettingAsync(_localizationSettings);
		logger.LogDebug("TearDown(): _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled → {Arg0}", _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled);
		logger.LogDebug("TearDown(): _forumSettings.AllowPrivateMessages → {Arg0}", _forumSettings.AllowPrivateMessages);
        _forumSettings.AllowPrivateMessages = false;
        await _settingsService.SaveSettingAsync(_forumSettings);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareLogoModel()
    {
		logger.LogDebug(">>> Entering CanPrepareLogoModel()");
        var model = await _commonModelFactory.PrepareLogoModelAsync();
        model.StoreName.Should().NotBeNullOrEmpty();
		logger.LogDebug("CanPrepareLogoModel(): model → {Model}", model);
        model.StoreName.Should().Be("Your store name");
        model.LogoPath.Should().NotBeNullOrEmpty();
        model.LogoPath.Should()
            .Be($"http://{NopTestsDefaults.HostIpAddress}/Themes/DefaultClean/Content/images/logo.png");
			logger.LogDebug("<<< Exiting CanPrepareLogoModel()");
    }

    [Test]
    public async Task CanPrepareLanguageSelectorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareLanguageSelectorModel()");
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		logger.LogDebug("CanPrepareLanguageSelectorModel(): model → {Model}", model);
        model.CurrentLanguageId.Should().Be(1);
        model.UseImages.Should().Be(_localizationSettings.UseImagesForLanguageSelection);

        model.AvailableLanguages.Should().NotBeNullOrEmpty();
        var lang = model.AvailableLanguages.FirstOrDefault();
        lang.Should().NotBeNull();
		logger.LogDebug("CanPrepareLanguageSelectorModel(): lang → {Lang}", lang);
        lang?.Name.Should().Be("English");
        lang?.FlagImageFileName.Should().Be("us.png");
		logger.LogDebug("<<< Exiting CanPrepareLanguageSelectorModel()");
    }

    [Test]
    public async Task CanPrepareCurrencySelectorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareCurrencySelectorModel()");
        var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
        model.CurrentCurrencyId.Should().Be(1);
		logger.LogDebug("CanPrepareCurrencySelectorModel(): model → {Model}", model);
        model.AvailableCurrencies.Should().NotBeNullOrEmpty();
        model.AvailableCurrencies.Count.Should().Be(1);
		logger.LogDebug("<<< Exiting CanPrepareCurrencySelectorModel()");
    }

    [Test]
    public async Task CanPrepareTaxTypeSelectorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareTaxTypeSelectorModel()");
        var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
        model.CurrentTaxType.Should().Be(await _workContext.GetTaxDisplayTypeAsync());
		logger.LogDebug("<<< Exiting CanPrepareTaxTypeSelectorModel()");
		logger.LogDebug("CanPrepareTaxTypeSelectorModel(): model → {Model}", model);
    }

    [Test]
    public async Task CanPrepareHeaderLinksModel()
    {
		logger.LogDebug(">>> Entering CanPrepareHeaderLinksModel()");
        var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

		logger.LogDebug("CanPrepareHeaderLinksModel(): model → {Model}", model);
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
		logger.LogDebug("<<< Exiting CanPrepareHeaderLinksModel()");
    }

    [Test]
    public async Task CanPrepareAdminHeaderLinksModel()
    {
		logger.LogDebug(">>> Entering CanPrepareAdminHeaderLinksModel()");
        var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
        model.ImpersonatedCustomerName.Should().Be("John");
		logger.LogDebug("CanPrepareAdminHeaderLinksModel(): model → {Model}", model);
        model.IsCustomerImpersonated.Should().BeFalse();
        model.DisplayAdminLink.Should().BeTrue();
        model.EditPageUrl.Should().BeNull();
		logger.LogDebug("<<< Exiting CanPrepareAdminHeaderLinksModel()");
    }

    [Test]
    public async Task CanPrepareSocialModel()
    {
		logger.LogDebug(">>> Entering CanPrepareSocialModel()");
        var model = await _commonModelFactory.PrepareSocialModelAsync();

		logger.LogDebug("CanPrepareSocialModel(): model → {Model}", model);
        model.FacebookLink.Should().Be(_storeInformationSettings.FacebookLink);
        model.TwitterLink.Should().Be(_storeInformationSettings.TwitterLink);
        model.YoutubeLink.Should().Be(_storeInformationSettings.YoutubeLink);
        model.InstagramLink.Should().Be(_storeInformationSettings.InstagramLink);
        model.WorkingLanguageId.Should().Be(1);
        model.NewsEnabled.Should().Be(_newsSettings.Enabled);
		logger.LogDebug("<<< Exiting CanPrepareSocialModel()");
    }

    [Test]
    public async Task CanPrepareFooterModel()
    {
		logger.LogDebug(">>> Entering CanPrepareFooterModel()");
        var model = await _commonModelFactory.PrepareFooterModelAsync();

		logger.LogDebug("CanPrepareFooterModel(): model → {Model}", model);
        model.StoreName.Should().Be("Your store name");
        model.HidePoweredByNopCommerce.Should().Be(_storeInformationSettings.HidePoweredByNopCommerce);
		logger.LogDebug("<<< Exiting CanPrepareFooterModel()");
    }

    [Test]
    public async Task CanPrepareContactUsModel()
    {
		logger.LogDebug(">>> Entering CanPrepareContactUsModel()");
        var model = new ContactUsModel();
        model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);
		logger.LogDebug("CanPrepareContactUsModel(): model → {Model}", model);
		logger.LogDebug("CanPrepareContactUsModel(): model.SubjectEnabled → {Arg0}", model.SubjectEnabled);

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha.Should().BeFalse();
        model.Email.Should().BeNullOrEmpty();
        model.FullName.Should().BeNullOrEmpty();

        model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha.Should().BeFalse();
        model.Email.Should().Be(NopTestsDefaults.AdminEmail);
        model.FullName.Should().Be("John Smith");
		logger.LogDebug("<<< Exiting CanPrepareContactUsModel()");
    }

    [Test]
    public void PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()
    {
		logger.LogDebug(">>> Entering PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()");
        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactUsModelAsync(null, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactUsModelAsync(null, false).Wait());
			logger.LogDebug("<<< Exiting PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()");
    }

    [Test]
    public async Task CanPrepareContactVendorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareContactVendorModel()");
        var model = new ContactVendorModel();
        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, true);
		logger.LogDebug("CanPrepareContactVendorModel(): model → {Model}", model);
        model.Email.Should().BeNullOrEmpty();
        model.FullName.Should().BeNullOrEmpty();

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
		logger.LogDebug("CanPrepareContactVendorModel(): model.SubjectEnabled → {Arg0}", model.SubjectEnabled);
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
		logger.LogDebug("<<< Exiting CanPrepareContactVendorModel()");
    }

    [Test]
    public void PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()
    {
		logger.LogDebug(">>> Entering PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()");
        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, false).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, true).Wait());

        Assert.Throws<AggregateException>(() =>
            _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, false).Wait());
			logger.LogDebug("<<< Exiting PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()");
    }

    [Test]
    public async Task CanPrepareStoreThemeSelectorModel()
    {
		logger.LogDebug(">>> Entering CanPrepareStoreThemeSelectorModel()");
        var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
        model.CurrentStoreTheme.Should().NotBeNull();
		logger.LogDebug("CanPrepareStoreThemeSelectorModel(): model → {Model}", model);
        model.CurrentStoreTheme.Name.Should().Be("DefaultClean");
        model.CurrentStoreTheme.Title.Should().Be("Default clean");
        model.AvailableStoreThemes.Should().NotBeNull();
        model.AvailableStoreThemes.Count.Should().BeGreaterThan(0);
		logger.LogDebug("<<< Exiting CanPrepareStoreThemeSelectorModel()");
    }

    [Test]
    public async Task CanPrepareFaviconAndAppIconsModel()
    {
		logger.LogDebug(">>> Entering CanPrepareFaviconAndAppIconsModel()");
        var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
        model.HeadCode.Should().Be(_commonSettings.FaviconAndAppIconsHeadCode);
		logger.LogDebug("<<< Exiting CanPrepareFaviconAndAppIconsModel()");
		logger.LogDebug("CanPrepareFaviconAndAppIconsModel(): model → {Model}", model);
    }

    [Test]
    public async Task CanPrepareRobotsTextFile()
    {
		logger.LogDebug(">>> Entering CanPrepareRobotsTextFile()");
        var model = await _commonModelFactory.PrepareRobotsTextFileAsync();
        model.Should().NotBeNullOrEmpty();
		logger.LogDebug("CanPrepareRobotsTextFile(): model → {Model}", model);

        model.Trim().Split(Environment.NewLine).Length.Should().Be(165);
		logger.LogDebug("<<< Exiting CanPrepareRobotsTextFile()");
    }
}