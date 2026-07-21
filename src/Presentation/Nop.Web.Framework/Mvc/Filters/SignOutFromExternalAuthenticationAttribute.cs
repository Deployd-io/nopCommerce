using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Authentication;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that sign out from the external authentication scheme
/// </summary>
public sealed class SignOutFromExternalAuthenticationAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SignOutFromExternalAuthenticationAttribute() : base(typeof(SignOutFromExternalAuthenticationFilter))
    {
	logger.LogDebug(">>> Entering SignOutFromExternalAuthenticationAttribute()");
	logger.LogDebug("<<< Exiting SignOutFromExternalAuthenticationAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that sign out from the external authentication scheme
    /// </summary>
    private class SignOutFromExternalAuthenticationFilter : IAsyncAuthorizationFilter
    {
        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SignOutFromExternalAuthenticationAsync(AuthorizationFilterContext context)
        {
			logger.LogDebug(">>> Entering SignOutFromExternalAuthenticationAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //sign out from the external authentication scheme
            var authenticateResult = await context.HttpContext.AuthenticateAsync(NopAuthenticationDefaults.ExternalAuthenticationScheme);
            if (authenticateResult.Succeeded)
				logger.LogDebug("SignOutFromExternalAuthenticationAsync(context={Context}): authenticateResult → {AuthenticateResult}", context, authenticateResult);
                await context.HttpContext.SignOutAsync(NopAuthenticationDefaults.ExternalAuthenticationScheme);
				logger.LogDebug("<<< Exiting SignOutFromExternalAuthenticationAsync(context={Context})", context);
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
			logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await SignOutFromExternalAuthenticationAsync(context);
			logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}
