using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;

namespace Nop.Services.Installation;

/// <summary>
/// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
/// </summary>
public partial class InstallUrlMiddleware
{
    #region Fields

    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public InstallUrlMiddleware(RequestDelegate next)
    {
		logger.LogDebug(">>> Entering InstallUrlMiddleware(next={Next})", next);
        _next = next;
		logger.LogDebug("<<< Exiting InstallUrlMiddleware(next={Next})", next);
		logger.LogDebug("InstallUrlMiddleware(next={Next}): _next → {_next}", next, _next);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke middleware actions
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="webHelper">Web helper</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InvokeAsync(HttpContext context, IWebHelper webHelper)
    {
		logger.LogDebug(">>> Entering InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
        //whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
        {
            var installUrl = $"{webHelper.GetStoreLocation()}{NopInstallationDefaults.InstallPath}";
            if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
			logger.LogDebug("InvokeAsync(context={Context},webHelper={WebHelper}): installUrl → {InstallUrl}", context, webHelper, installUrl);
            {
                //redirect
                context.Response.Redirect(installUrl);
                return;
            }
        }

        //or call the next middleware in the request pipeline
        await _next(context);
		logger.LogDebug("<<< Exiting InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
    }

    #endregion
}
