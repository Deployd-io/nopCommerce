using Microsoft.Extensions.Logging;
namespace Nop.Services.Themes;

/// <summary>
/// Themes uploaded event
/// </summary>
public partial class ThemesUploadedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="uploadedThemes">Uploaded themes</param>
    public ThemesUploadedEvent(IList<ThemeDescriptor> uploadedThemes)
    {
		_logger.LogError(">>> Entering ThemesUploadedEvent(uploadedThemes={UploadedThemes})", uploadedThemes);
        UploadedThemes = uploadedThemes;
		_logger.LogDebug("ThemesUploadedEvent(uploadedThemes={UploadedThemes}): UploadedThemes → {UploadedThemes1}", uploadedThemes, UploadedThemes);
    _logger.LogWarning("<<< Exiting ThemesUploadedEvent(uploadedThemes={UploadedThemes})", uploadedThemes);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded themes
    /// </summary>
    public IList<ThemeDescriptor> UploadedThemes { get; }

    #endregion
}