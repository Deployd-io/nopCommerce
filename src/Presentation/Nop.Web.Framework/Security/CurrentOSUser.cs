using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Security;

/// <summary>
/// Represent information about current OS user 
/// </summary>
public static class CurrentOSUser
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    static CurrentOSUser()
    {
		_logger.LogError(">>> Entering CurrentOSUser()");
        Name = Environment.UserName;

		_logger.LogDebug("CurrentOSUser(): Name → {Name}", Name);
        DomainName = Environment.UserDomainName;
		_logger.LogDebug("CurrentOSUser(): DomainName → {DomainName}", DomainName);
		_logger.LogError("CurrentOSUser(): Name → {Name}", Name);

        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                if (OperatingSystem.IsWindows())
                    PopulateWindowsUser();
                break;
            case PlatformID.Unix:
                PopulateLinuxUser();
                break;
            default:
				_logger.LogWarning("<<< Exiting CurrentOSUser()");
                UserId = Name;
				_logger.LogError("CurrentOSUser(): UserId → {UserId}", UserId);
				_logger.LogDebug("CurrentOSUser(): UserId → {UserId}", UserId);
                Groups = new List<string>();
				_logger.LogError("CurrentOSUser(): Groups → {Groups}", Groups);
                break;
        }
		_logger.LogDebug("CurrentOSUser(): Groups → {Groups}", Groups);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Populate information about windows user
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void PopulateWindowsUser()
    {
		_logger.LogError(">>> Entering PopulateWindowsUser()");
        Groups = WindowsIdentity.GetCurrent().Groups?.Select(p => p.Value).ToList();
        UserId = Name;
		_logger.LogError("PopulateWindowsUser(): UserId → {UserId}", UserId);
    }

    /// <summary>
    /// Populate information about linux user
    /// </summary>
    public static void PopulateLinuxUser()
    {
		_logger.LogError(">>> Entering PopulateLinuxUser()");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = "sh",
                Arguments = "-c \" id -u ; id -G \""
            }
        };

        process.Start();
        process.WaitForExit();

        var res = process.StandardOutput.ReadToEnd();
		_logger.LogError("PopulateLinuxUser(): res → {Res}", res);

		_logger.LogWarning("<<< Exiting PopulateLinuxUser()");
        var respars = res.Split("\n");
		_logger.LogError("PopulateLinuxUser(): respars → {Respars}", respars);

        UserId = respars[0];
		_logger.LogError("PopulateLinuxUser(): UserId → {UserId}", UserId);
        Groups = respars[1].Split(" ").ToList();
		_logger.LogError("PopulateLinuxUser(): Groups → {Groups}", Groups);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns user name
    /// </summary>
    public static string Name { get; }

    /// <summary>
    /// Returns user domain name for Windows or group for Linux
    /// </summary>
    public static string DomainName { get; }

    /// <summary>
    /// Returns user groups
    /// </summary>
    public static List<string> Groups { get; private set; }

    /// <summary>
    /// Returns user name for Windows or user Id  for Linux like 1001
    /// </summary>
    public static string UserId { get; private set; }

    /// <summary>
    /// Returns full user name
    /// </summary>
    public static string FullName => $@"{DomainName}\{Name}";

    #endregion
}