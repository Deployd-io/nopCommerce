using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ForumController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly IForumService _forumService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public ForumController(IForumModelFactory forumModelFactory,
        IForumService forumService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
		_logger.LogDebug(">>> Entering ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService})", forumModelFactory, forumService, localizationService, notificationService, permissionService);
        _forumModelFactory = forumModelFactory;
		_logger.LogDebug("ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, forumService, localizationService, notificationService, permissionService, _forumModelFactory);
        _forumService = forumService;
		_logger.LogDebug("ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _forumService → {_forumService}", forumModelFactory, forumService, localizationService, notificationService, permissionService, _forumService);
        _localizationService = localizationService;
		_logger.LogDebug("ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", forumModelFactory, forumService, localizationService, notificationService, permissionService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", forumModelFactory, forumService, localizationService, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("ForumController(forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _permissionService → {_permissionService}", forumModelFactory, forumService, localizationService, notificationService, permissionService, _permissionService);
    }

    #endregion

    #region Methods

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.FORUMS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupSearchModelAsync(new ForumGroupSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_VIEW)]
    public virtual async Task<IActionResult> ForumGroupList(ForumGroupSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ForumGroupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupListModelAsync(searchModel);
		_logger.LogDebug("ForumGroupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ForumGroupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_VIEW)]
    public virtual async Task<IActionResult> ForumList(ForumSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ForumList(searchModel={SearchModel})", searchModel);
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(searchModel.ForumGroupId)
                         ?? throw new ArgumentException("No forum group found with the specified id");

        //prepare model
        var model = await _forumModelFactory.PrepareForumListModelAsync(searchModel, forumGroup);
		_logger.LogDebug("ForumList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ForumList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Create

    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateForumGroup()
    {
		_logger.LogDebug(">>> Entering CreateForumGroup()");
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupModelAsync(new ForumGroupModel(), null);
		_logger.LogDebug("CreateForumGroup(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting CreateForumGroup()");
		_logger.LogDebug("<<< Exiting CreateForumGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateForumGroup(ForumGroupModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var forumGroup = model.ToEntity<ForumGroup>();
            forumGroup.CreatedOnUtc = DateTime.UtcNow;
            forumGroup.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.InsertForumGroupAsync(forumGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Added"));

            return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumGroupModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateForum()
    {
		_logger.LogDebug(">>> Entering CreateForum()");
        //prepare model
        var model = await _forumModelFactory.PrepareForumModelAsync(new ForumModel(), null);
		_logger.LogDebug("CreateForum(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting CreateForum()");
		_logger.LogDebug("<<< Exiting CreateForum(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateForum(ForumModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var forum = model.ToEntity<Forum>();
            forum.CreatedOnUtc = DateTime.UtcNow;
            forum.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.InsertForumAsync(forum);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Added"));

            return continueEditing ? RedirectToAction("EditForum", new { forum.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion

    #region Edit

    [CheckPermission(StandardPermission.ContentManagement.FORUMS_VIEW)]
    public virtual async Task<IActionResult> EditForumGroup(int id)
    {
		_logger.LogDebug(">>> Entering EditForumGroup(id={Id})", id);
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
		_logger.LogDebug("EditForumGroup(id={Id}): forumGroup → {ForumGroup}", id, forumGroup);
        if (forumGroup == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupModelAsync(null, forumGroup);
		_logger.LogDebug("EditForumGroup(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting EditForumGroup(id={Id})", id);
		_logger.LogDebug("<<< Exiting EditForumGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditForumGroup(ForumGroupModel model, bool continueEditing)
    {
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(model.Id);
        if (forumGroup == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            forumGroup = model.ToEntity(forumGroup);
            forumGroup.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.UpdateForumGroupAsync(forumGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Updated"));

            return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumGroupModelAsync(model, forumGroup, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.FORUMS_VIEW)]
    public virtual async Task<IActionResult> EditForum(int id)
    {
		_logger.LogDebug(">>> Entering EditForum(id={Id})", id);
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogDebug("EditForum(id={Id}): forum → {Forum}", id, forum);
        if (forum == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _forumModelFactory.PrepareForumModelAsync(null, forum);
		_logger.LogDebug("EditForum(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting EditForum(id={Id})", id);
		_logger.LogDebug("<<< Exiting EditForum(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditForum(ForumModel model, bool continueEditing)
    {
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(model.Id);
        if (forum == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            forum = model.ToEntity(forum);
            forum.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.UpdateForumAsync(forum);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Updated"));

            return continueEditing ? RedirectToAction("EditForum", forum.Id) : RedirectToAction("List");
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumModelAsync(model, forum, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion

    #region Delete

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteForumGroup(int id)
    {
		_logger.LogDebug(">>> Entering DeleteForumGroup(id={Id})", id);
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
		_logger.LogDebug("DeleteForumGroup(id={Id}): forumGroup → {ForumGroup}", id, forumGroup);
        if (forumGroup == null)
            return RedirectToAction("List");

        await _forumService.DeleteForumGroupAsync(forumGroup);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Deleted"));

		_logger.LogDebug("<<< Exiting DeleteForumGroup(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteForum(int id)
    {
		_logger.LogDebug(">>> Entering DeleteForum(id={Id})", id);
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogDebug("DeleteForum(id={Id}): forum → {Forum}", id, forum);
        if (forum == null)
            return RedirectToAction("List");

        await _forumService.DeleteForumAsync(forum);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Deleted"));

		_logger.LogDebug("<<< Exiting DeleteForum(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #endregion
}