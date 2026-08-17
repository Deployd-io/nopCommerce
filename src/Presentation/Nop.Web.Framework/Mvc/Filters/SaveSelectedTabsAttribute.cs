using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Web.Framework.Controllers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that saves a selected tabs for tabs
/// </summary>
public sealed class SaveSelectedTabAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    /// <param name="persistForTheNextRequest">Whether a message should be persisted for the next request</param>
    public SaveSelectedTabAttribute(bool ignore = false, bool persistForTheNextRequest = true) : base(typeof(SaveSelectedTabFilter))
    {
		_logger.LogDebug(">>> Entering SaveSelectedTabAttribute(ignore={Ignore},persistForTheNextRequest={PersistForTheNextRequest})", ignore, persistForTheNextRequest);
        PersistForTheNextRequest = persistForTheNextRequest;
        IgnoreFilter = ignore;
		_logger.LogDebug("SaveSelectedTabAttribute(ignore={Ignore},persistForTheNextRequest={PersistForTheNextRequest}): IgnoreFilter → {IgnoreFilter}", ignore, persistForTheNextRequest, IgnoreFilter);
		_logger.LogDebug("SaveSelectedTabAttribute(ignore={Ignore},persistForTheNextRequest={PersistForTheNextRequest}): PersistForTheNextRequest → {PersistForTheNextRequest1}", ignore, persistForTheNextRequest, PersistForTheNextRequest);
        Arguments = [ignore, persistForTheNextRequest];
		_logger.LogDebug("<<< Exiting SaveSelectedTabAttribute(ignore={Ignore},persistForTheNextRequest={PersistForTheNextRequest})", ignore, persistForTheNextRequest);
		_logger.LogDebug("SaveSelectedTabAttribute(ignore={Ignore},persistForTheNextRequest={PersistForTheNextRequest}): Arguments → {Arguments}", ignore, persistForTheNextRequest, Arguments);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    /// <summary>
    /// Gets a value indicating whether a message should be persisted for the next request
    /// </summary>
    public bool PersistForTheNextRequest { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
    /// </summary>
    private class SaveSelectedTabFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;
        private bool _persistForTheNextRequest;
        protected readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public SaveSelectedTabFilter(bool ignoreFilter, bool persistForTheNextRequest,
            IWebHelper webHelper)
        {
            _ignoreFilter = ignoreFilter;
            _persistForTheNextRequest = persistForTheNextRequest;
			_logger.LogDebug("SaveSelectedTabFilter(ignoreFilter={IgnoreFilter},persistForTheNextRequest={PersistForTheNextRequest},webHelper={WebHelper}): _persistForTheNextRequest → {_persistForTheNextRequest}", ignoreFilter, persistForTheNextRequest, webHelper, _persistForTheNextRequest);
			_logger.LogDebug("SaveSelectedTabFilter(ignoreFilter={IgnoreFilter},persistForTheNextRequest={PersistForTheNextRequest},webHelper={WebHelper}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, persistForTheNextRequest, webHelper, _ignoreFilter);
            _webHelper = webHelper;
			_logger.LogDebug("<<< Exiting SaveSelectedTabFilter(ignoreFilter={IgnoreFilter},persistForTheNextRequest={PersistForTheNextRequest},webHelper={WebHelper})", ignoreFilter, persistForTheNextRequest, webHelper);
			_logger.LogDebug("SaveSelectedTabFilter(ignoreFilter={IgnoreFilter},persistForTheNextRequest={PersistForTheNextRequest},webHelper={WebHelper}): _webHelper → {_webHelper}", ignoreFilter, persistForTheNextRequest, webHelper, _webHelper);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SaveSelectedTabAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering SaveSelectedTabAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in POST requests
            if (!context.HttpContext.Request.IsPostRequest())
                return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<SaveSelectedTabAttribute>()
                .FirstOrDefault();

            //ignore filter
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            var persistForTheNextRequest = actionFilter?.PersistForTheNextRequest ?? _persistForTheNextRequest;

            if (context.Controller is BaseController controller)
                await controller.SaveSelectedTabNameAsync(persistForTheNextRequest: persistForTheNextRequest);
				_logger.LogDebug("<<< Exiting SaveSelectedTabAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
			_logger.LogDebug(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            await next();
            await SaveSelectedTabAsync(context);
			_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}