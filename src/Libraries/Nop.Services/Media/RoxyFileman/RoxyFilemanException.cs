using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Media.RoxyFileman;

/// <summary>
/// Represents errors that occur when working with uploaded files
/// </summary>
public partial class RoxyFilemanException : Exception
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static Dictionary<string, string> _languageResources;

    #endregion

    #region Ctor

    private RoxyFilemanException() : base()
    {
	_logger.LogDebug(">>> Entering RoxyFilemanException()");
	_logger.LogDebug("<<< Exiting RoxyFilemanException()");
	_logger.LogDebug(">>> Entering RoxyFilemanException(key={Key})", key);
    }

    /// <summary>
    /// Initializes a new instance of the RoxyFilemanException
    /// </summary>
    /// <param name="key">Roxy language resource key</param>
    public RoxyFilemanException(string key) : base(GetLocalizedMessage(key))
    {
	_logger.LogDebug("<<< Exiting RoxyFilemanException(key={Key})", key);
	_logger.LogDebug(">>> Entering RoxyFilemanException(key={Key},innerException={InnerException})", key, innerException);
    }

    /// <summary>
    /// Initializes a new instance of the RoxyFilemanException
    /// </summary>
    /// <param name="key">Roxy language resource key</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public RoxyFilemanException(string key, Exception innerException) : base(GetLocalizedMessage(key), innerException)
    {
	_logger.LogDebug("<<< Exiting RoxyFilemanException(key={Key},innerException={InnerException})", key, innerException);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get the language resource value
    /// </summary>
    /// <param name="key">Language resource key</param>
    /// <returns>
    /// The language resource value
    /// </returns>
    protected static string GetLocalizedMessage(string key)
    {
		_logger.LogDebug(">>> Entering GetLocalizedMessage(key={Key})", key);
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		_logger.LogDebug("GetLocalizedMessage(key={Key}): fileProvider → {FileProvider}", key, fileProvider);
        var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;
        var languageFile = fileProvider.GetAbsolutePath($"{NopRoxyFilemanDefaults.LanguageDirectory}/{roxyConfig.LANG}.json");
		_logger.LogDebug("GetLocalizedMessage(key={Key}): languageFile → {LanguageFile}", key, languageFile);

        if (!fileProvider.FileExists(languageFile))
            languageFile = fileProvider.GetAbsolutePath($"{NopRoxyFilemanDefaults.LanguageDirectory}/en.json");

        if (_languageResources is null)
        {
            var json = fileProvider.ReadAllTextAsync(languageFile, Encoding.UTF8).Result;
			_logger.LogDebug("GetLocalizedMessage(key={Key}): json → {Json}", key, json);
            _languageResources = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
		_logger.LogDebug("GetLocalizedMessage(key={Key}): _languageResources → {_languageResources}", key, _languageResources);

        if (_languageResources is null)
            return key;

        if (_languageResources.TryGetValue(key, out var value))
            return value;

		_logger.LogDebug("<<< Exiting GetLocalizedMessage(key={Key})", key);
        return key;
    }

    #endregion
}