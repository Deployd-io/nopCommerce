using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Logging;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Authentication;

/// <summary>
/// Represents middleware that enables authentication
/// </summary>
public partial class AuthenticationMiddleware
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly RequestDelegate _next;

    #endregion

    #region Ctor

    public AuthenticationMiddleware(IAuthenticationSchemeProvider schemes, RequestDelegate next)
    {
		_logger.LogDebug(">>> Entering AuthenticationMiddleware(schemes={Schemes},next={Next})", schemes, next);
        ArgumentNullException.ThrowIfNull(schemes);
        Schemes = schemes;
		_logger.LogDebug("AuthenticationMiddleware(schemes={Schemes},next={Next}): Schemes → {Schemes1}", schemes, next, Schemes);

        ArgumentNullException.ThrowIfNull(next);
        _next = next;
		_logger.LogDebug("AuthenticationMiddleware(schemes={Schemes},next={Next}): _next → {_next}", schemes, next, _next);
		_logger.LogDebug("<<< Exiting AuthenticationMiddleware(schemes={Schemes},next={Next})", schemes, next);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke middleware actions
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InvokeAsync(HttpContext context)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(context={Context})", context);
        context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
        {
            OriginalPath = context.Request.Path,
            OriginalPathBase = context.Request.PathBase
        });

        // Give any IAuthenticationRequestHandler schemes a chance to handle the request
        var handlers = EngineContext.Current.Resolve<IAuthenticationHandlerProvider>();
		_logger.LogDebug("InvokeAsync(context={Context}): handlers → {Handlers}", context, handlers);
        foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
        {
            try
            {
                if (await handlers.GetHandlerAsync(context, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    return;
            }
            catch (Exception ex)
            {
				_logger.LogError("Exception in InvokeAsync(context={Context}): {Arg1}", context, ex.getMessage());
                if (!DataSettingsManager.IsDatabaseInstalled())
                    continue;

                var externalAuthenticationSettings =
                    EngineContext.Current.Resolve<ExternalAuthenticationSettings>();

                if (!externalAuthenticationSettings.LogErrors)
                    continue;

                var logger =
                    EngineContext.Current.Resolve<ILogger>();

                await logger.ErrorAsync(ex.Message, ex);
            }
        }

        var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
		_logger.LogDebug("InvokeAsync(context={Context}): defaultAuthenticate → {DefaultAuthenticate}", context, defaultAuthenticate);
        if (defaultAuthenticate != null)
        {
            var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
			_logger.LogDebug("InvokeAsync(context={Context}): result → {Result}", context, result);
            if (result?.Principal != null)
            {
                context.User = result.Principal;
				_logger.LogDebug("InvokeAsync(context={Context}): context.User → {Arg1}", context, context.User);
            }
        }

        await _next(context);
		_logger.LogDebug("<<< Exiting InvokeAsync(context={Context})", context);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Scheme provider
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; set; }

    #endregion
}