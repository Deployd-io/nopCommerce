using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class MessageTemplateController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IMessageTemplateModelFactory _messageTemplateModelFactory;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion Fields

    #region Ctor

    public MessageTemplateController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IMessageTemplateModelFactory messageTemplateModelFactory,
        IMessageTemplateService messageTemplateService,
        INotificationService notificationService,
        IStoreMappingService storeMappingService,
        IWorkflowMessageService workflowMessageService)
    {
		_logger.LogError(">>> Entering MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService})", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService);
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _localizedEntityService);
        _messageTemplateModelFactory = messageTemplateModelFactory;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _messageTemplateModelFactory → {_messageTemplateModelFactory}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _messageTemplateModelFactory);
        _messageTemplateService = messageTemplateService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _messageTemplateService → {_messageTemplateService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _messageTemplateService);
        _notificationService = notificationService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _notificationService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _storeMappingService → {_storeMappingService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _storeMappingService);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("MessageTemplateController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},messageTemplateModelFactory={MessageTemplateModelFactory},messageTemplateService={MessageTemplateService},notificationService={NotificationService},storeMappingService=<redacted>,workflowMessageService={WorkflowMessageService}): _workflowMessageService → {_workflowMessageService}", customerActivityService, localizationService, localizedEntityService, messageTemplateModelFactory, messageTemplateService, notificationService, workflowMessageService, _workflowMessageService);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(MessageTemplate mt, MessageTemplateModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(mt={Mt},model={Model})", mt, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.BccEmailAddresses,
                localized.BccEmailAddresses,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.Subject,
                localized.Subject,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.Body,
                localized.Body,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(mt,
                x => x.EmailAccountId,
                localized.EmailAccountId,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(mt={Mt},model={Model})", mt, model);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateSearchModelAsync(new MessageTemplateSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> List(MessageTemplateSearchModel searchModel)
    {
        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
		_logger.LogError("Edit(id={Id}): messageTemplate → {MessageTemplate}", id, messageTemplate);
        if (messageTemplate == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(null, messageTemplate);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(MessageTemplateModel model, bool continueEditing)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            messageTemplate = model.ToEntity(messageTemplate);

            //attached file
            if (!model.HasAttachedDownload)
                messageTemplate.AttachedDownloadId = 0;
            if (model.SendImmediately)
                messageTemplate.DelayBeforeSend = null;
            await _messageTemplateService.UpdateMessageTemplateAsync(messageTemplate);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMessageTemplate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMessageTemplate"), messageTemplate.Id), messageTemplate);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(messageTemplate, model.SelectedStoreIds);

            //locales
            await UpdateLocalesAsync(messageTemplate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = messageTemplate.Id });
        }

        //prepare model
        model = await _messageTemplateModelFactory.PrepareMessageTemplateModelAsync(model, messageTemplate, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
		_logger.LogError("Delete(id={Id}): messageTemplate → {MessageTemplate}", id, messageTemplate);
        if (messageTemplate == null)
            return RedirectToAction("List");

        await _messageTemplateService.DeleteMessageTemplateAsync(messageTemplate);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMessageTemplate",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id), messageTemplate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("message-template-copy")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CopyTemplate(MessageTemplateModel model)
    {
		_logger.LogError(">>> Entering CopyTemplate(model={Model})", model);
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
		_logger.LogError("CopyTemplate(model={Model}): messageTemplate → {MessageTemplate}", model, messageTemplate);
        if (messageTemplate == null)
            return RedirectToAction("List");

        try
        {
            var newMessageTemplate = await _messageTemplateService.CopyMessageTemplateAsync(messageTemplate);
			_logger.LogError("CopyTemplate(model={Model}): newMessageTemplate → {NewMessageTemplate}", model, newMessageTemplate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Copied"));

            return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in CopyTemplate(model={Model}): {Arg1}", model, exc.Message);
            _notificationService.ErrorNotification(exc.Message);
			_logger.LogWarning("<<< Exiting CopyTemplate(model={Model})", model);
            return RedirectToAction("Edit", new { id = model.Id });
        }
    }

    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW)]
    public virtual async Task<IActionResult> TestTemplate(int id, int languageId = 0)
    {
		_logger.LogError(">>> Entering TestTemplate(id={Id},languageId={LanguageId})", id, languageId);
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(id);
		_logger.LogError("TestTemplate(id={Id},languageId={LanguageId}): messageTemplate → {MessageTemplate}", id, languageId, messageTemplate);
        if (messageTemplate == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _messageTemplateModelFactory
            .PrepareTestMessageTemplateModelAsync(new TestMessageTemplateModel(), messageTemplate, languageId);

		_logger.LogWarning("<<< Exiting TestTemplate(model={Model},form={Form})", model, form);
        return View(model);
    }

    [HttpPost, ActionName("TestTemplate")]
    [FormValueRequired("send-test")]
    [CheckPermission(StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TestTemplate(TestMessageTemplateModel model, IFormCollection form)
    {
        //try to get a message template with the specified id
        var messageTemplate = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
        if (messageTemplate == null)
            return RedirectToAction("List");

        var tokens = new List<Token>();
        foreach (var formKey in form.Keys)
            if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
            {
                var tokenKey = formKey["token_".Length..].Replace("%", string.Empty);
                var stringValue = form[formKey].ToString();

                //try get non-string value
                object tokenValue;
                if (bool.TryParse(stringValue, out var boolValue))
                    tokenValue = boolValue;
                else if (int.TryParse(stringValue, out var intValue))
                    tokenValue = intValue;
                else if (decimal.TryParse(stringValue, out var decimalValue))
                    tokenValue = decimalValue;
                else
                    tokenValue = stringValue;

                tokens.Add(new Token(tokenKey, tokenValue));
            }

        await _workflowMessageService.SendTestEmailAsync(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

        if (ModelState.IsValid)
        {
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Test.Success"));
        }

        return RedirectToAction("Edit", new { id = messageTemplate.Id });
    }

    #endregion
}