using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Attributes;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the customer model factory
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly GdprSettings _gdprSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly SecuritySettings _securitySettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        ForumSettings forumSettings,
        GdprSettings gdprSettings,
        IAddressModelFactory addressModelFactory,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAuthenticationPluginManager authenticationPluginManager,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IExternalAuthenticationModelFactory externalAuthenticationModelFactory,
        IExternalAuthenticationService externalAuthenticationService,
        IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IOrderService orderService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        RewardPointsSettings rewardPointsSettings,
        SecuritySettings securitySettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
        _externalAuthenticationService = externalAuthenticationService;
        _externalAuthenticationSettings = externalAuthenticationSettings;
        _forumSettings = forumSettings;
        _gdprSettings = gdprSettings;
        _addressModelFactory = addressModelFactory;
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
        _authenticationPluginManager = authenticationPluginManager;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _orderService = orderService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _securitySettings = securitySettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsent consent, bool accepted)
    {
		_logger.LogError(">>> Entering PrepareGdprConsentModelAsync(consent={Consent},accepted={Accepted})", consent, accepted);
        ArgumentNullException.ThrowIfNull(consent);

        var requiredMessage = await _localizationService.GetLocalizedAsync(consent, x => x.RequiredMessage);
		_logger.LogError("PrepareGdprConsentModelAsync(consent={Consent},accepted={Accepted}): requiredMessage → {RequiredMessage}", consent, accepted, requiredMessage);
		_logger.LogWarning("<<< Exiting PrepareGdprConsentModelAsync(consent={Consent},accepted={Accepted})", consent, accepted);
        return new GdprConsentModel
        {
            Id = consent.Id,
            Message = await _localizationService.GetLocalizedAsync(consent, x => x.Message),
            IsRequired = consent.IsRequired,
            RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
            Accepted = accepted
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the customer info model
    /// </summary>
    /// <param name="model">Customer info model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer info model
    /// </returns>
    public virtual async Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
        bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
    {
		_logger.LogError(">>> Entering PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml})", model, customer, excludeProperties, overrideCustomCustomerAttributesXml);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(customer);

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.AllowCustomersToSetTimeZone → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.AllowCustomersToSetTimeZone);
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): store → {Store}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, store);
        if (!excludeProperties)
        {
            model.VatNumber = customer.VatNumber;
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.VatNumber → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.VatNumber);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.FirstName → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.FirstName);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.LastName → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.LastName);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Gender → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Gender);
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Gender = customer.Gender;
            var dateOfBirth = customer.DateOfBirth;
            if (dateOfBirth.HasValue)
            {
                var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                model.DateOfBirthDay = currentCalendar.GetDayOfMonth(dateOfBirth.Value);
                model.DateOfBirthMonth = currentCalendar.GetMonth(dateOfBirth.Value);
                model.DateOfBirthYear = currentCalendar.GetYear(dateOfBirth.Value);
            }
            model.Company = customer.Company;
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Company → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Company);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddress → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddress);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddress2 → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddress2);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.ZipPostalCode → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.ZipPostalCode);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.City → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.City);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.County → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.County);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CountryId → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CountryId);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StateProvinceId → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StateProvinceId);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Phone → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Phone);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Fax → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Fax);
            model.StreetAddress = customer.StreetAddress;
            model.StreetAddress2 = customer.StreetAddress2;
            model.ZipPostalCode = customer.ZipPostalCode;
            model.City = customer.City;
            model.County = customer.County;
            model.CountryId = customer.CountryId;
            model.StateProvinceId = customer.StateProvinceId;
            model.Phone = customer.Phone;
            model.Fax = customer.Fax;

            //newsletter subscriptions
            var currentSubscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(customer.Email, storeId: store.Id);
			_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): currentSubscriptions → {CurrentSubscriptions}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, currentSubscriptions);
            var newsLetterSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id);
			_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): newsLetterSubscriptionTypes → {NewsLetterSubscriptionTypes}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, newsLetterSubscriptionTypes);
            foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
            {
                var nsModel = new NewsLetterSubscriptionModel
                {
                    TypeId = newsLetterSubscriptionType.Id,
                    Name = await _localizationService.GetLocalizedAsync(newsLetterSubscriptionType, x => x.Name),
                    IsActive = currentSubscriptions.Any(subscription => subscription.TypeId == newsLetterSubscriptionType.Id && subscription.Active)
                };
                model.NewsLetterSubscriptions.Add(nsModel);
            }

            model.Signature = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute);
            model.Email = customer.Email;
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Email → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Email);
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.Username → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.Username);
            model.Username = customer.Username;
        }
        else
        {
            if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                model.Username = customer.Username;
        }

        if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            model.EmailToRevalidate = customer.EmailToRevalidate;
			_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.EmailToRevalidate → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.EmailToRevalidate);

        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): currentLanguage → {CurrentLanguage}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, currentLanguage);
        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            if (model.CountryId == 0)
                model.CountryId = _customerSettings.DefaultCountryId ?? 0;

            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
				_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): states → {States}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, states);
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
					_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): anyCountrySelected → {AnyCountrySelected}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, anyCountrySelected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
						_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): Value → {Value}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, Value);
                    });
                }

            }
        }

        model.DisplayVatNumber = _taxSettings.EuVatEnabled;
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.DisplayVatNumber → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.DisplayVatNumber);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.VatNumberRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.VatNumberRequired);
        model.VatNumberRequired = _taxSettings.EuVatRequired;
        model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.FirstNameEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.FirstNameEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.LastNameEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.LastNameEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.FirstNameRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.FirstNameRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.LastNameRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.LastNameRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.GenderEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.GenderEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.NeutralGenderEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.NeutralGenderEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.DateOfBirthEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.DateOfBirthEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.DateOfBirthRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.DateOfBirthRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CompanyEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CompanyEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CompanyRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CompanyRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddressEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddressEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddressRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddressRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddress2Enabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddress2Enabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StreetAddress2Required → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StreetAddress2Required);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.ZipPostalCodeEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.ZipPostalCodeEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.ZipPostalCodeRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.ZipPostalCodeRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CityEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CityEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CityRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CityRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CountyEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CountyEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CountyRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CountyRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CountryEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CountryEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CountryRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CountryRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StateProvinceEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StateProvinceEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.StateProvinceRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.StateProvinceRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.PhoneEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.PhoneEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.PhoneRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.PhoneRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.FaxEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.FaxEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.FaxRequired → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.FaxRequired);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.NewsletterEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.NewsletterEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.UsernamesEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.UsernamesEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.AllowUsersToChangeUsernames → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.AllowUsersToChangeUsernames);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.CheckUsernameAvailabilityEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.CheckUsernameAvailabilityEnabled);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.SignatureEnabled → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.SignatureEnabled);
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

        //external authentication
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): currentCustomer → {CurrentCustomer}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, currentCustomer);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.AllowCustomersToRemoveAssociations → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.AllowCustomersToRemoveAssociations);
		_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): model.NumberOfExternalAuthenticationProviders → {Arg4}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, model.NumberOfExternalAuthenticationProviders);
        model.AllowCustomersToRemoveAssociations = _externalAuthenticationSettings.AllowCustomersToRemoveAssociations;
        var authenticationProviders = await _externalAuthenticationModelFactory.PrepareExternalMethodsModelAsync();
		_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): authenticationProviders → {AuthenticationProviders}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, authenticationProviders);
        model.NumberOfExternalAuthenticationProviders = authenticationProviders.Count;
        foreach (var record in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
        {
            var authMethod = await _authenticationPluginManager
                .LoadPluginBySystemNameAsync(record.ProviderSystemName, currentCustomer, store.Id);
            if (!_authenticationPluginManager.IsPluginActive(authMethod))
                continue;

            model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
            {
                Id = record.Id,
                Email = record.Email,
                ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                    ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                AuthMethodName = await _localizationService.GetLocalizedFriendlyNameAsync(authMethod, currentLanguage.Id)
            });
				_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): ExternalIdentifier → {ExternalIdentifier}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, ExternalIdentifier);
				_logger.LogDebug("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): AuthMethodName → {AuthMethodName}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, AuthMethodName);
        }

        //custom customer attributes
        var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
		_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): customAttributes → {CustomAttributes}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, customAttributes);
        foreach (var attribute in customAttributes)
            model.CustomerAttributes.Add(attribute);

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
			_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): consents → {Consents}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, consents);
            foreach (var consent in consents)
            {
                var accepted = await _gdprService.IsConsentAcceptedAsync(consent.Id, currentCustomer.Id);
				_logger.LogError("PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml}): accepted → {Accepted}", model, customer, excludeProperties, overrideCustomCustomerAttributesXml, accepted);
                model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, accepted.HasValue && accepted.Value));
            }
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerInfoModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml})", model, customer, excludeProperties, overrideCustomCustomerAttributesXml);
        return model;
    }

    /// <summary>
    /// Prepare the customer register model
    /// </summary>
    /// <param name="model">Customer register model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer register model
    /// </returns>
    public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
        string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)
    {
		_logger.LogError(">>> Entering PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues})", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues);
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): customer → {Customer}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, customer);

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.AllowCustomersToSetTimeZone → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.AllowCustomersToSetTimeZone);
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        //VAT
        model.DisplayVatNumber = _taxSettings.EuVatEnabled;
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.DisplayVatNumber → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.DisplayVatNumber);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.VatNumberRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.VatNumberRequired);
        model.VatNumberRequired = _taxSettings.EuVatRequired;
        if (_taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
            model.VatNumber = customer.VatNumber;
			_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.VatNumber → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.VatNumber);

        //form fields
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.FirstNameEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.FirstNameEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.LastNameEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.LastNameEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.FirstNameRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.FirstNameRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.LastNameRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.LastNameRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.GenderEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.GenderEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.NeutralGenderEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.NeutralGenderEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.DateOfBirthEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.DateOfBirthEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.DateOfBirthRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.DateOfBirthRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CompanyEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CompanyEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CompanyRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CompanyRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StreetAddressEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StreetAddressEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StreetAddressRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StreetAddressRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StreetAddress2Enabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StreetAddress2Enabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StreetAddress2Required → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StreetAddress2Required);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.ZipPostalCodeEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.ZipPostalCodeEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.ZipPostalCodeRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.ZipPostalCodeRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CityEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CityEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CityRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CityRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CountyEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CountyEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CountyRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CountyRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CountryEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CountryEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CountryRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CountryRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StateProvinceEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StateProvinceEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.StateProvinceRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.StateProvinceRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.PhoneEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.PhoneEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.PhoneRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.PhoneRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.FaxEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.FaxEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.FaxRequired → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.FaxRequired);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.NewsletterEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.NewsletterEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.AcceptPrivacyPolicyEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.AcceptPrivacyPolicyEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.AcceptPrivacyPolicyPopup → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.AcceptPrivacyPolicyPopup);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.UsernamesEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.UsernamesEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CheckUsernameAvailabilityEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CheckUsernameAvailabilityEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.HoneypotEnabled → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.HoneypotEnabled);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.DisplayCaptcha → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.DisplayCaptcha);
		_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.EnteringEmailTwice → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.EnteringEmailTwice);
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
        model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
        model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
        if (setDefaultValues)
        {
            //newsletter subscriptions
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): store → {Store}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, store);
            var newsLetterSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id);
			_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): newsLetterSubscriptionTypes → {NewsLetterSubscriptionTypes}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, newsLetterSubscriptionTypes);
            foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
            {
                var nsModel = new NewsLetterSubscriptionModel
                {
                    TypeId = newsLetterSubscriptionType.Id,
                    Name = await _localizationService.GetLocalizedAsync(newsLetterSubscriptionType, x => x.Name),
                    IsActive = newsLetterSubscriptionType.TickedByDefault
                };
                model.NewsLetterSubscriptions.Add(nsModel);
            }
        }

        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            model.CountryId = _customerSettings.DefaultCountryId ?? 0;
			_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): model.CountryId → {Arg4}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, model.CountryId);
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
			_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): currentLanguage → {CurrentLanguage}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, currentLanguage);
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
				_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): states → {States}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, states);
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
					_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): anyCountrySelected → {AnyCountrySelected}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, anyCountrySelected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
						_logger.LogDebug("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): Value → {Value}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, Value);
                    });
                }

            }
        }

        //custom customer attributes
        var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
		_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): customAttributes → {CustomAttributes}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, customAttributes);
        foreach (var attribute in customAttributes)
            model.CustomerAttributes.Add(attribute);

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
			_logger.LogError("PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues}): consents → {Consents}", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues, consents);
            foreach (var consent in consents)
            {
                model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, false));
            }
			_logger.LogError(">>> Entering PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml})", customer, overrideAttributesXml);
			_logger.LogDebug("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): item.IsPreSelected → {Arg2}", customer, overrideAttributesXml, item.IsPreSelected);
			_logger.LogWarning("<<< Exiting PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml})", customer, overrideAttributesXml);
        }

		_logger.LogWarning("<<< Exiting PrepareRegisterModelAsync(model={Model},excludeProperties={ExcludeProperties},overrideCustomCustomerAttributesXml={OverrideCustomCustomerAttributesXml},setDefaultValues={SetDefaultValues})", model, excludeProperties, overrideCustomCustomerAttributesXml, setDefaultValues);
        return model;
    }

    /// <summary>
    /// Prepare the login model
    /// </summary>
    /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the login model
    /// </returns>
    public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
    {
		_logger.LogError(">>> Entering PrepareLoginModelAsync(checkoutAsGuest={CheckoutAsGuest})", checkoutAsGuest);
        var model = new LoginModel
        {
            UsernamesEnabled = _customerSettings.UsernamesEnabled,
            RegistrationType = _customerSettings.UserRegistrationType,
            CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
        };

		_logger.LogWarning("<<< Exiting PrepareLoginModelAsync(checkoutAsGuest={CheckoutAsGuest})", checkoutAsGuest);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the password recovery model
    /// </summary>
    /// <param name="model">Password recovery model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the password recovery model
    /// </returns>
    public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
    {
		_logger.LogError(">>> Entering PreparePasswordRecoveryModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;
		_logger.LogDebug("PreparePasswordRecoveryModelAsync(model={Model}): model.DisplayCaptcha → {Arg1}", model, model.DisplayCaptcha);

		_logger.LogWarning("<<< Exiting PreparePasswordRecoveryModelAsync(model={Model})", model);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the register result model
    /// </summary>
    /// <param name="resultId">Value of UserRegistrationType enum</param>
    /// <param name="returnUrl">URL to redirect</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the register result model
    /// </returns>
    public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
    {
		_logger.LogError(">>> Entering PrepareRegisterResultModelAsync(resultId={ResultId},returnUrl={ReturnUrl})", resultId, returnUrl);
        var resultText = (UserRegistrationType)resultId switch
        {
            UserRegistrationType.Disabled => await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"),
            UserRegistrationType.Standard => await _localizationService.GetResourceAsync("Account.Register.Result.Standard"),
            UserRegistrationType.AdminApproval => await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
            UserRegistrationType.EmailValidation => await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
            _ => null
        };

        var model = new RegisterResultModel
        {
            Result = resultText,
            ReturnUrl = returnUrl
        };

		_logger.LogWarning("<<< Exiting PrepareRegisterResultModelAsync(resultId={ResultId},returnUrl={ReturnUrl})", resultId, returnUrl);
        return model;
    }

    /// <summary>
    /// Prepare the customer navigation model
    /// </summary>
    /// <param name="selectedTabId">Identifier of the selected tab</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer navigation model
    /// </returns>
    public virtual async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
    {
		_logger.LogError(">>> Entering PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId})", selectedTabId);
        var model = new CustomerNavigationModel();

		_logger.LogError("PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId}): model → {Model}", selectedTabId, model);
        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_INFO,
            Title = await _localizationService.GetResourceAsync("Account.CustomerInfo"),
            Tab = (int)CustomerNavigationEnum.Info,
            ItemClass = "customer-info"
			_logger.LogDebug("PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId}): ItemClass → {ItemClass}", selectedTabId, ItemClass);
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
            Title = await _localizationService.GetResourceAsync("Account.CustomerAddresses"),
            Tab = (int)CustomerNavigationEnum.Addresses,
            ItemClass = "customer-addresses"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
            Title = await _localizationService.GetResourceAsync("Account.CustomerOrders"),
            Tab = (int)CustomerNavigationEnum.Orders,
            ItemClass = "customer-orders"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS,
            Title = await _localizationService.GetResourceAsync("Account.CustomerRecurringPayments"),
            Tab = (int)CustomerNavigationEnum.RecurringPayments,
            ItemClass = "customer-recurring-payments"
        });

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId}): store → {Store}", selectedTabId, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId}): customer → {Customer}", selectedTabId, customer);

        if (_orderSettings.ReturnRequestsEnabled &&
            (await _returnRequestService.SearchReturnRequestsAsync(store.Id,
                customer.Id, pageIndex: 0, pageSize: 1)).Any())
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_RETURN_REQUESTS,
                Title = await _localizationService.GetResourceAsync("Account.CustomerReturnRequests"),
                Tab = (int)CustomerNavigationEnum.ReturnRequests,
                ItemClass = "return-requests"
            });
        }

        if (!_customerSettings.HideDownloadableProductsTab)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_DOWNLOADABLE_PRODUCTS,
                Title = await _localizationService.GetResourceAsync("Account.DownloadableProducts"),
                Tab = (int)CustomerNavigationEnum.DownloadableProducts,
                ItemClass = "downloadable-products"
            });
        }

        if (!_customerSettings.HideBackInStockSubscriptionsTab)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS,
                Title = await _localizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                Tab = (int)CustomerNavigationEnum.BackInStockSubscriptions,
                ItemClass = "back-in-stock-subscriptions"
            });
        }

        if (_rewardPointsSettings.Enabled)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_REWARD_POINTS,
                Title = await _localizationService.GetResourceAsync("Account.RewardPoints"),
                Tab = (int)CustomerNavigationEnum.RewardPoints,
                ItemClass = "reward-points"
            });
        }

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.Standard.CUSTOMER_CHANGE_PASSWORD,
            Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
            Tab = (int)CustomerNavigationEnum.ChangePassword,
            ItemClass = "change-password"
        });

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_AVATAR,
                Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                Tab = (int)CustomerNavigationEnum.Avatar,
                ItemClass = "customer-avatar"
            });
        }

        if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_FORUM_SUBSCRIPTIONS,
                Title = await _localizationService.GetResourceAsync("Account.ForumSubscriptions"),
                Tab = (int)CustomerNavigationEnum.ForumSubscriptions,
                ItemClass = "forum-subscriptions"
            });
        }
        if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS,
                Title = await _localizationService.GetResourceAsync("Account.CustomerProductReviews"),
                Tab = (int)CustomerNavigationEnum.ProductReviews,
                ItemClass = "customer-reviews"
            });
        }
        if (_vendorSettings.AllowVendorsToEditInfo && await _workContext.GetCurrentVendorAsync() != null)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_VENDOR_INFO,
                Title = await _localizationService.GetResourceAsync("Account.VendorInfo"),
                Tab = (int)CustomerNavigationEnum.VendorInfo,
                ItemClass = "customer-vendor-info"
            });
        }
        if (_gdprSettings.GdprEnabled)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.GDPR_TOOLS,
                Title = await _localizationService.GetResourceAsync("Account.Gdpr"),
                Tab = (int)CustomerNavigationEnum.GdprTools,
                ItemClass = "customer-gdpr"
            });
        }

        if (_customerSettings.AllowCustomersToCheckGiftCardBalance)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
                Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                Tab = (int)CustomerNavigationEnum.CheckGiftCardBalance,
                ItemClass = "customer-check-gift-card-balance"
            });
        }

        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION) &&
            await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS,
                Title = await _localizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                Tab = (int)CustomerNavigationEnum.MultiFactorAuthentication,
                ItemClass = "customer-multiFactor-authentication"
            });
        }

        model.SelectedTab = selectedTabId;
		_logger.LogDebug("PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId}): model.SelectedTab → {Arg1}", selectedTabId, model.SelectedTab);

		_logger.LogWarning("<<< Exiting PrepareCustomerNavigationModelAsync(selectedTabId={SelectedTabId})", selectedTabId);
        return model;
    }

    /// <summary>
    /// Prepare the customer address list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCustomerAddressListModelAsync()");
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogError("PrepareCustomerAddressListModelAsync(): customer → {Customer}", customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            //enabled for the current store
            .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
            .ToListAsync();
			_logger.LogError("PrepareCustomerAddressListModelAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        var model = new CustomerAddressListModel();
		_logger.LogError("PrepareCustomerAddressListModelAsync(): model → {Model}", model);
        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
			_logger.LogError("PrepareCustomerAddressListModelAsync(): addressModel → {AddressModel}", addressModel);
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));
            model.Addresses.Add(addressModel);
        }
		_logger.LogWarning("<<< Exiting PrepareCustomerAddressListModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare the customer downloadable products model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer downloadable products model
    /// </returns>
    public virtual async Task<CustomerDownloadableProductsModel> PrepareCustomerDownloadableProductsModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCustomerDownloadableProductsModelAsync()");
        var model = new CustomerDownloadableProductsModel();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerDownloadableProductsModelAsync(): customer → {Customer}", customer);
		_logger.LogError("PrepareCustomerDownloadableProductsModelAsync(): model → {Model}", model);
        var items = await _orderService.GetDownloadableOrderItemsAsync(customer.Id);
        foreach (var item in items)
        {
			_logger.LogError("PrepareCustomerDownloadableProductsModelAsync(): items → {Items}", items);
            var order = await _orderService.GetOrderByIdAsync(item.OrderId);
			_logger.LogError("PrepareCustomerDownloadableProductsModelAsync(): order → {Order}", order);
            var product = await _productService.GetProductByIdAsync(item.ProductId);
			_logger.LogError("PrepareCustomerDownloadableProductsModelAsync(): product → {Product}", product);

            var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
            {
                OrderItemGuid = item.OrderItemGuid,
                OrderId = order.Id,
                CustomOrderNumber = order.CustomOrderNumber,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                ProductAttributes = item.AttributeDescription,
                ProductId = item.ProductId
            };
            model.Items.Add(itemModel);

            if (await _orderService.IsDownloadAllowedAsync(item))
                itemModel.DownloadId = product.DownloadId;
				_logger.LogDebug("PrepareCustomerDownloadableProductsModelAsync(): itemModel.DownloadId → {Arg0}", itemModel.DownloadId);

            if (await _orderService.IsLicenseDownloadAllowedAsync(item))
                itemModel.LicenseId = item.LicenseDownloadId ?? 0;
				_logger.LogDebug("PrepareCustomerDownloadableProductsModelAsync(): itemModel.LicenseId → {Arg0}", itemModel.LicenseId);
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerDownloadableProductsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare the user agreement model
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user agreement model
    /// </returns>
    public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product)
    {
		_logger.LogError(">>> Entering PrepareUserAgreementModelAsync(orderItem={OrderItem},product={Product})", orderItem, product);
        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(product);

        var model = new UserAgreementModel
        {
            UserAgreementText = product.UserAgreementText,
            OrderItemGuid = orderItem.OrderItemGuid
        };

		_logger.LogWarning("<<< Exiting PrepareUserAgreementModelAsync(orderItem={OrderItem},product={Product})", orderItem, product);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the change password model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the change password model
    /// </returns>
    public virtual async Task<ChangePasswordModel> PrepareChangePasswordModelAsync(Customer customer)
    {
		_logger.LogError(">>> Entering PrepareChangePasswordModelAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

		_logger.LogWarning("<<< Exiting PrepareChangePasswordModelAsync(customer={Customer})", customer);
        return new ChangePasswordModel()
        {
            PasswordExpired = await _customerService.IsPasswordExpiredAsync(customer),
            PasswordMustBeChanged = customer.MustChangePassword
        };
    }

    /// <summary>
    /// Prepare the customer avatar model
    /// </summary>
    /// <param name="model">Customer avatar model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer avatar model
    /// </returns>
    public virtual async Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model)
    {
		_logger.LogError(">>> Entering PrepareCustomerAvatarModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
            await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AvatarPictureIdAttribute),
            _mediaSettings.AvatarPictureSize,
            false);

		_logger.LogWarning("<<< Exiting PrepareCustomerAvatarModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare the GDPR tools model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR tools model
    /// </returns>
    public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
    {
		_logger.LogError(">>> Entering PrepareGdprToolsModelAsync()");
        var model = new GdprToolsModel();

		_logger.LogError("PrepareGdprToolsModelAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting PrepareGdprToolsModelAsync()");
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the check gift card balance madel
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check gift card balance madel
    /// </returns>
    public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCheckGiftCardBalanceModelAsync()");
        var model = new CheckGiftCardBalanceModel { DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnCheckGiftCardBalance };

		_logger.LogWarning("<<< Exiting PrepareCheckGiftCardBalanceModelAsync()");
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the multi-factor authentication model
    /// </summary>
    /// <param name="model">Multi-factor authentication model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
    {
		_logger.LogError(">>> Entering PrepareMultiFactorAuthenticationModelAsync(model={Model})", model);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogError("PrepareMultiFactorAuthenticationModelAsync(model={Model}): customer → {Customer}", model, customer);
        model.IsEnabled = !string.IsNullOrEmpty(
            await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareMultiFactorAuthenticationModelAsync(model={Model}): store → {Store}", model, store);
        var multiFactorAuthenticationProviders = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id)).ToList();
		_logger.LogError("PrepareMultiFactorAuthenticationModelAsync(model={Model}): multiFactorAuthenticationProviders → {MultiFactorAuthenticationProviders}", model, multiFactorAuthenticationProviders);
        foreach (var multiFactorAuthenticationProvider in multiFactorAuthenticationProviders)
        {
            var providerModel = new MultiFactorAuthenticationProviderModel();
			_logger.LogError("PrepareMultiFactorAuthenticationModelAsync(model={Model}): providerModel → {ProviderModel}", model, providerModel);
            var sysName = multiFactorAuthenticationProvider.PluginDescriptor.SystemName;
            providerModel = await PrepareMultiFactorAuthenticationProviderModelAsync(providerModel, sysName);
            model.Providers.Add(providerModel);
        }

		_logger.LogWarning("<<< Exiting PrepareMultiFactorAuthenticationModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare the multi-factor authentication provider model
    /// </summary>
    /// <param name="providerModel">Multi-factor authentication provider model</param>
    /// <param name="sysName">Multi-factor authentication provider system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
    {
		_logger.LogError(">>> Entering PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin})", providerModel, sysName, isLogin);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
		_logger.LogError("PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin}): selectedProvider → {SelectedProvider}", providerModel, sysName, isLogin, selectedProvider);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin}): store → {Store}", providerModel, sysName, isLogin, store);
		_logger.LogError("PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin}): customer → {Customer}", providerModel, sysName, isLogin, customer);

        var multiFactorAuthenticationProvider = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id))
            .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

        if (multiFactorAuthenticationProvider != null)
        {
            providerModel.Name = await _localizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await _workContext.GetWorkingLanguageAsync()).Id);
            providerModel.SystemName = sysName;
			_logger.LogDebug("PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin}): providerModel.SystemName → {Arg3}", providerModel, sysName, isLogin, providerModel.SystemName);
            providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
            providerModel.LogoUrl = await _multiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
            providerModel.ViewComponent = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponent() : multiFactorAuthenticationProvider.GetPublicViewComponent();
            providerModel.Selected = sysName == selectedProvider;
        }

		_logger.LogWarning("<<< Exiting PrepareMultiFactorAuthenticationProviderModelAsync(providerModel={ProviderModel},sysName={SysName},isLogin={IsLogin})", providerModel, sysName, isLogin);
        return providerModel;
    }

    /// <summary>
    /// Prepare the custom customer attribute models
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the customer attribute model
    /// </returns>
    public virtual async Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "")
    {
		_logger.LogError(">>> Entering PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml})", customer, overrideAttributesXml);
        ArgumentNullException.ThrowIfNull(customer);

        var result = new List<CustomerAttributeModel>();
		_logger.LogError("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): result → {Result}", customer, overrideAttributesXml, result);

        var customerAttributes = await _customerAttributeService.GetAllAttributesAsync();
		_logger.LogError("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): customerAttributes → {CustomerAttributes}", customer, overrideAttributesXml, customerAttributes);
        foreach (var attribute in customerAttributes)
        {
            var attributeModel = new CustomerAttributeModel
            {
                Id = attribute.Id,
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogError("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): attributeValues → {AttributeValues}", customer, overrideAttributesXml, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new CustomerAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(valueModel);
                }
            }

            //set already selected attributes
            var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                overrideAttributesXml :
                customer.CustomCustomerAttributesXML;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(selectedAttributesXml))
                    {
                        if (!_customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id).Any())
                            break;

                        //clear default selection                                
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;

                        //select new values
                        var selectedValues = await _customerAttributeParser.ParseAttributeValuesAsync(selectedAttributesXml);
						_logger.LogError("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): selectedValues → {SelectedValues}", customer, overrideAttributesXml, selectedValues);
                        foreach (var attributeValue in selectedValues)
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(selectedAttributesXml))
                    {
                        var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
						_logger.LogError("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): enteredText → {EnteredText}", customer, overrideAttributesXml, enteredText);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
							_logger.LogDebug("PrepareCustomCustomerAttributesAsync(customer={Customer},overrideAttributesXml={OverrideAttributesXml}): attributeModel.DefaultValue → {Arg2}", customer, overrideAttributesXml, attributeModel.DefaultValue);
                    }
                }
                    break;
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                default:
                    //not supported attribute control types
                    break;
            }

            result.Add(attributeModel);
        }

        return result;
    }

    #endregion
}