using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the vendor model factory implementation
/// </summary>
public partial class VendorModelFactory : IVendorModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public VendorModelFactory(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        VendorSettings vendorSettings)
    {
		_logger.LogDebug(">>> Entering VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings})", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings);
        _currencySettings = currencySettings;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _currencySettings → {_currencySettings}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _currencySettings);
        _currencyService = currencyService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _currencyService → {_currencyService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _currencyService);
        _addressModelFactory = addressModelFactory;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _addressModelFactory → {_addressModelFactory}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _addressModelFactory);
        _addressService = addressService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _addressService → {_addressService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _addressService);
        _vendorAttributeParser = vendorAttributeParser;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _vendorAttributeParser → {_vendorAttributeParser}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _vendorAttributeParser);
        _vendorAttributeService = vendorAttributeService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _vendorAttributeService → {_vendorAttributeService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _vendorAttributeService);
        _customerService = customerService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _customerService → {_customerService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _dateTimeHelper → {_dateTimeHelper}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _dateTimeHelper);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _genericAttributeService → {_genericAttributeService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _localizationService → {_localizationService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _localizedModelFactory → {_localizedModelFactory}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _localizedModelFactory);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _urlRecordService → {_urlRecordService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _urlRecordService);
        _vendorService = vendorService;
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _vendorService → {_vendorService}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _vendorService);
        _vendorSettings = vendorSettings;
		_logger.LogDebug("<<< Exiting VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings})", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings);
		_logger.LogDebug("VendorModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},addressModelFactory={AddressModelFactory},addressService={AddressService},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},urlRecordService={UrlRecordService},vendorService={VendorService},vendorSettings={VendorSettings}): _vendorSettings → {_vendorSettings}", currencySettings, currencyService, addressModelFactory, addressService, vendorAttributeParser, vendorAttributeService, customerService, dateTimeHelper, genericAttributeService, localizationService, localizedModelFactory, urlRecordService, vendorService, vendorSettings, _vendorSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor associated customer models
    /// </summary>
    /// <param name="models">List of vendor associated customer models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAssociatedCustomerModelsAsync(IList<VendorAssociatedCustomerModel> models, Vendor vendor)
    {
		_logger.LogDebug(">>> Entering PrepareAssociatedCustomerModelsAsync(models={Models},vendor={Vendor})", models, vendor);
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(vendor);

        var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
		_logger.LogDebug("PrepareAssociatedCustomerModelsAsync(models={Models},vendor={Vendor}): associatedCustomers → {AssociatedCustomers}", models, vendor, associatedCustomers);
        foreach (var customer in associatedCustomers)
        {
            models.Add(new VendorAssociatedCustomerModel
            {
                Id = customer.Id,
                Email = customer.Email
            });
        }
		_logger.LogDebug("<<< Exiting PrepareAssociatedCustomerModelsAsync(models={Models},vendor={Vendor})", models, vendor);
    }

    /// <summary>
    /// Prepare vendor attribute models
    /// </summary>
    /// <param name="models">List of vendor attribute models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareVendorAttributeModelsAsync(IList<VendorModel.VendorAttributeModel> models, Vendor vendor)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor})", models, vendor);
        ArgumentNullException.ThrowIfNull(models);

        //get available vendor attributes
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
		_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): vendorAttributes → {VendorAttributes}", models, vendor, vendorAttributes);
        foreach (var attribute in vendorAttributes)
        {
            var attributeModel = new VendorModel.VendorAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): attributeValues → {AttributeValues}", models, vendor, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new VendorModel.VendorAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            if (vendor != null)
            {
                var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
				_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): selectedVendorAttributes → {SelectedVendorAttributes}", models, vendor, selectedVendorAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;
									_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): item.IsPreSelected → {Arg2}", models, vendor, item.IsPreSelected);

                                //select new values
                                var selectedValues = await _vendorAttributeParser.ParseAttributeValuesAsync(selectedVendorAttributes);
								_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): selectedValues → {SelectedValues}", models, vendor, selectedValues);
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
                            if (!string.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                var enteredText = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
								_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): enteredText → {EnteredText}", models, vendor, enteredText);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
									_logger.LogDebug("PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor}): attributeModel.DefaultValue → {Arg2}", models, vendor, attributeModel.DefaultValue);
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
		_logger.LogDebug("<<< Exiting PrepareVendorAttributeModelsAsync(models={Models},vendor={Vendor})", models, vendor);
    }

    /// <summary>
    /// Prepare vendor note search model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>Vendor note search model</returns>
    protected virtual VendorNoteSearchModel PrepareVendorNoteSearchModel(VendorNoteSearchModel searchModel, Vendor vendor)
    {
		_logger.LogDebug(">>> Entering PrepareVendorNoteSearchModel(searchModel={SearchModel},vendor={Vendor})", searchModel, vendor);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        searchModel.VendorId = vendor.Id;
		_logger.LogDebug("PrepareVendorNoteSearchModel(searchModel={SearchModel},vendor={Vendor}): searchModel.VendorId → {Arg2}", searchModel, vendor, searchModel.VendorId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareVendorNoteSearchModel(searchModel={SearchModel},vendor={Vendor})", searchModel, vendor);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare vendor customer search model
    /// </summary>
    /// <param name="searchModel">Vendor customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor customer search model
    /// </returns>
    public virtual Task<VendorCustomerSearchModel> PrepareVendorCustomerSearchModelAsync(VendorCustomerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareVendorCustomerSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor customer list model
    /// </summary>
    /// <param name="searchModel">Vendor customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor customer list model
    /// </returns>
    public virtual async Task<VendorCustomerListModel> PrepareVendorCustomerListModelAsync(VendorCustomerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customers
        var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
		_logger.LogDebug("PrepareVendorCustomerListModelAsync(searchModel={SearchModel}): searchCustomerRoleIds → {SearchCustomerRoleIds}", searchModel, searchCustomerRoleIds);
        var customers = await _customerService.GetAllCustomersAsync(
            email: searchModel.SearchEmail,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            company: searchModel.SearchCompany,
            customerRoleIds: searchCustomerRoleIds,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new VendorCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer => new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Company = customer.Company,
            });
        });

		_logger.LogDebug("<<< Exiting PrepareVendorCustomerListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare vendor search model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor search model
    /// </returns>
    public virtual Task<VendorSearchModel> PrepareVendorSearchModelAsync(VendorSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareVendorSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor list model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor list model
    /// </returns>
    public virtual async Task<VendorListModel> PrepareVendorListModelAsync(VendorSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get vendors
        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true,
            name: searchModel.SearchName,
            email: searchModel.SearchEmail,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new VendorListModel().PrepareToGridAsync(searchModel, vendors, () =>
        {
            //fill in model values from the entity
            return vendors.SelectAwait(async vendor =>
            {
                var vendorModel = vendor.ToModel<VendorModel>();
				_logger.LogDebug("PrepareVendorListModelAsync(searchModel={SearchModel}): vendorModel → {VendorModel}", searchModel, vendorModel);

                vendorModel.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);

                return vendorModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareVendorListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare vendor model
    /// </summary>
    /// <param name="model">Vendor model</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor model
    /// </returns>
    public virtual async Task<VendorModel> PrepareVendorModelAsync(VendorModel model, Vendor vendor, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties})", model, vendor, excludeProperties);
        Func<VendorLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (vendor != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = vendor.ToModel<VendorModel>();
				_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model → {Model1}", model, vendor, excludeProperties, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);
            }

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Description, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(vendor, languageId, false, false);
            };

            //prepare associated customers
            await PrepareAssociatedCustomerModelsAsync(model.AssociatedCustomers, vendor);

            if (vendor.PmCustomerId > 0)
            {
                var pmCustomer = await _customerService.GetCustomerByIdAsync(vendor.PmCustomerId.Value);
				_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): pmCustomer → {PmCustomer}", model, vendor, excludeProperties, pmCustomer);
				_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PmCustomerInfo → {Arg3}", model, vendor, excludeProperties, model.PmCustomerInfo);
                model.PmCustomerInfo = pmCustomer.Email;
            }

            //prepare nested search models
            PrepareVendorNoteSearchModel(model.VendorNoteSearchModel, vendor);
        }

        //set default values for the new model
        if (vendor == null)
        {
            model.PageSize = 6;
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PageSize → {Arg3}", model, vendor, excludeProperties, model.PageSize);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.Active → {Arg3}", model, vendor, excludeProperties, model.Active);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.AllowCustomersToSelectPageSize → {Arg3}", model, vendor, excludeProperties, model.AllowCustomersToSelectPageSize);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PageSizeOptions → {Arg3}", model, vendor, excludeProperties, model.PageSizeOptions);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PriceRangeFiltering → {Arg3}", model, vendor, excludeProperties, model.PriceRangeFiltering);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.ManuallyPriceRange → {Arg3}", model, vendor, excludeProperties, model.ManuallyPriceRange);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PriceFrom → {Arg3}", model, vendor, excludeProperties, model.PriceFrom);
			_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): model.PriceTo → {Arg3}", model, vendor, excludeProperties, model.PriceTo);
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare model vendor attributes
        await PrepareVendorAttributeModelsAsync(model.VendorAttributes, vendor);

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(vendor?.AddressId ?? 0);
		_logger.LogDebug("PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties}): address → {Address}", model, vendor, excludeProperties, address);
        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);

		_logger.LogDebug("<<< Exiting PrepareVendorModelAsync(model={Model},vendor={Vendor},excludeProperties={ExcludeProperties})", model, vendor, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged vendor note list model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor note list model
    /// </returns>
    public virtual async Task<VendorNoteListModel> PrepareVendorNoteListModelAsync(VendorNoteSearchModel searchModel, Vendor vendor)
    {
		_logger.LogDebug(">>> Entering PrepareVendorNoteListModelAsync(searchModel={SearchModel},vendor={Vendor})", searchModel, vendor);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        //get vendor notes
        var vendorNotes = await _vendorService.GetVendorNotesByVendorAsync(vendor.Id, searchModel.Page - 1, searchModel.PageSize);
		_logger.LogDebug("PrepareVendorNoteListModelAsync(searchModel={SearchModel},vendor={Vendor}): vendorNotes → {VendorNotes}", searchModel, vendor, vendorNotes);

        //prepare list model
        var model = await new VendorNoteListModel().PrepareToGridAsync(searchModel, vendorNotes, () =>
        {
            //fill in model values from the entity
            return vendorNotes.SelectAwait(async note =>
            {
                //fill in model values from the entity        
                var vendorNoteModel = note.ToModel<VendorNoteModel>();
				_logger.LogDebug("PrepareVendorNoteListModelAsync(searchModel={SearchModel},vendor={Vendor}): vendorNoteModel → {VendorNoteModel}", searchModel, vendor, vendorNoteModel);

                //convert dates to the user time
                vendorNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(note.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                vendorNoteModel.Note = _vendorService.FormatVendorNoteText(note);

                return vendorNoteModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareVendorNoteListModelAsync(searchModel={SearchModel},vendor={Vendor})", searchModel, vendor);
        return model;
    }

    #endregion
}