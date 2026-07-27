namespace Nop.Services.Themes;

/// <summary>
/// Themes uploaded event
/// </summary>
public partial class ThemesUploadedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="uploadedThemes">Uploaded themes</param>
    public ThemesUploadedEvent(IList<ThemeDescriptor> uploadedThemes)
    {
		logger.LogDebug(">>> Entering ThemesUploadedEvent(uploadedThemes={UploadedThemes})", uploadedThemes);
        UploadedThemes = uploadedThemes;
		logger.LogDebug("<<< Exiting ThemesUploadedEvent(uploadedThemes={UploadedThemes})", uploadedThemes);
		logger.LogDebug("ThemesUploadedEvent(uploadedThemes={UploadedThemes}): UploadedThemes → {UploadedThemes1}", uploadedThemes, UploadedThemes);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded themes
    /// </summary>
    public IList<ThemeDescriptor> UploadedThemes { get; }

    #endregion
}