using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Http;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents custom overridden redirect result executor
/// </summary>
public partial class NopLocalRedirectResultExecutor : LocalRedirectResultExecutor
{
    #region Fields

    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly SecuritySettings _securitySettings;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public NopLocalRedirectResultExecutor(ILoggerFactory loggerFactory,
        IUrlHelperFactory urlHelperFactory,
        SecuritySettings securitySettings,
        IWebHelper webHelper) : base(loggerFactory, urlHelperFactory)
    {
		_logger.LogDebug(">>> Entering NopLocalRedirectResultExecutor(loggerFactory={LoggerFactory},urlHelperFactory={UrlHelperFactory},securitySettings={SecuritySettings},webHelper={WebHelper})", loggerFactory, urlHelperFactory, securitySettings, webHelper);
        _urlHelperFactory = urlHelperFactory;
        _securitySettings = securitySettings;
		_logger.LogDebug("NopLocalRedirectResultExecutor(loggerFactory={LoggerFactory},urlHelperFactory={UrlHelperFactory},securitySettings={SecuritySettings},webHelper={WebHelper}): _securitySettings → {_securitySettings}", loggerFactory, urlHelperFactory, securitySettings, webHelper, _securitySettings);
		_logger.LogDebug("NopLocalRedirectResultExecutor(loggerFactory={LoggerFactory},urlHelperFactory={UrlHelperFactory},securitySettings={SecuritySettings},webHelper={WebHelper}): _urlHelperFactory → {_urlHelperFactory}", loggerFactory, urlHelperFactory, securitySettings, webHelper, _urlHelperFactory);
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting NopLocalRedirectResultExecutor(loggerFactory={LoggerFactory},urlHelperFactory={UrlHelperFactory},securitySettings={SecuritySettings},webHelper={WebHelper})", loggerFactory, urlHelperFactory, securitySettings, webHelper);
		_logger.LogDebug("NopLocalRedirectResultExecutor(loggerFactory={LoggerFactory},urlHelperFactory={UrlHelperFactory},securitySettings={SecuritySettings},webHelper={WebHelper}): _webHelper → {_webHelper}", loggerFactory, urlHelperFactory, securitySettings, webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute passed redirect result
    /// </summary>
    /// <param name="context">Action context</param>
    /// <param name="result">Redirect result</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ExecuteAsync(ActionContext context, LocalRedirectResult result)
    {
		_logger.LogDebug(">>> Entering ExecuteAsync(context={Context},result={Result})", context, result);
        ArgumentNullException.ThrowIfNull(result);

        if (_securitySettings.AllowNonAsciiCharactersInHeaders)
        {
            //passed redirect URL may contain non-ASCII characters, that are not allowed now (see https://github.com/aspnet/KestrelHttpServer/issues/1144)
            //so we force to encode this URL before processing
            var url = WebUtility.UrlDecode(result.Url);
			_logger.LogDebug("ExecuteAsync(context={Context},result={Result}): url → {Url}", context, result, url);
            var urlHelper = result.UrlHelper ?? _urlHelperFactory.GetUrlHelper(context);
			_logger.LogDebug("ExecuteAsync(context={Context},result={Result}): urlHelper → {UrlHelper}", context, result, urlHelper);
            var isLocalUrl = urlHelper.IsLocalUrl(url);
			_logger.LogDebug("ExecuteAsync(context={Context},result={Result}): isLocalUrl → {IsLocalUrl}", context, result, isLocalUrl);

            var uriStr = url;
			_logger.LogDebug("ExecuteAsync(context={Context},result={Result}): uriStr → {UriStr}", context, result, uriStr);
            if (isLocalUrl)
            {
                var pathBase = context.HttpContext.Request.PathBase;
                uriStr = $"{_webHelper.GetStoreLocation().TrimEnd('/')}{(url.StartsWith(pathBase) && !string.IsNullOrEmpty(pathBase) ? url.Replace(pathBase, "") : url)}";
            }
            var uri = new Uri(uriStr, UriKind.Absolute);
			_logger.LogDebug("ExecuteAsync(context={Context},result={Result}): uri → {Uri}", context, result, uri);

            //Allowlist redirect URI schemes to http and https
            if ((uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && urlHelper.IsLocalUrl(uri.AbsolutePath))
                result.Url = isLocalUrl ? uri.PathAndQuery : $"{uri.GetLeftPart(UriPartial.Query)}{uri.Fragment}";
            else
                result.Url = urlHelper.RouteUrl(NopRouteNames.General.HOMEPAGE);
        }

		_logger.LogDebug("<<< Exiting ExecuteAsync(context={Context},result={Result})", context, result);
        return base.ExecuteAsync(context, result);
    }

    #endregion
}