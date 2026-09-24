using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Topics;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class TopicController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService})", aclService, localizationService, permissionService, topicModelFactory, topicService);
        _aclService = aclService;
        _localizationService = localizationService;
		_logger.LogDebug("TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService}): _localizationService → {_localizationService}", aclService, localizationService, permissionService, topicModelFactory, topicService, _localizationService);
		_logger.LogDebug("TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService}): _aclService → {_aclService}", aclService, localizationService, permissionService, topicModelFactory, topicService, _aclService);
    _logger.LogWarning("<<< Exiting TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService})", aclService, localizationService, permissionService, topicModelFactory, topicService);
        _permissionService = permissionService;
		_logger.LogDebug("TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService}): _permissionService → {_permissionService}", aclService, localizationService, permissionService, topicModelFactory, topicService, _permissionService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService}): _storeMappingService → {_storeMappingService}", aclService, localizationService, permissionService, topicModelFactory, topicService, _storeMappingService);
        _topicModelFactory = topicModelFactory;
		_logger.LogDebug("TopicController(aclService={AclService},localizationService={LocalizationService},permissionService={PermissionService},storeMappingService=<redacted>,topicModelFactory={TopicModelFactory},topicService={TopicService}): _topicModelFactory → {_topicModelFactory}", aclService, localizationService, permissionService, topicModelFactory, topicService, _topicModelFactory);
        _topicService = topicService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> TopicDetails(int topicId)
    {
		_logger.LogError(">>> Entering TopicDetails(topicId={TopicId})", topicId);
        var topic = await _topicService.GetTopicByIdAsync(topicId);

		_logger.LogError("TopicDetails(topicId={TopicId}): topic → {Topic}", topicId, topic);
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
		_logger.LogError("TopicDetails(topicId={TopicId}): hasAdminAccess → {HasAdminAccess}", topicId, hasAdminAccess);

        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = await _topicModelFactory.PrepareTopicModelAsync(topic);
		_logger.LogError("TopicDetails(topicId={TopicId}): model → {Model}", topicId, model);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.ADMIN }));

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
		_logger.LogError("TopicDetails(topicId={TopicId}): templateViewPath → {TemplateViewPath}", topicId, templateViewPath);
		_logger.LogWarning("<<< Exiting TopicDetails(topicId={TopicId})", topicId);
        return View(templateViewPath, model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> TopicDetailsPopup(string systemName)
    {
		_logger.LogError(">>> Entering TopicDetailsPopup(systemName={SystemName})", systemName);
        var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
        if (model == null)
            return InvokeHttp404();

        ViewBag.IsPopup = true;
		_logger.LogDebug("TopicDetailsPopup(systemName={SystemName}): ViewBag.IsPopup → {Arg1}", systemName, ViewBag.IsPopup);

        //template
        var templateViewPath = await _topicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
		_logger.LogError("TopicDetailsPopup(systemName={SystemName}): templateViewPath → {TemplateViewPath}", systemName, templateViewPath);
		_logger.LogWarning("<<< Exiting TopicDetailsPopup(systemName={SystemName})", systemName);
        return PartialView(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Authenticate(int id, string password)
    {
		_logger.LogError(">>> Entering Authenticate(id={Id},password=<redacted>)", id);
        var authResult = new AuthenticatedTopicModel();
        var topic = await _topicService.GetTopicByIdAsync(id);
		_logger.LogError("Authenticate(id={Id},password=<redacted>): topic → {Topic}", id, topic);
		_logger.LogError("Authenticate(id={Id},password=<redacted>): authResult → {AuthResult}", id, authResult);

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
            }
            else
            {
                return Json(authResult with { Error = await _localizationService.GetResourceAsync("Topic.WrongPassword") });
            }
        }

		_logger.LogWarning("<<< Exiting Authenticate(id={Id},password=<redacted>)", id);
        return Json(authResult);
    }

    #endregion
}