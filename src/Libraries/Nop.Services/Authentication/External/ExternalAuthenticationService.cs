using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Authentication.External;

/// <summary>
/// Represents external authentication service implementation
/// </summary>
public partial class ExternalAuthenticationService : IExternalAuthenticationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ExternalAuthenticationService(CustomerSettings customerSettings,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        IActionContextAccessor actionContextAccessor,
        IAuthenticationPluginManager authenticationPluginManager,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings)
    {
		_logger.LogDebug(">>> Entering ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerSettings → {_customerSettings}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _customerSettings);
        _externalAuthenticationSettings = externalAuthenticationSettings;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _externalAuthenticationSettings → {_externalAuthenticationSettings}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _externalAuthenticationSettings);
        _actionContextAccessor = actionContextAccessor;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _actionContextAccessor → {_actionContextAccessor}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _actionContextAccessor);
        _authenticationPluginManager = authenticationPluginManager;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _authenticationPluginManager → {_authenticationPluginManager}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _authenticationPluginManager);
        _customerRegistrationService = customerRegistrationService;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerRegistrationService → {_customerRegistrationService}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _customerRegistrationService);
        _customerService = customerService;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerService → {_customerService}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _eventPublisher → {_eventPublisher}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _eventPublisher);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _genericAttributeService → {_genericAttributeService}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _genericAttributeService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _httpContextAccessor → {_httpContextAccessor}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _httpContextAccessor);
        _localizationService = localizationService;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationService → {_localizationService}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _localizationService);
        _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _externalAuthenticationRecordRepository → {_externalAuthenticationRecordRepository}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _externalAuthenticationRecordRepository);
        _storeContext = storeContext;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _storeContext → {_storeContext}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _storeContext);
        _urlHelperFactory = urlHelperFactory;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _urlHelperFactory → {_urlHelperFactory}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _urlHelperFactory);
        _workContext = workContext;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workContext → {_workContext}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workflowMessageService → {_workflowMessageService}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("<<< Exiting ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings);
		_logger.LogDebug("ExternalAuthenticationService(customerSettings={CustomerSettings},externalAuthenticationSettings={ExternalAuthenticationSettings},actionContextAccessor={ActionContextAccessor},authenticationPluginManager={AuthenticationPluginManager},customerRegistrationService={CustomerRegistrationService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},externalAuthenticationRecordRepository={ExternalAuthenticationRecordRepository},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", customerSettings, externalAuthenticationSettings, actionContextAccessor, authenticationPluginManager, customerRegistrationService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, localizationService, externalAuthenticationRecordRepository, storeContext, urlHelperFactory, workContext, workflowMessageService, localizationSettings, _localizationSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Authenticate user with existing associated external account
    /// </summary>
    /// <param name="associatedUser">Associated with passed external authentication parameters user</param>
    /// <param name="currentLoggedInUser">Current logged-in user</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of an authentication
    /// </returns>
    protected virtual async Task<IActionResult> AuthenticateExistingUserAsync(Customer associatedUser, Customer currentLoggedInUser, string returnUrl)
    {
        //log in guest user
        if (currentLoggedInUser == null)
            return await _customerRegistrationService.SignInCustomerAsync(associatedUser, returnUrl);

        //account is already assigned to another user
        if (currentLoggedInUser.Id != associatedUser.Id)
            return await ErrorAuthenticationAsync(new[]
            {
                await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.AccountAlreadyAssigned")
            }, returnUrl);

        //or the user try to log in as himself. bit weird
		_logger.LogDebug("<<< Exiting ErrorAuthenticationAsync(errors={Errors},returnUrl={ReturnUrl})", errors, returnUrl);
        return SuccessfulAuthentication(returnUrl);
    }

    /// <summary>
    /// Authenticate current user and associate new external account with user
    /// </summary>
    /// <param name="currentLoggedInUser">Current logged-in user</param>
    /// <param name="parameters">Authentication parameters received from external authentication method</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of an authentication
    /// </returns>
    protected virtual async Task<IActionResult> AuthenticateNewUserAsync(Customer currentLoggedInUser, ExternalAuthenticationParameters parameters, string returnUrl)
    {
        //associate external account with logged-in user
        if (currentLoggedInUser != null)
        {
            await AssociateExternalAccountWithUserAsync(currentLoggedInUser, parameters);

            return SuccessfulAuthentication(returnUrl);
        }

        //or try to register new user
        if (_customerSettings.UserRegistrationType != UserRegistrationType.Disabled)
            return await RegisterNewUserAsync(parameters, returnUrl);

        //registration is disabled
        return await ErrorAuthenticationAsync(new[] { "Registration is disabled" }, returnUrl);
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="parameters">Authentication parameters received from external authentication method</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of an authentication
    /// </returns>
    protected virtual async Task<IActionResult> RegisterNewUserAsync(ExternalAuthenticationParameters parameters, string returnUrl)
    {
		_logger.LogDebug(">>> Entering RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl})", parameters, returnUrl);
        //check whether the specified email has been already registered
        if (await _customerService.GetCustomerByEmailAsync(parameters.Email) != null)
        {
            var alreadyExistsError = string.Format(await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.EmailAlreadyExists"),
                !string.IsNullOrEmpty(parameters.ExternalDisplayIdentifier) ? parameters.ExternalDisplayIdentifier : parameters.ExternalIdentifier);
            return await ErrorAuthenticationAsync(new[] { alreadyExistsError }, returnUrl);
        }

        //registration is approved if validation isn't required
        var registrationIsApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard ||
                                     (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation && !_externalAuthenticationSettings.RequireEmailValidation);

        //create registration request
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl}): customer → {Customer}", parameters, returnUrl, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl}): store → {Store}", parameters, returnUrl, store);
        var registrationRequest = new CustomerRegistrationRequest(customer,
            parameters.Email, parameters.Email,
            CommonHelper.GenerateRandomDigitCode(20),
            PasswordFormat.Hashed,
            store.Id,
            registrationIsApproved);

        //whether registration request has been completed successfully
        var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
		_logger.LogDebug("RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl}): registrationResult → {RegistrationResult}", parameters, returnUrl, registrationResult);
        if (!registrationResult.Success)
            return await ErrorAuthenticationAsync(registrationResult.Errors, returnUrl);

        //allow to save other customer values by consuming this event
        await _eventPublisher.PublishAsync(new CustomerAutoRegisteredByExternalMethodEvent(customer, parameters));

        //raise customer registered event
        await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

        //store owner notifications
        if (_customerSettings.NotifyNewCustomerRegistration)
            await _workflowMessageService.SendCustomerRegisteredStoreOwnerNotificationMessageAsync(customer, _localizationSettings.DefaultAdminLanguageId);

        //associate external account with registered user
        await AssociateExternalAccountWithUserAsync(customer, parameters);

        //authenticate
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl}): currentLanguage → {CurrentLanguage}", parameters, returnUrl, currentLanguage);
        if (registrationIsApproved)
        {
            await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, currentLanguage.Id);

            //raise event       
            await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

			_logger.LogDebug(">>> Entering ErrorAuthenticationAsync(errors={Errors},returnUrl={ReturnUrl})", errors, returnUrl);
            return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, true);
        }

        //registration is succeeded but isn't activated
        if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
        {
            //email validation message
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
            await _workflowMessageService.SendCustomerEmailValidationMessageAsync(customer, currentLanguage.Id);

            return new RedirectToRouteResult(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });
        }

        //registration is succeeded but isn't approved by admin
        if (_customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval)
		_logger.LogDebug("<<< Exiting RegisterNewUserAsync(parameters={Parameters},returnUrl={ReturnUrl})", parameters, returnUrl);
            return new RedirectToRouteResult(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

        return await ErrorAuthenticationAsync(new[] { "Error on registration" }, returnUrl);
    }

    /// <summary>
    /// Add errors that occurred during authentication
    /// </summary>
    /// <param name="errors">Collection of errors</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>Result of an authentication</returns>
    protected virtual async Task<IActionResult> ErrorAuthenticationAsync(IEnumerable<string> errors, string returnUrl)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        if (session != null)
        {
            var existsErrors = (await session.GetAsync<IList<string>>(NopAuthenticationDefaults.ExternalAuthenticationErrorsSessionKey))?.ToList() ?? new List<string>();

            existsErrors.AddRange(errors);

            await session.SetAsync(NopAuthenticationDefaults.ExternalAuthenticationErrorsSessionKey, existsErrors);
        }

        return new RedirectToActionResult("Login", "Customer", !string.IsNullOrEmpty(returnUrl) ? new { ReturnUrl = returnUrl } : null);
    }

    /// <summary>
    /// Redirect the user after successful authentication
    /// </summary>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>Result of an authentication</returns>
    protected virtual IActionResult SuccessfulAuthentication(string returnUrl)
    {
		_logger.LogDebug(">>> Entering SuccessfulAuthentication(returnUrl={ReturnUrl})", returnUrl);
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

		_logger.LogDebug("SuccessfulAuthentication(returnUrl={ReturnUrl}): urlHelper → {UrlHelper}", returnUrl, urlHelper);
        //redirect to the return URL if it's specified
        if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
		_logger.LogDebug("<<< Exiting SuccessfulAuthentication(returnUrl={ReturnUrl})", returnUrl);
            return new RedirectResult(returnUrl);

        return new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
    }

    #endregion

    #region Methods

    #region Authentication

    /// <summary>
    /// Authenticate user by passed parameters
    /// </summary>
    /// <param name="parameters">External authentication parameters</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of an authentication
    /// </returns>
    public virtual async Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null)
    {
		_logger.LogDebug(">>> Entering AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl})", parameters, returnUrl);
        ArgumentNullException.ThrowIfNull(parameters);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl}): customer → {Customer}", parameters, returnUrl, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl}): store → {Store}", parameters, returnUrl, store);
        if (!await _authenticationPluginManager.IsPluginActiveAsync(parameters.ProviderSystemName, customer, store.Id))
            return await ErrorAuthenticationAsync(new[] { "External authentication method cannot be loaded" }, returnUrl);

        //get current logged-in user
        var currentLoggedInUser = await _customerService.IsRegisteredAsync(customer) ? customer : null;
		_logger.LogDebug("AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl}): currentLoggedInUser → {CurrentLoggedInUser}", parameters, returnUrl, currentLoggedInUser);

        //authenticate associated user if already exists
        var associatedUser = await GetUserByExternalAuthenticationParametersAsync(parameters);
		_logger.LogDebug("AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl}): associatedUser → {AssociatedUser}", parameters, returnUrl, associatedUser);
        if (associatedUser != null)
		_logger.LogDebug("<<< Exiting AuthenticateAsync(parameters={Parameters},returnUrl={ReturnUrl})", parameters, returnUrl);
            return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);

        //or associate and authenticate new user
        return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
    }

    #endregion

    /// <summary>
    /// Get the external authentication records by identifier
    /// </summary>
    /// <param name="externalAuthenticationRecordId">External authentication record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByIdAsync(int externalAuthenticationRecordId)
    {
		_logger.LogDebug(">>> Entering GetExternalAuthenticationRecordByIdAsync(externalAuthenticationRecordId={ExternalAuthenticationRecordId})", externalAuthenticationRecordId);
		_logger.LogDebug("<<< Exiting GetExternalAuthenticationRecordByIdAsync(externalAuthenticationRecordId={ExternalAuthenticationRecordId})", externalAuthenticationRecordId);
		_logger.LogDebug(">>> Entering GetUserByExternalAuthenticationParametersAsync(parameters={Parameters})", parameters);
		_logger.LogInformation("GetUserByExternalAuthenticationParametersAsync(parameters)={Parameters}: database query executed in {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetUserByExternalAuthenticationParametersAsync(parameters={Parameters})", parameters);
        return await _externalAuthenticationRecordRepository.GetByIdAsync(externalAuthenticationRecordId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get list of the external authentication records by customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<ExternalAuthenticationRecord>> GetCustomerExternalAuthenticationRecordsAsync(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var associationRecords = _externalAuthenticationRecordRepository.Table.Where(ear => ear.CustomerId == customer.Id);

        return await associationRecords.ToListAsync();
    }

    /// <summary>
    /// Delete the external authentication record
    /// </summary>
    /// <param name="externalAuthenticationRecord">External authentication record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteExternalAuthenticationRecordAsync(ExternalAuthenticationRecord externalAuthenticationRecord)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(externalAuthenticationRecord);

        await _externalAuthenticationRecordRepository.DeleteAsync(externalAuthenticationRecord, false);
    }

    /// <summary>
    /// Get the external authentication record
    /// </summary>
    /// <param name="parameters">External authentication parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
    {
		_logger.LogDebug(">>> Entering GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(parameters={Parameters})", parameters);
        ArgumentNullException.ThrowIfNull(parameters);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
            record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
			_logger.LogInformation("GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(parameters)={Parameters}: database query executed in {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(parameters={Parameters})", parameters);
        return associationRecord;
    }

    /// <summary>
    /// Associate external account with customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="parameters">External authentication parameters</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AssociateExternalAccountWithUserAsync(Customer customer, ExternalAuthenticationParameters parameters)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var externalAuthenticationRecord = new ExternalAuthenticationRecord
        {
            CustomerId = customer.Id,
            Email = parameters.Email,
            ExternalIdentifier = parameters.ExternalIdentifier,
            ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
            OAuthAccessToken = parameters.AccessToken,
            ProviderSystemName = parameters.ProviderSystemName
        };

        await _externalAuthenticationRecordRepository.InsertAsync(externalAuthenticationRecord, false);
    }

    /// <summary>
    /// Get the particular user with specified parameters
    /// </summary>
    /// <param name="parameters">External authentication parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> GetUserByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
            record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
			_logger.LogInformation("GetUserByExternalAuthenticationParametersAsync(parameters)={Parameters}: database query executed in {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);
        if (associationRecord == null)
            return null;

        return await _customerService.GetCustomerByIdAsync(associationRecord.CustomerId);
    }

    /// <summary>
    /// Remove the association
    /// </summary>
    /// <param name="parameters">External authentication parameters</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveAssociationAsync(ExternalAuthenticationParameters parameters)
    {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug(">>> Entering RemoveAssociationAsync(parameters={Parameters})", parameters);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(parameters);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
            record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
			_logger.LogInformation("RemoveAssociationAsync(parameters)={Parameters}: database query executed in {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);

        if (associationRecord != null)
            await _externalAuthenticationRecordRepository.DeleteAsync(associationRecord, false);
			_logger.LogDebug("<<< Exiting RemoveAssociationAsync(parameters={Parameters})", parameters);
			_logger.LogInformation("RemoveAssociationAsync(parameters)={Parameters}: external service call took {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);
    }


    #endregion
}