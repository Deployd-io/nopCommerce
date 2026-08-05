using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace Nop.Web.Framework.Security;

/// <summary>
/// Represent information about current OS user 
/// </summary>
public static class CurrentOSUser
{
    #region Ctor

    static CurrentOSUser()
    {
		logger.LogDebug(">>> Entering CurrentOSUser()");
        Name = Environment.UserName;

		logger.LogDebug("CurrentOSUser(): Name → {Name}", Name);
        DomainName = Environment.UserDomainName;
		logger.LogDebug("CurrentOSUser(): DomainName → {DomainName}", DomainName);

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
				logger.LogDebug("CurrentOSUser(): UserId → {UserId}", UserId);
                Groups = new List<string>();
				logger.LogDebug("CurrentOSUser(): Groups → {Groups}", Groups);
                break;
        }
		logger.LogDebug("<<< Exiting CurrentOSUser()");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Populate information about windows user
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void PopulateWindowsUser()
    {
		logger.LogDebug(">>> Entering PopulateWindowsUser()");
        Groups = WindowsIdentity.GetCurrent().Groups?.Select(p => p.Value).ToList();
        UserId = Name;
		logger.LogDebug("<<< Exiting PopulateWindowsUser()");
		logger.LogDebug("PopulateWindowsUser(): Groups → {Groups}", Groups);
		logger.LogDebug("PopulateWindowsUser(): UserId → {UserId}", UserId);
    }

    /// <summary>
    /// Populate information about linux user
    /// </summary>
    public static void PopulateLinuxUser()
    {
		logger.LogDebug(">>> Entering PopulateLinuxUser()");
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

		logger.LogDebug("PopulateLinuxUser(): res → {Res}", res);
        var respars = res.Split("\n");

		logger.LogDebug("PopulateLinuxUser(): respars → {Respars}", respars);
		logger.LogDebug("PopulateLinuxUser(): Groups → {Groups}", Groups);
        UserId = respars[0];
		logger.LogDebug("PopulateLinuxUser(): UserId → {UserId}", UserId);
        Groups = respars[1].Split(" ").ToList();
		logger.LogDebug("<<< Exiting PopulateLinuxUser()");
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