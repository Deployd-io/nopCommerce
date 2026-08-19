using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the newsletter model factory
/// </summary>
public partial class NewsLetterModelFactory : INewsLetterModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public NewsLetterModelFactory(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering NewsLetterModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},localizationService={LocalizationService})", captchaSettings, customerSettings, localizationService);
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
		_logger.LogDebug("NewsLetterModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},localizationService={LocalizationService}): _customerSettings → {_customerSettings}", captchaSettings, customerSettings, localizationService, _customerSettings);
		_logger.LogDebug("NewsLetterModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},localizationService={LocalizationService}): _captchaSettings → {_captchaSettings}", captchaSettings, customerSettings, localizationService, _captchaSettings);
		_logger.LogDebug("<<< Exiting NewsLetterModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},localizationService={LocalizationService})", captchaSettings, customerSettings, localizationService);
        _localizationService = localizationService;
    }
	_logger.LogDebug("NewsLetterModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},localizationService={LocalizationService}): _localizationService → {_localizationService}", captchaSettings, customerSettings, localizationService, _localizationService);

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the newsletter box model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter box model
    /// </returns>
    public virtual Task<NewsLetterBoxModel> PrepareNewsLetterBoxModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareNewsLetterBoxModelAsync()");
        var model = new NewsLetterBoxModel
        {
            AllowToUnsubscribe = _customerSettings.NewsletterBlockAllowToUnsubscribe,
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsletterPage,
            IsReCaptchaV3 = _captchaSettings.CaptchaType == CaptchaType.ReCaptchaV3,
            ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey
        };

		_logger.LogDebug("<<< Exiting PrepareNewsLetterBoxModelAsync()");
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the subscription activation model
    /// </summary>
    /// <param name="active">Whether the subscription has been activated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscription activation model
    /// </returns>
    public virtual async Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active)
    {
		_logger.LogDebug(">>> Entering PrepareSubscriptionActivationModelAsync(active={Active})", active);
        var model = new SubscriptionActivationModel
        {
            Result = active
                ? await _localizationService.GetResourceAsync("Newsletter.ResultActivated")
                : await _localizationService.GetResourceAsync("Newsletter.ResultDeactivated")
        };

		_logger.LogDebug("<<< Exiting PrepareSubscriptionActivationModelAsync(active={Active})", active);
        return model;
    }

    #endregion
}