using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents class language parameter transformer
/// </summary>
public partial class LanguageParameterTransformer : IOutboundParameterTransformer
{
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;

    #endregion

    #region Ctor

    public LanguageParameterTransformer(IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService)
    {
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Transforms the specified route value to a string for inclusion in a URI
    /// </summary>
    /// <param name="value">The route value to transform</param>
    /// <returns>The transformed value</returns>
    public string TransformOutbound(object value)
    {
		logger.LogDebug(">>> Entering TransformOutbound(value={Value})", value);
        //first try to get a language code from the route values
        var routeValues = _httpContextAccessor.HttpContext.Request.RouteValues;
        if (routeValues.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var routeValue))
        {
            //ensure this language is available
            var code = routeValue?.ToString();
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();
			logger.LogDebug("TransformOutbound(value={Value}): code → {Code}", value, code);
			logger.LogDebug("TransformOutbound(value={Value}): storeContext → {StoreContext}", value, storeContext);
            var store = storeContext.GetCurrentStore();
            var languages = _languageService.GetAllLanguages(storeId: store.Id);
			logger.LogDebug("TransformOutbound(value={Value}): store → {Store}", value, store);
			logger.LogDebug("TransformOutbound(value={Value}): languages → {Languages}", value, languages);
            var language = languages
                .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(code, StringComparison.InvariantCultureIgnoreCase));
            if (language is not null)
                return language.UniqueSeoCode.ToLowerInvariant();
        }

        //if there is no code in the route values, check whether the value is passed
        if (value is null)
            return string.Empty;

        //or use the current language code
        //we don't use the passed value, since it's always either the same as the current one or it's the default value (empty)
        var workContext = EngineContext.Current.Resolve<IWorkContext>();
        var currentLanguage = workContext.GetWorkingLanguageAsync().Result;
		logger.LogDebug("TransformOutbound(value={Value}): workContext → {WorkContext}", value, workContext);
		logger.LogDebug("TransformOutbound(value={Value}): currentLanguage → {CurrentLanguage}", value, currentLanguage);
		logger.LogDebug("<<< Exiting TransformOutbound(value={Value})", value);
        return currentLanguage.UniqueSeoCode.ToLowerInvariant();
    }

    #endregion
}