using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Services;

/// <summary>
/// Represents RFQ message service
/// </summary>
public class RfqMessageService : WorkflowMessageService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;
    private readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public RfqMessageService(CommonSettings commonSettings,
        EmailAccountSettings emailAccountSettings,
        INopUrlHelper nopUrlHelper,
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
        IWebHelper webHelper,
        LocalizationSettings localizationSettings,
        MessagesSettings messagesSettings)
        : base(commonSettings,
            emailAccountSettings,
            addressService,
            affiliateService,
            customerService,
            emailAccountService,
            eventPublisher,
            languageService,
            localizationService,
            messageTemplateService,
            messageTokenProvider,
            orderService,
            productService,
            queuedEmailService,
            storeContext,
            storeService,
            tokenizer,
            messagesSettings)
    {
		_logger.LogDebug(">>> Entering RfqMessageService(commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},nopUrlHelper={NopUrlHelper},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},queuedEmailService={QueuedEmailService},storeContext={StoreContext},storeService={StoreService},tokenizer=<redacted>,webHelper={WebHelper},localizationSettings={LocalizationSettings},messagesSettings={MessagesSettings})", commonSettings, emailAccountSettings, nopUrlHelper, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, orderService, productService, queuedEmailService, storeContext, storeService, webHelper, localizationSettings, messagesSettings);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("RfqMessageService(commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},nopUrlHelper={NopUrlHelper},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},queuedEmailService={QueuedEmailService},storeContext={StoreContext},storeService={StoreService},tokenizer=<redacted>,webHelper={WebHelper},localizationSettings={LocalizationSettings},messagesSettings={MessagesSettings}): _nopUrlHelper → {_nopUrlHelper}", commonSettings, emailAccountSettings, nopUrlHelper, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, orderService, productService, queuedEmailService, storeContext, storeService, webHelper, localizationSettings, messagesSettings, _nopUrlHelper);
        _webHelper = webHelper;
		_logger.LogDebug("RfqMessageService(commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},nopUrlHelper={NopUrlHelper},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},queuedEmailService={QueuedEmailService},storeContext={StoreContext},storeService={StoreService},tokenizer=<redacted>,webHelper={WebHelper},localizationSettings={LocalizationSettings},messagesSettings={MessagesSettings}): _webHelper → {_webHelper}", commonSettings, emailAccountSettings, nopUrlHelper, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, orderService, productService, queuedEmailService, storeContext, storeService, webHelper, localizationSettings, messagesSettings, _webHelper);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("<<< Exiting RfqMessageService(commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},nopUrlHelper={NopUrlHelper},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},queuedEmailService={QueuedEmailService},storeContext={StoreContext},storeService={StoreService},tokenizer=<redacted>,webHelper={WebHelper},localizationSettings={LocalizationSettings},messagesSettings={MessagesSettings})", commonSettings, emailAccountSettings, nopUrlHelper, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, orderService, productService, queuedEmailService, storeContext, storeService, webHelper, localizationSettings, messagesSettings);
		_logger.LogDebug("RfqMessageService(commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},nopUrlHelper={NopUrlHelper},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},queuedEmailService={QueuedEmailService},storeContext={StoreContext},storeService={StoreService},tokenizer=<redacted>,webHelper={WebHelper},localizationSettings={LocalizationSettings},messagesSettings={MessagesSettings}): _localizationSettings → {_localizationSettings}", commonSettings, emailAccountSettings, nopUrlHelper, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, orderService, productService, queuedEmailService, storeContext, storeService, webHelper, localizationSettings, messagesSettings, _localizationSettings);
    }

    #endregion

    #region Utilities

    private async Task AddAdminRequestQuoteTokensAsync(IList<Token> tokens, RequestQuote requestQuote, Language language)
    {
		_logger.LogDebug(">>> Entering AddAdminRequestQuoteTokensAsync(tokens=<redacted>,requestQuote={RequestQuote},language={Language})", requestQuote, language);
        tokens.Add(new Token("RequestQuote.Id", requestQuote.Id));
        tokens.Add(new Token("RequestQuote.CreatedOn", requestQuote.CreatedOnUtc.ToString("D", new CultureInfo(language.LanguageCulture))));
        tokens.Add(new Token("RequestQuote.URL", $"{_webHelper.GetStoreLocation()}Admin/RfqAdmin/AdminRequest/{requestQuote.Id}"));

        //event notification
        await _eventPublisher.EntityTokensAddedAsync(requestQuote, tokens);
		_logger.LogDebug("<<< Exiting AddAdminRequestQuoteTokensAsync(tokens=<redacted>,requestQuote={RequestQuote},language={Language})", requestQuote, language);
    }

    private async Task AddCustomerQuoteTokensAsync(IList<Token> tokens, Quote quote, Language language)
    {
		_logger.LogDebug(">>> Entering AddCustomerQuoteTokensAsync(tokens=<redacted>,quote={Quote},language={Language})", quote, language);
        tokens.Add(new Token("Quote.Id", quote.Id));
        tokens.Add(new Token("Quote.CreatedOn", quote.CreatedOnUtc.ToString("D", new CultureInfo(language.LanguageCulture))));
        var expirationDate = quote.ExpirationDateUtc?.ToString("D", new CultureInfo(language.LanguageCulture));
		_logger.LogDebug("AddCustomerQuoteTokensAsync(tokens=<redacted>,quote={Quote},language={Language}): expirationDate → {ExpirationDate}", quote, language, expirationDate);
        tokens.Add(new Token("Quote.ExpirationOn", expirationDate));
        tokens.Add(new Token("Quote.ExpirationOnIsSet", !string.IsNullOrWhiteSpace(expirationDate)));

        var url = _nopUrlHelper.RouteUrl(RfqDefaults.CustomerQuoteRouteName, new { quoteId = quote.Id }, _webHelper.GetCurrentRequestProtocol());
		_logger.LogDebug("AddCustomerQuoteTokensAsync(tokens=<redacted>,quote={Quote},language={Language}): url → {Url}", quote, language, url);
        tokens.Add(new Token("Quote.URL", url));

        //event notification
        await _eventPublisher.EntityTokensAddedAsync(quote, tokens);
		_logger.LogDebug("<<< Exiting AddCustomerQuoteTokensAsync(tokens=<redacted>,quote={Quote},language={Language})", quote, language);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends "Customer sent new request quote" message 
    /// </summary>
    /// <param name="requestQuote">Request quote</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public async Task<IList<int>> CustomerSentNewRequestQuoteAsync(RequestQuote requestQuote)
    {
		_logger.LogDebug(">>> Entering CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote})", requestQuote);
        ArgumentNullException.ThrowIfNull(requestQuote);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote}): store → {Store}", requestQuote, store);

        var messageTemplates = await GetActiveMessageTemplatesAsync(RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE, store.Id);
		_logger.LogDebug("CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote}): messageTemplates → {MessageTemplates}", requestQuote, messageTemplates);
        if (!messageTemplates.Any())
            return new List<int>();

        var languageId = await EnsureLanguageIsActiveAsync(_localizationSettings.DefaultAdminLanguageId, store.Id);
		_logger.LogDebug("CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote}): languageId → {LanguageId}", requestQuote, languageId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var language = await _languageService.GetLanguageByIdAsync(languageId);
		_logger.LogDebug("CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote}): language → {Language}", requestQuote, language);
        var customer = await _customerService.GetCustomerByIdAsync(requestQuote.CustomerId);
		_logger.LogDebug("CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote}): customer → {Customer}", requestQuote, customer);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(tokens, requestQuote.CustomerId);
            await AddAdminRequestQuoteTokensAsync(tokens, requestQuote, language);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            var replyToEmail = messageTemplate.AllowDirectReply ? customer.Email : "";
            var replyToName = messageTemplate.AllowDirectReply ? $"{customer.FirstName} {customer.LastName}" : "";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
		_logger.LogInformation("CustomerSentNewRequestQuoteAsync(requestQuote)={RequestQuote}: database query executed in {Arg1} ms", requestQuote, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CustomerSentNewRequestQuoteAsync(requestQuote={RequestQuote})", requestQuote);
    }

    /// <summary>
    /// Sends "Administrator sent new quote" message 
    /// </summary>
    /// <param name="quote">The quote</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public async Task<IList<int>> AdminSentNewQuoteAsync(Quote quote)
    {
		_logger.LogDebug(">>> Entering AdminSentNewQuoteAsync(quote={Quote})", quote);
        ArgumentNullException.ThrowIfNull(quote);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("AdminSentNewQuoteAsync(quote={Quote}): store → {Store}", quote, store);

        var messageTemplates = await GetActiveMessageTemplatesAsync(RfqDefaults.ADMIN_SENT_NEW_QUOTE, store.Id);
		_logger.LogDebug("AdminSentNewQuoteAsync(quote={Quote}): messageTemplates → {MessageTemplates}", quote, messageTemplates);
        if (!messageTemplates.Any())
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId);
		_logger.LogDebug("AdminSentNewQuoteAsync(quote={Quote}): customer → {Customer}", quote, customer);
        var languageId = await EnsureLanguageIsActiveAsync(customer.LanguageId ?? 0, store.Id);
		_logger.LogDebug("AdminSentNewQuoteAsync(quote={Quote}): languageId → {LanguageId}", quote, languageId);
        var language = await _languageService.GetLanguageByIdAsync(languageId);
		_logger.LogDebug("AdminSentNewQuoteAsync(quote={Quote}): language → {Language}", quote, language);


		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(tokens, quote.CustomerId);
            await AddCustomerQuoteTokensAsync(tokens, quote, language);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, customer.Email, $"{customer.FirstName} {customer.LastName}");
        }).ToListAsync();
		_logger.LogInformation("AdminSentNewQuoteAsync(quote)={Quote}: database query executed in {Arg1} ms", quote, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting AdminSentNewQuoteAsync(quote={Quote})", quote);
    }

    #endregion
}
