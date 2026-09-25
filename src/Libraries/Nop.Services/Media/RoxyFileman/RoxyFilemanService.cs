using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Media.RoxyFileman;

public partial class RoxyFilemanService : IRoxyFilemanService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRoxyFilemanFileProvider _fileProvider;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public RoxyFilemanService(
        IRoxyFilemanFileProvider fileProvider,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering RoxyFilemanService(fileProvider={FileProvider},workContext={WorkContext})", fileProvider, workContext);
        _fileProvider = fileProvider;
        _workContext = workContext;
		_logger.LogDebug("RoxyFilemanService(fileProvider={FileProvider},workContext={WorkContext}): _workContext → {_workContext}", fileProvider, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether there are any restrictions on handling the file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <returns>
    /// The result contains true if the file can be handled; otherwise false
    /// </returns>
    protected virtual bool CanHandleFile(string path)
    {
		_logger.LogError(">>> Entering CanHandleFile(path={Path})", path);
        var result = false;

		_logger.LogError("CanHandleFile(path={Path}): result → {Result}", path, result);
        var fileExtension = Path.GetExtension(path).Replace(".", string.Empty).ToLowerInvariant();
		_logger.LogError("CanHandleFile(path={Path}): fileExtension → {FileExtension}", path, fileExtension);

        var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

        var forbiddenUploads = roxyConfig.FORBIDDEN_UPLOADS.Trim().ToLowerInvariant();
		_logger.LogError("CanHandleFile(path={Path}): forbiddenUploads → {ForbiddenUploads}", path, forbiddenUploads);

        if (!string.IsNullOrEmpty(forbiddenUploads))
            result = !WhiteSpaceRegex().Split(forbiddenUploads).Contains(fileExtension);

        var allowedUploads = roxyConfig.ALLOWED_UPLOADS.Trim().ToLowerInvariant();
		_logger.LogError("CanHandleFile(path={Path}): allowedUploads → {AllowedUploads}", path, allowedUploads);
		_logger.LogWarning("<<< Exiting CanHandleFile(path={Path})", path);
        if (string.IsNullOrEmpty(allowedUploads)) {
            return result;
        }

        return WhiteSpaceRegex().Split(allowedUploads).Contains(fileExtension);
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex WhiteSpaceRegex();

    #endregion

    #region Configuration

    /// <summary>
    /// Initial service configuration
    /// </summary>
    /// <param name="pathBase">The base path for the current request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ConfigureAsync(string pathBase)
    {
		_logger.LogError(">>> Entering ConfigureAsync(pathBase={PathBase})", pathBase);
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogWarning("<<< Exiting ConfigureAsync(pathBase={PathBase})", pathBase);
		_logger.LogError("ConfigureAsync(pathBase={PathBase}): currentLanguage → {CurrentLanguage}", pathBase, currentLanguage);
        await _fileProvider.GetOrCreateConfigurationAsync(pathBase, currentLanguage.UniqueSeoCode);
    }

    #endregion

    #region Directories

    /// <summary>
    /// Delete the directory
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual void DeleteDirectory(string path)
    {
		_logger.LogError(">>> Entering DeleteDirectory(path={Path})", path);
        _fileProvider.DeleteDirectory(path);
		_logger.LogWarning("<<< Exiting DeleteDirectory(path={Path})", path);
    }

    /// <summary>
    /// Download the directory from the server as a zip archive
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual byte[] DownloadDirectory(string path)
    {
		_logger.LogError(">>> Entering DownloadDirectory(path={Path})", path);
        _logger.LogWarning("<<< Exiting DownloadDirectory(path={Path})", path);
        return _fileProvider.CreateZipArchiveFromDirectory(path);
    }

    /// <summary>
    /// Copy the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="destinationPath">Path to the destination directory</param>
    public virtual void CopyDirectory(string sourcePath, string destinationPath)
    {
		_logger.LogError(">>> Entering CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
        _fileProvider.CopyDirectory(sourcePath, destinationPath);
		_logger.LogWarning("<<< Exiting CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
    }

    /// <summary>
    /// Create the new directory
    /// </summary>
    /// <param name="parentDirectoryPath">Path to the parent directory</param>
    /// <param name="name">Name of the new directory</param>
    public virtual void CreateDirectory(string parentDirectoryPath, string name)
    {
		_logger.LogError(">>> Entering CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name})", parentDirectoryPath, name);
        _fileProvider.CreateDirectory(parentDirectoryPath, name);
		_logger.LogWarning("<<< Exiting CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name})", parentDirectoryPath, name);
    }

    /// <summary>
    /// Get all available directories as a directory tree
    /// </summary>
    /// <param name="type">Type of the file</param>
    /// <returns>List of directories</returns>
    public virtual IEnumerable<object> GetDirectoryList(string type)
    {
		_logger.LogError(">>> Entering GetDirectoryList(type={Type})", type);
        var contents = _fileProvider.GetDirectories(type);

		_logger.LogError("GetDirectoryList(type={Type}): contents → {Contents}", type, contents);
        var result = new List<object>() { new
        {
            p = "/",
            f = _fileProvider.GetFiles("/", type).Count(),
            d = 0
        } };

        foreach (var (path, countFiles, countDirectories) in contents)
        {
            result.Add(new
            {
                p = path.Replace("\\", "/"),
                f = countFiles,
                d = countDirectories
            });
        }

		_logger.LogWarning("<<< Exiting GetDirectoryList(type={Type})", type);
        return result;
    }

    /// <summary>
    /// Move the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="destinationPath">Path to the destination directory</param>
    public virtual void MoveDirectory(string sourcePath, string destinationPath)
    {
		_logger.LogError(">>> Entering MoveDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
			_logger.LogWarning("<<< Exiting MoveDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
			_logger.LogError("Throwing RoxyFilemanException from MoveDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
        if (destinationPath.IndexOf(sourcePath, StringComparison.InvariantCulture) == 0) {
            throw new RoxyFilemanException("E_CannotMoveDirToChild");
        }

        _fileProvider.DirectoryMove(sourcePath, destinationPath);
    }

    /// <summary>
    /// Get files in the passed directory
    /// </summary>
    /// <param name="directoryPath">Path to the files directory</param>
    /// <param name="type">Type of the files</param>
    public virtual IEnumerable<object> GetFiles(string directoryPath, string type)
    {
		_logger.LogError(">>> Entering GetFiles(directoryPath={DirectoryPath},type={Type})", directoryPath, type);
		_logger.LogWarning("<<< Exiting GetFiles(directoryPath={DirectoryPath},type={Type})", directoryPath, type);
        return _fileProvider.GetFiles(directoryPath, type?.Trim('#'))
            .Select(f => new
            {
                p = f.RelativePath.Replace("\\", "/"),
                t = f.LastWriteTime.ToUnixTimeSeconds(),
                s = f.FileLength,
                w = f.Width,
                h = f.Height
            });
    }

    /// <summary>
    /// Rename the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="newName">New name of the directory</param>
    public virtual void RenameDirectory(string sourcePath, string newName)
    {
		_logger.LogError(">>> Entering RenameDirectory(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
        _fileProvider.RenameDirectory(sourcePath, newName);
		_logger.LogWarning("<<< Exiting RenameDirectory(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
    }

    /// <summary>
    /// Upload files to a directory on passed path
    /// </summary>
    /// <param name="directoryPath">Path to directory to upload files</param>
    /// <param name="files">Files sent with the HttpRequest</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UploadFilesAsync(string directoryPath, IEnumerable<IFormFile> files)
    {
		_logger.LogError(">>> Entering UploadFilesAsync(directoryPath={DirectoryPath},files={Files})", directoryPath, files);
        foreach (var formFile in files)
        {
            await _fileProvider.SaveFileAsync(directoryPath, formFile.FileName, formFile.ContentType, formFile.OpenReadStream());
        }
		_logger.LogWarning("<<< Exiting UploadFilesAsync(directoryPath={DirectoryPath},files={Files})", directoryPath, files);
    }

    #endregion

    #region Files

    /// <summary>
    /// Copy the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="destinationPath">Path to the destination file</param>
    public virtual void CopyFile(string sourcePath, string destinationPath)
    {
		_logger.LogError(">>> Entering CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
        _fileProvider.CopyFile(sourcePath, destinationPath);
		_logger.LogWarning("<<< Exiting CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
    }

    /// <summary>
    /// Get binary image thumbnail data
    /// </summary>
    /// <param name="path">Path to the image</param>
    /// <param name="contentType">The resulting MIME type</param>
    public virtual byte[] CreateImageThumbnail(string path, string contentType)
    {
		_logger.LogError(">>> Entering CreateImageThumbnail(path={Path},contentType={ContentType})", path, contentType);
        _logger.LogWarning("<<< Exiting CreateImageThumbnail(path={Path},contentType={ContentType})", path, contentType);
        return _fileProvider.CreateImageThumbnail(path, contentType);
    }

    /// <summary>
    /// Delete the directory
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual void DeleteFile(string path)
    {
		_logger.LogError(">>> Entering DeleteFile(path={Path})", path);
        _fileProvider.DeleteFile(path);
		_logger.LogWarning("<<< Exiting DeleteFile(path={Path})", path);
    }

    /// <summary>
    /// Get filename and read-only content stream
    /// </summary>
    /// <param name="path">Path to the file</param>
    public virtual (Stream stream, string name) GetFileStream(string path)
    {
		_logger.LogError(">>> Entering GetFileStream(path={Path})", path);
        var file = _fileProvider.GetFileInfo(path);

		_logger.LogError("GetFileStream(path={Path}): file → {File}", path, file);
			_logger.LogError("Throwing FileNotFoundException from GetFileStream(path={Path})", path);
		_logger.LogWarning("<<< Exiting GetFileStream(path={Path})", path);
        if (!file.Exists) {
            throw new FileNotFoundException();
        }

        return (file.CreateReadStream(), file.Name);
    }

    /// <summary>
    /// Move the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="destinationPath">Path to the destination file</param>
    public virtual void MoveFile(string sourcePath, string destinationPath)
    {
		_logger.LogError(">>> Entering MoveFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
			_logger.LogWarning("<<< Exiting MoveFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
			_logger.LogError("Throwing RoxyFilemanException from MoveFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
        if (!CanHandleFile(sourcePath) && !CanHandleFile(destinationPath)) {
            throw new RoxyFilemanException("E_FileExtensionForbidden");
        }

        _fileProvider.FileMove(sourcePath, destinationPath);
    }

    /// <summary>
    /// Rename the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="newName">New name of the file</param>
    public virtual void RenameFile(string sourcePath, string newName)
    {
		_logger.LogError(">>> Entering RenameFile(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
			_logger.LogWarning("<<< Exiting RenameFile(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
			_logger.LogError("Throwing RoxyFilemanException from RenameFile(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
        if (!CanHandleFile(sourcePath) && !CanHandleFile(newName)) {
            throw new RoxyFilemanException("E_FileExtensionForbidden");
        }

        _fileProvider.RenameFile(sourcePath, newName);
    }        

    #endregion
}