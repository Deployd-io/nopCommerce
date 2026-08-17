using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that checks SEO friendly URLs for multiple languages and properly redirect if necessary
/// </summary>
public sealed class CheckLanguageSeoCodeAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public CheckLanguageSeoCodeAttribute(bool ignore = false) : base(typeof(CheckLanguageSeoCodeFilter))
    {
		_logger.LogDebug(">>> Entering CheckLanguageSeoCodeAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
        Arguments = [ignore];
		_logger.LogDebug("<<< Exiting CheckLanguageSeoCodeAttribute(ignore={Ignore})", ignore);
		_logger.LogDebug("CheckLanguageSeoCodeAttribute(ignore={Ignore}): Arguments → {Arguments}", ignore, Arguments);
		_logger.LogDebug("CheckLanguageSeoCodeAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
    /// Represents a filter that checks SEO friendly URLs for multiple languages and properly redirect if necessary
    /// </summary>
    private class CheckLanguageSeoCodeFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public CheckLanguageSeoCodeFilter(bool ignoreFilter,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
			_logger.LogDebug(">>> Entering CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings})", ignoreFilter, webHelper, workContext, localizationSettings);
            _ignoreFilter = ignoreFilter;
            _webHelper = webHelper;
			_logger.LogDebug("CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings}): _webHelper → {_webHelper}", ignoreFilter, webHelper, workContext, localizationSettings, _webHelper);
			_logger.LogDebug("CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, webHelper, workContext, localizationSettings, _ignoreFilter);
            _workContext = workContext;
			_logger.LogDebug("CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings}): _workContext → {_workContext}", ignoreFilter, webHelper, workContext, localizationSettings, _workContext);
            _localizationSettings = localizationSettings;
			_logger.LogDebug("<<< Exiting CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings})", ignoreFilter, webHelper, workContext, localizationSettings);
			_logger.LogDebug("CheckLanguageSeoCodeFilter(ignoreFilter={IgnoreFilter},webHelper={WebHelper},workContext={WorkContext},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", ignoreFilter, webHelper, workContext, localizationSettings, _localizationSettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckLanguageSeoCodeAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering CheckLanguageSeoCodeAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckLanguageSeoCodeAttribute>()
                .FirstOrDefault();

            //ignore filter (an action doesn't need to be checked)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //whether SEO friendly URLs are enabled
            if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return;

            //check whether current page URL is already localized URL
            var pageUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
			_logger.LogDebug("CheckLanguageSeoCodeAsync(context={Context}): pageUrl → {PageUrl}", context, pageUrl);
            var result = await pageUrl.IsLocalizedUrlAsync(context.HttpContext.Request.PathBase, true);
			_logger.LogDebug("CheckLanguageSeoCodeAsync(context={Context}): result → {Result}", context, result);
            if (result.IsLocalized)
                return;

            //not localized yet, so redirect to the page with working language SEO code
            var language = await _workContext.GetWorkingLanguageAsync();
			_logger.LogDebug("CheckLanguageSeoCodeAsync(context={Context}): language → {Language}", context, language);
            pageUrl = pageUrl.AddLanguageSeoCodeToUrl(context.HttpContext.Request.PathBase, true, language);

            context.Result = new LocalRedirectResult(pageUrl, false);
			_logger.LogDebug("<<< Exiting CheckLanguageSeoCodeAsync(context={Context})", context);
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
            await CheckLanguageSeoCodeAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}