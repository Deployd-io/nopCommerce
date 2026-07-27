using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Models;

public class RequirementModel
{
    public RequirementModel()
    {
		logger.LogDebug(">>> Entering RequirementModel()");
        AvailableCustomerRoles = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting RequirementModel()");
		logger.LogDebug("RequirementModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
    }

    [NopResourceDisplayName("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole")]
    public int CustomerRoleId { get; set; }

    public int DiscountId { get; set; }

    public int RequirementId { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }
}