using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that check existence of passed form key and return result as an action parameter 
/// </summary>
public sealed class ParameterBasedOnFormNameAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute 
    /// </summary>
    /// <param name="formKeyName">The name of the form key whose existence is to be checked</param>
    /// <param name="actionParameterName">The name of the action parameter to which the result will be passed</param>
    public ParameterBasedOnFormNameAttribute(string formKeyName, string actionParameterName) : base(typeof(ParameterBasedOnFormNameFilter))
    {
		_logger.LogError(">>> Entering ParameterBasedOnFormNameAttribute(formKeyName={FormKeyName},actionParameterName={ActionParameterName})", formKeyName, actionParameterName);
        Arguments = [formKeyName, actionParameterName];
		_logger.LogDebug("ParameterBasedOnFormNameAttribute(formKeyName={FormKeyName},actionParameterName={ActionParameterName}): Arguments → {Arguments}", formKeyName, actionParameterName, Arguments);
    _logger.LogWarning("<<< Exiting ParameterBasedOnFormNameAttribute(formKeyName={FormKeyName},actionParameterName={ActionParameterName})", formKeyName, actionParameterName);
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that check existence of passed form key and return result as an action parameter
    /// </summary>
    private class ParameterBasedOnFormNameFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly string _formKeyName;
        protected readonly string _actionParameterName;

        #endregion

        #region Ctor

        public ParameterBasedOnFormNameFilter(string formKeyName, string actionParameterName)
        {
			_logger.LogError(">>> Entering ParameterBasedOnFormNameFilter(formKeyName={FormKeyName},actionParameterName={ActionParameterName})", formKeyName, actionParameterName);
            _formKeyName = formKeyName;
            _actionParameterName = actionParameterName;
			_logger.LogDebug("ParameterBasedOnFormNameFilter(formKeyName={FormKeyName},actionParameterName={ActionParameterName}): _formKeyName → {_formKeyName}", formKeyName, actionParameterName, _formKeyName);
        _logger.LogWarning("<<< Exiting ParameterBasedOnFormNameFilter(formKeyName={FormKeyName},actionParameterName={ActionParameterName})", formKeyName, actionParameterName);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckParameterBasedOnFormNameAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering CheckParameterBasedOnFormNameAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //if form key with '_formKeyName' exists, then set specified '_actionParameterName' to true
            context.ActionArguments[_actionParameterName] = await context.HttpContext.Request.IsFormAnyAsync(key => key.Equals(_formKeyName));

            //we check whether form key with '_formKeyName' exists only
            //uncomment the code below if you want to check whether form value is specified
            //context.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(await context.HttpContext.Request.GetFormValueAsync(_formKeyName));
			_logger.LogWarning("<<< Exiting CheckParameterBasedOnFormNameAsync(context={Context})", context);
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
			_logger.LogError(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            await CheckParameterBasedOnFormNameAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}