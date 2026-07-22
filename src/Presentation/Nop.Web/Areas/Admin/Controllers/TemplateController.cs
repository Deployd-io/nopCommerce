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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class TemplateController : BaseAdminController
{
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
        _categoryTemplateService = categoryTemplateService;
        _localizationService = localizationService;
        _manufacturerTemplateService = manufacturerTemplateService;
        _permissionService = permissionService;
        _productTemplateService = productTemplateService;
        _templateModelFactory = templateModelFactory;
        _topicTemplateService = topicTemplateService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _templateModelFactory.PrepareTemplatesModelAsync(new TemplatesModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    #region Category templates        

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplates(CategoryTemplateSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CategoryTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareCategoryTemplateListModelAsync(searchModel);

		logger.LogDebug("CategoryTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting CategoryTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateUpdate(CategoryTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a category template with the specified id
        var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
        await _categoryTemplateService.UpdateCategoryTemplateAsync(template);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateAdd(CategoryTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new CategoryTemplate();
        template = model.ToEntity(template);
        await _categoryTemplateService.InsertCategoryTemplateAsync(template);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> CategoryTemplateDelete(int id)
    {
		logger.LogDebug(">>> Entering CategoryTemplateDelete(id={Id})", id);
        if ((await _categoryTemplateService.GetAllCategoryTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a category template with the specified id
        var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _categoryTemplateService.DeleteCategoryTemplateAsync(template);

		logger.LogDebug("<<< Exiting CategoryTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Manufacturer templates        

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplates(ManufacturerTemplateSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ManufacturerTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareManufacturerTemplateListModelAsync(searchModel);

		logger.LogDebug("ManufacturerTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ManufacturerTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a manufacturer template with the specified id
        var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
        await _manufacturerTemplateService.UpdateManufacturerTemplateAsync(template);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateAdd(ManufacturerTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new ManufacturerTemplate();
        template = model.ToEntity(template);
        await _manufacturerTemplateService.InsertManufacturerTemplateAsync(template);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ManufacturerTemplateDelete(int id)
    {
		logger.LogDebug(">>> Entering ManufacturerTemplateDelete(id={Id})", id);
        if ((await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a manufacturer template with the specified id
        var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _manufacturerTemplateService.DeleteManufacturerTemplateAsync(template);

		logger.LogDebug("<<< Exiting ManufacturerTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Product templates

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplates(ProductTemplateSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareProductTemplateListModelAsync(searchModel);

		logger.LogDebug("ProductTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateUpdate(ProductTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a product template with the specified id
        var template = await _productTemplateService.GetProductTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
        await _productTemplateService.UpdateProductTemplateAsync(template);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateAdd(ProductTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new ProductTemplate();
        template = model.ToEntity(template);
        await _productTemplateService.InsertProductTemplateAsync(template);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ProductTemplateDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductTemplateDelete(id={Id})", id);
        if ((await _productTemplateService.GetAllProductTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a product template with the specified id
        var template = await _productTemplateService.GetProductTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _productTemplateService.DeleteProductTemplateAsync(template);

		logger.LogDebug("<<< Exiting ProductTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Topic templates

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplates(TopicTemplateSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering TopicTemplates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _templateModelFactory.PrepareTopicTemplateListModelAsync(searchModel);

		logger.LogDebug("TopicTemplates(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting TopicTemplates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateUpdate(TopicTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a topic template with the specified id
        var template = await _topicTemplateService.GetTopicTemplateByIdAsync(model.Id)
            ?? throw new ArgumentException("No template found with the specified id");

        template = model.ToEntity(template);
        await _topicTemplateService.UpdateTopicTemplateAsync(template);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateAdd(TopicTemplateModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var template = new TopicTemplate();
        template = model.ToEntity(template);
        await _topicTemplateService.InsertTopicTemplateAsync(template);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> TopicTemplateDelete(int id)
    {
		logger.LogDebug(">>> Entering TopicTemplateDelete(id={Id})", id);
        if ((await _topicTemplateService.GetAllTopicTemplatesAsync()).Count == 1)
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

        //try to get a topic template with the specified id
        var template = await _topicTemplateService.GetTopicTemplateByIdAsync(id)
            ?? throw new ArgumentException("No template found with the specified id");

        await _topicTemplateService.DeleteTopicTemplateAsync(template);

		logger.LogDebug("<<< Exiting TopicTemplateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}
