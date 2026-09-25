using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class VendorController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorModelFactory _vendorModelFactory;
    protected readonly IVendorService _vendorService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public VendorController(ForumSettings forumSettings,
        IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IUrlRecordService urlRecordService,
        IVendorModelFactory vendorModelFactory,
        IVendorService vendorService)
    {
		_logger.LogError(">>> Entering VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService})", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService);
        _forumSettings = forumSettings;
        _addressService = addressService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _addressService → {_addressService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _addressService);
        _addressAttributeParser = addressAttributeParser;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _addressAttributeParser → {_addressAttributeParser}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _addressAttributeParser);
        _vendorAttributeParser = vendorAttributeParser;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _vendorAttributeParser → {_vendorAttributeParser}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _vendorAttributeParser);
        _vendorAttributeService = vendorAttributeService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _vendorAttributeService → {_vendorAttributeService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _vendorAttributeService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _customerActivityService → {_customerActivityService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _customerService → {_customerService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _genericAttributeService → {_genericAttributeService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _localizationService → {_localizationService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _localizedEntityService → {_localizedEntityService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _notificationService → {_notificationService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _permissionService → {_permissionService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _permissionService);
        _pictureService = pictureService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _pictureService → {_pictureService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _pictureService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _urlRecordService → {_urlRecordService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _urlRecordService);
        _vendorModelFactory = vendorModelFactory;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _vendorModelFactory → {_vendorModelFactory}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _vendorModelFactory);
        _vendorService = vendorService;
		_logger.LogDebug("VendorController(forumSettings={ForumSettings},addressService={AddressService},addressAttributeParser={AddressAttributeParser},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService}): _vendorService → {_vendorService}", forumSettings, addressService, addressAttributeParser, vendorAttributeParser, vendorAttributeService, customerActivityService, customerService, genericAttributeService, localizationService, localizedEntityService, notificationService, permissionService, pictureService, urlRecordService, vendorModelFactory, vendorService, _vendorService);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
    {
		_logger.LogError(">>> Entering UpdatePictureSeoNamesAsync(vendor={Vendor})", vendor);
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
			_logger.LogError("UpdatePictureSeoNamesAsync(vendor={Vendor}): picture → {Picture}", vendor, picture);
        if (picture != null) {
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(vendor.Name));
        }
			_logger.LogWarning("<<< Exiting UpdatePictureSeoNamesAsync(vendor={Vendor})", vendor);
    }

    protected virtual async Task UpdateLocalesAsync(Vendor vendor, VendorModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(vendor={Vendor},model={Model})", vendor, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.Description,
                localized.Description,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(vendor, localized.SeName, localized.Name, false);
			_logger.LogError("UpdateLocalesAsync(vendor={Vendor},model={Model}): seName → {SeName}", vendor, model, seName);
            await _urlRecordService.SaveSlugAsync(vendor, seName, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(vendor={Vendor},model={Model})", vendor, model);
    }

    protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
    {
		_logger.LogError(">>> Entering ParseVendorAttributesAsync(form={Form})", form);
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
		_logger.LogError("ParseVendorAttributesAsync(form={Form}): vendorAttributes → {VendorAttributes}", form, vendorAttributes);
        foreach (var attribute in vendorAttributes)
        {
            var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
			_logger.LogError("ParseVendorAttributesAsync(form={Form}): controlId → {ControlId}", form, controlId);
            StringValues ctrlAttributes;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    ctrlAttributes = form[controlId];
					_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): ctrlAttributes → {CtrlAttributes}", form, ctrlAttributes);
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
						_logger.LogError("ParseVendorAttributesAsync(form={Form}): ctrlAttributes → {CtrlAttributes}", form, ctrlAttributes);
                        var selectedAttributeId = int.Parse(ctrlAttributes);
						_logger.LogError("ParseVendorAttributesAsync(form={Form}): selectedAttributeId → {SelectedAttributeId}", form, selectedAttributeId);
                        if (selectedAttributeId > 0)
                            attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.Checkboxes:
                    var cblAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                                attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }

                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
					_logger.LogError("ParseVendorAttributesAsync(form={Form}): attributeValues → {AttributeValues}", form, attributeValues);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
						_logger.LogError("ParseVendorAttributesAsync(form={Form}): enteredText → {EnteredText}", form, enteredText);
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, enteredText);
                    }

                    break;
                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                //not supported vendor attributes
                default:
                    break;
            }
        }

		_logger.LogWarning("<<< Exiting ParseVendorAttributesAsync(form={Form})", form);
        return attributesXml;
    }

    #endregion

    #region Vendors

    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopup()
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorCustomerSearchModelAsync(new VendorCustomerSearchModel());

		_logger.LogWarning("<<< Exiting AddCustomerToVendorPopup()");
        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopup([Bind(Prefix = nameof(AddCustomerToVendorModel))] AddCustomerToVendorModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
            return Content("Cannot load a customer");

        ViewBag.RefreshPage = true;
        ViewBag.customerId = customer.Id;
        ViewBag.customerInfo = customer.Email;

        return View(new VendorCustomerSearchModel());
    }

    [HttpPost]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopupList(VendorCustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering AddCustomerToVendorPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorCustomerListModelAsync(searchModel);
		_logger.LogError("AddCustomerToVendorPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting AddCustomerToVendorPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorSearchModelAsync(new VendorSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> List(VendorSearchModel searchModel)
    {
		_logger.LogError(">>> Entering List(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorListModelAsync(searchModel);
		_logger.LogError("List(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorModelAsync(new VendorModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing},form={Form})", model, continueEditing, form);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(VendorModel model, bool continueEditing, IFormCollection form)
    {
        //parse vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
        foreach (var warning in warnings)
        {
            ModelState.AddModelError(string.Empty, warning);
        }

        if (ModelState.IsValid)
        {
            var vendor = model.ToEntity<Vendor>();
            await _vendorService.InsertVendorAsync(vendor);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewVendor",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendor"), vendor.Id), vendor);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
            await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

            //address
            var address = model.Address.ToEntity<Address>();
            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;
            await _addressService.InsertAddressAsync(address);
            vendor.AddressId = address.Id;
            await _vendorService.UpdateVendorAsync(vendor);

            //vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //locales
            await UpdateLocalesAsync(vendor, model);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendor.Id });
        }

        //prepare model
        model = await _vendorModelFactory.PrepareVendorModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(id);
		_logger.LogError("Edit(id={Id}): vendor → {Vendor}", id, vendor);
        if (vendor == null || vendor.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _vendorModelFactory.PrepareVendorModelAsync(null, vendor);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

        if (!_forumSettings.AllowPrivateMessages && model.PmCustomerId > 0)
            _notificationService.WarningNotification("Private messages are disabled. Do not forget to enable them.");

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing},form={Form})", model, continueEditing, form);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(VendorModel model, bool continueEditing, IFormCollection form)
    {
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(model.Id);
        if (vendor == null || vendor.Deleted)
            return RedirectToAction("List");

        //parse vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
        foreach (var warning in warnings)
        {
            ModelState.AddModelError(string.Empty, warning);
        }

        //custom address attributes
        var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
        foreach (var error in customAttributeWarnings)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (ModelState.IsValid)
        {
            var prevPictureId = vendor.PictureId;
            vendor = model.ToEntity(vendor);
            await _vendorService.UpdateVendorAsync(vendor);

            //vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditVendor",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendor"), vendor.Id), vendor);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
            await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

            //address
            var address = await _addressService.GetAddressByIdAsync(vendor.AddressId);
            if (address == null)
            {
                address = model.Address.ToEntity<Address>();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);
                vendor.AddressId = address.Id;
                await _vendorService.UpdateVendorAsync(vendor);
            }
            else
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.UpdateAddressAsync(address);
            }

            //locales
            await UpdateLocalesAsync(vendor, model);

            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
            {
                var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }
            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendor.Id });
        }

        //prepare model
        model = await _vendorModelFactory.PrepareVendorModelAsync(model, vendor, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(id);
		_logger.LogError("Delete(id={Id}): vendor → {Vendor}", id, vendor);
        if (vendor == null)
            return RedirectToAction("List");

        //clear associated customer references
        var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
		_logger.LogError("Delete(id={Id}): associatedCustomers → {AssociatedCustomers}", id, associatedCustomers);
        foreach (var customer in associatedCustomers)
        {
            customer.VendorId = 0;
            await _customerService.UpdateCustomerAsync(customer);
        }

        //delete a vendor
        await _vendorService.DeleteVendorAsync(vendor);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteVendor",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendor"), vendor.Id), vendor);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #region Vendor notes

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> VendorNotesSelect(VendorNoteSearchModel searchModel)
    {
		_logger.LogError(">>> Entering VendorNotesSelect(searchModel={SearchModel})", searchModel);
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(searchModel.VendorId)
            ?? throw new ArgumentException("No vendor found with the specified id");

        //prepare model
        var model = await _vendorModelFactory.PrepareVendorNoteListModelAsync(searchModel, vendor);
		_logger.LogError("VendorNotesSelect(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting VendorNotesSelect(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VendorNoteAdd(int vendorId, string message)
    {
		_logger.LogError(">>> Entering VendorNoteAdd(vendorId={VendorId},message={Message})", vendorId, message);
        if (string.IsNullOrEmpty(message))
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Vendors.VendorNotes.Fields.Note.Validation"));

        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
		_logger.LogError("VendorNoteAdd(vendorId={VendorId},message={Message}): vendor → {Vendor}", vendorId, message, vendor);
        if (vendor == null)
            return ErrorJson("Vendor cannot be loaded");

        await _vendorService.InsertVendorNoteAsync(new VendorNote
        {
            Note = message,
            CreatedOnUtc = DateTime.UtcNow,
            VendorId = vendor.Id
        });
		_logger.LogDebug("VendorNoteAdd(vendorId={VendorId},message={Message}): VendorId → {VendorId1}", vendorId, message, VendorId);

		_logger.LogWarning("<<< Exiting VendorNoteAdd(vendorId={VendorId},message={Message})", vendorId, message);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VendorNoteDelete(int id)
    {
		_logger.LogError(">>> Entering VendorNoteDelete(id={Id})", id);
        //try to get a vendor note with the specified id
        var vendorNote = await _vendorService.GetVendorNoteByIdAsync(id)
            ?? throw new ArgumentException("No vendor note found with the specified id", nameof(id));

        await _vendorService.DeleteVendorNoteAsync(vendorNote);

		_logger.LogWarning("<<< Exiting VendorNoteDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}