using Nop.Core;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
///  Maintenance service
/// </summary>
public partial class MaintenanceService : IMaintenanceService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public MaintenanceService(INopFileProvider fileProvider)
    {
		_logger.LogDebug(">>> Entering MaintenanceService(fileProvider={FileProvider})", fileProvider);
        _fileProvider = fileProvider;
		_logger.LogDebug("MaintenanceService(fileProvider={FileProvider}): _fileProvider → {_fileProvider}", fileProvider, _fileProvider);
		_logger.LogDebug("<<< Exiting MaintenanceService(fileProvider={FileProvider})", fileProvider);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get directory path for backs
    /// </summary>
    /// <param name="ensureFolderCreated">A value indicating whether a directory should be created if it doesn't exist</param>
    /// <returns></returns>
    protected virtual string GetBackupDirectoryPath(bool ensureFolderCreated = true)
    {
		_logger.LogDebug(">>> Entering GetBackupDirectoryPath(ensureFolderCreated={EnsureFolderCreated})", ensureFolderCreated);
        var path = _fileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath);
        if (ensureFolderCreated)
            _fileProvider.CreateDirectory(path);
		_logger.LogDebug("<<< Exiting GetBackupDirectoryPath(ensureFolderCreated={EnsureFolderCreated})", ensureFolderCreated);
        return path;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all backup files
    /// </summary>
    /// <returns>Backup file collection</returns>
    public virtual IList<string> GetAllBackupFiles()
    {
		_logger.LogDebug(">>> Entering GetAllBackupFiles()");
        var path = GetBackupDirectoryPath();

		_logger.LogDebug("GetAllBackupFiles(): path → {Path}", path);
        if (!_fileProvider.DirectoryExists(path))
			_logger.LogWarning("Throwing NopException from GetAllBackupFiles()");
		_logger.LogDebug("<<< Exiting GetAllBackupFiles()");
            throw new NopException("Backup directory not exists");

        return _fileProvider.GetFiles(path, $"*.{NopCommonDefaults.DbBackupFileExtension}")
            .OrderByDescending(p => _fileProvider.GetLastWriteTime(p)).ToList();
    }

    /// <summary>
    /// Returns the path to the backup file
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>The path to the backup file</returns>
    public virtual string GetBackupPath(string backupFileName)
    {
		_logger.LogDebug(">>> Entering GetBackupPath(backupFileName={BackupFileName})", backupFileName);
		_logger.LogDebug("<<< Exiting GetBackupPath(backupFileName={BackupFileName})", backupFileName);
        return _fileProvider.Combine(GetBackupDirectoryPath(), backupFileName);
    }

    /// <summary>
    /// Creates a path to a new database backup file
    /// </summary>
    /// <returns>Path to a new database backup file</returns>
    public virtual string CreateNewBackupFilePath()
    {
		_logger.LogDebug(">>> Entering CreateNewBackupFilePath()");
		_logger.LogDebug("<<< Exiting CreateNewBackupFilePath()");
        return _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");
    }

    #endregion
}