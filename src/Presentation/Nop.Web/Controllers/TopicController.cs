using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Topics;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class TopicController : BasePublicController
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITopicModelFactory _topicModelFactory;
    protected readonly ITopicService _topicService;

    #endregion

    #region Ctor

    public TopicController(IAclService aclService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        ITopicModelFactory topicModelFactory,
        ITopicService topicService)
    {
        _aclService = aclService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _storeMappingService = storeMappingService;
        _topicModelFactory = topicModelFactory;
        _topicService = topicService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> TopicDetails(int topicId)
    {
		logger.LogDebug(">>> Entering TopicDetails(topicId={TopicId})", topicId);
        var topic = await _topicService.GetTopicByIdAsync(topicId);

		logger.LogDebug("TopicDetails(topicId={TopicId}): topic → {Topic}", topicId, topic);
        if (topic == null)
            return InvokeHttp404();

        var notAvailable = !topic.Published ||
                           //availability dates
                           !_topicService.TopicIsAvailable(topic) ||
                           //ACL (access control list)
                           !await _aclService.AuthorizeAsync(topic) ||
                           //store mapping
                           !await _storeMappingService.AuthorizeAsync(topic);

        //allow administrators to preview any topic
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.ContentManagement.TOPICS_VIEW);

		logger.LogDebug("TopicDetails(topicId={TopicId}): hasAdminAccess → {HasAdminAccess}", topicId, hasAdminAccess);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = await _topicModelFactory.PrepareTopicModelAsync(topic);

		logger.LogDebug("TopicDetails(topicId={TopicId}): model → {Model}", topicId, model);
        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.ADMIN }));

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
		logger.LogDebug("TopicDetails(topicId={TopicId}): templateViewPath → {TemplateViewPath}", topicId, templateViewPath);
		logger.LogDebug("<<< Exiting TopicDetails(topicId={TopicId})", topicId);
        return View(templateViewPath, model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> TopicDetailsPopup(string systemName)
    {
		logger.LogDebug(">>> Entering TopicDetailsPopup(systemName={SystemName})", systemName);
        var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
        if (model == null)
			logger.LogDebug("TopicDetailsPopup(systemName={SystemName}): model → {Model}", systemName, model);
            return InvokeHttp404();

        ViewBag.IsPopup = true;
		logger.LogDebug("TopicDetailsPopup(systemName={SystemName}): ViewBag.IsPopup → {Arg1}", systemName, ViewBag.IsPopup);

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
		logger.LogDebug("TopicDetailsPopup(systemName={SystemName}): templateViewPath → {TemplateViewPath}", systemName, templateViewPath);
		logger.LogDebug("<<< Exiting TopicDetailsPopup(systemName={SystemName})", systemName);
        return PartialView(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Authenticate(int id, string password)
    {
		logger.LogDebug(">>> Entering Authenticate(id={Id},password={Password})", id, password);
        var authResult = new AuthenticatedTopicModel();
        var topic = await _topicService.GetTopicByIdAsync(id);
		logger.LogDebug("Authenticate(id={Id},password={Password}): authResult → {AuthResult}", id, password, authResult);
		logger.LogDebug("Authenticate(id={Id},password={Password}): topic → {Topic}", id, password, topic);

        if (topic == null ||
            !topic.Published ||
            !_topicService.TopicIsAvailable(topic) ||
            !await _storeMappingService.AuthorizeAsync(topic) ||
            !await _aclService.AuthorizeAsync(topic))
        {
            return Json(authResult with { Error = "Topic is not found" });
        }

        if (topic.IsPasswordProtected)
        {
            if (topic.Password != null && topic.Password.Equals(password))
            {
                return Json(authResult with { 
                    Authenticated = true, 
                    Title = await _localizationService.GetLocalizedAsync(topic, x => x.Title),
                    Body = await _localizationService.GetLocalizedAsync(topic, x => x.Body)
                });
					logger.LogDebug("Authenticate(id={Id},password={Password}): Authenticated → {Authenticated}", id, password, Authenticated);
            }
            else
            {
                return Json(authResult with { Error = await _localizationService.GetResourceAsync("Topic.WrongPassword") });
            }
        }

		logger.LogDebug("<<< Exiting Authenticate(id={Id},password={Password})", id, password);
        return Json(authResult);
    }

    #endregion
}