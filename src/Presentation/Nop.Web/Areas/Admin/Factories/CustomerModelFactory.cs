using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Attributes;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer model factory implementation
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly GdprSettings _gdprSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomWishlistService _customWishlistService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGeoLookupService _geoLookupService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        GdprSettings gdprSettings,
        ForumSettings forumSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAuthenticationPluginManager authenticationPluginManager,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ICustomWishlistService customWishlistService,
        IDateTimeHelper dateTimeHelper,
        IExternalAuthenticationService externalAuthenticationService,
        IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        IGeoLookupService geoLookupService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IRewardPointService rewardPointService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreService storeService,
        ITaxService taxService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        RewardPointsSettings rewardPointsSettings,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _gdprSettings = gdprSettings;
        _forumSettings = forumSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _addressAttributeFormatter = addressAttributeFormatter;
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
        _authenticationPluginManager = authenticationPluginManager;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _customWishlistService = customWishlistService;
        _dateTimeHelper = dateTimeHelper;
        _externalAuthenticationService = externalAuthenticationService;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _geoLookupService = geoLookupService;
        _localizationService = localizationService;
        _orderService = orderService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _rewardPointService = rewardPointService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeService = storeService;
        _taxService = taxService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the reward points model to add to the customer
    /// </summary>
    /// <param name="model">Reward points model to add to the customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAddRewardPointsToCustomerModelAsync(AddRewardPointsToCustomerModel model)
    {
		_logger.LogError(">>> Entering PrepareAddRewardPointsToCustomerModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareAddRewardPointsToCustomerModelAsync(model={Model}): store → {Store}", model, store);

        model.Message = string.Empty;
		_logger.LogDebug("PrepareAddRewardPointsToCustomerModelAsync(model={Model}): model.Message → {Arg1}", model, model.Message);
		_logger.LogDebug("PrepareAddRewardPointsToCustomerModelAsync(model={Model}): model.StoreId → {Arg1}", model, model.StoreId);
        model.ActivatePointsImmediately = true;
		_logger.LogDebug("PrepareAddRewardPointsToCustomerModelAsync(model={Model}): model.ActivatePointsImmediately → {Arg1}", model, model.ActivatePointsImmediately);
        model.StoreId = store.Id;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
		_logger.LogWarning("<<< Exiting PrepareAddRewardPointsToCustomerModelAsync(model={Model})", model);
    }

    /// <summary>
    /// Prepare customer associated external authorization models
    /// </summary>
    /// <param name="models">List of customer associated external authorization models</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAssociatedExternalAuthModelsAsync(IList<CustomerAssociatedExternalAuthModel> models, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareAssociatedExternalAuthModelsAsync(models={Models},customer={Customer})", models, customer);
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(customer);

        foreach (var record in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
        {
            var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(record.ProviderSystemName);
			_logger.LogError("PrepareAssociatedExternalAuthModelsAsync(models={Models},customer={Customer}): method → {Method}", models, customer, method);
            if (method == null)
                continue;

            models.Add(new CustomerAssociatedExternalAuthModel
            {
				_logger.LogWarning("<<< Exiting PrepareAssociatedExternalAuthModelsAsync(models={Models},customer={Customer})", models, customer);
                Id = record.Id,
                Email = record.Email,
                ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                    ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                AuthMethodName = method.PluginDescriptor.FriendlyName
            });
			_logger.LogDebug("PrepareAssociatedExternalAuthModelsAsync(models={Models},customer={Customer}): ExternalIdentifier → {ExternalIdentifier}", models, customer, ExternalIdentifier);
			_logger.LogDebug("PrepareAssociatedExternalAuthModelsAsync(models={Models},customer={Customer}): AuthMethodName → {AuthMethodName}", models, customer, AuthMethodName);
        }
    }

    /// <summary>
    /// Prepare customer attribute models
    /// </summary>
    /// <param name="models">List of customer attribute models</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareCustomerAttributeModelsAsync(IList<CustomerModel.CustomerAttributeModel> models, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer})", models, customer);
        ArgumentNullException.ThrowIfNull(models);

        //get available customer attributes
        var customerAttributes = await _customerAttributeService.GetAllAttributesAsync();
		_logger.LogError("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): customerAttributes → {CustomerAttributes}", models, customer, customerAttributes);
        foreach (var attribute in customerAttributes)
        {
            var attributeModel = new CustomerModel.CustomerAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogError("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): attributeValues → {AttributeValues}", models, customer, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            if (customer != null)
            {
                var selectedCustomerAttributes = customer.CustomCustomerAttributesXML;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    {
                        if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;
								_logger.LogDebug("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): item.IsPreSelected → {Arg2}", models, customer, item.IsPreSelected);

                            //select new values
                            var selectedValues = await _customerAttributeParser.ParseAttributeValuesAsync(selectedCustomerAttributes);
							_logger.LogError("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): selectedValues → {SelectedValues}", models, customer, selectedValues);
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
                        if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                        {
                            var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
							_logger.LogError("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): enteredText → {EnteredText}", models, customer, enteredText);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
								_logger.LogDebug("PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer}): attributeModel.DefaultValue → {Arg2}", models, customer, attributeModel.DefaultValue);
                        }
                    }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }
            }

            models.Add(attributeModel);
        }
		_logger.LogWarning("<<< Exiting PrepareCustomerAttributeModelsAsync(models={Models},customer={Customer})", models, customer);
    }

    /// <summary>
    /// Prepare HTML string address
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
    {
		_logger.LogError(">>> Entering PrepareModelAddressHtmlAsync(model={Model},address={Address})", model, address);
        ArgumentNullException.ThrowIfNull(model);

        var separator = "<br />";
		_logger.LogError("PrepareModelAddressHtmlAsync(model={Model},address={Address}): separator → {Separator}", model, address, separator);
        var addressHtmlSb = new StringBuilder("<div>");
		_logger.LogError("PrepareModelAddressHtmlAsync(model={Model},address={Address}): addressHtmlSb → {AddressHtmlSb}", model, address, addressHtmlSb);
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogError("PrepareModelAddressHtmlAsync(model={Model},address={Address}): languageId → {LanguageId}", model, address, languageId);
        var (addressLine, _) = await _addressService.FormatAddressAsync(address, languageId, separator, true);
        addressHtmlSb.Append(addressLine);

        var customAttributesFormatted = await _addressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
		_logger.LogError("PrepareModelAddressHtmlAsync(model={Model},address={Address}): customAttributesFormatted → {CustomAttributesFormatted}", model, address, customAttributesFormatted);
        if (!string.IsNullOrEmpty(customAttributesFormatted))
        {
            //already encoded
            addressHtmlSb.AppendFormat($"{separator}{customAttributesFormatted}");
        }

        addressHtmlSb.Append("</div>");

        model.AddressHtml = addressHtmlSb.ToString();
		_logger.LogWarning("<<< Exiting PrepareModelAddressHtmlAsync(model={Model},address={Address})", model, address);
    }

    /// <summary>
    /// Prepare reward points search model
    /// </summary>
    /// <param name="searchModel">Reward points search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Reward points search model</returns>
    protected virtual CustomerRewardPointsSearchModel PrepareRewardPointsSearchModel(CustomerRewardPointsSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareRewardPointsSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareRewardPointsSearchModel(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareRewardPointsSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer address search model
    /// </summary>
    /// <param name="searchModel">Customer address search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer address search model</returns>
    protected virtual CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerAddressSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerAddressSearchModel(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerAddressSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer order search model
    /// </summary>
    /// <param name="searchModel">Customer order search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer order search model</returns>
    protected virtual CustomerOrderSearchModel PrepareCustomerOrderSearchModel(CustomerOrderSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerOrderSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerOrderSearchModel(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerOrderSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer shopping cart search model
    /// </summary>
    /// <param name="searchModel">Customer shopping cart search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer shopping cart search model
    /// </returns>
    protected virtual async Task<CustomerShoppingCartSearchModel> PrepareCustomerShoppingCartSearchModelAsync(CustomerShoppingCartSearchModel searchModel,
        Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerShoppingCartSearchModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerShoppingCartSearchModelAsync(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare available shopping cart types (search shopping cart by default)
        searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
        await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerShoppingCartSearchModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer activity log search model
    /// </summary>
    /// <param name="searchModel">Customer activity log search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer activity log search model</returns>
    protected virtual CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerActivityLogSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerActivityLogSearchModel(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerActivityLogSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer back in stock subscriptions search model
    /// </summary>
    /// <param name="searchModel">Customer back in stock subscriptions search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer back in stock subscriptions search model</returns>
    protected virtual CustomerBackInStockSubscriptionSearchModel PrepareCustomerBackInStockSubscriptionSearchModel(
        CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerBackInStockSubscriptionSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerBackInStockSubscriptionSearchModel(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerBackInStockSubscriptionSearchModel(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    /// <summary>
    /// Prepare customer back in stock subscriptions search model
    /// </summary>
    /// <param name="searchModel">Customer back in stock subscriptions search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer back in stock subscriptions search model
    /// </returns>
    protected virtual async Task<CustomerAssociatedExternalAuthRecordsSearchModel> PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(
        CustomerAssociatedExternalAuthRecordsSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;
		_logger.LogDebug("PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(searchModel={SearchModel},customer={Customer}): searchModel.CustomerId → {Arg2}", searchModel, customer, searchModel.CustomerId);

        //prepare page parameters
        searchModel.SetGridPageSize();
        //prepare external authentication records
        await PrepareAssociatedExternalAuthModelsAsync(searchModel.AssociatedExternalAuthRecords, customer);

		_logger.LogWarning("<<< Exiting PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer search model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer search model
    /// </returns>
    public virtual async Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.UsernamesEnabled = _customerSettings.UsernamesEnabled;
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.UsernamesEnabled → {Arg1}", searchModel, searchModel.UsernamesEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.FirstNameEnabled → {Arg1}", searchModel, searchModel.FirstNameEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.LastNameEnabled → {Arg1}", searchModel, searchModel.LastNameEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.DateOfBirthEnabled → {Arg1}", searchModel, searchModel.DateOfBirthEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.CompanyEnabled → {Arg1}", searchModel, searchModel.CompanyEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.PhoneEnabled → {Arg1}", searchModel, searchModel.PhoneEnabled);
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.ZipPostalCodeEnabled → {Arg1}", searchModel, searchModel.ZipPostalCodeEnabled);
        searchModel.AvatarEnabled = _customerSettings.AllowCustomersToUploadAvatars;
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.AvatarEnabled → {Arg1}", searchModel, searchModel.AvatarEnabled);
        searchModel.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        searchModel.LastNameEnabled = _customerSettings.LastNameEnabled;
        searchModel.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        searchModel.CompanyEnabled = _customerSettings.CompanyEnabled;
        searchModel.PhoneEnabled = _customerSettings.PhoneEnabled;
        searchModel.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;

        //search registered customers by default
        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
		_logger.LogError("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): registeredRole → {RegisteredRole}", searchModel, registeredRole);
        if (registeredRole != null)
            searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

        searchModel.AvailableActiveValues = new List<SelectListItem> {
            new(await _localizationService.GetResourceAsync("Admin.Common.All"), string.Empty),
            new(await _localizationService.GetResourceAsync("Admin.Common.Yes"), true.ToString(), true),
            new(await _localizationService.GetResourceAsync("Admin.Common.No"), false.ToString())
        };
		_logger.LogDebug("PrepareCustomerSearchModelAsync(searchModel={SearchModel}): searchModel.AvailableActiveValues → {Arg1}", searchModel, searchModel.AvailableActiveValues);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged customer list model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer list model
    /// </returns>
    public virtual async Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
        _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);
		_logger.LogError("PrepareCustomerListModelAsync(searchModel={SearchModel}): _ → {_}", searchModel, _);
        var createdFromUtc = !searchModel.SearchRegistrationDateFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdToUtc = !searchModel.SearchRegistrationDateTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var lastActivityFromUtc = !searchModel.SearchLastActivityFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var lastActivityToUtc = !searchModel.SearchLastActivityTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //exclude guests from the result when filter "by registration date" is used
        if (createdFromUtc.HasValue || createdToUtc.HasValue)
        {
            if (!searchModel.SelectedCustomerRoleIds.Any())
            {
                var customerRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
				_logger.LogError("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerRoles → {CustomerRoles}", searchModel, customerRoles);
                searchModel.SelectedCustomerRoleIds = customerRoles
                    .Where(cr => cr.SystemName != NopCustomerDefaults.GuestsRoleName).Select(cr => cr.Id).ToList();
					_logger.LogDebug("PrepareCustomerListModelAsync(searchModel={SearchModel}): searchModel.SelectedCustomerRoleIds → {Arg1}", searchModel, searchModel.SelectedCustomerRoleIds);
            }
            else
            {
                var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
				_logger.LogError("PrepareCustomerListModelAsync(searchModel={SearchModel}): guestRole → {GuestRole}", searchModel, guestRole);
                if (guestRole != null)
                    searchModel.SelectedCustomerRoleIds.Remove(guestRole.Id);
            }
        }

        //get customers
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            email: searchModel.SearchEmail,
            username: searchModel.SearchUsername,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            dayOfBirth: dayOfBirth,
            monthOfBirth: monthOfBirth,
            company: searchModel.SearchCompany,
            createdFromUtc: createdFromUtc,
            createdToUtc: createdToUtc,
            lastActivityFromUtc: lastActivityFromUtc,
            lastActivityToUtc: lastActivityToUtc,
            phone: searchModel.SearchPhone,
            zipPostalCode: searchModel.SearchZipPostalCode,
            ipAddress: searchModel.SearchIpAddress,
            isActive: searchModel.SearchIsActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<CustomerModel>();
				_logger.LogError("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerModel → {CustomerModel}", searchModel, customerModel);

                //convert dates to the user time
                customerModel.Email = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.FullName = await _customerService.GetCustomerFullNameAsync(customer);
                customerModel.Company = customer.Company;
				_logger.LogDebug("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerModel.Company → {Arg1}", searchModel, customerModel.Company);
				_logger.LogDebug("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerModel.ZipPostalCode → {Arg1}", searchModel, customerModel.ZipPostalCode);
                customerModel.Phone = customer.Phone;
				_logger.LogDebug("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerModel.Phone → {Arg1}", searchModel, customerModel.Phone);
                customerModel.ZipPostalCode = customer.ZipPostalCode;

                customerModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerRoleNames = string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);
					_logger.LogError("PrepareCustomerListModelAsync(searchModel={SearchModel}): avatarPictureId → {AvatarPictureId}", searchModel, avatarPictureId);
                    customerModel.AvatarUrl = await _pictureService
                        .GetPictureUrlAsync(avatarPictureId, _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
						_logger.LogDebug("PrepareCustomerListModelAsync(searchModel={SearchModel}): customerModel.AvatarUrl → {Arg1}", searchModel, customerModel.AvatarUrl);
                }

                return customerModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare customer model
    /// </summary>
    /// <param name="model">Customer model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer model
    /// </returns>
    public virtual async Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties})", model, customer, excludeProperties);
        if (customer != null)
        {
            //fill in model values from the entity
            model ??= new CustomerModel();

            model.Id = customer.Id;
			_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Id → {Arg3}", model, customer, excludeProperties, model.Id);
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
			_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.DisplayVatNumber → {Arg3}", model, customer, excludeProperties, model.DisplayVatNumber);
            model.AllowSendingOfPrivateMessage = await _customerService.IsRegisteredAsync(customer) &&
                                                 _forumSettings.AllowPrivateMessages;
            model.AllowSendingOfWelcomeMessage = await _customerService.IsRegisteredAsync(customer) &&
                                                 _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
            model.AllowReSendingOfActivationMessage = await _customerService.IsRegisteredAsync(customer) && !customer.Active &&
                                                      _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
            model.GdprEnabled = _gdprSettings.GdprEnabled;
			_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.GdprEnabled → {Arg3}", model, customer, excludeProperties, model.GdprEnabled);

            model.MultiFactorAuthenticationProvider = await _genericAttributeService
                .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.MultiFactorAuthenticationProvider → {Arg3}", model, customer, excludeProperties, model.MultiFactorAuthenticationProvider);

            //whether to fill in some of properties
            if (!excludeProperties)
            {
                model.Email = customer.Email;
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Email → {Arg3}", model, customer, excludeProperties, model.Email);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.VendorId → {Arg3}", model, customer, excludeProperties, model.VendorId);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.AdminComment → {Arg3}", model, customer, excludeProperties, model.AdminComment);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.IsTaxExempt → {Arg3}", model, customer, excludeProperties, model.IsTaxExempt);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Active → {Arg3}", model, customer, excludeProperties, model.Active);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.FirstName → {Arg3}", model, customer, excludeProperties, model.FirstName);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.LastName → {Arg3}", model, customer, excludeProperties, model.LastName);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Gender → {Arg3}", model, customer, excludeProperties, model.Gender);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.DateOfBirth → {Arg3}", model, customer, excludeProperties, model.DateOfBirth);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Company → {Arg3}", model, customer, excludeProperties, model.Company);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StreetAddress → {Arg3}", model, customer, excludeProperties, model.StreetAddress);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StreetAddress2 → {Arg3}", model, customer, excludeProperties, model.StreetAddress2);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.ZipPostalCode → {Arg3}", model, customer, excludeProperties, model.ZipPostalCode);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.City → {Arg3}", model, customer, excludeProperties, model.City);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.County → {Arg3}", model, customer, excludeProperties, model.County);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.CountryId → {Arg3}", model, customer, excludeProperties, model.CountryId);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StateProvinceId → {Arg3}", model, customer, excludeProperties, model.StateProvinceId);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Phone → {Arg3}", model, customer, excludeProperties, model.Phone);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Fax → {Arg3}", model, customer, excludeProperties, model.Fax);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.TimeZoneId → {Arg3}", model, customer, excludeProperties, model.TimeZoneId);
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.VatNumber → {Arg3}", model, customer, excludeProperties, model.VatNumber);
                model.Username = customer.Username;
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.Username → {Arg3}", model, customer, excludeProperties, model.Username);
                model.VendorId = customer.VendorId;
                model.AdminComment = customer.AdminComment;
                model.IsTaxExempt = customer.IsTaxExempt;
                model.Active = customer.Active;
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Gender = customer.Gender;
                model.DateOfBirth = customer.DateOfBirth;
                model.Company = customer.Company;
                model.StreetAddress = customer.StreetAddress;
                model.StreetAddress2 = customer.StreetAddress2;
                model.ZipPostalCode = customer.ZipPostalCode;
                model.City = customer.City;
                model.County = customer.County;
                model.CountryId = customer.CountryId;
                model.StateProvinceId = customer.StateProvinceId;
                model.Phone = customer.Phone;
                model.Fax = customer.Fax;
                model.TimeZoneId = customer.TimeZoneId;
                model.VatNumber = customer.VatNumber;
                model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
                model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);
                model.LastIpAddress = customer.LastIpAddress;
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.LastIpAddress → {Arg3}", model, customer, excludeProperties, model.LastIpAddress);
                model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute);
                model.SelectedCustomerRoleIds = (await _customerService.GetCustomerRoleIdsAsync(customer)).ToList();
                model.RegisteredInStore = (await _storeService.GetAllStoresAsync())
                    .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;
                model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                                                 (await _storeService.GetAllStoresAsync()).Select(x => x.Id).Count() > 1;
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);

                model.MustChangePassword = customer.MustChangePassword;
				_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.MustChangePassword → {Arg3}", model, customer, excludeProperties, model.MustChangePassword);

                //prepare model affiliate
                var affiliate = await _affiliateService.GetAffiliateByIdAsync(customer.AffiliateId);
				_logger.LogError("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): affiliate → {Affiliate}", model, customer, excludeProperties, affiliate);
                if (affiliate != null)
                {
                    model.AffiliateId = affiliate.Id;
					_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.AffiliateId → {Arg3}", model, customer, excludeProperties, model.AffiliateId);
                    model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
                }
            }
            //prepare reward points model
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
			_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.DisplayRewardPointsHistory → {Arg3}", model, customer, excludeProperties, model.DisplayRewardPointsHistory);
            if (model.DisplayRewardPointsHistory)
                await PrepareAddRewardPointsToCustomerModelAsync(model.AddRewardPoints);

            //prepare nested search models
            PrepareRewardPointsSearchModel(model.CustomerRewardPointsSearchModel, customer);
            PrepareCustomerAddressSearchModel(model.CustomerAddressSearchModel, customer);
            PrepareCustomerOrderSearchModel(model.CustomerOrderSearchModel, customer);
            await PrepareCustomerShoppingCartSearchModelAsync(model.CustomerShoppingCartSearchModel, customer);
            PrepareCustomerActivityLogSearchModel(model.CustomerActivityLogSearchModel, customer);
            PrepareCustomerBackInStockSubscriptionSearchModel(model.CustomerBackInStockSubscriptionSearchModel, customer);
            await PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(model.CustomerAssociatedExternalAuthRecordsSearchModel, customer);
        }
        else
        {
            //whether to fill in some of properties
            if (!excludeProperties)
            {
                //precheck Registered Role as a default role while creating a new customer through admin
                var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
				_logger.LogError("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): registeredRole → {RegisteredRole}", model, customer, excludeProperties, registeredRole);
                if (registeredRole != null)
                    model.SelectedCustomerRoleIds.Add(registeredRole.Id);
            }
        }

        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.UsernamesEnabled → {Arg3}", model, customer, excludeProperties, model.UsernamesEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.FirstNameEnabled → {Arg3}", model, customer, excludeProperties, model.FirstNameEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.LastNameEnabled → {Arg3}", model, customer, excludeProperties, model.LastNameEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.GenderEnabled → {Arg3}", model, customer, excludeProperties, model.GenderEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.NeutralGenderEnabled → {Arg3}", model, customer, excludeProperties, model.NeutralGenderEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.DateOfBirthEnabled → {Arg3}", model, customer, excludeProperties, model.DateOfBirthEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.CompanyEnabled → {Arg3}", model, customer, excludeProperties, model.CompanyEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StreetAddressEnabled → {Arg3}", model, customer, excludeProperties, model.StreetAddressEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StreetAddress2Enabled → {Arg3}", model, customer, excludeProperties, model.StreetAddress2Enabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.ZipPostalCodeEnabled → {Arg3}", model, customer, excludeProperties, model.ZipPostalCodeEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.CityEnabled → {Arg3}", model, customer, excludeProperties, model.CityEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.CountyEnabled → {Arg3}", model, customer, excludeProperties, model.CountyEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.CountryEnabled → {Arg3}", model, customer, excludeProperties, model.CountryEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.StateProvinceEnabled → {Arg3}", model, customer, excludeProperties, model.StateProvinceEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.PhoneEnabled → {Arg3}", model, customer, excludeProperties, model.PhoneEnabled);
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.FaxEnabled → {Arg3}", model, customer, excludeProperties, model.FaxEnabled);
        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
		_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): model.AllowCustomersToSetTimeZone → {Arg3}", model, customer, excludeProperties, model.AllowCustomersToSetTimeZone);
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.FaxEnabled = _customerSettings.FaxEnabled;

        //set default values for the new model
        if (customer == null)
        {
            model.Active = true;
            model.DisplayVatNumber = false;
        }

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Vendor.None"));

        //prepare model customer attributes
        await PrepareCustomerAttributeModelsAsync(model.CustomerAttributes, customer);

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
		_logger.LogError("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): availableRoles → {AvailableRoles}", model, customer, excludeProperties, availableRoles);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();
			_logger.LogDebug("PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties}): Selected → {Selected}", model, customer, excludeProperties, Selected);

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

        //prepare available countries and states
        if (_customerSettings.CountryEnabled)
        {
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
            if (_customerSettings.StateProvinceEnabled)
                await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerModelAsync(model={Model},customer={Customer},excludeProperties={ExcludeProperties})", model, customer, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged reward points list model
    /// </summary>
    /// <param name="searchModel">Reward points search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reward points list model
    /// </returns>
    public virtual async Task<CustomerRewardPointsListModel> PrepareRewardPointsListModelAsync(CustomerRewardPointsSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get reward points history
        var rewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(customer.Id,
            showNotActivated: true,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerRewardPointsListModel().PrepareToGridAsync(searchModel, rewardPoints, () =>
        {
            return rewardPoints.SelectAwait(async historyEntry =>
            {
                //fill in model values from the entity        
                var rewardPointsHistoryModel = historyEntry.ToModel<CustomerRewardPointsModel>();
				_logger.LogError("PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer}): rewardPointsHistoryModel → {RewardPointsHistoryModel}", searchModel, customer, rewardPointsHistoryModel);

                //convert dates to the user time
                var activatingDate = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
				_logger.LogError("PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer}): activatingDate → {ActivatingDate}", searchModel, customer, activatingDate);
                rewardPointsHistoryModel.CreatedOn = activatingDate;
				_logger.LogDebug("PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer}): rewardPointsHistoryModel.CreatedOn → {Arg2}", searchModel, customer, rewardPointsHistoryModel.CreatedOn);

                rewardPointsHistoryModel.PointsBalance = historyEntry.PointsBalance.HasValue
                    ? historyEntry.PointsBalance.ToString()
                    : string.Format((await _localizationService.GetResourceAsync("Admin.Customers.Customers.RewardPoints.ActivatedLater")), activatingDate);
					_logger.LogDebug("PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer}): rewardPointsHistoryModel.PointsBalance → {Arg2}", searchModel, customer, rewardPointsHistoryModel.PointsBalance);
                rewardPointsHistoryModel.EndDate = !historyEntry.EndDateUtc.HasValue
                    ? null
                    : (DateTime?)(await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc));
					_logger.LogDebug("PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer}): rewardPointsHistoryModel.EndDate → {Arg2}", searchModel, customer, rewardPointsHistoryModel.EndDate);

                //fill in additional values (not existing in the entity)
                rewardPointsHistoryModel.StoreName = (await _storeService.GetStoreByIdAsync(historyEntry.StoreId))?.Name ?? "Unknown";

                return rewardPointsHistoryModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareRewardPointsListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return model;
    }

    /// <summary>
    /// Prepare paged customer address list model
    /// </summary>
    /// <param name="searchModel">Customer address search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerAddressListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer addresses
        var addresses = (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new CustomerAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
        {
            return addresses.SelectAwait(async address =>
            {
                //fill in model values from the entity        
                var addressModel = address.ToModel<AddressModel>();
				_logger.LogError("PrepareCustomerAddressListModelAsync(searchModel={SearchModel},customer={Customer}): addressModel → {AddressModel}", searchModel, customer, addressModel);

                addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                //fill in additional values (not existing in the entity)
                await PrepareModelAddressHtmlAsync(addressModel, address);

                return addressModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCustomerAddressListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return model;
    }

    /// <summary>
    /// Prepare customer address model
    /// </summary>
    /// <param name="model">Customer address model</param>
    /// <param name="customer">Customer</param>
    /// <param name="address">Address</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address model
    /// </returns>
    public virtual async Task<CustomerAddressModel> PrepareCustomerAddressModelAsync(CustomerAddressModel model,
        Customer customer, Address address, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (address != null)
        {
            //fill in model values from the entity
            model ??= new CustomerAddressModel();

            //whether to fill in some of properties
            if (!excludeProperties)
                model.Address = address.ToModel(model.Address);
        }

        model.CustomerId = customer.Id;

        //prepare address model
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.FirstNameRequired = true;
        model.Address.LastNameRequired = true;
        model.Address.EmailRequired = true;
        model.Address.CompanyRequired = _addressSettings.CompanyRequired;
        model.Address.CityRequired = _addressSettings.CityRequired;
        model.Address.CountyRequired = _addressSettings.CountyRequired;
        model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
        model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
        model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
        model.Address.PhoneRequired = _addressSettings.PhoneRequired;
        model.Address.FaxRequired = _addressSettings.FaxRequired;

        return model;
    }

    /// <summary>
    /// Prepare paged customer order list model
    /// </summary>
    /// <param name="searchModel">Customer order search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer order list model
    /// </returns>
    public virtual async Task<CustomerOrderListModel> PrepareCustomerOrderListModelAsync(CustomerOrderSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerOrderListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer orders
        var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            return orders.SelectAwait(async order =>
            {
                //fill in model values from the entity
                var orderModel = order.ToModel<CustomerOrderModel>();
				_logger.LogError("PrepareCustomerOrderListModelAsync(searchModel={SearchModel},customer={Customer}): orderModel → {OrderModel}", searchModel, customer, orderModel);

                //convert dates to the user time
                orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Unknown";
                orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                return orderModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCustomerOrderListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return model;
    }

    /// <summary>
    /// Prepare paged customer shopping cart list model
    /// </summary>
    /// <param name="searchModel">Customer shopping cart search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer shopping cart list model
    /// </returns>
    public virtual async Task<CustomerShoppingCartListModel> PrepareCustomerShoppingCartListModelAsync(CustomerShoppingCartSearchModel searchModel,
        Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(customer);

        //get customer shopping cart
        var shoppingCart = (await _shoppingCartService
            .GetShoppingCartAsync(customer, (ShoppingCartType)searchModel.ShoppingCartTypeId, customWishlistId: 0))
            .ToPagedList(searchModel);
        var customWishlists = shoppingCart.Any(item => item.ShoppingCartType == ShoppingCartType.Wishlist)
            ? await _customWishlistService.GetAllCustomWishlistsAsync(customer.Id)
            : new List<CustomWishlist>();

        //prepare list model
        var model = await new CustomerShoppingCartListModel().PrepareToGridAsync(searchModel, shoppingCart, () =>
        {
            return shoppingCart.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var shoppingCartItemModel = item.ToModel<ShoppingCartItemModel>();

                var product = await _productService.GetProductByIdAsync(item.ProductId);

                //fill in additional values (not existing in the entity)
                shoppingCartItemModel.ProductName = product.Name;
                shoppingCartItemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Unknown";
                shoppingCartItemModel.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml);
                var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                shoppingCartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, unitPrice)).price);
                shoppingCartItemModel.UnitPriceValue = (await _taxService.GetProductPriceAsync(product, unitPrice)).price;
                var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                shoppingCartItemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, subTotal)).price);
                shoppingCartItemModel.TotalValue = (await _taxService.GetProductPriceAsync(product, subTotal)).price;

                //convert dates to the user time
                shoppingCartItemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                if (item.ShoppingCartType == ShoppingCartType.Wishlist)
                {
                    shoppingCartItemModel.CustomWishlistName = customWishlists
                        .FirstOrDefault(wishlist => wishlist.Id == item.CustomWishlistId) is CustomWishlist customWishlist
                        ? customWishlist.Name
                        : await _localizationService.GetResourceAsync("Wishlist.Default");
                }

                return shoppingCartItemModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged customer activity log list model
    /// </summary>
    /// <param name="searchModel">Customer activity log search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer activity log list model
    /// </returns>
    public virtual async Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareCustomerActivityLogListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer activity log
        var activityLog = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
        {
            return activityLog.SelectAwait(async logItem =>
            {
                //fill in model values from the entity
                var customerActivityLogModel = logItem.ToModel<CustomerActivityLogModel>();
				_logger.LogError("PrepareCustomerActivityLogListModelAsync(searchModel={SearchModel},customer={Customer}): customerActivityLogModel → {CustomerActivityLogModel}", searchModel, customer, customerActivityLogModel);

                //fill in additional values (not existing in the entity)
                customerActivityLogModel.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                //convert dates to the user time
                customerActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                return customerActivityLogModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCustomerActivityLogListModelAsync(searchModel={SearchModel},customer={Customer})", searchModel, customer);
        return model;
    }

    /// <summary>
    /// Prepare paged customer back in stock subscriptions list model
    /// </summary>
    /// <param name="searchModel">Customer back in stock subscriptions search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer back in stock subscriptions list model
    /// </returns>
    public virtual async Task<CustomerBackInStockSubscriptionListModel> PrepareCustomerBackInStockSubscriptionListModelAsync(
        CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer back in stock subscriptions
        var subscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerBackInStockSubscriptionListModel().PrepareToGridAsync(searchModel, subscriptions, () =>
        {
            return subscriptions.SelectAwait(async subscription =>
            {
                //fill in model values from the entity
                var subscriptionModel = subscription.ToModel<CustomerBackInStockSubscriptionModel>();

                //convert dates to the user time
                subscriptionModel.CreatedOn =
                    await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                subscriptionModel.StoreName = (await _storeService.GetStoreByIdAsync(subscription.StoreId))?.Name ?? "Unknown";
                subscriptionModel.ProductName = (await _productService.GetProductByIdAsync(subscription.ProductId))?.Name ?? "Unknown";

                return subscriptionModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare online customer search model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer search model
    /// </returns>
    public virtual Task<OnlineCustomerSearchModel> PrepareOnlineCustomerSearchModelAsync(OnlineCustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareOnlineCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareOnlineCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged online customer list model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer list model
    /// </returns>
    public virtual async Task<OnlineCustomerListModel> PrepareOnlineCustomerListModelAsync(OnlineCustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareOnlineCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes);
		_logger.LogError("PrepareOnlineCustomerListModelAsync(searchModel={SearchModel}): lastActivityFrom → {LastActivityFrom}", searchModel, lastActivityFrom);

        //get online customers
        var customers = await _customerService.GetOnlineCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            lastActivityFromUtc: lastActivityFrom,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new OnlineCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<OnlineCustomerModel>();
				_logger.LogError("PrepareOnlineCustomerListModelAsync(searchModel={SearchModel}): customerModel → {CustomerModel}", searchModel, customerModel);

                //convert dates to the user time
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.LastIpAddress = _customerSettings.StoreIpAddresses
                    ? customer.LastIpAddress
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled");
					_logger.LogDebug("PrepareOnlineCustomerListModelAsync(searchModel={SearchModel}): customerModel.LastIpAddress → {Arg1}", searchModel, customerModel.LastIpAddress);
                customerModel.Location = _geoLookupService.LookupCountryName(customer.LastIpAddress);
                customerModel.LastVisitedPage = _customerSettings.StoreLastVisitedPage
                    ? await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute)
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled");
					_logger.LogDebug("PrepareOnlineCustomerListModelAsync(searchModel={SearchModel}): customerModel.LastVisitedPage → {Arg1}", searchModel, customerModel.LastVisitedPage);

                return customerModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareOnlineCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare GDPR request (log) search model
    /// </summary>
    /// <param name="searchModel">GDPR request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR request search model
    /// </returns>
    public virtual async Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareGdprLogSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare request types
        await _baseAdminModelFactory.PrepareGdprRequestTypesAsync(searchModel.AvailableRequestTypes);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareGdprLogSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged GDPR request list model
    /// </summary>
    /// <param name="searchModel">GDPR request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR request list model
    /// </returns>
    public virtual async Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareGdprLogListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var customerId = 0;
		_logger.LogError("PrepareGdprLogListModelAsync(searchModel={SearchModel}): customerId → {CustomerId}", searchModel, customerId);
        var customerInfo = "";
		_logger.LogError("PrepareGdprLogListModelAsync(searchModel={SearchModel}): customerInfo → {CustomerInfo}", searchModel, customerInfo);
        if (!string.IsNullOrEmpty(searchModel.SearchEmail))
        {
            var customer = await _customerService.GetCustomerByEmailAsync(searchModel.SearchEmail);
			_logger.LogError("PrepareGdprLogListModelAsync(searchModel={SearchModel}): customer → {Customer}", searchModel, customer);
            if (customer != null)
                customerId = customer.Id;
            else
            {
                customerInfo = searchModel.SearchEmail;
            }
        }
        //get requests
        var gdprLog = await _gdprService.GetAllLogAsync(
            customerId: customerId,
            customerInfo: customerInfo,
            requestType: searchModel.SearchRequestTypeId > 0 ? (GdprRequestType?)searchModel.SearchRequestTypeId : null,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new GdprLogListModel().PrepareToGridAsync(searchModel, gdprLog, () =>
        {
            return gdprLog.SelectAwait(async log =>
            {
                //fill in model values from the entity
                var customer = await _customerService.GetCustomerByIdAsync(log.CustomerId);

                var requestModel = log.ToModel<GdprLogModel>();
				_logger.LogError("PrepareGdprLogListModelAsync(searchModel={SearchModel}): requestModel → {RequestModel}", searchModel, requestModel);

                //fill in additional values (not existing in the entity)
                requestModel.CustomerInfo = customer != null && !customer.Deleted && !string.IsNullOrEmpty(customer.Email)
                    ? customer.Email
                    : log.CustomerInfo;
                requestModel.RequestType = await _localizationService.GetLocalizedEnumAsync(log.RequestType);

                requestModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);

                return requestModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareGdprLogListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}