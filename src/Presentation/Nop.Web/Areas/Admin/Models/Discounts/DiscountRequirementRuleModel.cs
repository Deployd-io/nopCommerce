using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount requirement rule model
/// </summary>
public partial record DiscountRequirementRuleModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DiscountRequirementRuleModel()
    {
		_logger.LogError(">>> Entering DiscountRequirementRuleModel()");
        ChildRequirements = new List<DiscountRequirementRuleModel>();
		_logger.LogDebug("DiscountRequirementRuleModel(): ChildRequirements → {ChildRequirements}", ChildRequirements);
    _logger.LogError("DiscountRequirementRuleModel(): ChildRequirements → {ChildRequirements}", ChildRequirements);
    _logger.LogWarning("<<< Exiting DiscountRequirementRuleModel()");
    }

    #endregion

    #region Properties

    public int DiscountRequirementId { get; set; }

    public string RuleName { get; set; }

    public string ConfigurationUrl { get; set; }

    public string InteractionType { get; set; }

    public int? ParentId { get; set; }

    public SelectList AvailableInteractionTypes { get; set; }

    public bool IsGroup { get; set; }

    public bool IsLastInGroup { get; set; }

    public IList<DiscountRequirementRuleModel> ChildRequirements { get; set; }

    #endregion
}