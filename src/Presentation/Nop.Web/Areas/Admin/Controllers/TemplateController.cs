using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class TemplateController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICategoryTemplateService _categoryTemplateService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerTemplateService _manufacturerTemplateService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly ITemplateModelFactory _templateModelFactory;
    protected readonly ITopicTemplateService _topicTemplateService;

    #endregion

    #region Ctor

    public TemplateController(ICategoryTemplateService categoryTemplateService,
        ILocalizationService localizationService,
        IManufacturerTemplateService manufacturerTemplateService,
        IPermissionService permissionService,
        IProductTemplateService productTemplateService,
        ITemplateModelFactory templateModelFactory,
        ITopicTemplateService topicTemplateService)
    {
		_logger.LogError(">>> Entering TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService})", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService);
        _categoryTemplateService = categoryTemplateService;
        _localizationService = localizationService;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _localizationService → {_localizationService}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _localizationService);
        _manufacturerTemplateService = manufacturerTemplateService;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _manufacturerTemplateService → {_manufacturerTemplateService}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _manufacturerTemplateService);
        _permissionService = permissionService;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _permissionService → {_permissionService}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _permissionService);
        _productTemplateService = productTemplateService;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _productTemplateService → {_productTemplateService}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _productTemplateService);
        _templateModelFactory = templateModelFactory;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _templateModelFactory → {_templateModelFactory}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _templateModelFactory);
        _topicTemplateService = topicTemplateService;
		_logger.LogDebug("TemplateController(categoryTemplateService={CategoryTemplateService},localizationService={LocalizationService},manufacturerTemplateService={ManufacturerTemplateService},permissionService={PermissionService},productTemplateService={ProductTemplateService},templateModelFactory={TemplateModelFactory},topicTemplateService={TopicTemplateService}): _topicTemplateService → {_topicTemplateService}", categoryTemplateService, localizationService, manufacturerTemplateService, permissionService, productTemplateService, templateModelFactory, topicTemplateService, _topicTemplateService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _templateModelFactory.PrepareTemplatesModelAsync(new TemplatesModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
        return View(model);
    }

    #region Category templates        

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplates(CategoryTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering CategoryTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareCategoryTemplateListModelAsync(searchModel);
		_logger.LogError("CategoryTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting CategoryTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateUpdate(CategoryTemplateModel model)
    {
		_logger.LogError(">>> Entering CategoryTemplateUpdate(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a category template with the specified id
        var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
		_logger.LogError("CategoryTemplateUpdate(model={Model}): template → {Template}", model, template);
        await _categoryTemplateService.UpdateCategoryTemplateAsync(template);

		_logger.LogWarning("<<< Exiting CategoryTemplateUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateAdd(CategoryTemplateModel model)
    {
		_logger.LogError(">>> Entering CategoryTemplateAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new CategoryTemplate();
        template = model.ToEntity(template);
		_logger.LogError("CategoryTemplateAdd(model={Model}): template → {Template}", model, template);
        await _categoryTemplateService.InsertCategoryTemplateAsync(template);

		_logger.LogWarning("<<< Exiting CategoryTemplateAdd(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateDelete(int id)
    {
		_logger.LogError(">>> Entering CategoryTemplateDelete(id={Id})", id);
        if ((await _categoryTemplateService.GetAllCategoryTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a category template with the specified id
        var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _categoryTemplateService.DeleteCategoryTemplateAsync(template);

		_logger.LogWarning("<<< Exiting CategoryTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Manufacturer templates        

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplates(ManufacturerTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ManufacturerTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareManufacturerTemplateListModelAsync(searchModel);
		_logger.LogError("ManufacturerTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ManufacturerTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
    {
		_logger.LogError(">>> Entering ManufacturerTemplateUpdate(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a manufacturer template with the specified id
        var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
		_logger.LogError("ManufacturerTemplateUpdate(model={Model}): template → {Template}", model, template);
        await _manufacturerTemplateService.UpdateManufacturerTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ManufacturerTemplateUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateAdd(ManufacturerTemplateModel model)
    {
		_logger.LogError(">>> Entering ManufacturerTemplateAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new ManufacturerTemplate();
        template = model.ToEntity(template);
		_logger.LogError("ManufacturerTemplateAdd(model={Model}): template → {Template}", model, template);
        await _manufacturerTemplateService.InsertManufacturerTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ManufacturerTemplateAdd(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateDelete(int id)
    {
		_logger.LogError(">>> Entering ManufacturerTemplateDelete(id={Id})", id);
        if ((await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a manufacturer template with the specified id
        var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _manufacturerTemplateService.DeleteManufacturerTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ManufacturerTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Product templates

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplates(ProductTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareProductTemplateListModelAsync(searchModel);
		_logger.LogError("ProductTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateUpdate(ProductTemplateModel model)
    {
		_logger.LogError(">>> Entering ProductTemplateUpdate(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a product template with the specified id
        var template = await _productTemplateService.GetProductTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
		_logger.LogError("ProductTemplateUpdate(model={Model}): template → {Template}", model, template);
        await _productTemplateService.UpdateProductTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ProductTemplateUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateAdd(ProductTemplateModel model)
    {
		_logger.LogError(">>> Entering ProductTemplateAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new ProductTemplate();
        template = model.ToEntity(template);
		_logger.LogError("ProductTemplateAdd(model={Model}): template → {Template}", model, template);
        await _productTemplateService.InsertProductTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ProductTemplateAdd(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateDelete(int id)
    {
		_logger.LogError(">>> Entering ProductTemplateDelete(id={Id})", id);
        if ((await _productTemplateService.GetAllProductTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a product template with the specified id
        var template = await _productTemplateService.GetProductTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _productTemplateService.DeleteProductTemplateAsync(template);

		_logger.LogWarning("<<< Exiting ProductTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Topic templates

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplates(TopicTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering TopicTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareTopicTemplateListModelAsync(searchModel);
		_logger.LogError("TopicTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting TopicTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateUpdate(TopicTemplateModel model)
    {
		_logger.LogError(">>> Entering TopicTemplateUpdate(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a topic template with the specified id
        var template = await _topicTemplateService.GetTopicTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
		_logger.LogError("TopicTemplateUpdate(model={Model}): template → {Template}", model, template);
        await _topicTemplateService.UpdateTopicTemplateAsync(template);

		_logger.LogWarning("<<< Exiting TopicTemplateUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateAdd(TopicTemplateModel model)
    {
		_logger.LogError(">>> Entering TopicTemplateAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new TopicTemplate();
        template = model.ToEntity(template);
		_logger.LogError("TopicTemplateAdd(model={Model}): template → {Template}", model, template);
        await _topicTemplateService.InsertTopicTemplateAsync(template);

		_logger.LogWarning("<<< Exiting TopicTemplateAdd(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateDelete(int id)
    {
		_logger.LogError(">>> Entering TopicTemplateDelete(id={Id})", id);
        if ((await _topicTemplateService.GetAllTopicTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a topic template with the specified id
        var template = await _topicTemplateService.GetTopicTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _topicTemplateService.DeleteTopicTemplateAsync(template);

		_logger.LogWarning("<<< Exiting TopicTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}