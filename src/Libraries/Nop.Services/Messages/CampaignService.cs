using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Campaign service
/// </summary>
public partial class CampaignService : ICampaignService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IEmailSender _emailSender;
    protected readonly IMessageTokenProvider _messageTokenProvider;
    protected readonly IQueuedEmailService _queuedEmailService;
    protected readonly IRepository<Campaign> _campaignRepository;
    protected readonly IStoreContext _storeContext;
    protected readonly ITokenizer _tokenizer;

    #endregion

    #region Ctor

    public CampaignService(ICustomerService customerService,
        IEmailSender emailSender,
        IMessageTokenProvider messageTokenProvider,
        IQueuedEmailService queuedEmailService,
        IRepository<Campaign> campaignRepository,
        IStoreContext storeContext,
        ITokenizer tokenizer)
    {
		_logger.LogDebug(">>> Entering CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>)", customerService, emailSender, queuedEmailService, campaignRepository, storeContext);
        _customerService = customerService;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _customerService → {_customerService}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _customerService);
        _emailSender = emailSender;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _emailSender → {_emailSender}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _emailSender);
        _messageTokenProvider = messageTokenProvider;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _messageTokenProvider → {_messageTokenProvider}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _messageTokenProvider);
        _queuedEmailService = queuedEmailService;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _queuedEmailService → {_queuedEmailService}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _queuedEmailService);
        _campaignRepository = campaignRepository;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _campaignRepository → {_campaignRepository}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _campaignRepository);
        _storeContext = storeContext;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _storeContext → {_storeContext}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _storeContext);
        _tokenizer = tokenizer;
		_logger.LogDebug("CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>): _tokenizer → {_tokenizer}", customerService, emailSender, queuedEmailService, campaignRepository, storeContext, _tokenizer);
		_logger.LogDebug("<<< Exiting CampaignService(customerService={CustomerService},emailSender={EmailSender},messageTokenProvider=<redacted>,queuedEmailService={QueuedEmailService},campaignRepository={CampaignRepository},storeContext={StoreContext},tokenizer=<redacted>)", customerService, emailSender, queuedEmailService, campaignRepository, storeContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Copies campaign
    /// </summary>
    /// <param name="campaign">Campaign to copy</param>
    /// <param name="newName">The new name of campaign copy</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign copy
    /// </returns>
    public virtual async Task<Campaign> CopyCampaignAsync(Campaign campaign, string newName)
    {
		_logger.LogDebug(">>> Entering CopyCampaignAsync(campaign={Campaign},newName={NewName})", campaign, newName);
        ArgumentNullException.ThrowIfNull(campaign);
        ArgumentException.ThrowIfNullOrEmpty(newName);

        var campaignCopy = new Campaign
        {
            Name = newName,
            Subject = campaign.Subject,
            Body = campaign.Body,
            StoreId = campaign.StoreId,
            CustomerRoleId = campaign.CustomerRoleId,
            NewsLetterSubscriptionTypeId = campaign.NewsLetterSubscriptionTypeId,
            CreatedOnUtc = DateTime.UtcNow
        };

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("CopyCampaignAsync(campaign,newName)={Campaign},{NewName}: save query executed in {Arg2} ms", campaign, newName, __logStart.ElapsedMilliseconds);
        await _campaignRepository.InsertAsync(campaignCopy);

		_logger.LogDebug("<<< Exiting CopyCampaignAsync(campaign={Campaign},newName={NewName})", campaign, newName);
        return campaignCopy;
    }

    /// <summary>
    /// Inserts a campaign
    /// </summary>
    /// <param name="campaign">Campaign</param>        
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCampaignAsync(Campaign campaign)
    {
		_logger.LogDebug(">>> Entering InsertCampaignAsync(campaign={Campaign})", campaign);
        await _campaignRepository.InsertAsync(campaign);
		_logger.LogDebug("<<< Exiting InsertCampaignAsync(campaign={Campaign})", campaign);
    }

    /// <summary>
    /// Updates a campaign
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCampaignAsync(Campaign campaign)
    {
		_logger.LogDebug(">>> Entering UpdateCampaignAsync(campaign={Campaign})", campaign);
        await _campaignRepository.UpdateAsync(campaign);
		_logger.LogDebug("<<< Exiting UpdateCampaignAsync(campaign={Campaign})", campaign);
    }

    /// <summary>
    /// Deleted a queued email
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCampaignAsync(Campaign campaign)
    {
		_logger.LogDebug(">>> Entering DeleteCampaignAsync(campaign={Campaign})", campaign);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _campaignRepository.DeleteAsync(campaign);
		_logger.LogInformation("DeleteCampaignAsync(campaign)={Campaign}: external service call took {Arg1} ms", campaign, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteCampaignAsync(campaign={Campaign})", campaign);
    }

    /// <summary>
    /// Gets a campaign by identifier
    /// </summary>
    /// <param name="campaignId">Campaign identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign
    /// </returns>
    public virtual async Task<Campaign> GetCampaignByIdAsync(int campaignId)
    {
		_logger.LogDebug(">>> Entering GetCampaignByIdAsync(campaignId={CampaignId})", campaignId);
		_logger.LogDebug("<<< Exiting GetCampaignByIdAsync(campaignId={CampaignId})", campaignId);
        return await _campaignRepository.GetByIdAsync(campaignId, cache => default);
    }

    /// <summary>
    /// Gets all campaigns
    /// </summary>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaigns
    /// </returns>
    public virtual async Task<IList<Campaign>> GetAllCampaignsAsync(int storeId = 0)
    {
		_logger.LogDebug(">>> Entering GetAllCampaignsAsync(storeId={StoreId})", storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var campaigns = await _campaignRepository.GetAllAsync(query =>
        {
            if (storeId > 0)
                query = query.Where(c => c.StoreId == storeId);

			_logger.LogInformation("GetAllCampaignsAsync(storeId)={StoreId}: find query executed in {Arg1} ms", storeId, __logStart.ElapsedMilliseconds);
            query = query.OrderBy(c => c.CreatedOnUtc);

		_logger.LogDebug("<<< Exiting GetAllCampaignsAsync(storeId={StoreId})", storeId);
            return query;
        });

        return campaigns;
    }

    /// <summary>
    /// Sends a campaign to specified emails
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="subscriptions">Subscriptions</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the otal emails sent
    /// </returns>
    public virtual async Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
        IEnumerable<NewsLetterSubscription> subscriptions)
    {
		_logger.LogDebug(">>> Entering SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions})", campaign, emailAccount, subscriptions);
        ArgumentNullException.ThrowIfNull(campaign);

        ArgumentNullException.ThrowIfNull(emailAccount);

        var totalEmailsSent = 0;

        foreach (var subscription in subscriptions)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(subscription.Email);
			_logger.LogDebug("SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions}): customer → {Customer}", campaign, emailAccount, subscriptions, customer);
            //ignore deleted or inactive customers when sending newsletter campaigns
            if (customer != null && (!customer.Active || customer.Deleted))
                continue;

            var tokens = new List<Token>();
			_logger.LogDebug("SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions}): tokens → {Tokens}", campaign, emailAccount, subscriptions, tokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount, subscription.LanguageId);
            await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(tokens, subscription);
            if (customer != null)
                await _messageTokenProvider.AddCustomerTokensAsync(tokens, customer);

            var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
			_logger.LogDebug("SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions}): subject → {Subject}", campaign, emailAccount, subscriptions, subject);
            var body = _tokenizer.Replace(campaign.Body, tokens, true);
			_logger.LogDebug("SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions}): body → {Body}", campaign, emailAccount, subscriptions, body);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.Low,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = subscription.Email,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = campaign.DontSendBeforeDateUtc
            };
            await _queuedEmailService.InsertQueuedEmailAsync(email);
            totalEmailsSent++;
        }

		_logger.LogDebug("<<< Exiting SendCampaignAsync(campaign={Campaign},emailAccount={EmailAccount},subscriptions={Subscriptions})", campaign, emailAccount, subscriptions);
        return totalEmailsSent;
    }

    /// <summary>
    /// Sends a campaign to specified email
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="email">Email</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email, int languageId)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        ArgumentNullException.ThrowIfNull(emailAccount);

        var tokens = new List<Token>();
        await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount, languageId);
        var customer = await _customerService.GetCustomerByEmailAsync(email);
        if (customer != null)
            await _messageTokenProvider.AddCustomerTokensAsync(tokens, customer);

        var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
        var body = _tokenizer.Replace(campaign.Body, tokens, true);

        await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
    }

    #endregion
}