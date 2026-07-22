using System.Net;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Services.Messages;

/// <summary>
/// Workflow message service
/// </summary>
public partial class WorkflowMessageService : IWorkflowMessageService
{
    #region Fields

    protected readonly CommonSettings _commonSettings;
    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly ICustomerService _customerService;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IMessageTokenProvider _messageTokenProvider;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IQueuedEmailService _queuedEmailService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly ITokenizer _tokenizer;
    protected readonly MessagesSettings _messagesSettings;

    #endregion

    #region Ctor

    public WorkflowMessageService(CommonSettings commonSettings,
        EmailAccountSettings emailAccountSettings,
        IAddressService addressService,
        IAffiliateService affiliateService,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        IEventPublisher eventPublisher,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IMessageTokenProvider messageTokenProvider,
        IOrderService orderService,
        IProductService productService,
        IQueuedEmailService queuedEmailService,
        IStoreContext storeContext,
        IStoreService storeService,
        ITokenizer tokenizer,
        MessagesSettings messagesSettings)
    {
        _commonSettings = commonSettings;
        _emailAccountSettings = emailAccountSettings;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _customerService = customerService;
        _emailAccountService = emailAccountService;
        _eventPublisher = eventPublisher;
        _languageService = languageService;
        _localizationService = localizationService;
        _messageTemplateService = messageTemplateService;
        _messageTokenProvider = messageTokenProvider;
        _orderService = orderService;
        _productService = productService;
        _queuedEmailService = queuedEmailService;
        _storeContext = storeContext;
        _storeService = storeService;
        _tokenizer = tokenizer;
        _messagesSettings = messagesSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get active message templates by the name
    /// </summary>
    /// <param name="messageTemplateName">Message template name</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of message templates
    /// </returns>
    protected virtual async Task<IList<MessageTemplate>> GetActiveMessageTemplatesAsync(string messageTemplateName, int storeId)
    {
		logger.LogDebug(">>> Entering GetActiveMessageTemplatesAsync(messageTemplateName={MessageTemplateName},storeId={StoreId})", messageTemplateName, storeId);
        //get message templates by the name
        var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName, storeId);

		logger.LogDebug("GetActiveMessageTemplatesAsync(messageTemplateName={MessageTemplateName},storeId={StoreId}): messageTemplates → {MessageTemplates}", messageTemplateName, storeId, messageTemplates);
        //no template found
        if (!messageTemplates?.Any() ?? true)
            return new List<MessageTemplate>();

        //filter active templates
        messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

		logger.LogDebug("<<< Exiting GetActiveMessageTemplatesAsync(messageTemplateName={MessageTemplateName},storeId={StoreId})", messageTemplateName, storeId);
        return messageTemplates;
    }

    /// <summary>
    /// Get EmailAccount to use with a message templates
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the emailAccount
    /// </returns>
    protected virtual async Task<EmailAccount> GetEmailAccountOfMessageTemplateAsync(MessageTemplate messageTemplate, int languageId)
    {
        var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.EmailAccountId, languageId);
        //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
        if (emailAccountId == 0)
            emailAccountId = messageTemplate.EmailAccountId;

        var emailAccount = (await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId) ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)) ??
                           (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        return emailAccount;
    }

    /// <summary>
    /// Ensure language is active
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return a value language identifier
    /// </returns>
    protected virtual async Task<int> EnsureLanguageIsActiveAsync(int languageId, int storeId)
    {
        //load language by specified ID
        var language = await _languageService.GetLanguageByIdAsync(languageId);

        if (language == null || !language.Published)
        {
            //load any language from the specified store
            language = (await _languageService.GetAllLanguagesAsync(storeId: storeId)).FirstOrDefault();
        }

        if (language == null || !language.Published)
        {
            //load any language
            language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
        }

        if (language == null)
            throw new Exception("No active language could be loaded");

        return language.Id;
    }

    /// <summary>
    /// Get email and name to send email for store owner
    /// </summary>
    /// <param name="messageTemplateEmailAccount">Message template email account</param>
    /// <returns>Email address and name to send email fore store owner</returns>
    protected virtual async Task<(string email, string name)> GetStoreOwnerNameAndEmailAsync(EmailAccount messageTemplateEmailAccount)
    {
        var storeOwnerEmailAccount = _messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails ? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) : null;
        storeOwnerEmailAccount ??= messageTemplateEmailAccount;

        return (storeOwnerEmailAccount.Email, storeOwnerEmailAccount.DisplayName);
    }

    /// <summary>
    /// Get email and name to set ReplyTo property of email from customer 
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="customer">Customer</param>
    /// <returns>Email address and name when reply to email</returns>
    protected virtual async Task<(string email, string name)> GetCustomerReplyToNameAndEmailAsync(MessageTemplate messageTemplate, Customer customer)
    {
        if (!messageTemplate.AllowDirectReply)
            return (null, null);

        var replyToEmail = await _customerService.IsGuestAsync(customer)
            ? string.Empty
            : customer.Email;

        var replyToName = await _customerService.IsGuestAsync(customer)
            ? string.Empty
            : await _customerService.GetCustomerFullNameAsync(customer);

        return (replyToEmail, replyToName);
    }

    /// <summary>
    /// Get email and name to set ReplyTo property of email from order
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="order">Order</param>
    /// <returns>Email address and name when reply to email</returns>
    protected virtual async Task<(string email, string name)> GetCustomerReplyToNameAndEmailAsync(MessageTemplate messageTemplate, Order order)
    {
        if (!messageTemplate.AllowDirectReply)
            return (null, null);

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        return (billingAddress.Email, $"{billingAddress.FirstName} {billingAddress.LastName}");
    }

    #endregion

    #region Methods

    #region Customer workflow

    /// <summary>
    /// Sends 'Failed login attempt' notification message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerFailedLoginAttemptNotificationAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_FAILED_LOGIN_ATTEMPT_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerFailedLoginAttemptNotificationAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends 'New customer' notification message to a store owner
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerRegisteredStoreOwnerNotificationMessageAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_REGISTERED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerRegisteredStoreOwnerNotificationMessageAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends a welcome message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerWelcomeMessageAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_WELCOME_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerWelcomeMessageAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends an email validation message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerEmailValidationMessageAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_EMAIL_VALIDATION_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerEmailValidationMessageAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends an email re-validation message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerEmailRevalidationMessageAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_EMAIL_REVALIDATION_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            //email to re-validate
            var toEmail = customer.EmailToRevalidate;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerEmailRevalidationMessageAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends password recovery message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendCustomerPasswordRecoveryMessageAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CUSTOMER_PASSWORD_RECOVERY_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendCustomerPasswordRecoveryMessageAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    /// <summary>
    /// Sends 'New request to delete customer' message to a store owner
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendDeleteCustomerRequestStoreOwnerNotificationAsync(Customer customer, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendDeleteCustomerRequestStoreOwnerNotificationAsync(customer={Customer},languageId={LanguageId})", customer, languageId);
    }

    #endregion

    #region Order workflow

    /// <summary>
    /// Sends an order placed notification to a vendor
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="vendor">Vendor instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPlacedVendorNotificationAsync(Order order, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PLACED_VENDOR_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId, vendor.Id);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPlacedVendorNotificationAsync(order={Order},vendor={Vendor},languageId={LanguageId})", order, vendor, languageId);
    }

    /// <summary>
    /// Sends an order placed notification to a store owner
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPlacedStoreOwnerNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PLACED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPlacedStoreOwnerNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order placed notification to an affiliate
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPlacedAffiliateNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);

        ArgumentNullException.ThrowIfNull(affiliate);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PLACED_AFFILIATE_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
            var toEmail = affiliateAddress.Email;
            var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPlacedAffiliateNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order paid notification to a store owner
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPaidStoreOwnerNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PAID_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPaidStoreOwnerNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order paid notification to an affiliate
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPaidAffiliateNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);

        ArgumentNullException.ThrowIfNull(affiliate);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PAID_AFFILIATE_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
            var toEmail = affiliateAddress.Email;
            var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPaidAffiliateNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order paid notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPaidCustomerNotificationAsync(Order order, int languageId,
        string attachmentFilePath = null, string attachmentFileName = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PAID_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                attachmentFilePath, attachmentFileName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends an order paid notification to a vendor
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="vendor">Vendor instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPaidVendorNotificationAsync(Order order, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PAID_VENDOR_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId, vendor.Id);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderPaidVendorNotificationAsync(order={Order},vendor={Vendor},languageId={LanguageId})", order, vendor, languageId);
    }

    /// <summary>
    /// Sends an order placed notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderPlacedCustomerNotificationAsync(Order order, int languageId,
        string attachmentFilePath = null, string attachmentFileName = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PLACED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                attachmentFilePath, attachmentFileName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends a shipment sent notification to a customer
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendShipmentSentCustomerNotificationAsync(Shipment shipment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendShipmentSentCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): store → {Store}", shipment, languageId, store);
		logger.LogDebug("SendShipmentSentCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): languageId → {LanguageId1}", shipment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.SHIPMENT_SENT_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendShipmentSentCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", shipment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
		logger.LogDebug("SendShipmentSentCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): commonTokens → {CommonTokens}", shipment, languageId, commonTokens);
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendShipmentSentCustomerNotificationAsync(shipment,languageId)={Shipment},{LanguageId}: database query executed in {Arg2} ms", shipment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendShipmentSentCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId})", shipment, languageId);
    }

    /// <summary>
    /// Sends a shipment ready for pickup notification to a customer
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendShipmentReadyForPickupNotificationAsync(Shipment shipment, int languageId)
    {
        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendShipmentReadyForPickupNotificationAsync(shipment={Shipment},languageId={LanguageId}): store → {Store}", shipment, languageId, store);
		logger.LogDebug("SendShipmentReadyForPickupNotificationAsync(shipment={Shipment},languageId={LanguageId}): languageId → {LanguageId1}", shipment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.SHIPMENT_READY_FOR_PICKUP_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendShipmentReadyForPickupNotificationAsync(shipment={Shipment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", shipment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
		logger.LogDebug("SendShipmentReadyForPickupNotificationAsync(shipment={Shipment},languageId={LanguageId}): commonTokens → {CommonTokens}", shipment, languageId, commonTokens);
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendShipmentReadyForPickupNotificationAsync(shipment,languageId)={Shipment},{LanguageId}: database query executed in {Arg2} ms", shipment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendShipmentReadyForPickupNotificationAsync(shipment={Shipment},languageId={LanguageId})", shipment, languageId);
    }

    /// <summary>
    /// Sends a shipment delivered notification to a customer
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendShipmentDeliveredCustomerNotificationAsync(Shipment shipment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendShipmentDeliveredCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): store → {Store}", shipment, languageId, store);
		logger.LogDebug("SendShipmentDeliveredCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): languageId → {LanguageId1}", shipment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.SHIPMENT_DELIVERED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendShipmentDeliveredCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", shipment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddShipmentTokensAsync(commonTokens, shipment, languageId);
		logger.LogDebug("SendShipmentDeliveredCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId}): commonTokens → {CommonTokens}", shipment, languageId, commonTokens);
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendShipmentDeliveredCustomerNotificationAsync(shipment,languageId)={Shipment},{LanguageId}: database query executed in {Arg2} ms", shipment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendShipmentDeliveredCustomerNotificationAsync(shipment={Shipment},languageId={LanguageId})", shipment, languageId);
    }

    /// <summary>
    /// Sends an order processing notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderProcessingCustomerNotificationAsync(Order order, int languageId,
        string attachmentFilePath = null, string attachmentFileName = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_PROCESSING_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                attachmentFilePath, attachmentFileName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends an order completed notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderCompletedCustomerNotificationAsync(Order order, int languageId,
        string attachmentFilePath = null, string attachmentFileName = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_COMPLETED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                attachmentFilePath, attachmentFileName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends an order completed notification to a store owner
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderCompletedStoreOwnerNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_COMPLETED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderCompletedStoreOwnerNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order cancelled notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderCancelledCustomerNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_CANCELLED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderCancelledCustomerNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order cancelled notification to a store owner
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderCancelledStoreOwnerNotificationAsync(Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_CANCELLED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderCancelledStoreOwnerNotificationAsync(order={Order},languageId={LanguageId})", order, languageId);
    }

    /// <summary>
    /// Sends an order cancelled notification to a vendor
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="vendor">Vendor instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderCancelledVendorNotificationAsync(Order order, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId, vendor.Id);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderCancelledVendorNotificationAsync(order={Order},vendor={Vendor},languageId={LanguageId})", order, vendor, languageId);
    }

    /// <summary>
    /// Sends an order refunded notification to a store owner
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="refundedAmount">Amount refunded</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderRefundedStoreOwnerNotificationAsync(Order order, decimal refundedAmount, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_REFUNDED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddOrderRefundedTokensAsync(commonTokens, order, refundedAmount);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderRefundedStoreOwnerNotificationAsync(order={Order},refundedAmount={RefundedAmount},languageId={LanguageId})", order, refundedAmount, languageId);
    }

    /// <summary>
    /// Sends an order refunded notification to a customer
    /// </summary>
    /// <param name="order">Order instance</param>
    /// <param name="refundedAmount">Amount refunded</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendOrderRefundedCustomerNotificationAsync(Order order, decimal refundedAmount, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.ORDER_REFUNDED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddOrderRefundedTokensAsync(commonTokens, order, refundedAmount);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendOrderRefundedCustomerNotificationAsync(order={Order},refundedAmount={RefundedAmount},languageId={LanguageId})", order, refundedAmount, languageId);
    }

    /// <summary>
    /// Sends a new order note added notification to a customer
    /// </summary>
    /// <param name="orderNote">Order note</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewOrderNoteAddedCustomerNotificationAsync(OrderNote orderNote, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(orderNote);

        var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendNewOrderNoteAddedCustomerNotificationAsync(orderNote={OrderNote},languageId={LanguageId}): store → {Store}", orderNote, languageId, store);
		logger.LogDebug("SendNewOrderNoteAddedCustomerNotificationAsync(orderNote={OrderNote},languageId={LanguageId}): languageId → {LanguageId1}", orderNote, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_ORDER_NOTE_ADDED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendNewOrderNoteAddedCustomerNotificationAsync(orderNote={OrderNote},languageId={LanguageId}): messageTemplates → {MessageTemplates}", orderNote, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderNoteTokensAsync(commonTokens, orderNote);
		logger.LogDebug("SendNewOrderNoteAddedCustomerNotificationAsync(orderNote={OrderNote},languageId={LanguageId}): commonTokens → {CommonTokens}", orderNote, languageId, commonTokens);
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendNewOrderNoteAddedCustomerNotificationAsync(orderNote,languageId)={OrderNote},{LanguageId}: database query executed in {Arg2} ms", orderNote, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendNewOrderNoteAddedCustomerNotificationAsync(orderNote={OrderNote},languageId={LanguageId})", orderNote, languageId);
    }

    /// <summary>
    /// Sends a "Recurring payment cancelled" notification to a store owner
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendRecurringPaymentCancelledStoreOwnerNotificationAsync(RecurringPayment recurringPayment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(recurringPayment);

        var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): store → {Store}", recurringPayment, languageId, store);
		logger.LogDebug("SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): languageId → {LanguageId1}", recurringPayment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RECURRING_PAYMENT_CANCELLED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", recurringPayment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
		logger.LogDebug("SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): commonTokens → {CommonTokens}", recurringPayment, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);
        await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogInformation("SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment,languageId)={RecurringPayment},{LanguageId}: database query executed in {Arg2} ms", recurringPayment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId})", recurringPayment, languageId);
    }

    /// <summary>
    /// Sends a "Recurring payment cancelled" notification to a customer
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendRecurringPaymentCancelledCustomerNotificationAsync(RecurringPayment recurringPayment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(recurringPayment);

        var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): store → {Store}", recurringPayment, languageId, store);
		logger.LogDebug("SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): languageId → {LanguageId1}", recurringPayment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RECURRING_PAYMENT_CANCELLED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", recurringPayment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
		logger.LogDebug("SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): commonTokens → {CommonTokens}", recurringPayment, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);
        await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment,languageId)={RecurringPayment},{LanguageId}: database query executed in {Arg2} ms", recurringPayment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId})", recurringPayment, languageId);
    }

    /// <summary>
    /// Sends a "Recurring payment failed" notification to a customer
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendRecurringPaymentFailedCustomerNotificationAsync(RecurringPayment recurringPayment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(recurringPayment);

        var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) ?? throw new Exception("Order cannot be loaded");

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): store → {Store}", recurringPayment, languageId, store);
		logger.LogDebug("SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): languageId → {LanguageId1}", recurringPayment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RECURRING_PAYMENT_FAILED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", recurringPayment, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
		logger.LogDebug("SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId}): commonTokens → {CommonTokens}", recurringPayment, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);
        await _messageTokenProvider.AddRecurringPaymentTokensAsync(commonTokens, recurringPayment);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = billingAddress.Email;
            var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment,languageId)={RecurringPayment},{LanguageId}: database query executed in {Arg2} ms", recurringPayment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment={RecurringPayment},languageId={LanguageId})", recurringPayment, languageId);
    }

    #endregion

    #region Newsletter workflow

    /// <summary>
    /// Sends a newsletter subscription activation message
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription)
    {
		logger.LogDebug(">>> Entering SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription})", subscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(subscription);

        var store = await _storeContext.GetCurrentStoreAsync();
        var languageId = await EnsureLanguageIsActiveAsync(subscription.LanguageId, store.Id);
		logger.LogDebug("SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription}): store → {Store}", subscription, store);
		logger.LogDebug("SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription}): languageId → {LanguageId}", subscription, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEWSLETTER_SUBSCRIPTION_ACTIVATION_MESSAGE, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription}): messageTemplates → {MessageTemplates}", subscription, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);
		logger.LogDebug("SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription}): commonTokens → {CommonTokens}", subscription, commonTokens);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
        }).ToListAsync();
		logger.LogInformation("SendNewsLetterSubscriptionActivationMessageAsync(subscription)={Subscription}: database query executed in {Arg1} ms", subscription, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendNewsLetterSubscriptionActivationMessageAsync(subscription={Subscription})", subscription);
    }

    /// <summary>
    /// Sends a newsletter subscription deactivation message
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription)
    {
		logger.LogDebug(">>> Entering SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription})", subscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(subscription);

        var store = await _storeContext.GetCurrentStoreAsync();
        var languageId = await EnsureLanguageIsActiveAsync(subscription.LanguageId, store.Id);
		logger.LogDebug("SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription}): store → {Store}", subscription, store);
		logger.LogDebug("SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription}): languageId → {LanguageId}", subscription, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEWSLETTER_SUBSCRIPTION_DEACTIVATION_MESSAGE, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription}): messageTemplates → {MessageTemplates}", subscription, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(commonTokens, subscription);
		logger.LogDebug("SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription}): commonTokens → {CommonTokens}", subscription, commonTokens);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
        }).ToListAsync();
		logger.LogInformation("SendNewsLetterSubscriptionDeactivationMessageAsync(subscription)={Subscription}: database query executed in {Arg1} ms", subscription, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendNewsLetterSubscriptionDeactivationMessageAsync(subscription={Subscription})", subscription);
    }

    #endregion

    #region Send a message to a friend

    /// <summary>
    /// Sends "email a friend" message
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="product">Product instance</param>
    /// <param name="customerEmail">Customer's email</param>
    /// <param name="friendsEmail">Friend's email</param>
    /// <param name="personalMessage">Personal message</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendProductEmailAFriendMessageAsync(Customer customer, int languageId,
        Product product, string customerEmail, string friendsEmail, string personalMessage)
    {
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.EMAIL_A_FRIEND_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);
        commonTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
        commonTokens.Add(new Token("EmailAFriend.Email", customerEmail));

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends wishlist "email a friend" message
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="customerEmail">Customer's email</param>
    /// <param name="friendsEmail">Friend's email</param>
    /// <param name="personalMessage">Personal message</param>
    /// <param name="wishlistUrl">Wishlist URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendWishlistEmailAFriendMessageAsync(Customer customer, int languageId,
        string customerEmail, string friendsEmail, string personalMessage, string wishlistUrl)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.WISHLIST_TO_FRIEND_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        commonTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
        commonTokens.Add(new Token("Wishlist.Email", customerEmail));
        commonTokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        }).ToListAsync();
    }

    #endregion

    #region Return requests

    /// <summary>
    /// Sends 'New Return Request' message to a store owner
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="order">Order</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewReturnRequestStoreOwnerNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(returnRequest);

        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_RETURN_REQUEST_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, returnRequest.CustomerId);
        await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, order);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendNewReturnRequestStoreOwnerNotificationAsync(returnRequest={ReturnRequest},orderItem={OrderItem},order={Order},languageId={LanguageId})", returnRequest, orderItem, order, languageId);
    }

    /// <summary>
    /// Sends 'New Return Request' message to a customer
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewReturnRequestCustomerNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(returnRequest);

        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var languageId = await EnsureLanguageIsActiveAsync(order.CustomerLanguageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_RETURN_REQUEST_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(returnRequest.CustomerId);

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = (await _customerService.IsGuestAsync(customer))
                ? billingAddress.Email
                : customer.Email;
            var toName = (await _customerService.IsGuestAsync(customer))
                ? billingAddress.FirstName
                : await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendNewReturnRequestCustomerNotificationAsync(returnRequest={ReturnRequest},orderItem={OrderItem},order={Order})", returnRequest, orderItem, order);
    }

    /// <summary>
    /// Sends 'Return Request status changed' message to a customer
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendReturnRequestStatusChangedCustomerNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(returnRequest);

        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(order);

        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var languageId = await EnsureLanguageIsActiveAsync(order.CustomerLanguageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.RETURN_REQUEST_STATUS_CHANGED_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(returnRequest.CustomerId);

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        await _messageTokenProvider.AddReturnRequestTokensAsync(commonTokens, returnRequest, orderItem, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var toEmail = (await _customerService.IsGuestAsync(customer))
                ? billingAddress.Email
                : customer.Email;
            var toName = (await _customerService.IsGuestAsync(customer))
                ? billingAddress.FirstName
                : await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendReturnRequestStatusChangedCustomerNotificationAsync(returnRequest={ReturnRequest},orderItem={OrderItem},order={Order})", returnRequest, orderItem, order);
    }

    #endregion

    #region Forum Notifications

    /// <summary>
    /// Sends a forum subscription message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="forumTopic">Forum Topic</param>
    /// <param name="forum">Forum</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewForumTopicMessageAsync(Customer customer, ForumTopic forumTopic, Forum forum, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_FORUM_TOPIC_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic);
        await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendNewForumTopicMessageAsync(customer={Customer},forumTopic={ForumTopic},forum={Forum},languageId={LanguageId})", customer, forumTopic, forum, languageId);
    }

    /// <summary>
    /// Sends a forum subscription message to a customer
    /// </summary>
    /// <param name="customer">Customer instance</param>
    /// <param name="forumPost">Forum post</param>
    /// <param name="forumTopic">Forum Topic</param>
    /// <param name="forum">Forum</param>
    /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewForumPostMessageAsync(Customer customer, ForumPost forumPost, ForumTopic forumTopic,
        Forum forum, int friendlyForumTopicPageIndex, int languageId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_FORUM_POST_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddForumPostTokensAsync(commonTokens, forumPost);
        await _messageTokenProvider.AddForumTopicTokensAsync(commonTokens, forumTopic, friendlyForumTopicPageIndex, forumPost.Id);
        await _messageTokenProvider.AddForumTokensAsync(commonTokens, forum);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends a private message notification
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendPrivateMessageNotificationAsync(PrivateMessage privateMessage, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(privateMessage);

        var store = await _storeService.GetStoreByIdAsync(privateMessage.StoreId) ?? await _storeContext.GetCurrentStoreAsync();

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.PRIVATE_MESSAGE_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddPrivateMessageTokensAsync(commonTokens, privateMessage);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, privateMessage.ToCustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var customer = await _customerService.GetCustomerByIdAsync(privateMessage.ToCustomerId);
            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendPrivateMessageNotificationAsync(privateMessage={PrivateMessage},languageId={LanguageId})", privateMessage, languageId);
    }

    #endregion

    #region Misc

    /// <summary>
    /// Sends 'New vendor account submitted' message to a store owner
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewVendorAccountApplyStoreOwnerNotificationAsync(Customer customer, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_VENDOR_ACCOUNT_APPLY_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        await _messageTokenProvider.AddVendorTokensAsync(commonTokens, vendor);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            var vendorAddress = await _addressService.GetAddressByIdAsync(vendor.AddressId);
            var replyToEmail = messageTemplate.AllowDirectReply ? vendorAddress.Email : "";
            var replyToName = messageTemplate.AllowDirectReply ? $"{vendorAddress.FirstName} {vendorAddress.LastName}" : "";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogDebug("<<< Exiting SendNewVendorAccountApplyStoreOwnerNotificationAsync(customer={Customer},vendor={Vendor},languageId={LanguageId})", customer, vendor, languageId);
    }

    /// <summary>
    /// Sends 'Vendor information changed' message to a store owner
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendVendorInformationChangeStoreOwnerNotificationAsync(Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendVendorInformationChangeStoreOwnerNotificationAsync(vendor={Vendor},languageId={LanguageId}): store → {Store}", vendor, languageId, store);
		logger.LogDebug("SendVendorInformationChangeStoreOwnerNotificationAsync(vendor={Vendor},languageId={LanguageId}): languageId → {LanguageId1}", vendor, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.VENDOR_INFORMATION_CHANGE_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendVendorInformationChangeStoreOwnerNotificationAsync(vendor={Vendor},languageId={LanguageId}): messageTemplates → {MessageTemplates}", vendor, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddVendorTokensAsync(commonTokens, vendor);
		logger.LogDebug("SendVendorInformationChangeStoreOwnerNotificationAsync(vendor={Vendor},languageId={LanguageId}): commonTokens → {CommonTokens}", vendor, languageId, commonTokens);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            var vendorAddress = await _addressService.GetAddressByIdAsync(vendor.AddressId);
            var replyToEmail = messageTemplate.AllowDirectReply ? vendorAddress.Email : "";
            var replyToName = messageTemplate.AllowDirectReply ? $"{vendorAddress.FirstName} {vendorAddress.LastName}" : "";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogInformation("SendVendorInformationChangeStoreOwnerNotificationAsync(vendor,languageId)={Vendor},{LanguageId}: database query executed in {Arg2} ms", vendor, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendVendorInformationChangeStoreOwnerNotificationAsync(vendor={Vendor},languageId={LanguageId})", vendor, languageId);
    }

    /// <summary>
    /// Sends a gift card notification
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendGiftCardNotificationAsync(GiftCard giftCard, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(giftCard);

        var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);
        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var store = order != null ? await _storeService.GetStoreByIdAsync(order.StoreId) ?? currentStore : currentStore;

        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

		logger.LogDebug("SendGiftCardNotificationAsync(giftCard={GiftCard},languageId={LanguageId}): languageId → {LanguageId1}", giftCard, languageId, languageId);
        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.GIFT_CARD_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendGiftCardNotificationAsync(giftCard={GiftCard},languageId={LanguageId}): messageTemplates → {MessageTemplates}", giftCard, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddGiftCardTokensAsync(commonTokens, giftCard, languageId);
		logger.LogDebug("SendGiftCardNotificationAsync(giftCard={GiftCard},languageId={LanguageId}): commonTokens → {CommonTokens}", giftCard, languageId, commonTokens);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = giftCard.RecipientEmail;
            var toName = giftCard.RecipientName;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendGiftCardNotificationAsync(giftCard,languageId)={GiftCard},{LanguageId}: database query executed in {Arg2} ms", giftCard, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendGiftCardNotificationAsync(giftCard={GiftCard},languageId={LanguageId})", giftCard, languageId);
    }

    /// <summary>
    /// Sends a product review notification message to a store owner
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendProductReviewStoreOwnerNotificationMessageAsync(ProductReview productReview, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(productReview);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendProductReviewStoreOwnerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): store → {Store}", productReview, languageId, store);
		logger.LogDebug("SendProductReviewStoreOwnerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): languageId → {LanguageId1}", productReview, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.PRODUCT_REVIEW_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendProductReviewStoreOwnerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): messageTemplates → {MessageTemplates}", productReview, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddProductReviewTokensAsync(commonTokens, productReview);
		logger.LogDebug("SendProductReviewStoreOwnerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): commonTokens → {CommonTokens}", productReview, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, productReview.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var customer = await _customerService.GetCustomerByIdAsync(productReview.CustomerId);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogInformation("SendProductReviewStoreOwnerNotificationMessageAsync(productReview,languageId)={ProductReview},{LanguageId}: database query executed in {Arg2} ms", productReview, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendProductReviewStoreOwnerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId})", productReview, languageId);
    }

    /// <summary>
    /// Sends a product review reply notification message to a customer
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendProductReviewReplyCustomerNotificationMessageAsync(ProductReview productReview, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(productReview);

        var store = await _storeService.GetStoreByIdAsync(productReview.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): store → {Store}", productReview, languageId, store);
		logger.LogDebug("SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): languageId → {LanguageId1}", productReview, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.PRODUCT_REVIEW_REPLY_CUSTOMER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): messageTemplates → {MessageTemplates}", productReview, languageId, messageTemplates);
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(productReview.CustomerId);

		logger.LogDebug("SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): customer → {Customer}", productReview, languageId, customer);
        //We should not send notifications to guests
        if (await _customerService.IsGuestAsync(customer))
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddProductReviewTokensAsync(commonTokens, productReview);
		logger.LogDebug("SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId}): commonTokens → {CommonTokens}", productReview, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendProductReviewReplyCustomerNotificationMessageAsync(productReview,languageId)={ProductReview},{LanguageId}: database query executed in {Arg2} ms", productReview, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendProductReviewReplyCustomerNotificationMessageAsync(productReview={ProductReview},languageId={LanguageId})", productReview, languageId);
    }

    /// <summary>
    /// Sends a "quantity below" notification to a store owner
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendQuantityBelowStoreOwnerNotificationAsync(Product product, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(product);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QUANTITY_BELOW_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug("<<< Exiting SendQuantityBelowStoreOwnerNotificationAsync(product={Product},languageId={LanguageId})", product, languageId);
    }

    /// <summary>
    /// Sends a "quantity below" notification to a store owner
    /// </summary>
    /// <param name="combination">Attribute combination</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendQuantityBelowStoreOwnerNotificationAsync(ProductAttributeCombination combination, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(combination);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendQuantityBelowStoreOwnerNotificationAsync(combination={Combination},languageId={LanguageId}): store → {Store}", combination, languageId, store);
		logger.LogDebug("SendQuantityBelowStoreOwnerNotificationAsync(combination={Combination},languageId={LanguageId}): languageId → {LanguageId1}", combination, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendQuantityBelowStoreOwnerNotificationAsync(combination={Combination},languageId={LanguageId}): messageTemplates → {MessageTemplates}", combination, languageId, messageTemplates);
            return new List<int>();

        var commonTokens = new List<Token>();
        var product = await _productService.GetProductByIdAsync(combination.ProductId);
		logger.LogDebug("SendQuantityBelowStoreOwnerNotificationAsync(combination={Combination},languageId={LanguageId}): commonTokens → {CommonTokens}", combination, languageId, commonTokens);
		logger.LogDebug("SendQuantityBelowStoreOwnerNotificationAsync(combination={Combination},languageId={LanguageId}): product → {Product}", combination, languageId, product);

        await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);
        await _messageTokenProvider.AddAttributeCombinationTokensAsync(commonTokens, combination, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendQuantityBelowStoreOwnerNotificationAsync(combination,languageId)={Combination},{LanguageId}: database query executed in {Arg2} ms", combination, languageId, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Sends a "quantity below" notification to a vendor
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendQuantityBelowVendorNotificationAsync(Product product, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QUANTITY_BELOW_VENDOR_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug("<<< Exiting SendQuantityBelowVendorNotificationAsync(product={Product},vendor={Vendor},languageId={LanguageId})", product, vendor, languageId);
    }

    /// <summary>
    /// Sends a "quantity below" notification to a vendor
    /// </summary>
    /// <param name="combination">Attribute combination</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendQuantityBelowVendorNotificationAsync(ProductAttributeCombination combination, Vendor vendor, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(combination);
        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendQuantityBelowVendorNotificationAsync(combination={Combination},vendor={Vendor},languageId={LanguageId}): store → {Store}", combination, vendor, languageId, store);
		logger.LogDebug("SendQuantityBelowVendorNotificationAsync(combination={Combination},vendor={Vendor},languageId={LanguageId}): languageId → {LanguageId1}", combination, vendor, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.QUANTITY_BELOW_ATTRIBUTE_COMBINATION_VENDOR_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendQuantityBelowVendorNotificationAsync(combination={Combination},vendor={Vendor},languageId={LanguageId}): messageTemplates → {MessageTemplates}", combination, vendor, languageId, messageTemplates);
            return new List<int>();

        var commonTokens = new List<Token>();
        var product = await _productService.GetProductByIdAsync(combination.ProductId);
		logger.LogDebug("SendQuantityBelowVendorNotificationAsync(combination={Combination},vendor={Vendor},languageId={LanguageId}): commonTokens → {CommonTokens}", combination, vendor, languageId, commonTokens);
		logger.LogDebug("SendQuantityBelowVendorNotificationAsync(combination={Combination},vendor={Vendor},languageId={LanguageId}): product → {Product}", combination, vendor, languageId, product);

        await _messageTokenProvider.AddProductTokensAsync(commonTokens, product, languageId);
        await _messageTokenProvider.AddAttributeCombinationTokensAsync(commonTokens, combination, languageId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendQuantityBelowVendorNotificationAsync(combination,vendor,languageId)={Combination},{Vendor},{LanguageId}: database query executed in {Arg3} ms", combination, vendor, languageId, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Sends a "new VAT submitted" notification to a store owner
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="vatName">Received VAT name</param>
    /// <param name="vatAddress">Received VAT address</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewVatSubmittedStoreOwnerNotificationAsync(Customer customer,
        string vatName, string vatAddress, int languageId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEW_VAT_SUBMITTED_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
        commonTokens.Add(new Token("VatValidationResult.Name", vatName));
        commonTokens.Add(new Token("VatValidationResult.Address", vatAddress));

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends a blog comment notification message to a store owner
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of queued email identifiers
    /// </returns>
    public virtual async Task<IList<int>> SendBlogCommentStoreOwnerNotificationMessageAsync(BlogComment blogComment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(blogComment);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId}): store → {Store}", blogComment, languageId, store);
		logger.LogDebug("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId}): languageId → {LanguageId1}", blogComment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.BLOG_COMMENT_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", blogComment, languageId, messageTemplates);
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(blogComment.CustomerId);

		logger.LogDebug("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId}): customer → {Customer}", blogComment, languageId, customer);
        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddBlogCommentTokensAsync(commonTokens, blogComment);
		logger.LogDebug("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId}): commonTokens → {CommonTokens}", blogComment, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, blogComment.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogInformation("SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment,languageId)={BlogComment},{LanguageId}: database query executed in {Arg2} ms", blogComment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendBlogCommentStoreOwnerNotificationMessageAsync(blogComment={BlogComment},languageId={LanguageId})", blogComment, languageId);
    }

    /// <summary>
    /// Sends a news comment notification message to a store owner
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendNewsCommentStoreOwnerNotificationMessageAsync(NewsComment newsComment, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(newsComment);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId}): store → {Store}", newsComment, languageId, store);
		logger.LogDebug("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId}): languageId → {LanguageId1}", newsComment, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.NEWS_COMMENT_STORE_OWNER_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId}): messageTemplates → {MessageTemplates}", newsComment, languageId, messageTemplates);
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(newsComment.CustomerId);

		logger.LogDebug("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId}): customer → {Customer}", newsComment, languageId, customer);
        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddNewsCommentTokensAsync(commonTokens, newsComment);
		logger.LogDebug("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId}): commonTokens → {CommonTokens}", newsComment, languageId, commonTokens);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, newsComment.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		logger.LogInformation("SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment,languageId)={NewsComment},{LanguageId}: database query executed in {Arg2} ms", newsComment, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendNewsCommentStoreOwnerNotificationMessageAsync(newsComment={NewsComment},languageId={LanguageId})", newsComment, languageId);
    }

    /// <summary>
    /// Sends a 'Back in stock' notification message to a customer
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendBackInStockNotificationAsync(BackInStockSubscription subscription, int languageId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(subscription);

        var customer = await _customerService.GetCustomerByIdAsync(subscription.CustomerId);

        ArgumentNullException.ThrowIfNull(customer);

        //ensure that customer is registered (simple and fast way)
        if (!CommonHelper.IsValidEmail(customer.Email))
            return new List<int>();

        var store = await _storeService.GetStoreByIdAsync(subscription.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);
		logger.LogDebug("SendBackInStockNotificationAsync(subscription={Subscription},languageId={LanguageId}): store → {Store}", subscription, languageId, store);
		logger.LogDebug("SendBackInStockNotificationAsync(subscription={Subscription},languageId={LanguageId}): languageId → {LanguageId1}", subscription, languageId, languageId);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.BACK_IN_STOCK_NOTIFICATION, store.Id);
        if (!messageTemplates.Any())
			logger.LogDebug("SendBackInStockNotificationAsync(subscription={Subscription},languageId={LanguageId}): messageTemplates → {MessageTemplates}", subscription, languageId, messageTemplates);
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>();
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);
		logger.LogDebug("SendBackInStockNotificationAsync(subscription={Subscription},languageId={LanguageId}): commonTokens → {CommonTokens}", subscription, languageId, commonTokens);
        await _messageTokenProvider.AddBackInStockTokensAsync(commonTokens, subscription);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = customer.Email;
            var toName = await _customerService.GetCustomerFullNameAsync(customer);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }).ToListAsync();
		logger.LogInformation("SendBackInStockNotificationAsync(subscription,languageId)={Subscription},{LanguageId}: database query executed in {Arg2} ms", subscription, languageId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting SendBackInStockNotificationAsync(subscription={Subscription},languageId={LanguageId})", subscription, languageId);
    }

    /// <summary>
    /// Sends "contact us" message
    /// </summary>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
    /// <param name="body">Email body</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendContactUsMessageAsync(int languageId, string senderEmail,
        string senderName, string subject, string body)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CONTACT_US_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>
        {
            new("ContactUs.SenderEmail", senderEmail),
            new("ContactUs.SenderName", senderName)
        };

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            tokens.Add(new Token("ContactUs.Body", body, true));

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends "contact vendor" message
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="languageId">Message language identifier</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
    /// <param name="body">Email body</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<IList<int>> SendContactVendorMessageAsync(Vendor vendor, int languageId, string senderEmail,
        string senderName, string subject, string body)
    {
        ArgumentNullException.ThrowIfNull(vendor);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CONTACT_VENDOR_MESSAGE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        //tokens
        var commonTokens = new List<Token>
        {
            new("ContactUs.SenderEmail", senderEmail),
            new("ContactUs.SenderName", senderName),
            new("ContactUs.Body", body, true)
        };

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends a test email
    /// </summary>
    /// <param name="messageTemplateId">Message template identifier</param>
    /// <param name="sendToEmail">Send to email</param>
    /// <param name="tokens">Tokens</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<int> SendTestEmailAsync(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
    {
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(messageTemplateId) ?? throw new ArgumentException("Template cannot be loaded");

        //email account
        var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

        //event notification
        await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

        return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null, ignoreDelayBeforeSend: true);
    }

    #endregion

    #region Common

    /// <summary>
    /// Send notification
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="tokens">Tokens</param>
    /// <param name="toEmailAddress">Recipient email address</param>
    /// <param name="toName">Recipient name</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name</param>
    /// <param name="replyToEmailAddress">"Reply to" email</param>
    /// <param name="replyToName">"Reply to" name</param>
    /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
    /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
    /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
    /// <param name="ignoreDelayBeforeSend">A value indicating whether to ignore the delay before sending message</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public virtual async Task<int> SendNotificationAsync(MessageTemplate messageTemplate,
        EmailAccount emailAccount, int languageId, IList<Token> tokens,
        string toEmailAddress, string toName,
        string attachmentFilePath = null, string attachmentFileName = null,
        string replyToEmailAddress = null, string replyToName = null,
        string fromEmail = null, string fromName = null, string subject = null,
        bool ignoreDelayBeforeSend = false)
    {
        ArgumentNullException.ThrowIfNull(messageTemplate);

        ArgumentNullException.ThrowIfNull(emailAccount);

        //retrieve localized message template data
        var bcc = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.BccEmailAddresses, languageId);
        if (string.IsNullOrEmpty(subject))
            subject = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Subject, languageId);
        var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);

        //Replace subject and body tokens 
        var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
        var bodyReplaced = _tokenizer.Replace(body, tokens, true);

        //limit name length
        toName = CommonHelper.EnsureMaximumLength(toName, 300);

        var email = new QueuedEmail
        {
            Priority = QueuedEmailPriority.High,
            From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
            FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
            To = toEmailAddress,
            ToName = toName,
            ReplyTo = replyToEmailAddress,
            ReplyToName = replyToName,
            CC = string.Empty,
            Bcc = bcc,
            Subject = subjectReplaced,
            Body = bodyReplaced,
            AttachmentFilePath = attachmentFilePath,
            AttachmentFileName = attachmentFileName,
            AttachedDownloadId = messageTemplate.AttachedDownloadId,
            CreatedOnUtc = DateTime.UtcNow,
            EmailAccountId = emailAccount.Id,
            DontSendBeforeDateUtc = ignoreDelayBeforeSend || !messageTemplate.DelayBeforeSend.HasValue ? null
                : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
        };

        await _queuedEmailService.InsertQueuedEmailAsync(email);
        return email.Id;
    }

    #endregion

    #endregion
}
