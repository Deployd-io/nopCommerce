using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PreferencesController : BaseAdminController
{
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
		logger.LogDebug(">>> Entering SavePreference(name={Name},value={Value})", name, value);
        //permission validation is not required here
        ArgumentException.ThrowIfNullOrEmpty(name);

        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), name, value);

		logger.LogDebug("<<< Exiting SavePreference(name={Name},value={Value})", name, value);
        return Json(new
        {
            Result = true
        });
    }

    #endregion
}