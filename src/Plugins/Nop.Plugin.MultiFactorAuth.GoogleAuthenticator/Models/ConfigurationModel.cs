using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationModel()
    {
		_logger.LogError(">>> Entering ConfigurationModel()");
        GoogleAuthenticatorSearchModel = new GoogleAuthenticatorSearchModel();
		_logger.LogDebug("ConfigurationModel(): GoogleAuthenticatorSearchModel → {GoogleAuthenticatorSearchModel}", GoogleAuthenticatorSearchModel);
    _logger.LogError("ConfigurationModel(): GoogleAuthenticatorSearchModel → {GoogleAuthenticatorSearchModel}", GoogleAuthenticatorSearchModel);
    _logger.LogWarning("<<< Exiting ConfigurationModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.QRPixelsPerModule")]
    public int QRPixelsPerModule { get; set; }

    [NopResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.BusinessPrefix")]
    public string BusinessPrefix { get; set; }

    public GoogleAuthenticatorSearchModel GoogleAuthenticatorSearchModel { get; set; }

    #endregion
}