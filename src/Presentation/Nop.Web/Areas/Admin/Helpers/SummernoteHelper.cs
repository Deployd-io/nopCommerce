using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Helpers;

/// <summary>
/// Summernote helper
/// </summary>
public partial class SummernoteHelper : ISummernoteHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;

    public SummernoteHelper(
        INopFileProvider nopFileProvider, 
        IStaticCacheManager staticCacheManager, 
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering SummernoteHelper(nopFileProvider={NopFileProvider},staticCacheManager={StaticCacheManager},workContext={WorkContext})", nopFileProvider, staticCacheManager, workContext);
        _nopFileProvider = nopFileProvider;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("SummernoteHelper(nopFileProvider={NopFileProvider},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", nopFileProvider, staticCacheManager, workContext, _staticCacheManager);
        _workContext = workContext;
		_logger.LogDebug("SummernoteHelper(nopFileProvider={NopFileProvider},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _workContext → {_workContext}", nopFileProvider, staticCacheManager, workContext, _workContext);
    }

    /// <summary>
    /// Get Summernote language name for current language 
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Summernote language name
    /// </returns>
    public virtual async Task<string> GetRichEditorLanguageAsync()
    {
		_logger.LogError(">>> Entering GetRichEditorLanguageAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var languageCulture = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;

		_logger.LogError("GetRichEditorLanguageAsync(): languageCulture → {LanguageCulture}", languageCulture);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SummernoteLanguageKey,
            languageCulture);

        return await _staticCacheManager.GetAsync(cacheKey, () =>
        {
            var langFile = $"summernote-{languageCulture}.min.js";
            var directoryPath = _nopFileProvider.GetAbsolutePath(@"lib_npm\summernote\lang");
            var fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");

            return fileExists ? languageCulture : string.Empty;
        });
		_logger.LogError("GetRichEditorLanguageAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetRichEditorLanguageAsync()");
    }
}
