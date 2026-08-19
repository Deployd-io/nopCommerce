using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Models.Configuration;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class AvalaraController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AvalaraController(AvalaraTaxManager avalaraTaxManager,
        AvalaraTaxSettings avalaraTaxSettings,
        CurrencySettings currencySettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _avalaraTaxManager);
        _avalaraTaxSettings = avalaraTaxSettings;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _avalaraTaxSettings → {_avalaraTaxSettings}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _avalaraTaxSettings);
        _currencySettings = currencySettings;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _currencySettings → {_currencySettings}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _currencySettings);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _baseAdminModelFactory → {_baseAdminModelFactory}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _baseAdminModelFactory);
        _countryService = countryService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _countryService → {_countryService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _countryService);
        _currencyService = currencyService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _currencyService → {_currencyService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _currencyService);
        _customerService = customerService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _customerService → {_customerService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _genericAttributeService → {_genericAttributeService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _localizationService → {_localizationService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _notificationService → {_notificationService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _permissionService → {_permissionService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _settingService → {_settingService}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _settingService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext);
		_logger.LogDebug("AvalaraController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _workContext → {_workContext}", avalaraTaxManager, avalaraTaxSettings, currencySettings, baseAdminModelFactory, countryService, currencyService, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _workContext);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Configure(string testTaxResult = null)
    {
		_logger.LogDebug(">>> Entering Configure(testTaxResult={TestTaxResult})", testTaxResult);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): customer → {Customer}", testTaxResult, customer);
        var model = new ConfigurationModel
        {
            AccountId = _avalaraTaxSettings.AccountId,
            LicenseKey = _avalaraTaxSettings.LicenseKey,
            CompanyCode = _avalaraTaxSettings.CompanyCode,
            UseSandbox = _avalaraTaxSettings.UseSandbox,
            UseItemClassification = _avalaraTaxSettings.UseItemClassification,
            CommitTransactions = _avalaraTaxSettings.CommitTransactions,
            ValidateAddress = _avalaraTaxSettings.ValidateAddress,
            TaxOriginAddressTypeId = (int)_avalaraTaxSettings.TaxOriginAddressType,
            EnableLogging = _avalaraTaxSettings.EnableLogging,
            UseTaxRateTables = _avalaraTaxSettings.UseTaxRateTables,
            GetTaxRateByAddressOnly = _avalaraTaxSettings.GetTaxRateByAddressOnly,
            EnableCertificates = _avalaraTaxSettings.EnableCertificates,
            AutoValidateCertificate = _avalaraTaxSettings.AutoValidateCertificate,
            AllowEditCustomer = _avalaraTaxSettings.AllowEditCustomer,
            DisplayNoValidCertificatesMessage = _avalaraTaxSettings.DisplayNoValidCertificatesMessage,
            SelectedCustomerRoleIds = _avalaraTaxSettings.CustomerRoleIds,
            SelectedCountryIds = _avalaraTaxSettings.SelectedCountryIds,
            TestTaxResult = testTaxResult
        };
        model.IsConfigured = !string.IsNullOrEmpty(_avalaraTaxSettings.AccountId) && !string.IsNullOrEmpty(_avalaraTaxSettings.LicenseKey);
        model.TaxOriginAddressTypes = (await TaxOriginAddressType.DefaultTaxAddress.ToSelectListAsync(false))
            .Select(type => new SelectListItem(type.Text, type.Value)).ToList();
        model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, AvalaraTaxDefaults.HideGeneralBlock);
        model.HideItemClassificationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, AvalaraTaxDefaults.HideItemClassificationBlock);
        model.HideLogBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, AvalaraTaxDefaults.HideLogBlock);

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): availableRoles → {AvailableRoles}", testTaxResult, availableRoles);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();

        //prepare available of item classification countries (for sync)
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries, false);

        //prepare item classification search model
        model.ItemClassificationSearchModel.AvailableCountries = (await _countryService
                .GetCountriesByIdsAsync(_avalaraTaxSettings.SelectedCountryIds?.ToArray()))
            .Select(country => new SelectListItem(country.Name, country.Id.ToString()))
            .ToList();
        model.ItemClassificationSearchModel.AvailableCountries.Insert(0, new SelectListItem { Text = "*", Value = "0" });

        model.ItemClassificationSearchModel.SetGridPageSize();

        //prepare address model
        await _baseAdminModelFactory.PrepareCountriesAsync(model.TestAddress.AvailableCountries);
        await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.TestAddress.AvailableStates, model.TestAddress.CountryId);

        //prepare tax transaction log model
        model.TaxTransactionLogSearchModel.SetGridPageSize();

        //get active account companies
        var activeCompanies = model.IsConfigured ? await _avalaraTaxManager.GetAccountCompaniesAsync() : null;
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): activeCompanies → {ActiveCompanies}", testTaxResult, activeCompanies);
        if (activeCompanies?.Any() ?? false)
        {
            model.Companies = activeCompanies.OrderBy(company => company.isDefault ?? false ? 0 : 1).Select(company => new SelectListItem
            {
                Text = company.isTest ?? false ? $"{company.name} (Test)" : company.name,
                Value = company.companyCode
            }).ToList();
        }

        var defaultCompanyCode = _avalaraTaxSettings.CompanyCode;
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): defaultCompanyCode → {DefaultCompanyCode}", testTaxResult, defaultCompanyCode);
        if (!model.Companies.Any())
        {
            //add the special item for 'there are no companies' with empty guid value
            var noCompaniesText = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.Company.NotExist");
			_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): noCompaniesText → {NoCompaniesText}", testTaxResult, noCompaniesText);
            model.Companies.Add(new SelectListItem { Text = noCompaniesText, Value = Guid.Empty.ToString() });
            defaultCompanyCode = Guid.Empty.ToString();
        }
        else if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
            defaultCompanyCode = model.Companies.FirstOrDefault()?.Value;

        //set the default company
        var selectedCompany = activeCompanies?.FirstOrDefault(company => company.companyCode.Equals(defaultCompanyCode));
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): selectedCompany → {SelectedCompany}", testTaxResult, selectedCompany);
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): model.CompanyCode → {Arg1}", testTaxResult, model.CompanyCode);
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): _avalaraTaxSettings.CompanyCode → {Arg1}", testTaxResult, _avalaraTaxSettings.CompanyCode);
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): _avalaraTaxSettings.CompanyId → {Arg1}", testTaxResult, _avalaraTaxSettings.CompanyId);
        model.CompanyCode = defaultCompanyCode;
        _avalaraTaxSettings.CompanyCode = defaultCompanyCode;
        _avalaraTaxSettings.CompanyId = selectedCompany?.id;
        await _settingService.SaveSettingAsync(_avalaraTaxSettings);

        //display warning in case of company currency differ from the primary store currency
        var primaryCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("Configure(testTaxResult={TestTaxResult}): primaryCurrency → {PrimaryCurrency}", testTaxResult, primaryCurrency);
        if (!selectedCompany?.baseCurrencyCode?.Equals(primaryCurrency?.CurrencyCode, StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.Company.Currency.Warning"),
                selectedCompany.name, selectedCompany.baseCurrencyCode, primaryCurrency?.CurrencyCode);
            _notificationService.WarningNotification(warning);
        }

		_logger.LogDebug("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Tax.Avalara/Views/Configuration/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        _avalaraTaxSettings.AccountId = model.AccountId;
        _avalaraTaxSettings.LicenseKey = model.LicenseKey;
        _avalaraTaxSettings.CompanyCode = model.CompanyCode;
        _avalaraTaxSettings.UseSandbox = model.UseSandbox;
        _avalaraTaxSettings.CommitTransactions = model.CommitTransactions;
        _avalaraTaxSettings.ValidateAddress = model.ValidateAddress;
        _avalaraTaxSettings.TaxOriginAddressType = (TaxOriginAddressType)model.TaxOriginAddressTypeId;
        _avalaraTaxSettings.EnableLogging = model.EnableLogging;
        _avalaraTaxSettings.UseTaxRateTables = model.UseTaxRateTables;
        _avalaraTaxSettings.GetTaxRateByAddressOnly = model.GetTaxRateByAddressOnly;
        _avalaraTaxSettings.EnableCertificates = model.EnableCertificates;
        _avalaraTaxSettings.AutoValidateCertificate = model.AutoValidateCertificate;
        _avalaraTaxSettings.AllowEditCustomer = model.AllowEditCustomer;
        _avalaraTaxSettings.DisplayNoValidCertificatesMessage = model.DisplayNoValidCertificatesMessage;
        _avalaraTaxSettings.CustomerRoleIds = model.SelectedCustomerRoleIds.ToList();
        await _settingService.SaveSettingAsync(_avalaraTaxSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("saveIC")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> SaveItemClassification(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering SaveItemClassification(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        _avalaraTaxSettings.SelectedCountryIds = model.SelectedCountryIds.ToList();
        _avalaraTaxSettings.UseItemClassification = model.UseItemClassification;
		_logger.LogDebug("SaveItemClassification(model={Model}): _avalaraTaxSettings.UseItemClassification → {Arg1}", model, _avalaraTaxSettings.UseItemClassification);
        await _settingService.SaveSettingAsync(_avalaraTaxSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug("<<< Exiting SaveItemClassification(model={Model})", model);
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("check-credentials")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> CheckCredentials()
    {
		_logger.LogDebug(">>> Entering CheckCredentials()");
        //verify credentials
        var credentials = await _avalaraTaxManager.PingAsync();
		_logger.LogDebug("CheckCredentials(): credentials → {Credentials}", credentials);
        if (credentials?.authenticated ?? false)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Credentials.Verified");
			_logger.LogDebug("CheckCredentials(): locale → {Locale}", locale);
            _notificationService.SuccessNotification(locale);
        }
        else
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Credentials.Declined");
            _notificationService.ErrorNotification(locale);
        }

        //check certificate setup status
        var status = await _avalaraTaxManager.GetCertificateSetupStatusAsync();
		_logger.LogDebug("CheckCredentials(): status → {Status}", status);
        if (status is null)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.NotProvisioned");
            _notificationService.ErrorNotification(locale);
        }
        else if (status == false)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.InProgress");
            _notificationService.WarningNotification(locale);
        }
        else
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.Provisioned");
            _notificationService.SuccessNotification(locale);
        }

		_logger.LogDebug("<<< Exiting CheckCredentials()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("request-certificate-setup")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> RequestCertificateSetup()
    {
		_logger.LogDebug(">>> Entering RequestCertificateSetup()");
        //request the certificate setup and display current status
        var status = await _avalaraTaxManager.GetCertificateSetupStatusAsync(true);
		_logger.LogDebug("RequestCertificateSetup(): status → {Status}", status);
        if (status is null)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.NotProvisioned");
			_logger.LogDebug("RequestCertificateSetup(): locale → {Locale}", locale);
            _notificationService.ErrorNotification(locale);
        }
        else if (status == false)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.InProgress");
            _notificationService.WarningNotification(locale);
        }
        else
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Configuration.Certificates.Provisioned");
            _notificationService.SuccessNotification(locale);
        }

		_logger.LogDebug("<<< Exiting RequestCertificateSetup()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("test-tax")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> TestTaxRequest(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering TestTaxRequest(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        //get result
        var transaction = await _avalaraTaxManager.CreateTestTaxTransactionAsync(new Address
        {
            City = model.TestAddress?.City,
            CountryId = model.TestAddress?.CountryId,
            Address1 = model.TestAddress?.Address1,
            ZipPostalCode = model.TestAddress?.ZipPostalCode,
            StateProvinceId = model.TestAddress?.StateProvinceId
        });

        var testTaxResult = string.Empty;
		_logger.LogDebug("TestTaxRequest(model={Model}): testTaxResult → {TestTaxResult}", model, testTaxResult);
        if (transaction?.totalTax != null)
        {
            //display tax rates by jurisdictions
            testTaxResult = $"Total tax rate: {transaction.totalTax:0.000}% {Environment.NewLine}";
            if (transaction.summary?.Any() ?? false)
            {
                testTaxResult = transaction.summary.Aggregate(testTaxResult, (resultString, rate) =>
                    $"{resultString}Jurisdiction: {rate?.jurisName}, Tax rate: {(rate?.rate ?? 0) * 100:0.000}% {Environment.NewLine}");
            }
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TestTax.Success"));
        }
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TestTax.Error"));

		_logger.LogDebug("<<< Exiting TestTaxRequest(model={Model})", model);
        return await Configure(testTaxResult);
    }

    public async Task<IActionResult> ChangeOriginAddressType(int typeId)
    {
		_logger.LogDebug(">>> Entering ChangeOriginAddressType(typeId={TypeId})", typeId);
        var message = (TaxOriginAddressType)typeId switch
        {
            TaxOriginAddressType.ShippingOrigin => string.Format(await _localizationService
                .GetResourceAsync("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.ShippingOrigin.Warning"), Url.Action("Shipping", "Setting")),
            TaxOriginAddressType.DefaultTaxAddress => string.Format(await _localizationService
                .GetResourceAsync("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.DefaultTaxAddress.Warning"), Url.Action("Tax", "Setting")),
            _ => null
        };

		_logger.LogDebug("<<< Exiting ChangeOriginAddressType(typeId={TypeId})", typeId);
        return Json(new { Result = message });
    }

    public async Task<IActionResult> ChangeEnableCertificates(bool enabled)
    {
		_logger.LogDebug(">>> Entering ChangeEnableCertificates(enabled={Enabled})", enabled);
        var message = string.Empty;
        if (enabled)
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.EnableCertificates.Warning");
			_logger.LogDebug("ChangeEnableCertificates(enabled={Enabled}): locale → {Locale}", enabled, locale);
            message = string.Format(locale, Url.Action("CustomerUser", "Setting"));
        }

		_logger.LogDebug("<<< Exiting ChangeEnableCertificates(enabled={Enabled})", enabled);
        return Json(new { Result = message });
    }

    #endregion
}