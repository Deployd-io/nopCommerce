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
		_logger.LogDebug(">>> Entering CurrentOSUser()");
        Name = Environment.UserName;

		_logger.LogDebug("CurrentOSUser(): Name → {Name}", Name);
        DomainName = Environment.UserDomainName;
		_logger.LogDebug("CurrentOSUser(): DomainName → {DomainName}", DomainName);

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
                UserId = Name;
				_logger.LogDebug("CurrentOSUser(): UserId → {UserId}", UserId);
                Groups = new List<string>();
				_logger.LogDebug("CurrentOSUser(): Groups → {Groups}", Groups);
                break;
        }
		_logger.LogDebug("<<< Exiting CurrentOSUser()");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Populate information about windows user
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void PopulateWindowsUser()
    {
		_logger.LogDebug(">>> Entering PopulateWindowsUser()");
        Groups = WindowsIdentity.GetCurrent().Groups?.Select(p => p.Value).ToList();
        UserId = Name;
		_logger.LogDebug("<<< Exiting PopulateWindowsUser()");
		_logger.LogDebug("PopulateWindowsUser(): UserId → {UserId}", UserId);
		_logger.LogDebug("PopulateWindowsUser(): Groups → {Groups}", Groups);
    }

    /// <summary>
    /// Populate information about linux user
    /// </summary>
    public static void PopulateLinuxUser()
    {
		_logger.LogDebug(">>> Entering PopulateLinuxUser()");
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
		_logger.LogDebug("PopulateLinuxUser(): res → {Res}", res);

        var respars = res.Split("\n");
		_logger.LogDebug("PopulateLinuxUser(): respars → {Respars}", respars);

        UserId = respars[0];
		_logger.LogDebug("PopulateLinuxUser(): UserId → {UserId}", UserId);
        Groups = respars[1].Split(" ").ToList();
		_logger.LogDebug("<<< Exiting PopulateLinuxUser()");
		_logger.LogDebug("PopulateLinuxUser(): Groups → {Groups}", Groups);
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