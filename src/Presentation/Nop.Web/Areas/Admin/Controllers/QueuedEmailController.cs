using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class QueuedEmailController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IQueuedEmailModelFactory _queuedEmailModelFactory;
    protected readonly IQueuedEmailService _queuedEmailService;

    #endregion

    #region Ctor

    public QueuedEmailController(IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IQueuedEmailModelFactory queuedEmailModelFactory,
        IQueuedEmailService queuedEmailService)
    {
		_logger.LogDebug(">>> Entering QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService})", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _dateTimeHelper → {_dateTimeHelper}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _dateTimeHelper);
        _localizationService = localizationService;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _localizationService → {_localizationService}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _notificationService → {_notificationService}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _permissionService → {_permissionService}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _permissionService);
        _queuedEmailModelFactory = queuedEmailModelFactory;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _queuedEmailModelFactory → {_queuedEmailModelFactory}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _queuedEmailModelFactory);
        _queuedEmailService = queuedEmailService;
		_logger.LogDebug("QueuedEmailController(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},queuedEmailModelFactory={QueuedEmailModelFactory},queuedEmailService={QueuedEmailService}): _queuedEmailService → {_queuedEmailService}", dateTimeHelper, localizationService, notificationService, permissionService, queuedEmailModelFactory, queuedEmailService, _queuedEmailService);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _queuedEmailModelFactory.PrepareQueuedEmailSearchModelAsync(new QueuedEmailSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> QueuedEmailList(QueuedEmailSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering QueuedEmailList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _queuedEmailModelFactory.PrepareQueuedEmailListModelAsync(searchModel);
		_logger.LogDebug("QueuedEmailList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting QueuedEmailList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("go-to-email-by-number")]
    public virtual async Task<IActionResult> GoToEmailByNumber(QueuedEmailSearchModel model)
    {
		_logger.LogDebug(">>> Entering GoToEmailByNumber(model={Model})", model);
        //try to get a queued email with the specified id
        var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(model.GoDirectlyToNumber);
		_logger.LogDebug("GoToEmailByNumber(model={Model}): queuedEmail → {QueuedEmail}", model, queuedEmail);
        if (queuedEmail == null)
            return await List();

        return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
    }

    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a queued email with the specified id
        var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): email → {Email}", id, email);
        if (email == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _queuedEmailModelFactory.PrepareQueuedEmailModelAsync(null, email);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> Edit(QueuedEmailModel model, bool continueEditing)
    {
        //try to get a queued email with the specified id
        var email = await _queuedEmailService.GetQueuedEmailByIdAsync(model.Id);
        if (email == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            email = model.ToEntity(email);
            email.DontSendBeforeDateUtc = model.SendImmediately || !model.DontSendBeforeDate.HasValue ?
                null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value);
            await _queuedEmailService.UpdateQueuedEmailAsync(email);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _queuedEmailModelFactory.PrepareQueuedEmailModelAsync(model, email, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> Requeue(QueuedEmailModel queuedEmailModel)
    {
		_logger.LogDebug(">>> Entering Requeue(queuedEmailModel={QueuedEmailModel})", queuedEmailModel);
        //try to get a queued email with the specified id
        var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(queuedEmailModel.Id);
		_logger.LogDebug("Requeue(queuedEmailModel={QueuedEmailModel}): queuedEmail → {QueuedEmail}", queuedEmailModel, queuedEmail);
        if (queuedEmail == null)
            return RedirectToAction("List");

        var requeuedEmail = new QueuedEmail
        {
            PriorityId = queuedEmail.PriorityId,
            From = queuedEmail.From,
            FromName = queuedEmail.FromName,
            To = queuedEmail.To,
            ToName = queuedEmail.ToName,
            ReplyTo = queuedEmail.ReplyTo,
            ReplyToName = queuedEmail.ReplyToName,
            CC = queuedEmail.CC,
            Bcc = queuedEmail.Bcc,
            Subject = queuedEmail.Subject,
            Body = queuedEmail.Body,
            AttachmentFilePath = queuedEmail.AttachmentFilePath,
            AttachmentFileName = queuedEmail.AttachmentFileName,
            AttachedDownloadId = queuedEmail.AttachedDownloadId,
            CreatedOnUtc = DateTime.UtcNow,
            EmailAccountId = queuedEmail.EmailAccountId,
            DontSendBeforeDateUtc = queuedEmailModel.SendImmediately || !queuedEmailModel.DontSendBeforeDate.HasValue ?
                null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(queuedEmailModel.DontSendBeforeDate.Value)
        };
        await _queuedEmailService.InsertQueuedEmailAsync(requeuedEmail);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Requeued"));

		_logger.LogDebug("<<< Exiting Requeue(queuedEmailModel={QueuedEmailModel})", queuedEmailModel);
        return RedirectToAction("Edit", new { id = requeuedEmail.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a queued email with the specified id
        var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): email → {Email}", id, email);
        if (email == null)
            return RedirectToAction("List");

        await _queuedEmailService.DeleteQueuedEmailAsync(email);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _queuedEmailService.DeleteQueuedEmailsAsync(await _queuedEmailService.GetQueuedEmailsByIdsAsync(selectedIds.ToArray()));

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("delete-all")]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> DeleteAll()
    {
		_logger.LogDebug(">>> Entering DeleteAll()");
        await _queuedEmailService.DeleteAllEmailsAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.QueuedEmails.DeletedAll"));

		_logger.LogDebug("<<< Exiting DeleteAll()");
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MESSAGE_QUEUE)]
    public virtual async Task<IActionResult> RequeueSelected(int[] selectedIds)
    {
		_logger.LogDebug(">>> Entering RequeueSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || selectedIds.Length == 0)
            return NoContent();

        var emailsToRequeue = await _queuedEmailService.GetQueuedEmailsByIdsAsync(selectedIds);
		_logger.LogDebug("RequeueSelected(selectedIds={SelectedIds}): emailsToRequeue → {EmailsToRequeue}", selectedIds, emailsToRequeue);

        if (!emailsToRequeue.Any())
            return NotFound();

        await _queuedEmailService.RequeueQueuedEmailsAsync(emailsToRequeue);

		_logger.LogDebug("<<< Exiting RequeueSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion
}