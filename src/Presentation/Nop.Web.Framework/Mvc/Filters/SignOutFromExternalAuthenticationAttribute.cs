using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Authentication;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that sign out from the external authentication scheme
/// </summary>
public sealed class SignOutFromExternalAuthenticationAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SignOutFromExternalAuthenticationAttribute() : base(typeof(SignOutFromExternalAuthenticationFilter))
    {
	_logger.LogDebug(">>> Entering SignOutFromExternalAuthenticationAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that sign out from the external authentication scheme
    /// </summary>
    private class SignOutFromExternalAuthenticationFilter : IAsyncAuthorizationFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SignOutFromExternalAuthenticationAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering SignOutFromExternalAuthenticationAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //sign out from the external authentication scheme
            var authenticateResult = await context.HttpContext.AuthenticateAsync(NopAuthenticationDefaults.ExternalAuthenticationScheme);
			_logger.LogDebug("SignOutFromExternalAuthenticationAsync(context={Context}): authenticateResult → {AuthenticateResult}", context, authenticateResult);
            if (authenticateResult.Succeeded)
                await context.HttpContext.SignOutAsync(NopAuthenticationDefaults.ExternalAuthenticationScheme);
				_logger.LogDebug("<<< Exiting SignOutFromExternalAuthenticationAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await SignOutFromExternalAuthenticationAsync(context);
			_logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}