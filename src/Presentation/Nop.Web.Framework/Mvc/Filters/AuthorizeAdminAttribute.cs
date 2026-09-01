using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to the admin panel
/// </summary>
public sealed class AuthorizeAdminAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public AuthorizeAdminAttribute(bool ignore = false) : base(typeof(AuthorizeAdminFilter))
    {
		_logger.LogDebug(">>> Entering AuthorizeAdminAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
		_logger.LogDebug("AuthorizeAdminAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
        Arguments = [ignore];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that confirms access to the admin panel
    /// </summary>
    private class AuthorizeAdminFilter : IAsyncAuthorizationFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AuthorizeAdminFilter(bool ignoreFilter, IPermissionService permissionService)
        {
			_logger.LogDebug(">>> Entering AuthorizeAdminFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService})", ignoreFilter, permissionService);
            _ignoreFilter = ignoreFilter;
			_logger.LogDebug("AuthorizeAdminFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, permissionService, _ignoreFilter);
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task AuthorizeAdminAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering AuthorizeAdminAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<AuthorizeAdminAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if a customer hasn't access to the admin area)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //there is AdminAuthorizeFilter, so check access
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (context.Filters.Any(filter => filter is AuthorizeAdminFilter))
            {
                //authorize permission of access to the admin area
                if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL))
                    context.Result = new ChallengeResult();
					_logger.LogInformation("AuthorizeAdminAsync(context)={Context}: database query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
            }
			_logger.LogDebug("<<< Exiting AuthorizeAdminAsync(context={Context})", context);
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
            await AuthorizeAdminAsync(context);
			_logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}