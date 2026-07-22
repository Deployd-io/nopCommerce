namespace Nop.Web.Infrastructure.Installation;

/// <summary>
/// Language class for installation process
/// </summary>
public partial class InstallationLanguage
{
    public InstallationLanguage()
    {
		logger.LogDebug(">>> Entering InstallationLanguage()");
        Resources = new List<InstallationLocaleResource>();
		logger.LogDebug("<<< Exiting InstallationLanguage()");
		logger.LogDebug("InstallationLanguage(): Resources → {Resources}", Resources);
    }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsDefault { get; set; }
    public bool IsRightToLeft { get; set; }

    public List<InstallationLocaleResource> Resources { get; protected set; }
}

public partial class InstallationLocaleResource
{
    public string Name { get; set; }
    public string Value { get; set; }
}
