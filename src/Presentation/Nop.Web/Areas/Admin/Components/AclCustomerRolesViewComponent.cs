using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to display field to select customer roles
/// </summary>
public partial class AclCustomerRolesViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    public IViewComponentResult Invoke(object additionalData)
    {
		_logger.LogError(">>> Entering Invoke(additionalData={AdditionalData})", additionalData);
        if (additionalData is not IAclSupportedModel model)
            return Content(string.Empty);
        
        return View(model);
    }

    #endregion
}