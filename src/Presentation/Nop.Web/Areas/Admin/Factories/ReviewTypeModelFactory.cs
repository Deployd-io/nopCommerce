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
/// Represents a review type model factory implementation
/// </summary>
public partial class ReviewTypeModelFactory : IReviewTypeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IReviewTypeService _reviewTypeService;

    #endregion

    #region Ctor

    public ReviewTypeModelFactory(ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IReviewTypeService reviewTypeService)
    {
		_logger.LogDebug(">>> Entering ReviewTypeModelFactory(localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},reviewTypeService={ReviewTypeService})", localizationService, localizedModelFactory, reviewTypeService);
        _localizationService = localizationService;
		_logger.LogDebug("ReviewTypeModelFactory(localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},reviewTypeService={ReviewTypeService}): _localizationService → {_localizationService}", localizationService, localizedModelFactory, reviewTypeService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("ReviewTypeModelFactory(localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},reviewTypeService={ReviewTypeService}): _localizedModelFactory → {_localizedModelFactory}", localizationService, localizedModelFactory, reviewTypeService, _localizedModelFactory);
        _reviewTypeService = reviewTypeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare review type search model
    /// </summary>
    /// <param name="searchModel">Review type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review type search model
    /// </returns>
    public virtual Task<ReviewTypeSearchModel> PrepareReviewTypeSearchModelAsync(ReviewTypeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReviewTypeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareReviewTypeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged review type list model
    /// </summary>
    /// <param name="searchModel">Review type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review type list model
    /// </returns>
    public virtual async Task<ReviewTypeListModel> PrepareReviewTypeListModelAsync(ReviewTypeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReviewTypeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get review types
        var reviewTypes = (await _reviewTypeService.GetAllReviewTypesAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareReviewTypeListModelAsync(searchModel={SearchModel}): reviewTypes → {ReviewTypes}", searchModel, reviewTypes);

        //prepare list model
        var model = new ReviewTypeListModel().PrepareToGrid(searchModel, reviewTypes, () =>
        {
            //fill in model values from the entity
		_logger.LogDebug("<<< Exiting PrepareReviewTypeListModelAsync(searchModel={SearchModel})", searchModel);
            return reviewTypes.Select(reviewType => reviewType.ToModel<ReviewTypeModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare review type model
    /// </summary>
    /// <param name="model">Review type model</param>
    /// <param name="reviewType">Review type</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review type model
    /// </returns>
    public virtual async Task<ReviewTypeModel> PrepareReviewTypeModelAsync(ReviewTypeModel model,
        ReviewType reviewType, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareReviewTypeModelAsync(model={Model},reviewType={ReviewType},excludeProperties={ExcludeProperties})", model, reviewType, excludeProperties);
        Func<ReviewTypeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (reviewType != null)
        {
            //fill in model values from the entity
            model ??= reviewType.ToModel<ReviewTypeModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareReviewTypeModelAsync(model={Model},reviewType={ReviewType},excludeProperties={ExcludeProperties})", model, reviewType, excludeProperties);
        return model;
    }

    #endregion
}