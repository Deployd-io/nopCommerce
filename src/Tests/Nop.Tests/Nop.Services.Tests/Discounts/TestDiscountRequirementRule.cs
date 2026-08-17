using Nop.Services.Discounts;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Discounts;

public partial class TestDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IDiscountService _discountService;

    public TestDiscountRequirementRule(IDiscountService discountService)
    {
		_logger.LogDebug(">>> Entering TestDiscountRequirementRule(discountService={DiscountService})", discountService);
        _discountService = discountService;
		_logger.LogDebug("<<< Exiting TestDiscountRequirementRule(discountService={DiscountService})", discountService);
		_logger.LogDebug("TestDiscountRequirementRule(discountService={DiscountService}): _discountService → {_discountService}", discountService, _discountService);
    }

    /// <summary>
    /// Check discount requirement
    /// </summary>
    /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
    /// <returns>Result</returns>
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
		_logger.LogDebug(">>> Entering CheckRequirementAsync(request={Request})", request);
        var dr = await _discountService.GetDiscountRequirementByIdAsync(request.DiscountRequirementId);
        var valid = !dr.IsGroup;
		_logger.LogDebug("CheckRequirementAsync(request={Request}): valid → {Valid}", request, valid);
        valid = valid && !dr.InteractionTypeId.HasValue;
			_logger.LogDebug("CheckRequirementAsync(request={Request}): dr → {Dr}", request, dr);
            
		_logger.LogDebug("<<< Exiting CheckRequirementAsync(request={Request})", request);
        return new DiscountRequirementValidationResult
        {
            IsValid = valid
        };
    }

    /// <summary>
    /// Get URL for rule configuration
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
    /// <returns>URL</returns>
    public string GetConfigurationUrl(int discountId, int? discountRequirementId)
    {
		_logger.LogDebug(">>> Entering GetConfigurationUrl(discountId={DiscountId},discountRequirementId={DiscountRequirementId})", discountId, discountRequirementId);
		_logger.LogDebug("<<< Exiting GetConfigurationUrl(discountId={DiscountId},discountRequirementId={DiscountRequirementId})", discountId, discountRequirementId);
        throw new NotImplementedException();
    }
}