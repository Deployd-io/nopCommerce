using Microsoft.Extensions.Logging;
namespace Nop.Web.Infrastructure.Installation;

/// <summary>
/// Language class for installation process
/// </summary>
public partial class InstallationLanguage
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public InstallationLanguage()
    {
		_logger.LogError(">>> Entering InstallationLanguage()");
        Resources = new List<InstallationLocaleResource>();
		_logger.LogDebug("InstallationLanguage(): Resources → {Resources}", Resources);
    _logger.LogError("InstallationLanguage(): Resources → {Resources}", Resources);
    _logger.LogWarning("<<< Exiting InstallationLanguage()");
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