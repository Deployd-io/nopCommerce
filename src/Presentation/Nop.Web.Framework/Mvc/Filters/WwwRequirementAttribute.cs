using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that checks WWW at the beginning of the URL and properly redirect if necessary
/// </summary>
public sealed class WwwRequirementAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public WwwRequirementAttribute() : base(typeof(WwwRequirementFilter))
    {
	_logger.LogDebug(">>> Entering WwwRequirementAttribute()");
	_logger.LogDebug("<<< Exiting WwwRequirementAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that checks WWW at the beginning of the URL and properly redirect if necessary
    /// </summary>
    private class WwwRequirementFilter : IAsyncAuthorizationFilter
    {
        #region Fields

        protected readonly IWebHelper _webHelper;
        protected readonly SeoSettings _seoSettings;

        #endregion

        #region Ctor

        public WwwRequirementFilter(IWebHelper webHelper,
            SeoSettings seoSettings)
        {
			_logger.LogDebug(">>> Entering WwwRequirementFilter(webHelper={WebHelper},seoSettings={SeoSettings})", webHelper, seoSettings);
            _webHelper = webHelper;
            _seoSettings = seoSettings;
			_logger.LogDebug("<<< Exiting WwwRequirementFilter(webHelper={WebHelper},seoSettings={SeoSettings})", webHelper, seoSettings);
			_logger.LogDebug("WwwRequirementFilter(webHelper={WebHelper},seoSettings={SeoSettings}): _seoSettings → {_seoSettings}", webHelper, seoSettings, _seoSettings);
			_logger.LogDebug("WwwRequirementFilter(webHelper={WebHelper},seoSettings={SeoSettings}): _webHelper → {_webHelper}", webHelper, seoSettings, _webHelper);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check WWW prefix at the beginning of the URL and properly redirect if necessary
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <param name="withWww">Whether URL must start with WWW</param>
        private void RedirectRequest(AuthorizationFilterContext context, bool withWww)
        {
			_logger.LogDebug(">>> Entering RedirectRequest(context={Context},withWww={WithWww})", context, withWww);
            //get scheme depending on securing connection
            var urlScheme = $"{_webHelper.GetCurrentRequestProtocol()}{Uri.SchemeDelimiter}";
			_logger.LogDebug("RedirectRequest(context={Context},withWww={WithWww}): urlScheme → {UrlScheme}", context, withWww, urlScheme);

            //compose start of URL with WWW
            var urlWith3W = $"{urlScheme}www.";
			_logger.LogDebug("RedirectRequest(context={Context},withWww={WithWww}): urlWith3W → {UrlWith3W}", context, withWww, urlWith3W);

            //get requested URL
            var currentUrl = _webHelper.GetThisPageUrl(true);
			_logger.LogDebug("RedirectRequest(context={Context},withWww={WithWww}): currentUrl → {CurrentUrl}", context, withWww, currentUrl);

            //whether requested URL starts with WWW
            var urlStartsWith3W = currentUrl.StartsWith(urlWith3W, StringComparison.OrdinalIgnoreCase);
			_logger.LogDebug("RedirectRequest(context={Context},withWww={WithWww}): urlStartsWith3W → {UrlStartsWith3W}", context, withWww, urlStartsWith3W);

            //page should have WWW prefix, so set 301 (permanent) redirection to URL with WWW
            if (withWww && !urlStartsWith3W)
                context.Result = new RedirectResult(currentUrl.Replace(urlScheme, urlWith3W), true);

            //page shouldn't have WWW prefix, so set 301 (permanent) redirection to URL without WWW
            if (!withWww && urlStartsWith3W)
                context.Result = new RedirectResult(currentUrl.Replace(urlWith3W, urlScheme), true);
				_logger.LogDebug("<<< Exiting RedirectRequest(context={Context},withWww={WithWww})", context, withWww);
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        private void CheckWwwRequirement(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering CheckWwwRequirement(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests, otherwise the browser might not propagate the verb and request body correctly.
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //ignore this rule for localhost
            if (_webHelper.IsLocalRequest(context.HttpContext.Request))
                return;

            switch (_seoSettings.WwwRequirement)
            {
                case WwwRequirement.WithWww:
                    //redirect to URL with starting WWW
                    RedirectRequest(context, true);
                    break;

                case WwwRequirement.WithoutWww:
                    //redirect to URL without starting WWW
                    RedirectRequest(context, false);
                    break;

                case WwwRequirement.NoMatter:
                    //do nothing
                    break;

                default:
			_logger.LogDebug("<<< Exiting CheckWwwRequirement(context={Context})", context);
                    throw new NopException("Not supported WwwRequirement parameter");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            CheckWwwRequirement(context);
			_logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
            return Task.CompletedTask;
        }

        #endregion
    }

    #endregion
}