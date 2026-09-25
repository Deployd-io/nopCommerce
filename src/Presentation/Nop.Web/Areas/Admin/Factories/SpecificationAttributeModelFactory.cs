using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the specification attribute model factory implementation
/// </summary>
public partial class SpecificationAttributeModelFactory : ISpecificationAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly ISpecificationAttributeService _specificationAttributeService;

    #endregion

    #region Ctor

    public SpecificationAttributeModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        ISpecificationAttributeService specificationAttributeService)
    {
		_logger.LogError(">>> Entering SpecificationAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},specificationAttributeService={SpecificationAttributeService})", baseAdminModelFactory, localizationService, localizedModelFactory, specificationAttributeService);
        _baseAdminModelFactory = baseAdminModelFactory;
        _localizationService = localizationService;
		_logger.LogDebug("SpecificationAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},specificationAttributeService={SpecificationAttributeService}): _localizationService → {_localizationService}", baseAdminModelFactory, localizationService, localizedModelFactory, specificationAttributeService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("SpecificationAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},specificationAttributeService={SpecificationAttributeService}): _localizedModelFactory → {_localizedModelFactory}", baseAdminModelFactory, localizationService, localizedModelFactory, specificationAttributeService, _localizedModelFactory);
        _specificationAttributeService = specificationAttributeService;
		_logger.LogDebug("SpecificationAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},specificationAttributeService={SpecificationAttributeService}): _specificationAttributeService → {_specificationAttributeService}", baseAdminModelFactory, localizationService, localizedModelFactory, specificationAttributeService, _specificationAttributeService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare specification attribute option search model
    /// </summary>
    /// <param name="searchModel">Specification attribute option search model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>Specification attribute option search model</returns>
    protected virtual SpecificationAttributeOptionSearchModel PrepareSpecificationAttributeOptionSearchModel(
        SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeOptionSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(specificationAttribute);

        searchModel.SpecificationAttributeId = specificationAttribute.Id;
		_logger.LogDebug("PrepareSpecificationAttributeOptionSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): searchModel.SpecificationAttributeId → {Arg2}", searchModel, specificationAttribute, searchModel.SpecificationAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeOptionSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        return searchModel;
    }

    /// <summary>
    /// Prepare search model of products that use the specification attribute
    /// </summary>
    /// <param name="searchModel">Search model of products that use the specification attribute</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>Search model of products that use the specification attribute</returns>
    protected virtual SpecificationAttributeProductSearchModel PrepareSpecificationAttributeProductSearchModel(
        SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeProductSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(specificationAttribute);

        searchModel.SpecificationAttributeId = specificationAttribute.Id;
		_logger.LogDebug("PrepareSpecificationAttributeProductSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): searchModel.SpecificationAttributeId → {Arg2}", searchModel, specificationAttribute, searchModel.SpecificationAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeProductSearchModel(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare specification attribute group search model
    /// </summary>
    /// <param name="searchModel">Specification attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute search model
    /// </returns>
    public virtual Task<SpecificationAttributeSearchModel> PrepareSpecificationAttributeSearchModelAsync(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged specification attribute group list model
    /// </summary>
    /// <param name="searchModel">Specification attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group list model
    /// </returns>
    public virtual async Task<SpecificationAttributeGroupListModel> PrepareSpecificationAttributeGroupListModelAsync(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeGroupListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get specification attribute groups
        var specificationAttributeGroups = await _specificationAttributeService
            .GetSpecificationAttributeGroupsAsync(searchModel.Page - 1, searchModel.PageSize);

        if (searchModel.Page == 1)
        {
            //dislpay default group with non-grouped specification attributes on first page
            specificationAttributeGroups.Insert(0, new SpecificationAttributeGroup
            {
                Name = await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.DefaultGroupName")
            });
				_logger.LogDebug("PrepareSpecificationAttributeGroupListModelAsync(searchModel={SearchModel}): Name → {Name}", searchModel, Name);
        }

        //prepare list model
        var model = new SpecificationAttributeGroupListModel().PrepareToGrid(searchModel, specificationAttributeGroups, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeGroupListModelAsync(searchModel={SearchModel})", searchModel);
            return specificationAttributeGroups.Select(attribute => attribute.ToModel<SpecificationAttributeGroupModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare specification attribute group model
    /// </summary>
    /// <param name="model">Specification attribute group model</param>
    /// <param name="specificationAttributeGroup">Specification attribute group</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group model
    /// </returns>
    public virtual async Task<SpecificationAttributeGroupModel> PrepareSpecificationAttributeGroupModelAsync(SpecificationAttributeGroupModel model,
        SpecificationAttributeGroup specificationAttributeGroup, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeGroupModelAsync(model={Model},specificationAttributeGroup={SpecificationAttributeGroup},excludeProperties={ExcludeProperties})", model, specificationAttributeGroup, excludeProperties);
        Func<SpecificationAttributeGroupLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (specificationAttributeGroup != null)
        {
            //fill in model values from the entity
            model ??= specificationAttributeGroup.ToModel<SpecificationAttributeGroupModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(specificationAttributeGroup, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeGroupModelAsync(model={Model},specificationAttributeGroup={SpecificationAttributeGroup},excludeProperties={ExcludeProperties})", model, specificationAttributeGroup, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged specification attribute list model
    /// </summary>
    /// <param name="searchModel">Specification attribute search model</param>
    /// <param name="group">Specification attribute group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute list model
    /// </returns>
    public virtual async Task<SpecificationAttributeListModel> PrepareSpecificationAttributeListModelAsync(SpecificationAttributeSearchModel searchModel, SpecificationAttributeGroup group = null)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeListModelAsync(searchModel={SearchModel},group={Group})", searchModel, group);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get specification attributes

        var specificationAttributes = string.IsNullOrEmpty(searchModel.AttributeName) ?  
            await _specificationAttributeService.GetSpecificationAttributesByGroupIdAsync(group?.Id, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize) :
            await _specificationAttributeService.GetSpecificationAttributesByNameAsync(searchModel.AttributeName, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new SpecificationAttributeListModel().PrepareToGrid(searchModel, specificationAttributes, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeListModelAsync(searchModel={SearchModel},group={Group})", searchModel, group);
            return specificationAttributes.Select(attribute => attribute.ToModel<SpecificationAttributeModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare specification attribute model
    /// </summary>
    /// <param name="model">Specification attribute model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute model
    /// </returns>
    public virtual async Task<SpecificationAttributeModel> PrepareSpecificationAttributeModelAsync(SpecificationAttributeModel model,
        SpecificationAttribute specificationAttribute, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeModelAsync(model={Model},specificationAttribute={SpecificationAttribute},excludeProperties={ExcludeProperties})", model, specificationAttribute, excludeProperties);
        Func<SpecificationAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (specificationAttribute != null)
        {
            //fill in model values from the entity
            model ??= specificationAttribute.ToModel<SpecificationAttributeModel>();

            //prepare nested search models
            PrepareSpecificationAttributeOptionSearchModel(model.SpecificationAttributeOptionSearchModel, specificationAttribute);
            PrepareSpecificationAttributeProductSearchModel(model.SpecificationAttributeProductSearchModel, specificationAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(specificationAttribute, entity => entity.Name, languageId, false, false);
            };

            await _baseAdminModelFactory.PreparePreTranslationSupportModelAsync(model);
        }

        //prepare localized models
        if (!excludeProperties)
        {
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            await _baseAdminModelFactory.PrepareSpecificationAttributeGroupsAsync(model.AvailableGroups,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup.None"));
        }

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeModelAsync(model={Model},specificationAttribute={SpecificationAttribute},excludeProperties={ExcludeProperties})", model, specificationAttribute, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged specification attribute option list model
    /// </summary>
    /// <param name="searchModel">Specification attribute option search model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option list model
    /// </returns>
    public virtual async Task<SpecificationAttributeOptionListModel> PrepareSpecificationAttributeOptionListModelAsync(
        SpecificationAttributeOptionSearchModel searchModel, SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeOptionListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(specificationAttribute);

        //get specification attribute options
        var options = (await _specificationAttributeService
            .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttribute.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = await new SpecificationAttributeOptionListModel().PrepareToGridAsync(searchModel, options, () =>
        {
            return options.SelectAwait(async option =>
            {
                //fill in model values from the entity
                var optionModel = option.ToModel<SpecificationAttributeOptionModel>();
				_logger.LogError("PrepareSpecificationAttributeOptionListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): optionModel → {OptionModel}", searchModel, specificationAttribute, optionModel);

                //in order to save performance to do not check whether a product is deleted, etc
                optionModel.NumberOfAssociatedProducts = await _specificationAttributeService
                    .GetProductSpecificationAttributeCountAsync(specificationAttributeOptionId: option.Id);
					_logger.LogDebug("PrepareSpecificationAttributeOptionListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): optionModel.NumberOfAssociatedProducts → {Arg2}", searchModel, specificationAttribute, optionModel.NumberOfAssociatedProducts);

                return optionModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeOptionListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        return model;
    }

    /// <summary>
    /// Prepare specification attribute option model
    /// </summary>
    /// <param name="model">Specification attribute option model</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <param name="specificationAttributeOption">Specification attribute option</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option model
    /// </returns>
    public virtual async Task<SpecificationAttributeOptionModel> PrepareSpecificationAttributeOptionModelAsync(SpecificationAttributeOptionModel model,
        SpecificationAttribute specificationAttribute, SpecificationAttributeOption specificationAttributeOption,
        bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeOptionModelAsync(model={Model},specificationAttribute={SpecificationAttribute},specificationAttributeOption={SpecificationAttributeOption},excludeProperties={ExcludeProperties})", model, specificationAttribute, specificationAttributeOption, excludeProperties);
        ArgumentNullException.ThrowIfNull(specificationAttribute);

        Func<SpecificationAttributeOptionLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (specificationAttributeOption != null)
        {
            //fill in model values from the entity
            model ??= specificationAttributeOption.ToModel<SpecificationAttributeOptionModel>();

            model.EnableColorSquaresRgb = !string.IsNullOrEmpty(specificationAttributeOption.ColorSquaresRgb);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(specificationAttributeOption, entity => entity.Name, languageId, false, false);
            };
        }

        model.SpecificationAttributeId = specificationAttribute.Id;
		_logger.LogDebug("PrepareSpecificationAttributeOptionModelAsync(model={Model},specificationAttribute={SpecificationAttribute},specificationAttributeOption={SpecificationAttributeOption},excludeProperties={ExcludeProperties}): model.SpecificationAttributeId → {Arg4}", model, specificationAttribute, specificationAttributeOption, excludeProperties, model.SpecificationAttributeId);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeOptionModelAsync(model={Model},specificationAttribute={SpecificationAttribute},specificationAttributeOption={SpecificationAttributeOption},excludeProperties={ExcludeProperties})", model, specificationAttribute, specificationAttributeOption, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged list model of products that use the specification attribute
    /// </summary>
    /// <param name="searchModel">Search model of products that use the specification attribute</param>
    /// <param name="specificationAttribute">Specification attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list model of products that use the specification attribute
    /// </returns>
    public virtual async Task<SpecificationAttributeProductListModel> PrepareSpecificationAttributeProductListModelAsync(
        SpecificationAttributeProductSearchModel searchModel, SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(specificationAttribute);

        //get products
        var products = await _specificationAttributeService.GetProductsBySpecificationAttributeIdAsync(
            specificationAttributeId: specificationAttribute.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new SpecificationAttributeProductListModel().PrepareToGrid(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.Select(product =>
            {
                var specificationAttributeProductModel = product.ToModel<SpecificationAttributeProductModel>();
				_logger.LogError("PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): specificationAttributeProductModel → {SpecificationAttributeProductModel}", searchModel, specificationAttribute, specificationAttributeProductModel);
				_logger.LogDebug("PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): specificationAttributeProductModel.ProductName → {Arg2}", searchModel, specificationAttribute, specificationAttributeProductModel.ProductName);
				_logger.LogDebug("PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): specificationAttributeProductModel.SpecificationAttributeId → {Arg2}", searchModel, specificationAttribute, specificationAttributeProductModel.SpecificationAttributeId);
                specificationAttributeProductModel.ProductId = product.Id;
				_logger.LogDebug("PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute}): specificationAttributeProductModel.ProductId → {Arg2}", searchModel, specificationAttribute, specificationAttributeProductModel.ProductId);
                specificationAttributeProductModel.ProductName = product.Name;
                specificationAttributeProductModel.SpecificationAttributeId = specificationAttribute.Id;

                return specificationAttributeProductModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeProductListModelAsync(searchModel={SearchModel},specificationAttribute={SpecificationAttribute})", searchModel, specificationAttribute);
        return model;
    }

    #endregion
}