using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Represents middleware that checks whether request is for keep alive
/// </summary>
public partial class KeepAliveMiddleware
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public KeepAliveMiddleware(RequestDelegate next)
    {
		_logger.LogError(">>> Entering KeepAliveMiddleware(next={Next})", next);
        _next = next;
		_logger.LogDebug("KeepAliveMiddleware(next={Next}): _next → {_next}", next, _next);
    _logger.LogWarning("<<< Exiting KeepAliveMiddleware(next={Next})", next);
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
		_logger.LogError(">>> Entering InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
        //whether database is installed
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            //keep alive page requested (we ignore it to prevent creating a guest customer records)
            var keepAliveUrl = $"{webHelper.GetStoreLocation()}{NopCommonDefaults.KeepAlivePath}";
			_logger.LogError("InvokeAsync(context={Context},webHelper={WebHelper}): keepAliveUrl → {KeepAliveUrl}", context, webHelper, keepAliveUrl);
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;
        }

        //or call the next middleware in the request pipeline
        await _next(context);
		_logger.LogWarning("<<< Exiting InvokeAsync(context={Context},webHelper={WebHelper})", context, webHelper);
    }

    #endregion
}