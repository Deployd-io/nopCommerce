using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PollController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPollModelFactory _pollModelFactory;
    protected readonly IPollService _pollService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public PollController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPollModelFactory pollModelFactory,
        IPollService pollService,
        IStoreMappingService storeMappingService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _pollModelFactory = pollModelFactory;
        _pollService = pollService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Polls

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _pollModelFactory.PreparePollSearchModelAsync(new PollSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> List(PollSearchModel searchModel)
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(new PollModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(PollModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var poll = model.ToEntity<Poll>();
            await _pollService.InsertPollAsync(poll);

            //save store mappings
            await _storeMappingService.SaveStoreMappingsAsync(poll, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): poll → {Poll}", id, poll);
        if (poll == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(null, poll);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(PollModel model, bool continueEditing)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(model.Id);
        if (poll == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            poll = model.ToEntity(poll);
            await _pollService.UpdatePollAsync(poll);

            //save store mappings
            await _storeMappingService.SaveStoreMappingsAsync(poll, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, poll, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): poll → {Poll}", id, poll);
        if (poll == null)
            return RedirectToAction("List");

        await _pollService.DeletePollAsync(poll);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #region Poll answer

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_VIEW)]
    public virtual async Task<IActionResult> PollAnswers(PollAnswerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PollAnswers(searchModel={SearchModel})", searchModel);
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(searchModel.PollId)
            ?? throw new ArgumentException("No poll found with the specified id");

        //prepare model
        var model = await _pollModelFactory.PreparePollAnswerListModelAsync(searchModel, poll);
		_logger.LogDebug("PollAnswers(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting PollAnswers(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerUpdate([Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(model.Id)
            ?? throw new ArgumentException("No poll answer found with the specified id");

        pollAnswer = model.ToEntity(pollAnswer);

        await _pollService.UpdatePollAnswerAsync(pollAnswer);

        return new NullJsonResult();
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerAdd(int pollId, [Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //fill entity from model
        await _pollService.InsertPollAnswerAsync(model.ToEntity<PollAnswer>());

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.POLLS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PollAnswerDelete(int id)
    {
		_logger.LogDebug(">>> Entering PollAnswerDelete(id={Id})", id);
        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(id)
            ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

        await _pollService.DeletePollAnswerAsync(pollAnswer);

		_logger.LogDebug("<<< Exiting PollAnswerDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}