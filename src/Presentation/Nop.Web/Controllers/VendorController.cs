using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Vendors;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class VendorController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerService _customerService;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorModelFactory _vendorModelFactory;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public VendorController(CaptchaSettings captchaSettings,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerService customerService,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IUrlRecordService urlRecordService,
        IVendorModelFactory vendorModelFactory,
        IVendorService vendorService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        VendorSettings vendorSettings)
    {
		_logger.LogDebug(">>> Entering VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings})", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings);
        _captchaSettings = captchaSettings;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _captchaSettings → {_captchaSettings}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _captchaSettings);
        _vendorAttributeParser = vendorAttributeParser;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _vendorAttributeParser → {_vendorAttributeParser}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _vendorAttributeParser);
        _vendorAttributeService = vendorAttributeService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _vendorAttributeService → {_vendorAttributeService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _vendorAttributeService);
        _customerService = customerService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _customerService → {_customerService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _customerService);
        _downloadService = downloadService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _downloadService → {_downloadService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _downloadService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _genericAttributeService → {_genericAttributeService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _genericAttributeService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _htmlFormatter → {_htmlFormatter}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _htmlFormatter);
        _localizationService = localizationService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _localizationService → {_localizationService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _localizationService);
        _pictureService = pictureService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _pictureService → {_pictureService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _pictureService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _urlRecordService → {_urlRecordService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _urlRecordService);
        _vendorModelFactory = vendorModelFactory;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _vendorModelFactory → {_vendorModelFactory}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _vendorModelFactory);
        _vendorService = vendorService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _vendorService → {_vendorService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _vendorService);
        _workContext = workContext;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _workContext → {_workContext}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _workflowMessageService → {_workflowMessageService}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _localizationSettings → {_localizationSettings}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _localizationSettings);
        _vendorSettings = vendorSettings;
		_logger.LogDebug("<<< Exiting VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings})", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings);
		_logger.LogDebug("VendorController(captchaSettings={CaptchaSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},customerService={CustomerService},downloadService={DownloadService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},pictureService={PictureService},urlRecordService={UrlRecordService},vendorModelFactory={VendorModelFactory},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},vendorSettings={VendorSettings}): _vendorSettings → {_vendorSettings}", captchaSettings, vendorAttributeParser, vendorAttributeService, customerService, downloadService, genericAttributeService, htmlFormatter, localizationService, pictureService, urlRecordService, vendorModelFactory, vendorService, workContext, workflowMessageService, localizationSettings, vendorSettings, _vendorSettings);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
    {
		_logger.LogDebug(">>> Entering UpdatePictureSeoNamesAsync(vendor={Vendor})", vendor);
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
        if (picture != null)
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(vendor.Name));
			_logger.LogDebug("<<< Exiting UpdatePictureSeoNamesAsync(vendor={Vendor})", vendor);
    }

    protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ParseVendorAttributesAsync(form={Form})", form);
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = "";
		_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): attributesXml → {AttributesXml}", form, attributesXml);
        var attributes = await _vendorAttributeService.GetAllAttributesAsync();
		_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): attributes → {Attributes}", form, attributes);
        foreach (var attribute in attributes)
        {
            var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
			_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): controlId → {ControlId}", form, controlId);
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
						_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): selectedAttributeId → {SelectedAttributeId}", form, selectedAttributeId);
                        if (selectedAttributeId > 0)
                            attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                    }
                }
                    break;
                case AttributeControlType.Checkboxes:
                {
                    var cblAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                                )
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                                attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //load read-only (already server-side selected) values
                    var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
						_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): attributeValues → {AttributeValues}", form, attributeValues);
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
						_logger.LogDebug("ParseVendorAttributesAsync(form={Form}): enteredText → {EnteredText}", form, enteredText);
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, enteredText);
                    }
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

		_logger.LogDebug("<<< Exiting ParseVendorAttributesAsync(form={Form})", form);
        return attributesXml;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> ApplyVendor()
    {
		_logger.LogDebug(">>> Entering ApplyVendor()");
        if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = new ApplyVendorModel();
		_logger.LogDebug("ApplyVendor(): model → {Model}", model);
        model = await _vendorModelFactory.PrepareApplyVendorModelAsync(model, true, false, null);
		_logger.LogDebug("<<< Exiting ApplyVendor()");
        return View(model);
    }

    [HttpPost, ActionName("ApplyVendor")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ApplyVendorSubmit(ApplyVendorModel model, bool captchaValid, IFormFile uploadedFile, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form})", model, captchaValid, uploadedFile, form);
        if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): customer → {Customer}", model, captchaValid, uploadedFile, form, customer);
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (await _customerService.IsAdminAsync(customer))
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.IsAdmin"));

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var pictureId = 0;
		_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): pictureId → {PictureId}", model, captchaValid, uploadedFile, form, pictureId);

        if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
        {
            try
            {
                var contentType = uploadedFile.ContentType.ToLowerInvariant();
				_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): contentType → {ContentType}", model, captchaValid, uploadedFile, form, contentType);

                if (!contentType.StartsWith("image/") || contentType.StartsWith("image/svg"))
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Picture.ErrorMessage"));
                else
                {
                    var vendorPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
					_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): vendorPictureBinary → {VendorPictureBinary}", model, captchaValid, uploadedFile, form, vendorPictureBinary);
                    var picture = await _pictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);
					_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): picture → {Picture}", model, captchaValid, uploadedFile, form, picture);

                    if (picture != null)
                        pictureId = picture.Id;
                }
            }
            catch (Exception)
            {
				_logger.LogError("Exception in ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): {Arg4}", model, captchaValid, uploadedFile, form, Exception.Message);
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Picture.ErrorMessage"));
            }
        }

        //vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
		_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): vendorAttributesXml → {VendorAttributesXml}", model, captchaValid, uploadedFile, form, vendorAttributesXml);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
		_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): warnings → {Warnings}", model, captchaValid, uploadedFile, form, warnings);
        foreach (var warning in warnings)
        {
            ModelState.AddModelError(string.Empty, warning);
        }

        if (ModelState.IsValid)
        {
            var description = _htmlFormatter.FormatText(model.Description, false, false, true, false, false, false);
			_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): description → {Description}", model, captchaValid, uploadedFile, form, description);
            //disabled by default
            var vendor = new Vendor
            {
                Name = model.Name,
                Email = model.Email,
                //some default settings
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions,
                PictureId = pictureId,
                Description = WebUtility.HtmlEncode(description)
            };
            await _vendorService.InsertVendorAsync(vendor);
            //search engine name (the same as vendor name)
            var seName = await _urlRecordService.ValidateSeNameAsync(vendor, vendor.Name, vendor.Name, true);
			_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): seName → {SeName}", model, captchaValid, uploadedFile, form, seName);
            await _urlRecordService.SaveSlugAsync(vendor, seName, 0);

            //associate to the current customer
            //but a store owner will have to manually add this customer role to "Vendors" role
            //if he wants to grant access to admin area
            customer.VendorId = vendor.Id;
			_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): customer.VendorId → {Arg4}", model, captchaValid, uploadedFile, form, customer.VendorId);
            await _customerService.UpdateCustomerAsync(customer);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            //save vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //notify store owner here (email)
            await _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(customer,
                vendor, _localizationSettings.DefaultAdminLanguageId);

            model.DisableFormInput = true;
			_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): model.DisableFormInput → {Arg4}", model, captchaValid, uploadedFile, form, model.DisableFormInput);
            model.Result = await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Submitted");
            return View(model);
        }

        //If we got this far, something failed, redisplay form
        model = await _vendorModelFactory.PrepareApplyVendorModelAsync(model, false, true, vendorAttributesXml);
		_logger.LogDebug("ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form}): model → {Model1}", model, captchaValid, uploadedFile, form, model);
		_logger.LogDebug("<<< Exiting ApplyVendorSubmit(model={Model},captchaValid={CaptchaValid},uploadedFile={UploadedFile},form={Form})", model, captchaValid, uploadedFile, form);
        return View(model);
    }

    public virtual async Task<IActionResult> Info()
    {
		_logger.LogDebug(">>> Entering Info()");
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (await _workContext.GetCurrentVendorAsync() == null || !_vendorSettings.AllowVendorsToEditInfo)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = new VendorInfoModel();
		_logger.LogDebug("Info(): model → {Model}", model);
        model = await _vendorModelFactory.PrepareVendorInfoModelAsync(model, false);
		_logger.LogDebug("<<< Exiting Info()");
		_logger.LogDebug("<<< Exiting Info(model={Model},uploadedFile={UploadedFile},form={Form})", model, uploadedFile, form);
        return View(model);
    }

    [HttpPost, ActionName("Info")]
    [FormValueRequired("save-info-button")]
    public virtual async Task<IActionResult> Info(VendorInfoModel model, IFormFile uploadedFile, IFormCollection form)
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var vendor = await _workContext.GetCurrentVendorAsync();
        if (vendor == null || !_vendorSettings.AllowVendorsToEditInfo)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        Picture picture = null;

        if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
        {
            try
            {
                var contentType = uploadedFile.ContentType.ToLowerInvariant();

                if (!contentType.StartsWith("image/") || contentType.StartsWith("image/svg"))
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.VendorInfo.Picture.ErrorMessage"));
                else
                {
                    var vendorPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                    picture = await _pictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.VendorInfo.Picture.ErrorMessage"));
            }
        }

        var prevPicture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);

        //vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
        foreach (var warning in warnings)
        {
            ModelState.AddModelError(string.Empty, warning);
        }

        if (ModelState.IsValid)
        {
            var description = _htmlFormatter.FormatText(model.Description, false, false, true, false, false, false);

            vendor.Name = model.Name;
            vendor.Email = model.Email;
            vendor.Description = WebUtility.HtmlEncode(description);

            if (picture != null)
            {
                vendor.PictureId = picture.Id;

                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            await _vendorService.UpdateVendorAsync(vendor);

            //save vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //notifications
            if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(vendor, _localizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }

        //If we got this far, something failed, redisplay form
        model = await _vendorModelFactory.PrepareVendorInfoModelAsync(model, true, vendorAttributesXml);
        return View(model);
    }

    [HttpPost, ActionName("Info")]
    [FormValueRequired("remove-picture")]
    public virtual async Task<IActionResult> RemovePicture()
    {
		_logger.LogDebug(">>> Entering RemovePicture()");
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("RemovePicture(): vendor → {Vendor}", vendor);
        if (vendor == null || !_vendorSettings.AllowVendorsToEditInfo)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
		_logger.LogDebug("RemovePicture(): picture → {Picture}", picture);

        if (picture != null)
            await _pictureService.DeletePictureAsync(picture);

        vendor.PictureId = 0;
		_logger.LogDebug("RemovePicture(): vendor.PictureId → {Arg0}", vendor.PictureId);
        await _vendorService.UpdateVendorAsync(vendor);

        //notifications
        if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
            await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(vendor, _localizationSettings.DefaultAdminLanguageId);

		_logger.LogDebug("<<< Exiting RemovePicture()");
        return RedirectToAction("Info");
    }

    #endregion
}