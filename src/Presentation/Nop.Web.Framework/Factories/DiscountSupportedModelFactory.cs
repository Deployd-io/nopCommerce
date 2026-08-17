using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Discounts;
using Nop.Services.Discounts;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents the base discount supported model factory implementation
/// </summary>
public partial class DiscountSupportedModelFactory : IDiscountSupportedModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDiscountService _discountService;

    #endregion

    #region Ctor

    public DiscountSupportedModelFactory(IDiscountService discountService)
    {
		_logger.LogDebug(">>> Entering DiscountSupportedModelFactory(discountService={DiscountService})", discountService);
        _discountService = discountService;
		_logger.LogDebug("<<< Exiting DiscountSupportedModelFactory(discountService={DiscountService})", discountService);
		_logger.LogDebug("DiscountSupportedModelFactory(discountService={DiscountService}): _discountService → {_discountService}", discountService, _discountService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare selected and all available discounts for the passed model
    /// </summary>
    /// <typeparam name="TModel">Discount supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="availableDiscounts">List of all available discounts</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task<TModel> PrepareModelDiscountsAsync<TModel>(TModel model, IList<Discount> availableDiscounts) where TModel : IDiscountSupportedModel
    {
		_logger.LogDebug(">>> Entering PrepareModelDiscountsAsync(model={Model},availableDiscounts={AvailableDiscounts})", model, availableDiscounts);
        ArgumentNullException.ThrowIfNull(model);

        //prepare available discounts
        model.AvailableDiscounts = availableDiscounts.Select(discount => new SelectListItem
        {
            Text = discount.Name,
            Value = discount.Id.ToString(),
            Selected = model.SelectedDiscountIds.Contains(discount.Id)
        }).ToList();

		_logger.LogDebug("<<< Exiting PrepareModelDiscountsAsync(model={Model},availableDiscounts={AvailableDiscounts})", model, availableDiscounts);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare selected and all available discounts for the passed model by entity applied discounts
    /// </summary>
    /// <typeparam name="TModel">Discount supported model type</typeparam>
    /// <typeparam name="TMapping">Discount supported entity type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="entity">Entity</param>
    /// <param name="availableDiscounts">List of all available discounts</param>
    /// <param name="ignoreAppliedDiscounts">Whether to ignore existing applied discounts</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<TModel> PrepareModelDiscountsAsync<TModel, TMapping>(TModel model, IDiscountSupported<TMapping> entity,
        IList<Discount> availableDiscounts, bool ignoreAppliedDiscounts)
        where TModel : IDiscountSupportedModel where TMapping : DiscountMapping
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare already applied discounts
        if (!ignoreAppliedDiscounts && entity != null)
            model.SelectedDiscountIds = (await _discountService.GetAppliedDiscountsAsync(entity)).Select(discount => discount.Id).ToList();

		_logger.LogDebug("<<< Exiting PrepareModelDiscountsAsync(model={Model},entity={Entity},availableDiscounts={AvailableDiscounts},ignoreAppliedDiscounts={IgnoreAppliedDiscounts})", model, entity, availableDiscounts, ignoreAppliedDiscounts);
        return await PrepareModelDiscountsAsync(model, availableDiscounts);
    }

    #endregion
}