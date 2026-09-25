using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Models.Vendors;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the vendor model factory
/// </summary>
public partial class VendorModelFactory : IVendorModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public VendorModelFactory(CaptchaSettings captchaSettings,
        CommonSettings commonSettings,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        VendorSettings vendorSettings)
    {
		_logger.LogError(">>> Entering VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings})", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings);
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _commonSettings → {_commonSettings}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _commonSettings);
        _vendorAttributeParser = vendorAttributeParser;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _vendorAttributeParser → {_vendorAttributeParser}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _vendorAttributeParser);
        _vendorAttributeService = vendorAttributeService;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _vendorAttributeService → {_vendorAttributeService}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _vendorAttributeService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _genericAttributeService → {_genericAttributeService}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _localizationService → {_localizationService}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _localizationService);
        _pictureService = pictureService;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _pictureService → {_pictureService}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _pictureService);
        _workContext = workContext;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _workContext → {_workContext}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _workContext);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _mediaSettings → {_mediaSettings}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _mediaSettings);
        _vendorSettings = vendorSettings;
		_logger.LogDebug("VendorModelFactory(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},vendorAttributeParser={VendorAttributeParser},vendorAttributeService={VendorAttributeService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},vendorSettings={VendorSettings}): _vendorSettings → {_vendorSettings}", captchaSettings, commonSettings, vendorAttributeParser, vendorAttributeService, genericAttributeService, localizationService, pictureService, workContext, mediaSettings, vendorSettings, _vendorSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor attribute models
    /// </summary>
    /// <param name="vendorAttributesXml">Vendor attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the vendor attribute model
    /// </returns>
    protected virtual async Task<IList<VendorAttributeModel>> PrepareVendorAttributesAsync(string vendorAttributesXml)
    {
		_logger.LogError(">>> Entering PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml})", vendorAttributesXml);
        var result = new List<VendorAttributeModel>();

		_logger.LogError("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): result → {Result}", vendorAttributesXml, result);
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
		_logger.LogError("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): vendorAttributes → {VendorAttributes}", vendorAttributesXml, vendorAttributes);
        foreach (var attribute in vendorAttributes)
        {
            var attributeModel = new VendorAttributeModel
            {
                Id = attribute.Id,
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogError("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): attributeValues → {AttributeValues}", vendorAttributesXml, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new VendorAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(valueModel);
                }
            }

            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(vendorAttributesXml))
                    {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;
							_logger.LogDebug("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): item.IsPreSelected → {Arg1}", vendorAttributesXml, item.IsPreSelected);

                        //select new values
                        var selectedValues = await _vendorAttributeParser.ParseAttributeValuesAsync(vendorAttributesXml);
						_logger.LogError("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): selectedValues → {SelectedValues}", vendorAttributesXml, selectedValues);
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
                    if (!string.IsNullOrEmpty(vendorAttributesXml))
                    {
                        var enteredText = _vendorAttributeParser.ParseValues(vendorAttributesXml, attribute.Id);
						_logger.LogError("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): enteredText → {EnteredText}", vendorAttributesXml, enteredText);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
							_logger.LogDebug("PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml}): attributeModel.DefaultValue → {Arg1}", vendorAttributesXml, attributeModel.DefaultValue);
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

		_logger.LogWarning("<<< Exiting PrepareVendorAttributesAsync(vendorAttributesXml={VendorAttributesXml})", vendorAttributesXml);
        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the apply vendor model
    /// </summary>
    /// <param name="model">The apply vendor model</param>
    /// <param name="validateVendor">Whether to validate that the customer is already a vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="vendorAttributesXml">Vendor attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the apply vendor model
    /// </returns>
    public virtual async Task<ApplyVendorModel> PrepareApplyVendorModelAsync(ApplyVendorModel model,
        bool validateVendor, bool excludeProperties, string vendorAttributesXml)
    {
		_logger.LogError(">>> Entering PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml})", model, validateVendor, excludeProperties, vendorAttributesXml);
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): customer → {Customer}", model, validateVendor, excludeProperties, vendorAttributesXml, customer);
        if (validateVendor && customer.VendorId > 0)
        {
            //already applied for vendor account
            model.DisableFormInput = true;
			_logger.LogDebug("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): model.DisableFormInput → {Arg4}", model, validateVendor, excludeProperties, vendorAttributesXml, model.DisableFormInput);
            model.Result = await _localizationService.GetResourceAsync("Vendors.ApplyAccount.AlreadyApplied");
        }

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage;
		_logger.LogDebug("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): model.DisplayCaptcha → {Arg4}", model, validateVendor, excludeProperties, vendorAttributesXml, model.DisplayCaptcha);
		_logger.LogDebug("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): model.TermsOfServicePopup → {Arg4}", model, validateVendor, excludeProperties, vendorAttributesXml, model.TermsOfServicePopup);
        model.TermsOfServiceEnabled = _vendorSettings.TermsOfServiceEnabled;
		_logger.LogDebug("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): model.TermsOfServiceEnabled → {Arg4}", model, validateVendor, excludeProperties, vendorAttributesXml, model.TermsOfServiceEnabled);
        model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;

        if (!excludeProperties)
        {
            model.Email = customer.Email;
			_logger.LogDebug("PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml}): model.Email → {Arg4}", model, validateVendor, excludeProperties, vendorAttributesXml, model.Email);
        }

        //vendor attributes
        model.VendorAttributes = await PrepareVendorAttributesAsync(vendorAttributesXml);

		_logger.LogWarning("<<< Exiting PrepareApplyVendorModelAsync(model={Model},validateVendor={ValidateVendor},excludeProperties={ExcludeProperties},vendorAttributesXml={VendorAttributesXml})", model, validateVendor, excludeProperties, vendorAttributesXml);
        return model;
    }

    /// <summary>
    /// Prepare the vendor info model
    /// </summary>
    /// <param name="model">Vendor info model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overriddenVendorAttributesXml">Overridden vendor attributes in XML format; pass null to use VendorAttributes of vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor info model
    /// </returns>
    public virtual async Task<VendorInfoModel> PrepareVendorInfoModelAsync(VendorInfoModel model,
        bool excludeProperties, string overriddenVendorAttributesXml = "")
    {
		_logger.LogError(">>> Entering PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml})", model, excludeProperties, overriddenVendorAttributesXml);
        ArgumentNullException.ThrowIfNull(model);

        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogError("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): vendor → {Vendor}", model, excludeProperties, overriddenVendorAttributesXml, vendor);
        if (!excludeProperties)
        {
            model.Description = vendor.Description;
			_logger.LogDebug("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): model.Description → {Arg3}", model, excludeProperties, overriddenVendorAttributesXml, model.Description);
			_logger.LogDebug("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): model.Name → {Arg3}", model, excludeProperties, overriddenVendorAttributesXml, model.Name);
            model.Email = vendor.Email;
			_logger.LogDebug("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): model.Email → {Arg3}", model, excludeProperties, overriddenVendorAttributesXml, model.Email);
            model.Name = vendor.Name;
        }

        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
		_logger.LogError("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): picture → {Picture}", model, excludeProperties, overriddenVendorAttributesXml, picture);
        var pictureSize = _mediaSettings.AvatarPictureSize;
        (model.PictureUrl, _) = picture != null ? await _pictureService.GetPictureUrlAsync(picture, pictureSize) : (string.Empty, null);

        //vendor attributes
        if (string.IsNullOrEmpty(overriddenVendorAttributesXml))
            overriddenVendorAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
			_logger.LogError("PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml}): overriddenVendorAttributesXml → {OverriddenVendorAttributesXml1}", model, excludeProperties, overriddenVendorAttributesXml, overriddenVendorAttributesXml);
        model.VendorAttributes = await PrepareVendorAttributesAsync(overriddenVendorAttributesXml);

		_logger.LogWarning("<<< Exiting PrepareVendorInfoModelAsync(model={Model},excludeProperties={ExcludeProperties},overriddenVendorAttributesXml={OverriddenVendorAttributesXml})", model, excludeProperties, overriddenVendorAttributesXml);
        return model;
    }

    #endregion
}