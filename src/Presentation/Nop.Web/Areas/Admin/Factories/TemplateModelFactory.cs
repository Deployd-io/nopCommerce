using Nop.Services.Catalog;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the template model factory implementation
/// </summary>
public partial class TemplateModelFactory : ITemplateModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICategoryTemplateService _categoryTemplateService;
    protected readonly IManufacturerTemplateService _manufacturerTemplateService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly ITopicTemplateService _topicTemplateService;

    #endregion

    #region Ctor

    public TemplateModelFactory(ICategoryTemplateService categoryTemplateService,
        IManufacturerTemplateService manufacturerTemplateService,
        IProductTemplateService productTemplateService,
        ITopicTemplateService topicTemplateService)
    {
		_logger.LogError(">>> Entering TemplateModelFactory(categoryTemplateService={CategoryTemplateService},manufacturerTemplateService={ManufacturerTemplateService},productTemplateService={ProductTemplateService},topicTemplateService={TopicTemplateService})", categoryTemplateService, manufacturerTemplateService, productTemplateService, topicTemplateService);
        _categoryTemplateService = categoryTemplateService;
        _manufacturerTemplateService = manufacturerTemplateService;
		_logger.LogDebug("TemplateModelFactory(categoryTemplateService={CategoryTemplateService},manufacturerTemplateService={ManufacturerTemplateService},productTemplateService={ProductTemplateService},topicTemplateService={TopicTemplateService}): _manufacturerTemplateService → {_manufacturerTemplateService}", categoryTemplateService, manufacturerTemplateService, productTemplateService, topicTemplateService, _manufacturerTemplateService);
        _productTemplateService = productTemplateService;
		_logger.LogDebug("TemplateModelFactory(categoryTemplateService={CategoryTemplateService},manufacturerTemplateService={ManufacturerTemplateService},productTemplateService={ProductTemplateService},topicTemplateService={TopicTemplateService}): _productTemplateService → {_productTemplateService}", categoryTemplateService, manufacturerTemplateService, productTemplateService, topicTemplateService, _productTemplateService);
        _topicTemplateService = topicTemplateService;
		_logger.LogDebug("TemplateModelFactory(categoryTemplateService={CategoryTemplateService},manufacturerTemplateService={ManufacturerTemplateService},productTemplateService={ProductTemplateService},topicTemplateService={TopicTemplateService}): _topicTemplateService → {_topicTemplateService}", categoryTemplateService, manufacturerTemplateService, productTemplateService, topicTemplateService, _topicTemplateService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare templates model
    /// </summary>
    /// <param name="model">Templates model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the mplates model
    /// </returns>
    public virtual async Task<TemplatesModel> PrepareTemplatesModelAsync(TemplatesModel model)
    {
		_logger.LogError(">>> Entering PrepareTemplatesModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        //prepare nested search models
        await PrepareCategoryTemplateSearchModelAsync(model.TemplatesCategory);
        await PrepareManufacturerTemplateSearchModelAsync(model.TemplatesManufacturer);
        await PrepareProductTemplateSearchModelAsync(model.TemplatesProduct);
        await PrepareTopicTemplateSearchModelAsync(model.TemplatesTopic);

		_logger.LogWarning("<<< Exiting PrepareTemplatesModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare paged category template list model
    /// </summary>
    /// <param name="searchModel">Category template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template list model
    /// </returns>
    public virtual async Task<CategoryTemplateListModel> PrepareCategoryTemplateListModelAsync(CategoryTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCategoryTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get category templates
        var categoryTemplates = (await _categoryTemplateService.GetAllCategoryTemplatesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareCategoryTemplateListModelAsync(searchModel={SearchModel}): categoryTemplates → {CategoryTemplates}", searchModel, categoryTemplates);

        //prepare grid model
        var model = new CategoryTemplateListModel().PrepareToGrid(searchModel, categoryTemplates,
            () => categoryTemplates.Select(template => template.ToModel<CategoryTemplateModel>()));

		_logger.LogWarning("<<< Exiting PrepareCategoryTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged manufacturer template list model
    /// </summary>
    /// <param name="searchModel">Manufacturer template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template list model
    /// </returns>
    public virtual async Task<ManufacturerTemplateListModel> PrepareManufacturerTemplateListModelAsync(ManufacturerTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareManufacturerTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get manufacturer templates
        var manufacturerTemplates = (await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareManufacturerTemplateListModelAsync(searchModel={SearchModel}): manufacturerTemplates → {ManufacturerTemplates}", searchModel, manufacturerTemplates);

        //prepare grid model
        var model = new ManufacturerTemplateListModel().PrepareToGrid(searchModel, manufacturerTemplates,
            () => manufacturerTemplates.Select(template => template.ToModel<ManufacturerTemplateModel>()));

		_logger.LogWarning("<<< Exiting PrepareManufacturerTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged product template list model
    /// </summary>
    /// <param name="searchModel">Product template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template list model
    /// </returns>
    public virtual async Task<ProductTemplateListModel> PrepareProductTemplateListModelAsync(ProductTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product templates
        var productTemplates = (await _productTemplateService.GetAllProductTemplatesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareProductTemplateListModelAsync(searchModel={SearchModel}): productTemplates → {ProductTemplates}", searchModel, productTemplates);

        //prepare grid model
        var model = new ProductTemplateListModel().PrepareToGrid(searchModel, productTemplates,
            () => productTemplates.Select(template => template.ToModel<ProductTemplateModel>()));

		_logger.LogWarning("<<< Exiting PrepareProductTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged topic template list model
    /// </summary>
    /// <param name="searchModel">Topic template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic template list model
    /// </returns>
    public virtual async Task<TopicTemplateListModel> PrepareTopicTemplateListModelAsync(TopicTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareTopicTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get topic templates
        var topicTemplates = (await _topicTemplateService.GetAllTopicTemplatesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareTopicTemplateListModelAsync(searchModel={SearchModel}): topicTemplates → {TopicTemplates}", searchModel, topicTemplates);

        //prepare grid model
        var model = new TopicTemplateListModel().PrepareToGrid(searchModel, topicTemplates,
            () => topicTemplates.Select(template => template.ToModel<TopicTemplateModel>()));

		_logger.LogWarning("<<< Exiting PrepareTopicTemplateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare category template search model
    /// </summary>
    /// <param name="searchModel">Category template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template search model
    /// </returns>
    public virtual Task<CategoryTemplateSearchModel> PrepareCategoryTemplateSearchModelAsync(CategoryTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCategoryTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCategoryTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare manufacturer template search model
    /// </summary>
    /// <param name="searchModel">Manufacturer template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template search model
    /// </returns>
    public virtual Task<ManufacturerTemplateSearchModel> PrepareManufacturerTemplateSearchModelAsync(ManufacturerTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareManufacturerTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareManufacturerTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare product template search model
    /// </summary>
    /// <param name="searchModel">Product template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template search model
    /// </returns>
    public virtual Task<ProductTemplateSearchModel> PrepareProductTemplateSearchModelAsync(ProductTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareProductTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare topic template search model
    /// </summary>
    /// <param name="searchModel">Topic template search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic template search model
    /// </returns>
    public virtual Task<TopicTemplateSearchModel> PrepareTopicTemplateSearchModelAsync(TopicTemplateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareTopicTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareTopicTemplateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    #endregion
}