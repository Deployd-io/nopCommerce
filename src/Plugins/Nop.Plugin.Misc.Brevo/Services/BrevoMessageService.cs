using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents overridden workflow message service
/// </summary>
public class BrevoMessageService : WorkflowMessageService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public BrevoMessageService(BrevoManager brevoEmailManager,
        CommonSettings commonSettings,
        EmailAccountSettings emailAccountSettings,
        IAddressService addressService,
        IAffiliateService affiliateService,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IMessageTokenProvider messageTokenProvider,
        IOrderService orderService,
        IProductService productService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IQueuedEmailService queuedEmailService,
        ITokenizer tokenizer,
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
			_logger.LogDebug("<<< Exiting BrevoMessageService(brevoEmailManager={BrevoEmailManager},commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},queuedEmailService={QueuedEmailService},tokenizer=<redacted>,messagesSettings={MessagesSettings})", brevoEmailManager, commonSettings, emailAccountSettings, addressService, affiliateService, customerService, emailAccountService, eventPublisher, genericAttributeService, languageService, localizationService, messageTemplateService, orderService, productService, settingService, storeContext, storeService, queuedEmailService, messagesSettings);
        _brevoEmailManager = brevoEmailManager;
		_logger.LogDebug("BrevoMessageService(brevoEmailManager={BrevoEmailManager},commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},queuedEmailService={QueuedEmailService},tokenizer=<redacted>,messagesSettings={MessagesSettings}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, commonSettings, emailAccountSettings, addressService, affiliateService, customerService, emailAccountService, eventPublisher, genericAttributeService, languageService, localizationService, messageTemplateService, orderService, productService, settingService, storeContext, storeService, queuedEmailService, messagesSettings, _brevoEmailManager);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("BrevoMessageService(brevoEmailManager={BrevoEmailManager},commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},queuedEmailService={QueuedEmailService},tokenizer=<redacted>,messagesSettings={MessagesSettings}): _genericAttributeService → {_genericAttributeService}", brevoEmailManager, commonSettings, emailAccountSettings, addressService, affiliateService, customerService, emailAccountService, eventPublisher, genericAttributeService, languageService, localizationService, messageTemplateService, orderService, productService, settingService, storeContext, storeService, queuedEmailService, messagesSettings, _genericAttributeService);
        _settingService = settingService;
		_logger.LogDebug("BrevoMessageService(brevoEmailManager={BrevoEmailManager},commonSettings={CommonSettings},emailAccountSettings={EmailAccountSettings},addressService={AddressService},affiliateService={AffiliateService},customerService={CustomerService},emailAccountService={EmailAccountService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},languageService={LanguageService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},queuedEmailService={QueuedEmailService},tokenizer=<redacted>,messagesSettings={MessagesSettings}): _settingService → {_settingService}", brevoEmailManager, commonSettings, emailAccountSettings, addressService, affiliateService, customerService, emailAccountService, eventPublisher, genericAttributeService, languageService, localizationService, messageTemplateService, orderService, productService, settingService, storeContext, storeService, queuedEmailService, messagesSettings, _settingService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Send SMS notification by message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="tokens">Tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task SendSmsNotificationAsync(MessageTemplate messageTemplate, IEnumerable<Token> tokens)
    {
		_logger.LogDebug(">>> Entering SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>)", messageTemplate);
        //get plugin settings
        var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
		_logger.LogDebug("SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>): storeId → {StoreId}", messageTemplate, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId ?? 0);
		_logger.LogDebug("SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>): brevoSettings → {BrevoSettings}", messageTemplate, brevoSettings);

        //ensure SMS notifications enabled
        if (!brevoSettings.UseSmsNotifications)
            return;

        //whether to send SMS by the passed message template
        var sendSmsForThisMessageTemplate = await _genericAttributeService
            .GetAttributeAsync<bool>(messageTemplate, BrevoDefaults.UseSmsAttribute);
        if (!sendSmsForThisMessageTemplate)
            return;

        //get text with replaced tokens
        var text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, BrevoDefaults.SmsTextAttribute);
		_logger.LogDebug("SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>): text → {Text}", messageTemplate, text);
        if (!string.IsNullOrEmpty(text))
            text = _tokenizer.Replace(text, tokens, false);

        //get phone number send to
        var phoneNumberTo = string.Empty;
		_logger.LogDebug("SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>): phoneNumberTo → {PhoneNumberTo}", messageTemplate, phoneNumberTo);
        var phoneType = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, BrevoDefaults.PhoneTypeAttribute);
		_logger.LogDebug("SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>): phoneType → {PhoneType}", messageTemplate, phoneType);
        switch (phoneType)
        {
            case 0:
                //merchant phone
                phoneNumberTo = brevoSettings.StoreOwnerPhoneNumber;
                break;
            case 1:
                //customer phone
                phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "Customer.PhoneNumber")?.Value?.ToString();
                break;
            case 2:
                //order billing address phone
                phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "Order.BillingPhoneNumber")?.Value?.ToString();
                break;
        }

        //try to send SMS
        await _brevoEmailManager.SendSMSAsync(phoneNumberTo, brevoSettings.SmsSenderName, text);
		_logger.LogDebug("<<< Exiting SendSmsNotificationAsync(messageTemplate={MessageTemplate},tokens=<redacted>)", messageTemplate);
    }

    /// <summary>
    /// Send email notification by message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="emailAccount">Email account</param>
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    protected async Task<int?> SendEmailNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, IEnumerable<Token> tokens,
        string toEmailAddress, string toName,
        string attachmentFilePath = null, string attachmentFileName = null,
        string replyToEmailAddress = null, string replyToName = null,
        string fromEmail = null, string fromName = null, string subject = null)
    {
        //get plugin settings
		_logger.LogDebug(">>> Entering SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject})", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): storeId → {StoreId}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId ?? 0);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): brevoSettings → {BrevoSettings}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, brevoSettings);

        //ensure email notifications enabled
        if (!brevoSettings.UseSmtp)
            return null;

        //whether to send email by the passed message template
        var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, BrevoDefaults.TemplateIdAttribute);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): templateId → {TemplateId}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, templateId);
        var sendEmailForThisMessageTemplate = templateId.HasValue;
        if (!sendEmailForThisMessageTemplate)
            return null;

        //get the specified email account from settings
        emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId) ?? emailAccount;
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): emailAccount → {EmailAccount1}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, emailAccount);

        //get an email from the template
        var email = await _brevoEmailManager.GetQueuedEmailFromTemplateAsync(templateId.Value)
                    ?? throw new NopException($"There is no template with id {templateId}");

        //replace body and subject tokens
        if (string.IsNullOrEmpty(subject))
            subject = email.Subject;
        if (!string.IsNullOrEmpty(subject))
            email.Subject = _tokenizer.Replace(subject, tokens, false);
        if (!string.IsNullOrEmpty(email.Body))
            email.Body = _tokenizer.Replace(email.Body, tokens, true);

        //set email parameters
        email.Priority = QueuedEmailPriority.High;
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.Priority → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.Priority);
        email.From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : email.From;
        email.FromName = !string.IsNullOrEmpty(fromName) ? fromName : email.FromName;
        email.To = toEmailAddress;
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.To → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.To);
        email.ToName = CommonHelper.EnsureMaximumLength(toName, 300);
        email.ReplyTo = replyToEmailAddress;
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.ReplyTo → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.ReplyTo);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.ReplyToName → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.ReplyToName);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.CC → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.CC);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.Bcc → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.Bcc);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.AttachmentFilePath → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.AttachmentFilePath);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.AttachmentFileName → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.AttachmentFileName);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.AttachedDownloadId → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.AttachedDownloadId);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.CreatedOnUtc → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.CreatedOnUtc);
		_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.EmailAccountId → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.EmailAccountId);
        email.ReplyToName = replyToName;
        email.CC = string.Empty;
        email.Bcc = messageTemplate.BccEmailAddresses;
        email.AttachmentFilePath = attachmentFilePath;
        email.AttachmentFileName = attachmentFileName;
        email.AttachedDownloadId = messageTemplate.AttachedDownloadId;
        email.CreatedOnUtc = DateTime.UtcNow;
        email.EmailAccountId = emailAccount.Id;
        email.DontSendBeforeDateUtc = messageTemplate.DelayBeforeSend.HasValue
            ? (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            : null;
			_logger.LogDebug("SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject}): email.DontSendBeforeDateUtc → {Arg12}", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, email.DontSendBeforeDateUtc);

        //queue email
        await _queuedEmailService.InsertQueuedEmailAsync(email);
		_logger.LogDebug("<<< Exiting SendEmailNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject})", messageTemplate, emailAccount, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        return email.Id;
    }

    #endregion

    #region Methods

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
    public override async Task<int> SendNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IList<Token> tokens,
        string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null,
        string replyToEmailAddress = null, string replyToName = null, string fromEmail = null, string fromName = null,
        string subject = null, bool ignoreDelayBeforeSend = false)
    {
		_logger.LogDebug(">>> Entering SendNotificationAsync(messageTemplate={MessageTemplate},emailAccount={EmailAccount},languageId={LanguageId},tokens=<redacted>,toEmailAddress={ToEmailAddress},toName={ToName},attachmentFilePath={AttachmentFilePath},attachmentFileName={AttachmentFileName},replyToEmailAddress={ReplyToEmailAddress},replyToName={ReplyToName},fromEmail={FromEmail},fromName={FromName},subject={Subject},ignoreDelayBeforeSend={IgnoreDelayBeforeSend})", messageTemplate, emailAccount, languageId, toEmailAddress, toName, attachmentFilePath, attachmentFileName, replyToEmailAddress, replyToName, fromEmail, fromName, subject, ignoreDelayBeforeSend);
        ArgumentNullException.ThrowIfNull(messageTemplate);
        ArgumentNullException.ThrowIfNull(emailAccount);

        //try to send SMS notification
        await SendSmsNotificationAsync(messageTemplate, tokens);

        //try to send email notification
        var emailId = await SendEmailNotificationAsync(messageTemplate, emailAccount, tokens,
            toEmailAddress, toName, attachmentFilePath, attachmentFileName,
            replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        if (emailId.HasValue)
            return emailId.Value;

        //send base notification
        return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
            toEmailAddress, toName, attachmentFilePath, attachmentFileName,
            replyToEmailAddress, replyToName, fromEmail, fromName, subject, ignoreDelayBeforeSend);
    }

    #endregion
}