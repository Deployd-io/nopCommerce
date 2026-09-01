using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Http;
using Nop.Data;
using Nop.Services.Security;
using Nop.Services.Topics;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to a closed store
/// </summary>
public sealed class CheckAccessClosedStoreAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public CheckAccessClosedStoreAttribute(bool ignore = false) : base(typeof(CheckAccessClosedStoreFilter))
    {
		_logger.LogDebug(">>> Entering CheckAccessClosedStoreAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
		_logger.LogDebug("CheckAccessClosedStoreAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
    /// Represents a filter that confirms access to closed store
    /// </summary>
    private class CheckAccessClosedStoreFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IPermissionService _permissionService;
        protected readonly IStoreContext _storeContext;
        protected readonly ITopicService _topicService;
        protected readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public CheckAccessClosedStoreFilter(bool ignoreFilter,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ITopicService topicService,
            StoreInformationSettings storeInformationSettings)
        {
			_logger.LogDebug(">>> Entering CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings})", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings);
            _ignoreFilter = ignoreFilter;
			_logger.LogDebug("CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings, _ignoreFilter);
            _permissionService = permissionService;
			_logger.LogDebug("CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings}): _permissionService → {_permissionService}", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings, _permissionService);
            _storeContext = storeContext;
			_logger.LogDebug("CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings}): _storeContext → {_storeContext}", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings, _storeContext);
            _topicService = topicService;
			_logger.LogDebug("CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings}): _topicService → {_topicService}", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings, _topicService);
            _storeInformationSettings = storeInformationSettings;
			_logger.LogDebug("CheckAccessClosedStoreFilter(ignoreFilter={IgnoreFilter},permissionService={PermissionService},storeContext={StoreContext},topicService={TopicService},storeInformationSettings={StoreInformationSettings}): _storeInformationSettings → {_storeInformationSettings}", ignoreFilter, permissionService, storeContext, topicService, storeInformationSettings, _storeInformationSettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckAccessClosedStoreAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering CheckAccessClosedStoreAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckAccessClosedStoreAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if a store is closed)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //store isn't closed
            if (!_storeInformationSettings.StoreClosed)
                return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;            

            //topics accessible when a store is closed
            if (controllerName.Equals("Topic", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("TopicDetails", StringComparison.InvariantCultureIgnoreCase))
            {
                //get identifiers of topics are accessible when a store is closed

                var store = await _storeContext.GetCurrentStoreAsync();
				_logger.LogDebug("CheckAccessClosedStoreAsync(context={Context}): store → {Store}", context, store);
                var allowedTopicIds = (await _topicService.GetAllTopicsAsync(store.Id))
                    .Where(topic => topic.AccessibleWhenStoreClosed)
                    .Select(topic => topic.Id);

                //check whether requested topic is allowed
                var requestedTopicId = context.RouteData.Values["topicId"] as int?;
				_logger.LogDebug("CheckAccessClosedStoreAsync(context={Context}): requestedTopicId → {RequestedTopicId}", context, requestedTopicId);
                if (requestedTopicId.HasValue && allowedTopicIds.Contains(requestedTopicId.Value))
                    return;
            }

            //check whether current customer has access to a closed store
            if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ACCESS_CLOSED_STORE))
                return;

            //store is closed and no access, so redirect to 'StoreClosed' page
            context.Result = new RedirectToRouteResult(NopRouteNames.Standard.STORE_CLOSED, null);
			_logger.LogDebug("<<< Exiting CheckAccessClosedStoreAsync(context={Context})", context);
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
            await CheckAccessClosedStoreAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}