using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class EmailAccountController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private const char SEPARATOR = '-';

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IEmailAccountModelFactory _emailAccountModelFactory;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEmailSender _emailSender;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public EmailAccountController(EmailAccountSettings emailAccountSettings,
        ICustomerActivityService customerActivityService,
        IEmailAccountModelFactory emailAccountModelFactory,
        IEmailAccountService emailAccountService,
        IEmailSender emailSender,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper})", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper);
        _emailAccountSettings = emailAccountSettings;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _emailAccountSettings → {_emailAccountSettings}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _emailAccountSettings);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _customerActivityService → {_customerActivityService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _customerActivityService);
        _emailAccountModelFactory = emailAccountModelFactory;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _emailAccountModelFactory → {_emailAccountModelFactory}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _emailAccountModelFactory);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _emailAccountService → {_emailAccountService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _emailAccountService);
        _emailSender = emailSender;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _emailSender → {_emailSender}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _emailSender);
        _localizationService = localizationService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _localizationService → {_localizationService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _localizationService);
        _fileProvider = fileProvider;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _fileProvider → {_fileProvider}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _fileProvider);
        _notificationService = notificationService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _notificationService → {_notificationService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _permissionService → {_permissionService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _settingService → {_settingService}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _storeContext → {_storeContext}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _storeContext);
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper})", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper);
		_logger.LogDebug("EmailAccountController(emailAccountSettings={EmailAccountSettings},customerActivityService={CustomerActivityService},emailAccountModelFactory={EmailAccountModelFactory},emailAccountService={EmailAccountService},emailSender={EmailSender},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _webHelper → {_webHelper}", emailAccountSettings, customerActivityService, emailAccountModelFactory, emailAccountService, emailSender, localizationService, fileProvider, notificationService, permissionService, settingService, storeContext, webHelper, _webHelper);
    }

    #endregion

    #region Utilities

    protected virtual async Task<string> PrepareOAuthUrlAsync(EmailAccount emailAccount)
    {
		_logger.LogDebug(">>> Entering PrepareOAuthUrlAsync(emailAccount={EmailAccount})", emailAccount);
        if (string.IsNullOrEmpty(emailAccount.ClientSecret) || string.IsNullOrEmpty(emailAccount.ClientId))
        {
            ModelState.AddModelError(nameof(EmailAccountModel.ClientSecret), await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.Required"));
            return string.Empty;
        }

        var tokenFilePath = _fileProvider.MapPath(NopMessageDefaults.GmailAuthStorePath);
		_logger.LogDebug("PrepareOAuthUrlAsync(emailAccount={EmailAccount}): tokenFilePath → {TokenFilePath}", emailAccount, tokenFilePath);
        var credentialRoot = _fileProvider.Combine(tokenFilePath, emailAccount.Email);
		_logger.LogDebug("PrepareOAuthUrlAsync(emailAccount={EmailAccount}): credentialRoot → {CredentialRoot}", emailAccount, credentialRoot);

        var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = emailAccount.ClientId,
                ClientSecret = emailAccount.ClientSecret
            },
            Scopes = NopMessageDefaults.GmailScopes,
            DataStore = new FileDataStore(credentialRoot, true),
            UserDefinedQueryParams = new Dictionary<string, string> { ["emailAccountId"] = emailAccount.Id.ToString() }
        });

        var redirectUri = Url.Action(nameof(AuthReturn), null, null, _webHelper.GetCurrentRequestProtocol());
		_logger.LogDebug("PrepareOAuthUrlAsync(emailAccount={EmailAccount}): redirectUri → {RedirectUri}", emailAccount, redirectUri);
        var authCode = new AuthorizationCodeWebApp(codeFlow, redirectUri, $"{emailAccount.Id}{SEPARATOR}");
		_logger.LogDebug("PrepareOAuthUrlAsync(emailAccount={EmailAccount}): authCode → {AuthCode}", emailAccount, authCode);
        var authResult = await authCode.AuthorizeAsync(emailAccount.Email, CancellationToken.None);
		_logger.LogDebug("PrepareOAuthUrlAsync(emailAccount={EmailAccount}): authResult → {AuthResult}", emailAccount, authResult);

		_logger.LogDebug("<<< Exiting PrepareOAuthUrlAsync(emailAccount={EmailAccount})", emailAccount);
        return authResult.RedirectUri?.ToString();
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountSearchModelAsync(new EmailAccountSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> List(EmailAccountSearchModel searchModel)
    {
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> MarkAsDefaultEmail(int id)
    {
		_logger.LogDebug(">>> Entering MarkAsDefaultEmail(id={Id})", id);
        var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
        if (defaultEmailAccount == null)
            return RedirectToAction("List");

        _emailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
		_logger.LogDebug("MarkAsDefaultEmail(id={Id}): _emailAccountSettings.DefaultEmailAccountId → {Arg1}", id, _emailAccountSettings.DefaultEmailAccountId);
        await _settingService.SaveSettingAsync(_emailAccountSettings);

		_logger.LogDebug("<<< Exiting MarkAsDefaultEmail(id={Id})", id);
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(new EmailAccountModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Create(EmailAccountModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var emailAccount = model.ToEntity<EmailAccount>();

            //set password manually
            emailAccount.Password = model.Password;
            await _emailAccountService.InsertEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): emailAccount → {EmailAccount}", id, emailAccount);
        if (emailAccount == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(null, emailAccount);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

        if (emailAccount.EmailAuthenticationMethod == EmailAuthenticationMethod.GmailOAuth2)
            model.AuthUrl = await PrepareOAuthUrlAsync(emailAccount);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Edit(EmailAccountModel model, bool continueEditing)
    {
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
        if (emailAccount == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            emailAccount = model.ToEntity(emailAccount);
            await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);

        if (emailAccount.EmailAuthenticationMethod == EmailAuthenticationMethod.GmailOAuth2)
            model.AuthUrl = await PrepareOAuthUrlAsync(emailAccount);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("changesecret")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> ChangeSecret(EmailAccountModel model)
    {
		_logger.LogDebug(">>> Entering ChangeSecret(model={Model})", model);
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
		_logger.LogDebug("ChangeSecret(model={Model}): emailAccount → {EmailAccount}", model, emailAccount);
        if (emailAccount == null)
            return RedirectToAction("List");

        //do not validate model
        emailAccount.ClientSecret = model.ClientSecret;
		_logger.LogDebug("ChangeSecret(model={Model}): emailAccount.ClientSecret → {Arg1}", model, emailAccount.ClientSecret);
        await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.ClientSecret.ClientSecretChanged"));

		_logger.LogDebug("<<< Exiting ChangeSecret(model={Model})", model);
        return RedirectToAction("Edit", new { id = emailAccount.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("changepassword")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> ChangePassword(EmailAccountModel model)
    {
		_logger.LogDebug(">>> Entering ChangePassword(model={Model})", model);
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
		_logger.LogDebug("ChangePassword(model={Model}): emailAccount → {EmailAccount}", model, emailAccount);
        if (emailAccount == null)
            return RedirectToAction("List");

        //do not validate model
        emailAccount.Password = model.Password;
		_logger.LogDebug("ChangePassword(model={Model}): emailAccount.Password → {Arg1}", model, emailAccount.Password);
        await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged"));

		_logger.LogDebug("<<< Exiting ChangePassword(model={Model})", model);
        return RedirectToAction("Edit", new { id = emailAccount.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("sendtestemail")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> SendTestEmail(EmailAccountModel model)
    {
		_logger.LogDebug(">>> Entering SendTestEmail(model={Model})", model);
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
		_logger.LogDebug("SendTestEmail(model={Model}): emailAccount → {EmailAccount}", model, emailAccount);
        if (emailAccount == null)
            return RedirectToAction("List");

        if (!CommonHelper.IsValidEmail(model.SendTestEmailTo))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            return View(await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true));
        }

        try
        {
            if (string.IsNullOrWhiteSpace(model.SendTestEmailTo))
				_logger.LogWarning("Throwing NopException from SendTestEmail(model={Model})", model);
                throw new NopException("Enter test email address");
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("SendTestEmail(model={Model}): store → {Store}", model, store);
            var subject = store.Name + ". Testing email functionality.";
			_logger.LogDebug("SendTestEmail(model={Model}): subject → {Subject}", model, subject);
            var body = "Email works fine.";
			_logger.LogDebug("SendTestEmail(model={Model}): body → {Body}", model, body);
            await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, model.SendTestEmailTo, null);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.SendTestEmail.Success"));

            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SendTestEmail(model={Model}): {Arg1}", model, exc.getMessage());
            _notificationService.ErrorNotification(exc.Message);
        }

        //prepare model
        model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(model, emailAccount, true);
		_logger.LogDebug("SendTestEmail(model={Model}): model → {Model1}", model, model);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting SendTestEmail(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an email account with the specified id
        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): emailAccount → {EmailAccount}", id, emailAccount);
        if (emailAccount == null)
            return RedirectToAction("List");

        try
        {
            await _emailAccountService.DeleteEmailAccountAsync(emailAccount);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteEmailAccount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteEmailAccount"), emailAccount.Id), emailAccount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = emailAccount.Id });
        }
    }

    public virtual async Task<IActionResult> AuthReturn(AuthorizationCodeResponseUrl authorizationCode)
    {
		_logger.LogDebug(">>> Entering AuthReturn(authorizationCode=<redacted>)");
        if (string.IsNullOrEmpty(authorizationCode.State))
            return RedirectToAction(nameof(List));

        var strAccountId = authorizationCode.State.Split(SEPARATOR).FirstOrDefault();
		_logger.LogDebug("AuthReturn(authorizationCode=<redacted>): strAccountId → {StrAccountId}", strAccountId);

        if (!int.TryParse(strAccountId, out var accountId) ||
            await _emailAccountService.GetEmailAccountByIdAsync(accountId) is not EmailAccount emailAccount)
        {
            _notificationService.ErrorNotification("Email account could not be loaded");
            return RedirectToAction(nameof(List));
        }

        if (!string.IsNullOrEmpty(authorizationCode.Error))
        {
            _notificationService.ErrorNotification(authorizationCode.Error);
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });
        }

        if (string.IsNullOrEmpty(authorizationCode?.Code))
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });

        var tokenFilePath = _fileProvider.MapPath(NopMessageDefaults.GmailAuthStorePath);
		_logger.LogDebug("AuthReturn(authorizationCode=<redacted>): tokenFilePath → {TokenFilePath}", tokenFilePath);
        var credentialRoot = _fileProvider.Combine(tokenFilePath, emailAccount.Email);
		_logger.LogDebug("AuthReturn(authorizationCode=<redacted>): credentialRoot → {CredentialRoot}", credentialRoot);

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = emailAccount.ClientId,
                ClientSecret = emailAccount.ClientSecret
            },
            Scopes = NopMessageDefaults.GmailScopes,
            DataStore = new FileDataStore(credentialRoot, true)
        });

        var redirectUri = Url.Action(nameof(AuthReturn), null, null, _webHelper.GetCurrentRequestProtocol());
		_logger.LogDebug("AuthReturn(authorizationCode=<redacted>): redirectUri → {RedirectUri}", redirectUri);
        var tokenResponse = await flow.ExchangeCodeForTokenAsync(emailAccount.Email, authorizationCode.Code, redirectUri, CancellationToken.None);
		_logger.LogDebug("AuthReturn(authorizationCode=<redacted>): tokenResponse → {TokenResponse}", tokenResponse);

        if (tokenResponse is null || tokenResponse.IsStale)
            return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });

        _notificationService.SuccessNotification("The token was successfully retrieved from the server");

		_logger.LogDebug("<<< Exiting AuthReturn(authorizationCode=<redacted>)");
        return RedirectToAction(nameof(Edit), new { id = emailAccount.Id });
    }

    #endregion
}