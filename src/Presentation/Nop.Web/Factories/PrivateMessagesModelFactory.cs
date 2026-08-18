using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Common;
using Nop.Web.Models.PrivateMessages;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the private message model factory
/// </summary>
public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IForumService _forumService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PrivateMessagesModelFactory(CustomerSettings customerSettings,
        ForumSettings forumSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IForumService forumService,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext})", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext);
        _customerSettings = customerSettings;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _customerSettings → {_customerSettings}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _customerSettings);
        _forumSettings = forumSettings;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _forumSettings → {_forumSettings}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _forumSettings);
        _customerService = customerService;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _customerService → {_customerService}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _dateTimeHelper);
        _forumService = forumService;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _forumService → {_forumService}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _forumService);
        _localizationService = localizationService;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _localizationService → {_localizationService}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _localizationService);
        _storeContext = storeContext;
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext})", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext);
		_logger.LogDebug("PrivateMessagesModelFactory(customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _workContext → {_workContext}", customerSettings, forumSettings, customerService, dateTimeHelper, forumService, localizationService, storeContext, workContext, _workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the private message index model
    /// </summary>
    /// <param name="page">Number of items page; pass null to disable paging</param>
    /// <param name="tab">Tab name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private message index model
    /// </returns>
    public virtual Task<PrivateMessageIndexModel> PreparePrivateMessageIndexModelAsync(int? page, string tab)
    {
		_logger.LogDebug(">>> Entering PreparePrivateMessageIndexModelAsync(page={Page},tab={Tab})", page, tab);
        var inboxPage = 0;
        var sentItemsPage = 0;
		_logger.LogDebug("PreparePrivateMessageIndexModelAsync(page={Page},tab={Tab}): sentItemsPage → {SentItemsPage}", page, tab, sentItemsPage);
        var sentItemsTabSelected = false;
		_logger.LogDebug("PreparePrivateMessageIndexModelAsync(page={Page},tab={Tab}): sentItemsTabSelected → {SentItemsTabSelected}", page, tab, sentItemsTabSelected);
		_logger.LogDebug("PreparePrivateMessageIndexModelAsync(page={Page},tab={Tab}): inboxPage → {InboxPage}", page, tab, inboxPage);

        switch (tab)
        {
            case "inbox":
                if (page.HasValue)
                {
                    inboxPage = page.Value;
                }

                break;
            case "sent":
                if (page.HasValue)
                {
                    sentItemsPage = page.Value;
                }

                sentItemsTabSelected = true;

                break;
            default:
                break;
        }

        var model = new PrivateMessageIndexModel
        {
            InboxPage = inboxPage,
            SentItemsPage = sentItemsPage,
            SentItemsTabSelected = sentItemsTabSelected
        };

		_logger.LogDebug("<<< Exiting PreparePrivateMessageIndexModelAsync(page={Page},tab={Tab})", page, tab);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the inbox model
    /// </summary>
    /// <param name="page">Number of items page</param>
    /// <param name="tab">Tab name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private message list model
    /// </returns>
    public virtual async Task<PrivateMessageListModel> PrepareInboxModelAsync(int page, string tab)
    {
		_logger.LogDebug(">>> Entering PrepareInboxModelAsync(page={Page},tab={Tab})", page, tab);
        if (page > 0)
        {
            page -= 1;
        }

        var pageSize = _forumSettings.PrivateMessagesPageSize;

        var messages = new List<PrivateMessageModel>();
		_logger.LogDebug("PrepareInboxModelAsync(page={Page},tab={Tab}): messages → {Messages}", page, tab, messages);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareInboxModelAsync(page={Page},tab={Tab}): store → {Store}", page, tab, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareInboxModelAsync(page={Page},tab={Tab}): customer → {Customer}", page, tab, customer);
        var list = await _forumService.GetAllPrivateMessagesAsync(store.Id,
            0, customer.Id, null, null, false, string.Empty, page, pageSize);

        foreach (var pm in list)
            messages.Add(await PreparePrivateMessageModelAsync(pm));

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.PRIVATE_MESSAGES_PAGED,
            UseRouteLinks = true,
            RouteValues = new PrivateMessageRouteValues { PageNumber = page, Tab = tab }
        };

        var model = new PrivateMessageListModel
        {
            Messages = messages,
            PagerModel = pagerModel
        };

		_logger.LogDebug("<<< Exiting PrepareInboxModelAsync(page={Page},tab={Tab})", page, tab);
        return model;
    }

    /// <summary>
    /// Prepare the sent model
    /// </summary>
    /// <param name="page">Number of items page</param>
    /// <param name="tab">Tab name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private message list model
    /// </returns>
    public virtual async Task<PrivateMessageListModel> PrepareSentModelAsync(int page, string tab)
    {
		_logger.LogDebug(">>> Entering PrepareSentModelAsync(page={Page},tab={Tab})", page, tab);
        if (page > 0)
        {
            page -= 1;
        }

        var pageSize = _forumSettings.PrivateMessagesPageSize;

        var messages = new List<PrivateMessageModel>();
		_logger.LogDebug("PrepareSentModelAsync(page={Page},tab={Tab}): messages → {Messages}", page, tab, messages);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareSentModelAsync(page={Page},tab={Tab}): store → {Store}", page, tab, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareSentModelAsync(page={Page},tab={Tab}): customer → {Customer}", page, tab, customer);
        var list = await _forumService.GetAllPrivateMessagesAsync(store.Id,
            customer.Id, 0, null, false, null, string.Empty, page, pageSize);
        foreach (var pm in list)
            messages.Add(await PreparePrivateMessageModelAsync(pm));

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.PRIVATE_MESSAGES_PAGED,
            UseRouteLinks = true,
            RouteValues = new PrivateMessageRouteValues { PageNumber = page, Tab = tab }
        };

        var model = new PrivateMessageListModel
        {
            Messages = messages,
            PagerModel = pagerModel
        };

		_logger.LogDebug("<<< Exiting PrepareSentModelAsync(page={Page},tab={Tab})", page, tab);
        return model;
    }

    /// <summary>
    /// Prepare the send private message model
    /// </summary>
    /// <param name="customerTo">Customer, recipient of the message</param>
    /// <param name="replyToPM">Private message, pass if reply to a previous message is need</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the send private message model
    /// </returns>
    public virtual async Task<SendPrivateMessageModel> PrepareSendPrivateMessageModelAsync(Customer customerTo, PrivateMessage replyToPM)
    {
		_logger.LogDebug(">>> Entering PrepareSendPrivateMessageModelAsync(customerTo={CustomerTo},replyToPM={ReplyToPM})", customerTo, replyToPM);
        ArgumentNullException.ThrowIfNull(customerTo);

        var model = new SendPrivateMessageModel
        {
            ToCustomerId = customerTo.Id,
            CustomerToName = await _customerService.FormatUsernameAsync(customerTo),
            AllowViewingToProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customerTo)
        };

        if (replyToPM == null)
            return model;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareSendPrivateMessageModelAsync(customerTo={CustomerTo},replyToPM={ReplyToPM}): customer → {Customer}", customerTo, replyToPM, customer);
        if (replyToPM.ToCustomerId == customer.Id ||
            replyToPM.FromCustomerId == customer.Id)
        {
            model.ReplyToMessageId = replyToPM.Id;
			_logger.LogDebug("PrepareSendPrivateMessageModelAsync(customerTo={CustomerTo},replyToPM={ReplyToPM}): model.ReplyToMessageId → {Arg2}", customerTo, replyToPM, model.ReplyToMessageId);
			_logger.LogDebug("PrepareSendPrivateMessageModelAsync(customerTo={CustomerTo},replyToPM={ReplyToPM}): model.Subject → {Arg2}", customerTo, replyToPM, model.Subject);
            model.Subject = $"Re: {replyToPM.Subject}";
        }

		_logger.LogDebug("<<< Exiting PrepareSendPrivateMessageModelAsync(customerTo={CustomerTo},replyToPM={ReplyToPM})", customerTo, replyToPM);
        return model;
    }

    /// <summary>
    /// Prepare the private message model
    /// </summary>
    /// <param name="pm">Private message</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private message model
    /// </returns>
    public virtual async Task<PrivateMessageModel> PreparePrivateMessageModelAsync(PrivateMessage pm)
    {
		_logger.LogDebug(">>> Entering PreparePrivateMessageModelAsync(pm={Pm})", pm);
        ArgumentNullException.ThrowIfNull(pm);

        var fromCustomer = await _customerService.GetCustomerByIdAsync(pm.FromCustomerId);
		_logger.LogDebug("PreparePrivateMessageModelAsync(pm={Pm}): fromCustomer → {FromCustomer}", pm, fromCustomer);
        var toCustomer = await _customerService.GetCustomerByIdAsync(pm.ToCustomerId);
		_logger.LogDebug("PreparePrivateMessageModelAsync(pm={Pm}): toCustomer → {ToCustomer}", pm, toCustomer);

        var model = new PrivateMessageModel
        {
            Id = pm.Id,
            FromCustomerId = pm.FromCustomerId,
            CustomerFromName = await _customerService.FormatUsernameAsync(fromCustomer),
            AllowViewingFromProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(fromCustomer),
            ToCustomerId = pm.ToCustomerId,
            CustomerToName = await _customerService.FormatUsernameAsync(toCustomer),
            AllowViewingToProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(toCustomer),
            Subject = pm.Subject,
            Message = _forumService.FormatPrivateMessageText(pm),
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc),
            IsRead = pm.IsRead,
        };

		_logger.LogDebug("<<< Exiting PreparePrivateMessageModelAsync(pm={Pm})", pm);
        return model;
    }

    #endregion

    #region Nested class

    /// <summary>
    /// record that has a slug and page for route values. Used for Private Messages pagination
    /// </summary>
    public partial record PrivateMessageRouteValues : BaseRouteValues
    {
        public string Tab { get; set; }
    }

    #endregion
}