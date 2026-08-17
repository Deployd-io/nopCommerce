using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Security;

/// <summary>
/// File permission helper
/// </summary>
public static class FilePermissionHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    private static bool CheckUserFilePermissions(int userFilePermission, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
    {
		_logger.LogDebug(">>> Entering CheckUserFilePermissions(userFilePermission={UserFilePermission},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", userFilePermission, checkRead, checkWrite, checkModify, checkDelete);
        //read permissions
        var readPermissions = new[] { 5, 6, 7 };

        //write permissions
        var writePermissions = new[] { 2, 3, 6, 7 };

        if (checkRead && readPermissions.Contains(userFilePermission))
		_logger.LogDebug("<<< Exiting CheckUserFilePermissions(userFilePermission={UserFilePermission},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", userFilePermission, checkRead, checkWrite, checkModify, checkDelete);
            return true;

        return (checkWrite || checkModify || checkDelete) && writePermissions.Contains(userFilePermission);
    }

    [SupportedOSPlatform("windows")]
    private static void CheckAccessRule(FileSystemAccessRule rule,
        ref bool deleteIsDeny,
        ref bool modifyIsDeny,
        ref bool readIsDeny,
        ref bool writeIsDeny,
        ref bool deleteIsAllow,
        ref bool modifyIsAllow,
        ref bool readIsAllow,
        ref bool writeIsAllow)
    {
        switch (rule.AccessControlType)
        {
            case AccessControlType.Deny:
                if (CheckAccessRuleLocal(rule, FileSystemRights.Delete))
                    deleteIsDeny = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Modify))
                    modifyIsDeny = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Read))
                    readIsDeny = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Write))
                    writeIsDeny = true;

                return;
            case AccessControlType.Allow:
                if (CheckAccessRuleLocal(rule, FileSystemRights.Delete))
                    deleteIsAllow = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Modify))
                    modifyIsAllow = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Read))
                    readIsAllow = true;

                if (CheckAccessRuleLocal(rule, FileSystemRights.Write))
                    writeIsAllow = true;
                break;
        }
    }

    [SupportedOSPlatform("windows")]
    private static bool CheckAccessRuleLocal(FileSystemAccessRule fileSystemAccessRule, FileSystemRights fileSystemRights)
    {
		_logger.LogDebug(">>> Entering CheckAccessRuleLocal(fileSystemAccessRule={FileSystemAccessRule},fileSystemRights={FileSystemRights})", fileSystemAccessRule, fileSystemRights);
		_logger.LogDebug("<<< Exiting CheckAccessRuleLocal(fileSystemAccessRule={FileSystemAccessRule},fileSystemRights={FileSystemRights})", fileSystemAccessRule, fileSystemRights);
        return (fileSystemRights & fileSystemAccessRule.FileSystemRights) == fileSystemRights;
    }

    /// <summary>
    /// Check permissions
    /// </summary>
    /// <param name="fileProvider">File provider</param>
    /// <param name="path">Path</param>
    /// <param name="checkRead">Check read</param>
    /// <param name="checkWrite">Check write</param>
    /// <param name="checkModify">Check modify</param>
    /// <param name="checkDelete">Check delete</param>
    /// <returns>Result</returns>
    [SupportedOSPlatform("windows")]
    private static bool CheckPermissionsInWindows(INopFileProvider fileProvider, string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
    {
		_logger.LogDebug(">>> Entering CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete);
        var permissionsAreGranted = true;

		_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): permissionsAreGranted → {PermissionsAreGranted}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, permissionsAreGranted);
        try
        {
            var current = WindowsIdentity.GetCurrent();
			_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): current → {Current}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, current);

            var readIsDeny = false;
            var writeIsDeny = false;
            var modifyIsDeny = false;
            var deleteIsDeny = false;

            var readIsAllow = false;
			_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): readIsAllow → {ReadIsAllow}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, readIsAllow);
            var writeIsAllow = false;
			_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): writeIsAllow → {WriteIsAllow}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, writeIsAllow);
            var modifyIsAllow = false;
			_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): modifyIsAllow → {ModifyIsAllow}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, modifyIsAllow);
            var deleteIsAllow = false;
			_logger.LogDebug("CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): deleteIsAllow → {DeleteIsAllow}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, deleteIsAllow);

            var rules = fileProvider.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier))
                .Cast<FileSystemAccessRule>()
                .ToList();

            foreach (var rule in rules.Where(rule => current.User?.Equals(rule.IdentityReference) ?? false))
            {
                CheckAccessRule(rule, ref deleteIsDeny, ref modifyIsDeny, ref readIsDeny, ref writeIsDeny, ref deleteIsAllow, ref modifyIsAllow, ref readIsAllow, ref writeIsAllow);
            }

            if (current.Groups != null)
            {
                foreach (var reference in current.Groups)
                {
                    foreach (var rule in rules.Where(rule => reference.Equals(rule.IdentityReference)))
                    {
                        CheckAccessRule(rule, ref deleteIsDeny, ref modifyIsDeny, ref readIsDeny, ref writeIsDeny, ref deleteIsAllow, ref modifyIsAllow, ref readIsAllow, ref writeIsAllow);
                    }
                }
            }

            deleteIsAllow = !deleteIsDeny && deleteIsAllow;
            modifyIsAllow = !modifyIsDeny && modifyIsAllow;
            readIsAllow = !readIsDeny && readIsAllow;
            writeIsAllow = !writeIsDeny && writeIsAllow;

            if (checkRead)
                permissionsAreGranted = readIsAllow;

            if (checkWrite)
                permissionsAreGranted = permissionsAreGranted && writeIsAllow;

            if (checkModify)
                permissionsAreGranted = permissionsAreGranted && modifyIsAllow;

            if (checkDelete)
                permissionsAreGranted = permissionsAreGranted && deleteIsAllow;
        }
        catch (IOException)
        {
			_logger.LogError("Exception in CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): {Arg6}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, IOException.Message);
            return false;
        }
        catch
        {
		_logger.LogDebug("<<< Exiting CheckPermissionsInWindows(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete);
            return true;
        }

        return permissionsAreGranted;
    }

    /// <summary>
    /// Check permissions
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="checkRead">Check read</param>
    /// <param name="checkWrite">Check write</param>
    /// <param name="checkModify">Check modify</param>
    /// <param name="checkDelete">Check delete</param>
    /// <returns>Result</returns>
    private static bool CheckPermissionsInUnix(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
    {
		_logger.LogDebug(">>> Entering CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", path, checkRead, checkWrite, checkModify, checkDelete);
        //MacOSX file permission check differs slightly from linux
        IList<string> arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? ["-f", "%A %u %g", path]
            : ["-c", "%a %u %g", path ];

        try
        {
            //create bash command like: stat -c %a %u %g <file>
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("stat", arguments)
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            process.WaitForExit();

            //result look like: 555 1111 2222
            //where 555 - file permissions, 1111 - file owner ID, 2222 - file group ID
            var result = process.StandardOutput.ReadToEnd().Trim('\n').Split(' ');
			_logger.LogDebug("CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): result → {Result}", path, checkRead, checkWrite, checkModify, checkDelete, result);

            var filePermissions = result[0].Select(p => (int)char.GetNumericValue(p)).ToList();
			_logger.LogDebug("CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): filePermissions → {FilePermissions}", path, checkRead, checkWrite, checkModify, checkDelete, filePermissions);
            var isOwner = CurrentOSUser.UserId == result[1];
            var isInGroup = CurrentOSUser.Groups.Contains(result[2]);
			_logger.LogDebug("CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): isInGroup → {IsInGroup}", path, checkRead, checkWrite, checkModify, checkDelete, isInGroup);

            var filePermission =
                isOwner ? filePermissions[0] : (isInGroup ? filePermissions[1] : filePermissions[2]);

            return CheckUserFilePermissions(filePermission, checkRead, checkWrite, checkModify, checkDelete);
        }
        catch
        {
			_logger.LogError("Exception in CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): {Arg5}", path, checkRead, checkWrite, checkModify, checkDelete, IOException.Message);
			_logger.LogDebug("<<< Exiting CheckPermissionsInUnix(path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", path, checkRead, checkWrite, checkModify, checkDelete);
            return false;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check permissions
    /// </summary>
    /// <param name="fileProvider">File provider</param>
    /// <param name="path">Path</param>
    /// <param name="checkRead">Check read</param>
    /// <param name="checkWrite">Check write</param>
    /// <param name="checkModify">Check modify</param>
    /// <param name="checkDelete">Check delete</param>
    /// <returns>Result</returns>
    public static bool CheckPermissions(this INopFileProvider fileProvider, string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
    {
		_logger.LogDebug(">>> Entering CheckPermissions(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete);
        if (!(fileProvider.FileExists(path) || fileProvider.DirectoryExists(path)))
            return true;

        var result = false;
		_logger.LogDebug("CheckPermissions(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete}): result → {Result}", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete, result);

        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                if (OperatingSystem.IsWindows())
                    result = CheckPermissionsInWindows(fileProvider, path, checkRead, checkWrite, checkModify, checkDelete);
                break;
            case PlatformID.Unix:
                result = CheckPermissionsInUnix(path, checkRead, checkWrite, checkModify, checkDelete);
                break;
        }

		_logger.LogDebug("<<< Exiting CheckPermissions(fileProvider={FileProvider},path={Path},checkRead={CheckRead},checkWrite={CheckWrite},checkModify={CheckModify},checkDelete={CheckDelete})", fileProvider, path, checkRead, checkWrite, checkModify, checkDelete);
        return result;
    }

    /// <summary>
    /// Gets a list of directories (physical paths) which require write permission
    /// </summary>
    /// <returns>Result</returns>
    public static IEnumerable<string> GetDirectoriesWrite(this INopFileProvider fileProvider)
    {
		_logger.LogDebug(">>> Entering GetDirectoriesWrite(fileProvider={FileProvider})", fileProvider);
        var rootDir = fileProvider.MapPath("~/");

		_logger.LogDebug("GetDirectoriesWrite(fileProvider={FileProvider}): rootDir → {RootDir}", fileProvider, rootDir);
        var dirsToCheck = new List<string>
        {
            fileProvider.Combine(rootDir, "App_Data"),
            fileProvider.Combine(rootDir, "bin"),
            fileProvider.Combine(rootDir, "logs"),
            fileProvider.Combine(rootDir, "Plugins"),
            fileProvider.Combine(rootDir, @"Plugins\Uploaded"),
            fileProvider.Combine(rootDir, @"wwwroot\.well-known"),
            fileProvider.Combine(rootDir, @"wwwroot\bundles"),
            fileProvider.Combine(rootDir, @"wwwroot\db_backups"),
            fileProvider.Combine(rootDir, @"wwwroot\files"),
            fileProvider.Combine(rootDir, @"wwwroot\files\exportimport"),
            fileProvider.Combine(rootDir, @"wwwroot\icons"),
            fileProvider.Combine(rootDir, @"wwwroot\images"),
            fileProvider.Combine(rootDir, @"wwwroot\images\thumbs"),
            fileProvider.Combine(rootDir, @"wwwroot\images\uploaded"),
            fileProvider.Combine(rootDir, @"wwwroot\sitemaps")
        };

		_logger.LogDebug("<<< Exiting GetDirectoriesWrite(fileProvider={FileProvider})", fileProvider);
        return dirsToCheck;
    }

    /// <summary>
    /// Gets a list of files (physical paths) which require write permission
    /// </summary>
    /// <returns>Result</returns>
    public static IEnumerable<string> GetFilesWrite(this INopFileProvider fileProvider)
    {
		_logger.LogDebug(">>> Entering GetFilesWrite(fileProvider={FileProvider})", fileProvider);
		_logger.LogDebug("<<< Exiting GetFilesWrite(fileProvider={FileProvider})", fileProvider);
        return new List<string>
        {
            fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath),
            fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath)
        };
    }

    #endregion
}