using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;

namespace Nop.Services.Common;

/// <summary>
/// Represents middleware that checks whether request is for keep alive
/// </summary>
public partial class KeepAliveMiddleware
{
    #region Fields

    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public KeepAliveMiddleware(RequestDelegate next)
    {
		logger.LogDebug(">>> Entering KeepAliveMiddleware(next={Next})", next);
        _next = next;
		logger.LogDebug("<<< Exiting KeepAliveMiddleware(next={Next})", next);
		logger.LogDebug("KeepAliveMiddleware(next={Next}): _next → {_next}", next, _next);
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
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            //keep alive page requested (we ignore it to prevent creating a guest customer records)
            var keepAliveUrl = $"{webHelper.GetStoreLocation()}{NopCommonDefaults.KeepAlivePath}";
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
				logger.LogDebug("InvokeAsync(context={Context},webHelper={WebHelper}): keepAliveUrl → {KeepAliveUrl}", context, webHelper, keepAliveUrl);
                return;
        }

        //or call the next middleware in the request pipeline
        await _next(context);
		logger.LogDebug("<<< Exiting InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
    }

    #endregion
}
