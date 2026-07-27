using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class PrivateMessagesController : BasePublicController
{
    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IForumService _forumService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PrivateMessagesController(ForumSettings forumSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IForumService forumService,
        ILocalizationService localizationService,
        IPrivateMessagesModelFactory privateMessagesModelFactory,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _forumSettings = forumSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _forumService = forumService;
        _localizationService = localizationService;
        _privateMessagesModelFactory = privateMessagesModelFactory;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
    {
		logger.LogDebug(">>> Entering Index(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        if (!_forumSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
        {
            return Challenge();
        }

        var model = await _privateMessagesModelFactory.PreparePrivateMessageIndexModelAsync(pageNumber, tab);
		logger.LogDebug("Index(pageNumber={PageNumber},tab={Tab}): model → {Model}", pageNumber, tab, model);
		logger.LogDebug("<<< Exiting Index(pageNumber={PageNumber},tab={Tab})", pageNumber, tab);
        return View(model);
    }

    [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
    public virtual async Task<IActionResult> DeleteInboxPM(IFormCollection formCollection)
    {
		logger.LogDebug(">>> Entering DeleteInboxPM(formCollection={FormCollection})", formCollection);
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("pm", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
				logger.LogDebug("DeleteInboxPM(formCollection={FormCollection}): id → {Id}", formCollection, id);
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
					logger.LogDebug("DeleteInboxPM(formCollection={FormCollection}): pm → {Pm}", formCollection, pm);
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

						logger.LogDebug("DeleteInboxPM(formCollection={FormCollection}): customer → {Customer}", formCollection, customer);
                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsDeletedByRecipient = true;
							logger.LogDebug("DeleteInboxPM(formCollection={FormCollection}): pm.IsDeletedByRecipient → {Arg1}", formCollection, pm.IsDeletedByRecipient);
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
		logger.LogDebug("<<< Exiting DeleteInboxPM(formCollection={FormCollection})", formCollection);
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
    public virtual async Task<IActionResult> MarkUnread(IFormCollection formCollection)
    {
		logger.LogDebug(">>> Entering MarkUnread(formCollection={FormCollection})", formCollection);
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("pm", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
				logger.LogDebug("MarkUnread(formCollection={FormCollection}): id → {Id}", formCollection, id);
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
					logger.LogDebug("MarkUnread(formCollection={FormCollection}): pm → {Pm}", formCollection, pm);
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

						logger.LogDebug("MarkUnread(formCollection={FormCollection}): customer → {Customer}", formCollection, customer);
                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsRead = false;
							logger.LogDebug("MarkUnread(formCollection={FormCollection}): pm.IsRead → {Arg1}", formCollection, pm.IsRead);
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
		logger.LogDebug("<<< Exiting MarkUnread(formCollection={FormCollection})", formCollection);
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    //updates sent items (deletes PrivateMessages)
    [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
    public virtual async Task<IActionResult> DeleteSentPM(IFormCollection formCollection)
    {
		logger.LogDebug(">>> Entering DeleteSentPM(formCollection={FormCollection})", formCollection);
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("si", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
				logger.LogDebug("DeleteSentPM(formCollection={FormCollection}): id → {Id}", formCollection, id);
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
					logger.LogDebug("DeleteSentPM(formCollection={FormCollection}): pm → {Pm}", formCollection, pm);
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

						logger.LogDebug("DeleteSentPM(formCollection={FormCollection}): customer → {Customer}", formCollection, customer);
                        if (pm.FromCustomerId == customer.Id)
                        {
                            pm.IsDeletedByAuthor = true;
							logger.LogDebug("DeleteSentPM(formCollection={FormCollection}): pm.IsDeletedByAuthor → {Arg1}", formCollection, pm.IsDeletedByAuthor);
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
		logger.LogDebug("<<< Exiting DeleteSentPM(formCollection={FormCollection})", formCollection);
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
    }

    public virtual async Task<IActionResult> SendPM(int toCustomerId, int? replyToMessageId)
    {
		logger.LogDebug(">>> Entering SendPM(toCustomerId={ToCustomerId},replyToMessageId={ReplyToMessageId})", toCustomerId, replyToMessageId);
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var customerTo = await _customerService.GetCustomerByIdAsync(toCustomerId);
        if (customerTo == null || await _customerService.IsGuestAsync(customerTo))
			logger.LogDebug("SendPM(toCustomerId={ToCustomerId},replyToMessageId={ReplyToMessageId}): customerTo → {CustomerTo}", toCustomerId, replyToMessageId, customerTo);
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

        PrivateMessage replyToPM = null;
        if (replyToMessageId.HasValue)
		logger.LogDebug("SendPM(toCustomerId={ToCustomerId},replyToMessageId={ReplyToMessageId}): replyToPM → {ReplyToPM}", toCustomerId, replyToMessageId, replyToPM);
        {
            //reply to a previous PM
            replyToPM = await _forumService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
        }

        var model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(customerTo, replyToPM);
		logger.LogDebug("SendPM(toCustomerId={ToCustomerId},replyToMessageId={ReplyToMessageId}): model → {Model}", toCustomerId, replyToMessageId, model);
		logger.LogDebug("<<< Exiting SendPM(toCustomerId={ToCustomerId},replyToMessageId={ReplyToMessageId})", toCustomerId, replyToMessageId);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
    {
        if (!_forumSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
        {
            return Challenge();
        }

        Customer toCustomer;
        var replyToPM = await _forumService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
        if (replyToPM != null)
        {
            //reply to a previous PM
            if (replyToPM.ToCustomerId == customer.Id || replyToPM.FromCustomerId == customer.Id)
            {
                //Reply to already sent PM (by current customer) should not be sent to yourself
                toCustomer = await _customerService.GetCustomerByIdAsync(replyToPM.FromCustomerId == customer.Id
                    ? replyToPM.ToCustomerId
                    : replyToPM.FromCustomerId);
            }
            else
            {
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
            }
        }
        else
        {
            //first PM
            toCustomer = await _customerService.GetCustomerByIdAsync(model.ToCustomerId);
        }

        if (toCustomer == null || await _customerService.IsGuestAsync(toCustomer))
        {
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
        }

        if (ModelState.IsValid)
        {
            try
            {
                var subject = model.Subject;
                if (_forumSettings.PMSubjectMaxLength > 0 && subject.Length > _forumSettings.PMSubjectMaxLength)
                {
                    subject = subject[0.._forumSettings.PMSubjectMaxLength];
                }

                var text = model.Message;
                if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                {
                    text = text[0.._forumSettings.PMTextMaxLength];
                }

                var nowUtc = DateTime.UtcNow;
                var store = await _storeContext.GetCurrentStoreAsync();

                var privateMessage = new PrivateMessage
                {
                    StoreId = store.Id,
                    ToCustomerId = toCustomer.Id,
                    FromCustomerId = customer.Id,
                    Subject = subject,
                    Text = text,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = nowUtc
                };

                await _forumService.InsertPrivateMessageAsync(privateMessage);

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.SendPM",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(toCustomer, replyToPM);
		logger.LogDebug("SendPM(model={Model}): model → {Model1}", model, model);
		logger.LogDebug("<<< Exiting SendPM(model={Model})", model);
        return View(model);
    }

    public virtual async Task<IActionResult> ViewPM(int privateMessageId)
    {
		logger.LogDebug(">>> Entering ViewPM(privateMessageId={PrivateMessageId})", privateMessageId);
        if (!_forumSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
		logger.LogDebug("ViewPM(privateMessageId={PrivateMessageId}): customer → {Customer}", privateMessageId, customer);
        {
            return Challenge();
        }

        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
		logger.LogDebug("ViewPM(privateMessageId={PrivateMessageId}): pm → {Pm}", privateMessageId, pm);
        {
            if (pm.ToCustomerId != customer.Id && pm.FromCustomerId != customer.Id)
            {
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
            }

            if (!pm.IsRead && pm.ToCustomerId == customer.Id)
            {
                pm.IsRead = true;
				logger.LogDebug("ViewPM(privateMessageId={PrivateMessageId}): pm.IsRead → {Arg1}", privateMessageId, pm.IsRead);
                await _forumService.UpdatePrivateMessageAsync(pm);
            }
        }
        else
        {
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
        }

        var model = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
		logger.LogDebug("ViewPM(privateMessageId={PrivateMessageId}): model → {Model}", privateMessageId, model);
		logger.LogDebug("<<< Exiting ViewPM(privateMessageId={PrivateMessageId})", privateMessageId);
        return View(model);
    }

    public virtual async Task<IActionResult> DeletePM(int privateMessageId)
    {
		logger.LogDebug(">>> Entering DeletePM(privateMessageId={PrivateMessageId})", privateMessageId);
        if (!_forumSettings.AllowPrivateMessages)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
		logger.LogDebug("DeletePM(privateMessageId={PrivateMessageId}): customer → {Customer}", privateMessageId, customer);
        {
            return Challenge();
        }

        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
		logger.LogDebug("DeletePM(privateMessageId={PrivateMessageId}): pm → {Pm}", privateMessageId, pm);
        {
            if (pm.FromCustomerId == customer.Id)
            {
                pm.IsDeletedByAuthor = true;
				logger.LogDebug("DeletePM(privateMessageId={PrivateMessageId}): pm.IsDeletedByAuthor → {Arg1}", privateMessageId, pm.IsDeletedByAuthor);
                await _forumService.UpdatePrivateMessageAsync(pm);
            }

            if (pm.ToCustomerId == customer.Id)
            {
                pm.IsDeletedByRecipient = true;
				logger.LogDebug("DeletePM(privateMessageId={PrivateMessageId}): pm.IsDeletedByRecipient → {Arg1}", privateMessageId, pm.IsDeletedByRecipient);
                await _forumService.UpdatePrivateMessageAsync(pm);
            }
        }
		logger.LogDebug("<<< Exiting DeletePM(privateMessageId={PrivateMessageId})", privateMessageId);
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    #endregion
}