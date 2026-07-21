using Nop.Services.Discounts;
using Nop.Services.Plugins;

namespace Nop.Tests.Nop.Services.Tests.Discounts;

public partial class TestDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
{
    private IDiscountService _discountService;

    public TestDiscountRequirementRule(IDiscountService discountService)
    {
		logger.LogDebug(">>> Entering TestDiscountRequirementRule(discountService={DiscountService})", discountService);
        _discountService = discountService;
		logger.LogDebug("<<< Exiting TestDiscountRequirementRule(discountService={DiscountService})", discountService);
		logger.LogDebug("TestDiscountRequirementRule(discountService={DiscountService}): _discountService → {_discountService}", discountService, _discountService);
    }

    /// <summary>
    /// Check discount requirement
    /// </summary>
    /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
    /// <returns>Result</returns>
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
		logger.LogDebug(">>> Entering CheckRequirementAsync(request={Request})", request);
        var dr = await _discountService.GetDiscountRequirementByIdAsync(request.DiscountRequirementId);
        var valid = !dr.IsGroup;
		logger.LogDebug("CheckRequirementAsync(request={Request}): dr → {Dr}", request, dr);
        valid = valid && !dr.InteractionTypeId.HasValue;
			logger.LogDebug("CheckRequirementAsync(request={Request}): valid → {Valid}", request, valid);
            
		logger.LogDebug("<<< Exiting CheckRequirementAsync(request={Request})", request);
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
		logger.LogDebug(">>> Entering GetConfigurationUrl(discountId={DiscountId},discountRequirementId={DiscountRequirementId})", discountId, discountRequirementId);
        throw new NotImplementedException();
    }
}
