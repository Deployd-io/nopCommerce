using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Controllers;

[AutoValidateAntiforgeryToken]
public class AuthenticationController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly GoogleAuthenticatorService _googleAuthenticatorService;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor
    public AuthenticationController(
        CustomerSettings customerSettings,
        GoogleAuthenticatorService googleAuthenticatorService,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext})", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext);
        _customerSettings = customerSettings;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _customerSettings → {_customerSettings}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _customerSettings);
        _googleAuthenticatorService = googleAuthenticatorService;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _googleAuthenticatorService → {_googleAuthenticatorService}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _googleAuthenticatorService);
        _customerRegistrationService = customerRegistrationService;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _customerRegistrationService → {_customerRegistrationService}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _customerRegistrationService);
        _customerService = customerService;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _customerService → {_customerService}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _localizationService → {_localizationService}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _notificationService → {_notificationService}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _notificationService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext})", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext);
		_logger.LogDebug("AuthenticationController(customerSettings={CustomerSettings},googleAuthenticatorService={GoogleAuthenticatorService},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _workContext → {_workContext}", customerSettings, googleAuthenticatorService, customerRegistrationService, customerService, localizationService, notificationService, workContext, _workContext);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> RegisterGoogleAuthenticator(AuthModel model)
    {
		_logger.LogDebug(">>> Entering RegisterGoogleAuthenticator(model={Model})", model);
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("RegisterGoogleAuthenticator(model={Model}): currentCustomer → {CurrentCustomer}", model, currentCustomer);
        var isValidToken = _googleAuthenticatorService.ValidateTwoFactorToken(model.SecretKey, model.Code);
		_logger.LogDebug("RegisterGoogleAuthenticator(model={Model}): isValidToken → {IsValidToken}", model, isValidToken);
        if (isValidToken)
        {
            //try to find config with current customer and update
            if (_googleAuthenticatorService.IsRegisteredCustomer(currentCustomer.Email))
            {
                await _googleAuthenticatorService.UpdateGoogleAuthenticatorAccountAsync(currentCustomer.Email, model.SecretKey);
            }
            else
            {
                await _googleAuthenticatorService.AddGoogleAuthenticatorAccountAsync(currentCustomer.Email, model.SecretKey);
            }
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Successful"));
        }
        else
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Unsuccessful"));
		_logger.LogDebug("<<< Exiting RegisterGoogleAuthenticator(model={Model})", model);
            return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_MULTI_FACTOR_AUTHENTICATION_PROVIDER_CONFIG, new { providerSysName = GoogleAuthenticatorDefaults.SystemName });
        }

        return RedirectToRoute(NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyGoogleAuthenticator(TokenModel model)
    {
		_logger.LogDebug(">>> Entering VerifyGoogleAuthenticator(model={Model})", model);
        var customerMultiFactorAuthenticationInfo = await HttpContext.Session.GetAsync<CustomerMultiFactorAuthenticationInfo>(NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo);
        var username = customerMultiFactorAuthenticationInfo.UserName;
		_logger.LogDebug("VerifyGoogleAuthenticator(model={Model}): username → {Username}", model, username);
		_logger.LogDebug("VerifyGoogleAuthenticator(model={Model}): customerMultiFactorAuthenticationInfo → {CustomerMultiFactorAuthenticationInfo}", model, customerMultiFactorAuthenticationInfo);
        var returnUrl = customerMultiFactorAuthenticationInfo.ReturnUrl;
        var isPersist = customerMultiFactorAuthenticationInfo.RememberMe;

        var customer = _customerSettings.UsernamesEnabled ? await _customerService.GetCustomerByUsernameAsync(username) : await _customerService.GetCustomerByEmailAsync(username);
		_logger.LogDebug("VerifyGoogleAuthenticator(model={Model}): customer → {Customer}", model, customer);
        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.LOGIN);

        var record = _googleAuthenticatorService.GetConfigurationByCustomerEmail(customer.Email);
		_logger.LogDebug("VerifyGoogleAuthenticator(model={Model}): record → {Record}", model, record);
        if (record != null)
        {
            var isValidToken = _googleAuthenticatorService.ValidateTwoFactorToken(record.SecretKey, model.Token);
			_logger.LogDebug("VerifyGoogleAuthenticator(model={Model}): isValidToken → {IsValidToken}", model, isValidToken);
            if (isValidToken)
            {
                await HttpContext.Session.SetAsync<CustomerMultiFactorAuthenticationInfo>(NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo, null);

                return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, isPersist);
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Unsuccessful"));
            }
        }
        else
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.MultiFactorAuth.GoogleAuthenticator.Record.Notfound"));
        }

		_logger.LogDebug("<<< Exiting VerifyGoogleAuthenticator(model={Model})", model);
        return RedirectToRoute(NopRouteNames.Standard.MULTIFACTOR_VERIFICATION);
    }

    #endregion
}