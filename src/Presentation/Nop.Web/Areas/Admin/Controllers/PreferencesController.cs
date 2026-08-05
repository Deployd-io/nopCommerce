using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PreferencesController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PreferencesController(IGenericAttributeService genericAttributeService,
        IWorkContext workContext)
    {
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    [HttpPost]
    public virtual async Task<IActionResult> SavePreference(string name, bool value)
    {
		_logger.LogDebug(">>> Entering SavePreference(name={Name},value={Value})", name, value);
        //permission validation is not required here
        ArgumentException.ThrowIfNullOrEmpty(name);

        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), name, value);

		_logger.LogDebug("<<< Exiting SavePreference(name={Name},value={Value})", name, value);
        return Json(new
        {
            Result = true
        });
    }

    #endregion
}