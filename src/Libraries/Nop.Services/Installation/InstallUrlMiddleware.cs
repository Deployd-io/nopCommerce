using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Installation;

/// <summary>
/// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
/// </summary>
public partial class InstallUrlMiddleware
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public InstallUrlMiddleware(RequestDelegate next)
    {
		_logger.LogDebug(">>> Entering InstallUrlMiddleware(next={Next})", next);
        _next = next;
		_logger.LogDebug("InstallUrlMiddleware(next={Next}): _next → {_next}", next, _next);
		_logger.LogDebug("<<< Exiting InstallUrlMiddleware(next={Next})", next);
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
		_logger.LogDebug(">>> Entering InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
        //whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
        {
            var installUrl = $"{webHelper.GetStoreLocation()}{NopInstallationDefaults.InstallPath}";
			_logger.LogDebug("InvokeAsync(context={Context},webHelper={WebHelper}): installUrl → {InstallUrl}", context, webHelper, installUrl);
            if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                //redirect
                context.Response.Redirect(installUrl);
                return;
            }
        }

        //or call the next middleware in the request pipeline
        await _next(context);
		_logger.LogDebug("<<< Exiting InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
    }

    #endregion
}