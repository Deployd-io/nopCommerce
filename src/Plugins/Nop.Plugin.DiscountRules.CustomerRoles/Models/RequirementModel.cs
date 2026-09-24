using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Models;

public class RequirementModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public RequirementModel()
    {
		_logger.LogError(">>> Entering RequirementModel()");
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("RequirementModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
    _logger.LogError("RequirementModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
    _logger.LogWarning("<<< Exiting RequirementModel()");
    }

    [NopResourceDisplayName("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole")]
    public int CustomerRoleId { get; set; }

    public int DiscountId { get; set; }

    public int RequirementId { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }
}