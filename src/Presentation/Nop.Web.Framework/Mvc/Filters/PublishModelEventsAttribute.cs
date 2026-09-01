using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Events;
using Nop.Core.Http.Extensions;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that publish ModelReceived event before the action executes, after model binding is complete
/// and publish ModelPrepared event after the action executes, before the action result
/// </summary>
public sealed class PublishModelEventsAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public PublishModelEventsAttribute(bool ignore = false) : base(typeof(PublishModelEventsFilter))
    {
		_logger.LogDebug(">>> Entering PublishModelEventsAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
		_logger.LogDebug("PublishModelEventsAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
    /// Represents filter that publish ModelReceived event before the action executes, after model binding is complete
    /// and publish ModelPrepared event after the action executes, before the action result
    /// </summary>
    private class PublishModelEventsFilter : IAsyncActionFilter, IAsyncResultFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public PublishModelEventsFilter(bool ignoreFilter,
            IEventPublisher eventPublisher)
        {
			_logger.LogDebug(">>> Entering PublishModelEventsFilter(ignoreFilter={IgnoreFilter},eventPublisher={EventPublisher})", ignoreFilter, eventPublisher);
            _ignoreFilter = ignoreFilter;
			_logger.LogDebug("PublishModelEventsFilter(ignoreFilter={IgnoreFilter},eventPublisher={EventPublisher}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, eventPublisher, _ignoreFilter);
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Whether to ignore this filter
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>Result</returns>
        protected virtual bool IgnoreFilter(FilterContext context)
        {
			_logger.LogDebug(">>> Entering IgnoreFilter(context={Context})", context);
            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<PublishModelEventsAttribute>()
                .FirstOrDefault();

			_logger.LogDebug("<<< Exiting IgnoreFilter(context={Context})", context);
            return actionFilter?.IgnoreFilter ?? _ignoreFilter;
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelReceivedEventAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering PublishModelReceivedEventAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in POST requests
            if (!context.HttpContext.Request.IsPostRequest())
                return;

            if (IgnoreFilter(context))
                return;

            //model received event
            foreach (var model in context.ActionArguments.Values.OfType<BaseNopModel>())
            {
                //we publish the ModelReceived event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelReceived<BaseNopModel>> interface to handle this event
                await _eventPublisher.ModelReceivedAsync(model, context.ModelState);
            }
			_logger.LogDebug("<<< Exiting PublishModelReceivedEventAsync(context={Context})", context);
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelPreparedEventAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering PublishModelPreparedEventAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Controller is Controller controller)
                await _eventPublisher.ModelPreparedAsync(controller.ViewData.Model);
				_logger.LogDebug("<<< Exiting PublishModelPreparedEventAsync(context={Context})", context);
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
            await PublishModelReceivedEventAsync(context);
            if (context.Result == null)
                await next();
            await PublishModelPreparedEventAsync(context);
			_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        /// <summary>Called asynchronously before the action result.</summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
			_logger.LogDebug(">>> Entering OnResultExecutionAsync(context={Context},next={Next})", context, next);
            ArgumentNullException.ThrowIfNull(context);

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Result is JsonResult result)
                await _eventPublisher.ModelPreparedAsync(result.Value);

            await next();
			_logger.LogDebug("<<< Exiting OnResultExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}