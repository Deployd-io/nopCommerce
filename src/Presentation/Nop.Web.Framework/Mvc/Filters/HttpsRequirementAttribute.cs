using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Data;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that checks whether current connection is secured and properly redirect if necessary
/// </summary>
public sealed class HttpsRequirementAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public HttpsRequirementAttribute(bool ignore = false) : base(typeof(HttpsRequirementFilter))
    {
		logger.LogDebug(">>> Entering HttpsRequirementAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
        Arguments = [ignore];
		logger.LogDebug("<<< Exiting HttpsRequirementAttribute(ignore={Ignore})", ignore);
		logger.LogDebug("HttpsRequirementAttribute(ignore={Ignore}): Arguments → {Arguments}", ignore, Arguments);
		logger.LogDebug("HttpsRequirementAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
    /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
    /// </summary>
    private class HttpsRequirementFilter : IAsyncAuthorizationFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IStoreContext _storeContext;
        protected readonly IWebHelper _webHelper;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public HttpsRequirementFilter(bool ignoreFilter, IStoreContext storeContext, IWebHelper webHelper, IWebHostEnvironment webHostEnvironment)
        {
			logger.LogDebug(">>> Entering HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment})", ignoreFilter, storeContext, webHelper, webHostEnvironment);
            _ignoreFilter = ignoreFilter;
            _storeContext = storeContext;
			logger.LogDebug("HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _storeContext → {_storeContext}", ignoreFilter, storeContext, webHelper, webHostEnvironment, _storeContext);
			logger.LogDebug("HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, storeContext, webHelper, webHostEnvironment, _ignoreFilter);
            _webHelper = webHelper;
			logger.LogDebug("HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _webHelper → {_webHelper}", ignoreFilter, storeContext, webHelper, webHostEnvironment, _webHelper);
            _webHostEnvironment = webHostEnvironment;
			logger.LogDebug("<<< Exiting HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment})", ignoreFilter, storeContext, webHelper, webHostEnvironment);
			logger.LogDebug("HttpsRequirementFilter(ignoreFilter={IgnoreFilter},storeContext={StoreContext},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _webHostEnvironment → {_webHostEnvironment}", ignoreFilter, storeContext, webHelper, webHostEnvironment, _webHostEnvironment);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckHttpsRequirementAsync(AuthorizationFilterContext context)
        {
			logger.LogDebug(">>> Entering CheckHttpsRequirementAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests, otherwise the browser might not propagate the verb and request body correctly
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<HttpsRequirementAttribute>()
                .FirstOrDefault();

            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            var store = await _storeContext.GetCurrentStoreAsync();

			logger.LogDebug("CheckHttpsRequirementAsync(context={Context}): store → {Store}", context, store);
            //whether current connection is secured
            var currentConnectionSecured = _webHelper.IsCurrentConnectionSecured();

			logger.LogDebug("CheckHttpsRequirementAsync(context={Context}): currentConnectionSecured → {CurrentConnectionSecured}", context, currentConnectionSecured);
            //link caching can cause unstable behavior in development environments, when we use permanent redirects
            var isPermanent = !_webHostEnvironment.IsDevelopment();

			logger.LogDebug("CheckHttpsRequirementAsync(context={Context}): isPermanent → {IsPermanent}", context, isPermanent);
            //page should be secured, so redirect (permanent) to HTTPS version of page
            if (store.SslEnabled && !currentConnectionSecured)
                context.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, true), isPermanent);

            //page shouldn't be secured, so redirect (permanent) to HTTP version of page
            if (!store.SslEnabled && currentConnectionSecured)
                context.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, false), isPermanent);
				logger.LogDebug("<<< Exiting CheckHttpsRequirementAsync(context={Context})", context);
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
            await CheckHttpsRequirementAsync(context);
			logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}
