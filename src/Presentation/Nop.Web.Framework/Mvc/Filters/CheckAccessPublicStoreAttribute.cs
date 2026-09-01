using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to public store
/// </summary>
public sealed class CheckAccessPublicStoreAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public CheckAccessPublicStoreAttribute(bool ignore = false) : base(typeof(CheckAccessPublicStoreFilter))
    {
		_logger.LogDebug(">>> Entering CheckAccessPublicStoreAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
		_logger.LogDebug("CheckAccessPublicStoreAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
    /// Represents a filter that confirms access to public store
    /// </summary>
    private class CheckAccessPublicStoreFilter : IAsyncAuthorizationFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CheckAccessPublicStoreFilter(bool ignoreFilter, IPermissionService permissionService)
        {
			_logger.LogDebug(">>> Entering CheckAccessPublicStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService})", ignoreFilter, permissionService);
            _ignoreFilter = ignoreFilter;
			_logger.LogDebug("CheckAccessPublicStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, permissionService, _ignoreFilter);
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckAccessPublicStoreAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering CheckAccessPublicStoreAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckAccessPublicStoreAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if navigation is not allowed)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //check whether current customer has access to a public store
            if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION))
                return;

            //customer hasn't access to a public store
            context.Result = new ChallengeResult();
			_logger.LogDebug("<<< Exiting CheckAccessPublicStoreAsync(context={Context})", context);
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
            await CheckAccessPublicStoreAsync(context);
			_logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}