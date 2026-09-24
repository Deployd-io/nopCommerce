using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers;

/// <summary>
/// Customer registration service
/// </summary>
public partial class CustomerRegistrationService : ICustomerRegistrationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly RewardPointsSettings _rewardPointsSettings;

    #endregion

    #region Ctor

    public CustomerRegistrationService(CustomerSettings customerSettings,
        IActionContextAccessor actionContextAccessor,
        IAuthenticationService authenticationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEncryptionService encryptionService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IRewardPointService rewardPointService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        RewardPointsSettings rewardPointsSettings)
    {
		_logger.LogError(">>> Entering CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings})", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings);
        _customerSettings = customerSettings;
        _actionContextAccessor = actionContextAccessor;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _actionContextAccessor → {_actionContextAccessor}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _actionContextAccessor);
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _customerSettings → {_customerSettings}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _customerSettings);
    _logger.LogWarning("<<< Exiting CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings})", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings);
        _authenticationService = authenticationService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _authenticationService → {_authenticationService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _authenticationService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _customerActivityService → {_customerActivityService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _customerService → {_customerService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _customerService);
        _encryptionService = encryptionService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _encryptionService → {_encryptionService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _encryptionService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _eventPublisher → {_eventPublisher}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _eventPublisher);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _genericAttributeService → {_genericAttributeService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _localizationService → {_localizationService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _localizationService);
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _multiFactorAuthenticationPluginManager → {_multiFactorAuthenticationPluginManager}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _multiFactorAuthenticationPluginManager);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _newsLetterSubscriptionService);
        _notificationService = notificationService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _notificationService → {_notificationService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _permissionService → {_permissionService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _permissionService);
        _rewardPointService = rewardPointService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _rewardPointService → {_rewardPointService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _rewardPointService);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _shoppingCartService → {_shoppingCartService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _shoppingCartService);
        _storeContext = storeContext;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _storeContext → {_storeContext}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _storeContext);
        _urlHelperFactory = urlHelperFactory;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _urlHelperFactory → {_urlHelperFactory}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _urlHelperFactory);
        _workContext = workContext;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _workContext → {_workContext}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("CustomerRegistrationService(customerSettings={CustomerSettings},actionContextAccessor={ActionContextAccessor},authenticationService={AuthenticationService},customerActivityService={CustomerActivityService},customerService={CustomerService},encryptionService={EncryptionService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},newsLetterSubscriptionService={NewsLetterSubscriptionService},notificationService={NotificationService},permissionService={PermissionService},rewardPointService={RewardPointService},shoppingCartService=<redacted>,storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},workContext={WorkContext},workflowMessageService={WorkflowMessageService},rewardPointsSettings={RewardPointsSettings}): _workflowMessageService → {_workflowMessageService}", customerSettings, actionContextAccessor, authenticationService, customerActivityService, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService, permissionService, rewardPointService, storeContext, urlHelperFactory, workContext, workflowMessageService, rewardPointsSettings, _workflowMessageService);
        _rewardPointsSettings = rewardPointsSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether the entered password matches with a saved one
    /// </summary>
    /// <param name="customerPassword">Customer password</param>
    /// <param name="enteredPassword">The entered password</param>
    /// <returns>True if passwords match; otherwise false</returns>
    protected bool PasswordsMatch(CustomerPassword customerPassword, string enteredPassword)
    {
		_logger.LogError(">>> Entering PasswordsMatch(customerPassword=<redacted>,enteredPassword=<redacted>)");
        if (customerPassword == null || string.IsNullOrEmpty(enteredPassword))
            return false;

        var savedPassword = string.Empty;
		_logger.LogError("PasswordsMatch(customerPassword=<redacted>,enteredPassword=<redacted>): savedPassword → {SavedPassword}", savedPassword);
        switch (customerPassword.PasswordFormat)
        {
            case PasswordFormat.Clear:
                savedPassword = enteredPassword;
                break;
            case PasswordFormat.Encrypted:
                savedPassword = _encryptionService.EncryptText(enteredPassword);
                break;
            case PasswordFormat.Hashed:
                savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, customerPassword.PasswordSalt, _customerSettings.HashedPasswordFormat);
                break;
        }

        if (customerPassword.Password == null)
            return false;

        return customerPassword.Password.Equals(savedPassword);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Validate customer
    /// </summary>
    /// <param name="usernameOrEmail">Username or email</param>
    /// <param name="password">Password</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<CustomerLoginResults> ValidateCustomerAsync(string usernameOrEmail, string password)
    {
		_logger.LogError(">>> Entering ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>)", usernameOrEmail);
        var customer = _customerSettings.UsernamesEnabled ?
            await _customerService.GetCustomerByUsernameAsync(usernameOrEmail) :
            await _customerService.GetCustomerByEmailAsync(usernameOrEmail);

        if (customer == null)
            return CustomerLoginResults.CustomerNotExist;
        if (customer.Deleted)
            return CustomerLoginResults.Deleted;
        if (!customer.Active)
            return CustomerLoginResults.NotActive;
        //only registered can login
        if (!await _customerService.IsRegisteredAsync(customer))
            return CustomerLoginResults.NotRegistered;
        //check whether a customer is locked out
        if (customer.CannotLoginUntilDateUtc.HasValue && customer.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
            return CustomerLoginResults.LockedOut;

        if (!PasswordsMatch(await _customerService.GetCurrentPasswordAsync(customer.Id), password))
        {
            //wrong password
            customer.FailedLoginAttempts++;
            if (_customerSettings.FailedPasswordAllowedAttempts > 0 &&
                customer.FailedLoginAttempts >= _customerSettings.FailedPasswordAllowedAttempts)
            {
                //lock out
                customer.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(_customerSettings.FailedPasswordLockoutMinutes);
                //reset the counter
                customer.FailedLoginAttempts = 0;
				_logger.LogDebug("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): customer.FailedLoginAttempts → {Arg2}", usernameOrEmail, customer.FailedLoginAttempts);
            }

            await _customerService.UpdateCustomerAsync(customer);

            return CustomerLoginResults.WrongPassword;
        }

        var selectedProvider = await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, customer)
            ? await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute)
            : null;
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): store → {Store}", usernameOrEmail, store);
        var methodIsActive = await _multiFactorAuthenticationPluginManager.IsPluginActiveAsync(selectedProvider, customer, store.Id);
		_logger.LogError("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): methodIsActive → {MethodIsActive}", usernameOrEmail, methodIsActive);
        if (methodIsActive)
            return CustomerLoginResults.MultiFactorAuthenticationRequired;

        if (!string.IsNullOrEmpty(selectedProvider))
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("MultiFactorAuthentication.Notification.SelectedMethodIsNotActive"));

        //update login details
        customer.FailedLoginAttempts = 0;
        customer.CannotLoginUntilDateUtc = null;
		_logger.LogDebug("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): customer.CannotLoginUntilDateUtc → {Arg2}", usernameOrEmail, customer.CannotLoginUntilDateUtc);
		_logger.LogDebug("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): customer.RequireReLogin → {Arg2}", usernameOrEmail, customer.RequireReLogin);
		_logger.LogDebug("ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>): customer.LastLoginDateUtc → {Arg2}", usernameOrEmail, customer.LastLoginDateUtc);
        customer.RequireReLogin = false;
        customer.LastLoginDateUtc = DateTime.UtcNow;
        await _customerService.UpdateCustomerAsync(customer);

		_logger.LogWarning("<<< Exiting ValidateCustomerAsync(usernameOrEmail={UsernameOrEmail},password=<redacted>)", usernameOrEmail);
        return CustomerLoginResults.Successful;
    }

    /// <summary>
    /// Register customer
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<CustomerRegistrationResult> RegisterCustomerAsync(CustomerRegistrationRequest request)
    {
		_logger.LogError(">>> Entering RegisterCustomerAsync(request={Request})", request);
        ArgumentNullException.ThrowIfNull(request);

        if (request.Customer == null)
			_logger.LogError("Throwing ArgumentException from RegisterCustomerAsync(request={Request})", request);
            throw new ArgumentException("Can't load current customer");

        var result = new CustomerRegistrationResult();
		_logger.LogError("RegisterCustomerAsync(request={Request}): result → {Result}", request, result);
        if (request.Customer.IsSearchEngineAccount())
        {
            result.AddError("Search engine can't be registered");
            return result;
        }

        if (request.Customer.IsBackgroundTaskAccount())
        {
            result.AddError("Background task account can't be registered");
            return result;
        }

        if (await _customerService.IsRegisteredAsync(request.Customer))
        {
            result.AddError("Current customer is already registered");
            return result;
        }

        if (string.IsNullOrEmpty(request.Email))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailIsNotProvided"));
            return result;
        }

        if (!CommonHelper.IsValidEmail(request.Email))
        {
            result.AddError(await _localizationService.GetResourceAsync("Common.WrongEmail"));
            return result;
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.PasswordIsNotProvided"));
            return result;
        }

        if (_customerSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameIsNotProvided"));
            return result;
        }

        //validate unique user
        if (await _customerService.GetCustomerByEmailAsync(request.Email) != null)
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.EmailAlreadyExists"));
            return result;
        }

        if (_customerSettings.UsernamesEnabled && await _customerService.GetCustomerByUsernameAsync(request.Username) != null)
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameAlreadyExists"));
            return result;
        }

        //at this point request is valid
        request.Customer.Username = request.Username;
        request.Customer.Email = request.Email;

        var customerPassword = new CustomerPassword
        {
            CustomerId = request.Customer.Id,
            PasswordFormat = request.PasswordFormat,
            CreatedOnUtc = DateTime.UtcNow
        };
        switch (request.PasswordFormat)
        {
            case PasswordFormat.Clear:
                customerPassword.Password = request.Password;
				_logger.LogDebug("RegisterCustomerAsync(request={Request}): customerPassword.Password → {Arg1}", request, customerPassword.Password);
                break;
            case PasswordFormat.Encrypted:
                customerPassword.Password = _encryptionService.EncryptText(request.Password);
                break;
            case PasswordFormat.Hashed:
                var saltKey = _encryptionService.CreateSaltKey(NopCustomerServicesDefaults.PasswordSaltKeySize);
				_logger.LogError("RegisterCustomerAsync(request={Request}): saltKey → {SaltKey}", request, saltKey);
				_logger.LogDebug("RegisterCustomerAsync(request={Request}): customerPassword.PasswordSalt → {Arg1}", request, customerPassword.PasswordSalt);
                customerPassword.PasswordSalt = saltKey;
                customerPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                break;
        }

        await _customerService.InsertCustomerPasswordAsync(customerPassword);

        request.Customer.Active = request.IsApproved;

        //add to 'Registered' role
        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName) ?? throw new NopException("'Registered' role could not be loaded");
		_logger.LogError("RegisterCustomerAsync(request={Request}): registeredRole → {RegisteredRole}", request, registeredRole);

        await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = request.Customer.Id, CustomerRoleId = registeredRole.Id });

        //remove from 'Guests' role            
        if (await _customerService.IsGuestAsync(request.Customer))
        {
            var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
			_logger.LogError("RegisterCustomerAsync(request={Request}): guestRole → {GuestRole}", request, guestRole);
            await _customerService.RemoveCustomerRoleMappingAsync(request.Customer, guestRole);
        }

        //add reward points for customer registration (if enabled)
        if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
        {
            var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
            await _rewardPointService.AddRewardPointsHistoryEntryAsync(request.Customer, _rewardPointsSettings.PointsForRegistration,
                request.StoreId, await _localizationService.GetResourceAsync("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
        }

        await _customerService.UpdateCustomerAsync(request.Customer);

		_logger.LogWarning("<<< Exiting RegisterCustomerAsync(request={Request})", request);
        return result;
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
		_logger.LogError(">>> Entering ChangePasswordAsync(request={Request})", request);
        ArgumentNullException.ThrowIfNull(request);

        var result = new ChangePasswordResult();
		_logger.LogError("ChangePasswordAsync(request={Request}): result → {Result}", request, result);
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailIsNotProvided"));
            return result;
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordIsNotProvided"));
            return result;
        }

        var customer = await _customerService.GetCustomerByEmailAsync(request.Email);
		_logger.LogError("ChangePasswordAsync(request={Request}): customer → {Customer}", request, customer);
        if (customer == null)
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailNotFound"));
            return result;
        }

        //request isn't valid
        if (request.ValidateRequest && !PasswordsMatch(await _customerService.GetCurrentPasswordAsync(customer.Id), request.OldPassword))
        {
            result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
            return result;
        }

        //check for duplicates
        if (_customerSettings.UnduplicatedPasswordsNumber > 0)
        {
            //get some of previous passwords
            var previousPasswords = await _customerService.GetCustomerPasswordsAsync(customer.Id, passwordsToReturn: _customerSettings.UnduplicatedPasswordsNumber);
			_logger.LogError("ChangePasswordAsync(request={Request}): previousPasswords → {PreviousPasswords}", request, previousPasswords);

            var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
			_logger.LogError("ChangePasswordAsync(request={Request}): newPasswordMatchesWithPrevious → {NewPasswordMatchesWithPrevious}", request, newPasswordMatchesWithPrevious);
            if (newPasswordMatchesWithPrevious)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                return result;
            }
        }

        //at this point request is valid
        var customerPassword = new CustomerPassword
        {
            CustomerId = customer.Id,
            PasswordFormat = request.NewPasswordFormat,
            CreatedOnUtc = DateTime.UtcNow
        };
        switch (request.NewPasswordFormat)
        {
            case PasswordFormat.Clear:
                customerPassword.Password = request.NewPassword;
				_logger.LogDebug("ChangePasswordAsync(request={Request}): customerPassword.Password → {Arg1}", request, customerPassword.Password);
                break;
            case PasswordFormat.Encrypted:
                customerPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                break;
            case PasswordFormat.Hashed:
                var saltKey = _encryptionService.CreateSaltKey(NopCustomerServicesDefaults.PasswordSaltKeySize);
				_logger.LogError("ChangePasswordAsync(request={Request}): saltKey → {SaltKey}", request, saltKey);
				_logger.LogDebug("ChangePasswordAsync(request={Request}): customerPassword.PasswordSalt → {Arg1}", request, customerPassword.PasswordSalt);
                customerPassword.PasswordSalt = saltKey;
                customerPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                    request.HashedPasswordFormat ?? _customerSettings.HashedPasswordFormat);
                break;
        }

        await _customerService.InsertCustomerPasswordAsync(customerPassword);

        if (customer.MustChangePassword)
        {
            customer.MustChangePassword = false;
			_logger.LogDebug("ChangePasswordAsync(request={Request}): customer.MustChangePassword → {Arg1}", request, customer.MustChangePassword);
            await _customerService.UpdateCustomerAsync(customer);
        }

        //publish event
        await _eventPublisher.PublishAsync(new CustomerPasswordChangedEvent(customerPassword));

		_logger.LogWarning("<<< Exiting ChangePasswordAsync(request={Request})", request);
        return result;
    }

    /// <summary>
    /// Login passed user
    /// </summary>
    /// <param name="customer">User to login</param>
    /// <param name="returnUrl">URL to which the user will return after authentication</param>
    /// <param name="isPersist">Is remember me</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of an authentication
    /// </returns>
    public virtual async Task<IActionResult> SignInCustomerAsync(Customer customer, string returnUrl, bool isPersist = false)
    {
		_logger.LogError(">>> Entering SignInCustomerAsync(customer={Customer},returnUrl={ReturnUrl},isPersist={IsPersist})", customer, returnUrl, isPersist);
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("SignInCustomerAsync(customer={Customer},returnUrl={ReturnUrl},isPersist={IsPersist}): currentCustomer → {CurrentCustomer}", customer, returnUrl, isPersist, currentCustomer);
        if (currentCustomer?.Id != customer.Id)
        {
            if (currentCustomer.AffiliateId != 0)
            {
                customer.AffiliateId = currentCustomer.AffiliateId;
				_logger.LogDebug("SignInCustomerAsync(customer={Customer},returnUrl={ReturnUrl},isPersist={IsPersist}): customer.AffiliateId → {Arg3}", customer, returnUrl, isPersist, customer.AffiliateId);
                await _customerService.UpdateCustomerAsync(customer);
            }
            //migrate shopping cart
            await _shoppingCartService.MigrateShoppingCartAsync(currentCustomer, customer, true);

            await _workContext.SetCurrentCustomerAsync(customer);
        }

        //sign in new customer
        await _authenticationService.SignInAsync(customer, isPersist);

        //raise event
        var guestCustomer = await _customerService.IsGuestAsync(currentCustomer) && currentCustomer?.Id != customer.Id ? currentCustomer : null;
		_logger.LogError("SignInCustomerAsync(customer={Customer},returnUrl={ReturnUrl},isPersist={IsPersist}): guestCustomer → {GuestCustomer}", customer, returnUrl, isPersist, guestCustomer);
        await _eventPublisher.PublishAsync(new CustomerLoggedinEvent(customer, guestCustomer));

        //activity log
        await _customerActivityService.InsertActivityAsync(customer, "PublicStore.SuccessfulLogin",
            await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login.Success"), customer);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
		_logger.LogError("SignInCustomerAsync(customer={Customer},returnUrl={ReturnUrl},isPersist={IsPersist}): urlHelper → {UrlHelper}", customer, returnUrl, isPersist, urlHelper);

        //redirect to the return URL if it's specified
        if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
            return new RedirectResult(returnUrl);

        return new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
    }

    /// <summary>
    /// Sets a user email
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="newEmail">New email</param>
    /// <param name="requireValidation">Require validation of new email address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetEmailAsync(Customer customer, string newEmail, bool requireValidation)
    {
		_logger.LogError(">>> Entering SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation})", customer, newEmail, requireValidation);
        ArgumentNullException.ThrowIfNull(customer);

        if (newEmail == null)
			_logger.LogError("Throwing NopException from SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation})", customer, newEmail, requireValidation);
            throw new NopException("Email cannot be null");

        newEmail = newEmail.Trim();
		_logger.LogError("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): newEmail → {NewEmail1}", customer, newEmail, requireValidation, newEmail);
        var oldEmail = customer.Email;

        if (!CommonHelper.IsValidEmail(newEmail))
            throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.NewEmailIsNotValid"));

        if (newEmail.Length > 100)
            throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailTooLong"));

        var customer2 = await _customerService.GetCustomerByEmailAsync(newEmail);
		_logger.LogError("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): customer2 → {Customer2}", customer, newEmail, requireValidation, customer2);
        if (customer2 != null && customer.Id != customer2.Id)
            throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailAlreadyExists"));

        if (requireValidation)
        {
            //re-validate email
            customer.EmailToRevalidate = newEmail;
			_logger.LogDebug("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): customer.EmailToRevalidate → {Arg3}", customer, newEmail, requireValidation, customer.EmailToRevalidate);
            await _customerService.UpdateCustomerAsync(customer);

            //email re-validation message
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, Guid.NewGuid().ToString());
            await _workflowMessageService.SendCustomerEmailRevalidationMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);
        }
        else
        {
            customer.Email = newEmail;
			_logger.LogDebug("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): customer.Email → {Arg3}", customer, newEmail, requireValidation, customer.Email);
            await _customerService.UpdateCustomerAsync(customer);

            if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                return;

            //copy active newsletter subscriptions and deactivate the old
            var subscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(oldEmail, isActive: true);
			_logger.LogError("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): subscriptions → {Subscriptions}", customer, newEmail, requireValidation, subscriptions);
            var subscriptionGuid = Guid.NewGuid();
			_logger.LogError("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): subscriptionGuid → {SubscriptionGuid}", customer, newEmail, requireValidation, subscriptionGuid);
            foreach (var subscription in subscriptions)
            {
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new()
                {
                    NewsLetterSubscriptionGuid = subscriptionGuid,
                    Email = newEmail,
                    Active = true,
                    TypeId = subscription.TypeId,
                    StoreId = subscription.StoreId,
                    LanguageId = subscription.LanguageId,
                    CreatedOnUtc = DateTime.UtcNow
                });
				_logger.LogDebug("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): CreatedOnUtc → {CreatedOnUtc}", customer, newEmail, requireValidation, CreatedOnUtc);

                subscription.Active = false;
				_logger.LogDebug("SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation}): subscription.Active → {Arg3}", customer, newEmail, requireValidation, subscription.Active);
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            }
        }
		_logger.LogWarning("<<< Exiting SetEmailAsync(customer={Customer},newEmail={NewEmail},requireValidation={RequireValidation})", customer, newEmail, requireValidation);
    }

    /// <summary>
    /// Sets a customer username
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="newUsername">New Username</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetUsernameAsync(Customer customer, string newUsername)
    {
		_logger.LogError(">>> Entering SetUsernameAsync(customer={Customer},newUsername={NewUsername})", customer, newUsername);
        ArgumentNullException.ThrowIfNull(customer);

        if (!_customerSettings.UsernamesEnabled)
			_logger.LogError("Throwing NopException from SetUsernameAsync(customer={Customer},newUsername={NewUsername})", customer, newUsername);
            throw new NopException("Usernames are disabled");

        newUsername = newUsername.Trim();
		_logger.LogError("SetUsernameAsync(customer={Customer},newUsername={NewUsername}): newUsername → {NewUsername1}", customer, newUsername, newUsername);

        if (newUsername.Length > NopCustomerServicesDefaults.CustomerUsernameLength)
            throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameTooLong"));

        var user2 = await _customerService.GetCustomerByUsernameAsync(newUsername);
		_logger.LogError("SetUsernameAsync(customer={Customer},newUsername={NewUsername}): user2 → {User2}", customer, newUsername, user2);
        if (user2 != null && customer.Id != user2.Id)
            throw new NopException(await _localizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameAlreadyExists"));

        customer.Username = newUsername;
        await _customerService.UpdateCustomerAsync(customer);
    }

    #endregion
}