using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Infrastructure;

/// <summary>
/// IO functions using the on-disk file system
/// </summary>
public partial class NopFileProvider : PhysicalFileProvider, INopFileProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of a NopFileProvider
    /// </summary>
    /// <param name="webHostEnvironment">Hosting environment</param>
    public NopFileProvider(IWebHostEnvironment webHostEnvironment)
        : base(File.Exists(webHostEnvironment.ContentRootPath) ? Path.GetDirectoryName(webHostEnvironment.ContentRootPath)! : webHostEnvironment.ContentRootPath)
    {
		_logger.LogError(">>> Entering NopFileProvider(webHostEnvironment={WebHostEnvironment})", webHostEnvironment);
        WebRootPath = File.Exists(webHostEnvironment.WebRootPath)
            ? Path.GetDirectoryName(webHostEnvironment.WebRootPath)
            : webHostEnvironment.WebRootPath;
			_logger.LogDebug("NopFileProvider(webHostEnvironment={WebHostEnvironment}): WebRootPath → {WebRootPath}", webHostEnvironment, WebRootPath);
    _logger.LogWarning("<<< Exiting NopFileProvider(webHostEnvironment={WebHostEnvironment})", webHostEnvironment);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Depth-first recursive delete
    /// </summary>
    /// <param name="path"></param>
    protected virtual void DeleteDirectoryRecursive(string path)
    {
		_logger.LogError(">>> Entering DeleteDirectoryRecursive(path={Path})", path);
        Directory.Delete(path, true);
        const int maxIterationToWait = 10;
        var curIteration = 0;

        //according to the documentation(https://msdn.microsoft.com/ru-ru/library/windows/desktop/aa365488.aspx) 
        //System.IO.Directory.Delete method ultimately (after removing the files) calls native 
        //RemoveDirectory function which marks the directory as "deleted". That's why we wait until 
        //the directory is actually deleted. For more details see https://stackoverflow.com/a/4245121
        while (Directory.Exists(path))
        {
            curIteration += 1;

            if (curIteration > maxIterationToWait)
                return;

            Thread.Sleep(100);
        }
		_logger.LogWarning("<<< Exiting DeleteDirectoryRecursive(path={Path})", path);
    }

    /// <summary>
    /// Determines if the string is a valid Universal Naming Convention (UNC)
    /// for a server and share path.
    /// </summary>
    /// <param name="path">The path to be tested.</param>
    /// <returns><see langword="true"/> if the path is a valid UNC path; 
    /// otherwise, <see langword="false"/>.</returns>
    protected static bool IsUncPath(string path)
    {
		_logger.LogError(">>> Entering IsUncPath(path={Path})", path);
        _logger.LogWarning("<<< Exiting IsUncPath(path={Path})", path);
        return Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsUnc;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Combines an array of strings into a path
    /// </summary>
    /// <param name="paths">An array of parts of the path</param>
    /// <returns>The combined paths</returns>
    public virtual string Combine(params string[] paths)
    {
		_logger.LogError(">>> Entering Combine(paths={Paths})", paths);
        var path = Path.Combine(paths.SelectMany(p => IsUncPath(p) ? [p] : p.Split('\\', '/')).ToArray());

		_logger.LogError("Combine(paths={Paths}): path → {Path}", paths, path);
        if (Environment.OSVersion.Platform == PlatformID.Unix && !IsUncPath(path))
            //add leading slash to correctly form path in the UNIX system
            path = "/" + path;

		_logger.LogWarning("<<< Exiting Combine(paths={Paths})", paths);
        return path;
    }

    /// <summary>
    /// Creates all directories and subdirectories in the specified path unless they already exist
    /// </summary>
    /// <param name="path">The directory to create</param>
    public virtual void CreateDirectory(string path)
    {
		_logger.LogError(">>> Entering CreateDirectory(path={Path})", path);
        if (!DirectoryExists(path))
            Directory.CreateDirectory(path);
			_logger.LogWarning("<<< Exiting CreateDirectory(path={Path})", path);
    }

    /// <summary>
    /// Creates a file in the specified path
    /// </summary>
    /// <param name="path">The path and name of the file to create</param>
    public virtual void CreateFile(string path)
    {
		_logger.LogError(">>> Entering CreateFile(path={Path})", path);
        if (FileExists(path))
            return;

        var fileInfo = new FileInfo(path);
		_logger.LogError("CreateFile(path={Path}): fileInfo → {FileInfo}", path, fileInfo);
        CreateDirectory(fileInfo.DirectoryName);

        //we use 'using' to close the file after it's created
        using (File.Create(path))
        {
        }
		_logger.LogWarning("<<< Exiting CreateFile(path={Path})", path);
    }

    /// <summary>
    ///  Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
    /// </summary>
    /// <param name="path">Directory path</param>
    public virtual void DeleteDirectory(string path)
    {
		_logger.LogError(">>> Entering DeleteDirectory(path={Path})", path);
        ArgumentException.ThrowIfNullOrEmpty(path);

        //find more info about directory deletion
        //and why we use this approach at https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true

        foreach (var directory in Directory.GetDirectories(path))
        {
            DeleteDirectory(directory);
        }

        try
        {
            DeleteDirectoryRecursive(path);
        }
        catch (IOException)
        {
			_logger.LogError("Exception in DeleteDirectory(path={Path}): {Arg1}", path, IOException.Message);
            DeleteDirectoryRecursive(path);
        }
        catch (UnauthorizedAccessException)
        {
			_logger.LogError("Exception in DeleteDirectory(path={Path}): {Arg1}", path, UnauthorizedAccessException.Message);
            DeleteDirectoryRecursive(path);
        }
		_logger.LogWarning("<<< Exiting DeleteDirectory(path={Path})", path);
    }

    /// <summary>
    /// Deletes the specified file
    /// </summary>
    /// <param name="filePath">The name of the file to be deleted. Wildcard characters are not supported</param>
    public virtual void DeleteFile(string filePath)
    {
		_logger.LogError(">>> Entering DeleteFile(filePath={FilePath})", filePath);
        if (!FileExists(filePath))
            return;

        File.Delete(filePath);
		_logger.LogWarning("<<< Exiting DeleteFile(filePath={FilePath})", filePath);
    }

    /// <summary>
    /// Determines whether the given path refers to an existing directory on disk
    /// </summary>
    /// <param name="path">The path to test</param>
    /// <returns>
    /// true if path refers to an existing directory; false if the directory does not exist or an error occurs when
    /// trying to determine if the specified file exists
    /// </returns>
    public virtual bool DirectoryExists(string path)
    {
		_logger.LogError(">>> Entering DirectoryExists(path={Path})", path);
        _logger.LogWarning("<<< Exiting DirectoryExists(path={Path})", path);
        return Directory.Exists(path);
    }

    /// <summary>
    /// Moves a file or a directory and its contents to a new location
    /// </summary>
    /// <param name="sourceDirName">The path of the file or directory to move</param>
    /// <param name="destDirName">
    /// The path to the new location for sourceDirName. If sourceDirName is a file, then destDirName
    /// must also be a file name
    /// </param>
    public virtual void DirectoryMove(string sourceDirName, string destDirName)
    {
		_logger.LogError(">>> Entering DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName})", sourceDirName, destDirName);
        Directory.Move(sourceDirName, destDirName);
		_logger.LogWarning("<<< Exiting DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName})", sourceDirName, destDirName);
    }

    /// <summary>
    /// Returns an enumerable collection of file names that match a search pattern in
    /// a specified path, and optionally searches subdirectories.
    /// </summary>
    /// <param name="directoryPath">The path to the directory to search</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files in path. This parameter
    /// can contain a combination of valid literal path and wildcard (* and ?) characters
    /// , but doesn't support regular expressions.
    /// </param>
    /// <param name="topDirectoryOnly">
    /// Specifies whether to search the current directory, or the current directory and all
    /// subdirectories
    /// </param>
    /// <returns>
    /// An enumerable collection of the full names (including paths) for the files in
    /// the directory specified by path and that match the specified search pattern
    /// </returns>
    public virtual IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern,
        bool topDirectoryOnly = true)
    {
		_logger.LogError(">>> Entering EnumerateFiles(directoryPath={DirectoryPath},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", directoryPath, searchPattern, topDirectoryOnly);
        _logger.LogWarning("<<< Exiting EnumerateFiles(directoryPath={DirectoryPath},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", directoryPath, searchPattern, topDirectoryOnly);
        return Directory.EnumerateFiles(directoryPath, searchPattern,
            topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
    }

    /// <summary>
    /// Copies an existing file to a new file. Overwriting a file of the same name is allowed
    /// </summary>
    /// <param name="sourceFileName">The file to copy</param>
    /// <param name="destFileName">The name of the destination file. This cannot be a directory</param>
    /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false</param>
    public virtual void FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
    {
		_logger.LogError(">>> Entering FileCopy(sourceFileName={SourceFileName},destFileName={DestFileName},overwrite={Overwrite})", sourceFileName, destFileName, overwrite);
        File.Copy(sourceFileName, destFileName, overwrite);
		_logger.LogWarning("<<< Exiting FileCopy(sourceFileName={SourceFileName},destFileName={DestFileName},overwrite={Overwrite})", sourceFileName, destFileName, overwrite);
    }

    /// <summary>
    /// Determines whether the specified file exists
    /// </summary>
    /// <param name="filePath">The file to check</param>
    /// <returns>
    /// True if the caller has the required permissions and path contains the name of an existing file; otherwise,
    /// false.
    /// </returns>
    public virtual bool FileExists(string filePath)
    {
		_logger.LogError(">>> Entering FileExists(filePath={FilePath})", filePath);
        _logger.LogWarning("<<< Exiting FileExists(filePath={FilePath})", filePath);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Gets the length of the file in bytes, or -1 for a directory or non-existing files
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>The length of the file</returns>
    public virtual long FileLength(string path)
    {
		_logger.LogError(">>> Entering FileLength(path={Path})", path);
        if (!FileExists(path))
            return -1;

        return new FileInfo(path).Length;
    }

    /// <summary>
    /// Moves a specified file to a new location, providing the option to specify a new file name
    /// </summary>
    /// <param name="sourceFileName">The name of the file to move. Can include a relative or absolute path</param>
    /// <param name="destFileName">The new path and name for the file</param>
    public virtual void FileMove(string sourceFileName, string destFileName)
    {
		_logger.LogError(">>> Entering FileMove(sourceFileName={SourceFileName},destFileName={DestFileName})", sourceFileName, destFileName);
        File.Move(sourceFileName, destFileName);
		_logger.LogWarning("<<< Exiting FileMove(sourceFileName={SourceFileName},destFileName={DestFileName})", sourceFileName, destFileName);
    }

    /// <summary>
    /// Returns the absolute path to the directory
    /// </summary>
    /// <param name="paths">An array of parts of the path</param>
    /// <returns>The absolute path to the directory</returns>
    public virtual string GetAbsolutePath(params string[] paths)
    {
		_logger.LogError(">>> Entering GetAbsolutePath(paths={Paths})", paths);
        var allPaths = new List<string>();

		_logger.LogError("GetAbsolutePath(paths={Paths}): allPaths → {AllPaths}", paths, allPaths);
        if (paths.Any() && !paths[0].Contains(WebRootPath, StringComparison.InvariantCulture))
            allPaths.Add(WebRootPath);

        allPaths.AddRange(paths);

		_logger.LogWarning("<<< Exiting GetAbsolutePath(paths={Paths})", paths);
        return Combine(allPaths.ToArray());
    }

    /// <summary>
    /// Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates the access control list (ACL) entries for a specified directory
    /// </summary>
    /// <param name="path">The path to a directory containing a System.Security.AccessControl.DirectorySecurity object that describes the file's access control list (ACL) information</param>
    /// <returns>An object that encapsulates the access control rules for the file described by the path parameter</returns>
    [SupportedOSPlatform("windows")]
    public virtual DirectorySecurity GetAccessControl(string path)
    {
		_logger.LogError(">>> Entering GetAccessControl(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetAccessControl(path={Path})", path);
        return new DirectoryInfo(path).GetAccessControl();
    }

    /// <summary>
    /// Returns the creation date and time of the specified file or directory
    /// </summary>
    /// <param name="path">The file or directory for which to obtain creation date and time information</param>
    /// <returns>
    /// A System.DateTime structure set to the creation date and time for the specified file or directory. This value
    /// is expressed in local time
    /// </returns>
    public virtual DateTime GetCreationTime(string path)
    {
		_logger.LogError(">>> Entering GetCreationTime(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetCreationTime(path={Path})", path);
        return File.GetCreationTime(path);
    }

    /// <summary>
    /// Returns the names of the subdirectories (including their paths) that match the
    /// specified search pattern in the specified directory
    /// </summary>
    /// <param name="path">The path to the directory to search</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of subdirectories in path. This
    /// parameter can contain a combination of valid literal and wildcard characters
    /// , but doesn't support regular expressions.
    /// </param>
    /// <param name="topDirectoryOnly">
    /// Specifies whether to search the current directory, or the current directory and all
    /// subdirectories
    /// </param>
    /// <returns>
    /// An array of the full names (including paths) of the subdirectories that match
    /// the specified criteria, or an empty array if no directories are found
    /// </returns>
    public virtual string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true)
    {
		_logger.LogError(">>> Entering GetDirectories(path={Path},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", path, searchPattern, topDirectoryOnly);
        if (string.IsNullOrEmpty(searchPattern))
            searchPattern = "*";
			_logger.LogError("GetDirectories(path={Path},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly}): searchPattern → {SearchPattern1}", path, searchPattern, topDirectoryOnly, searchPattern);

			_logger.LogWarning("<<< Exiting GetDirectories(path={Path},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", path, searchPattern, topDirectoryOnly);
        return Directory.GetDirectories(path, searchPattern,
            topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
    }

    /// <summary>
    /// Returns the directory information for the specified path string
    /// </summary>
    /// <param name="path">The path of a file or directory</param>
    /// <returns>
    /// Directory information for path, or null if path denotes a root directory or is null. Returns
    /// System.String.Empty if path does not contain directory information
    /// </returns>
    public virtual string GetDirectoryName(string path)
    {
		_logger.LogError(">>> Entering GetDirectoryName(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetDirectoryName(path={Path})", path);
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Returns the directory name only for the specified path string
    /// </summary>
    /// <param name="path">The path of directory</param>
    /// <returns>The directory name</returns>
    public virtual string GetDirectoryNameOnly(string path)
    {
		_logger.LogError(">>> Entering GetDirectoryNameOnly(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetDirectoryNameOnly(path={Path})", path);
        return new DirectoryInfo(path).Name;
    }

    /// <summary>
    /// Returns the extension of the specified path string
    /// </summary>
    /// <param name="filePath">The path string from which to get the extension</param>
    /// <returns>The extension of the specified path (including the period ".")</returns>
    public virtual string GetFileExtension(string filePath)
    {
		_logger.LogError(">>> Entering GetFileExtension(filePath={FilePath})", filePath);
        _logger.LogWarning("<<< Exiting GetFileExtension(filePath={FilePath})", filePath);
        return Path.GetExtension(filePath);
    }

    /// <summary>
    /// Returns the file name and extension of the specified path string
    /// </summary>
    /// <param name="path">The path string from which to obtain the file name and extension</param>
    /// <returns>The characters after the last directory character in path</returns>
    public virtual string GetFileName(string path)
    {
		_logger.LogError(">>> Entering GetFileName(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetFileName(path={Path})", path);
        return Path.GetFileName(path);
    }

    /// <summary>
    /// Returns the file name of the specified path string without the extension
    /// </summary>
    /// <param name="filePath">The path of the file</param>
    /// <returns>The file name, minus the last period (.) and all characters following it</returns>
    public virtual string GetFileNameWithoutExtension(string filePath)
    {
		_logger.LogError(">>> Entering GetFileNameWithoutExtension(filePath={FilePath})", filePath);
        _logger.LogWarning("<<< Exiting GetFileNameWithoutExtension(filePath={FilePath})", filePath);
        return Path.GetFileNameWithoutExtension(filePath);
    }

    /// <summary>
    /// Returns the names of files (including their paths) that match the specified search
    /// pattern in the specified directory, using a value to determine whether to search subdirectories.
    /// </summary>
    /// <param name="directoryPath">The path to the directory to search</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files in path. This parameter
    /// can contain a combination of valid literal path and wildcard (* and ?) characters
    /// , but doesn't support regular expressions.
    /// </param>
    /// <param name="topDirectoryOnly">
    /// Specifies whether to search the current directory, or the current directory and all
    /// subdirectories
    /// </param>
    /// <returns>
    /// An array of the full names (including paths) for the files in the specified directory
    /// that match the specified search pattern, or an empty array if no files are found.
    /// </returns>
    public virtual string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
    {
		_logger.LogError(">>> Entering GetFiles(directoryPath={DirectoryPath},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", directoryPath, searchPattern, topDirectoryOnly);
        if (string.IsNullOrEmpty(searchPattern))
            searchPattern = "*.*";
			_logger.LogError("GetFiles(directoryPath={DirectoryPath},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly}): searchPattern → {SearchPattern1}", directoryPath, searchPattern, topDirectoryOnly, searchPattern);

		_logger.LogWarning("<<< Exiting GetFiles(directoryPath={DirectoryPath},searchPattern={SearchPattern},topDirectoryOnly={TopDirectoryOnly})", directoryPath, searchPattern, topDirectoryOnly);
        return Directory.GetFileSystemEntries(directoryPath, searchPattern,
            new EnumerationOptions
            {
                IgnoreInaccessible = true,
                MatchCasing = MatchCasing.CaseInsensitive,
                RecurseSubdirectories = !topDirectoryOnly,

            });
    }

    /// <summary>
    /// Returns the date and time the specified file or directory was last accessed
    /// </summary>
    /// <param name="path">The file or directory for which to obtain access date and time information</param>
    /// <returns>A System.DateTime structure set to the date and time that the specified file</returns>
    public virtual DateTime GetLastAccessTime(string path)
    {
		_logger.LogError(">>> Entering GetLastAccessTime(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetLastAccessTime(path={Path})", path);
        return File.GetLastAccessTime(path);
    }

    /// <summary>
    /// Returns the date and time the specified file or directory was last written to
    /// </summary>
    /// <param name="path">The file or directory for which to obtain write date and time information</param>
    /// <returns>
    /// A System.DateTime structure set to the date and time that the specified file or directory was last written to.
    /// This value is expressed in local time
    /// </returns>
    public virtual DateTime GetLastWriteTime(string path)
    {
		_logger.LogError(">>> Entering GetLastWriteTime(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetLastWriteTime(path={Path})", path);
        return File.GetLastWriteTime(path);
    }

    /// <summary>
    /// Returns the date and time, in coordinated universal time (UTC), that the specified file or directory was last
    /// written to
    /// </summary>
    /// <param name="path">The file or directory for which to obtain write date and time information</param>
    /// <returns>
    /// A System.DateTime structure set to the date and time that the specified file or directory was last written to.
    /// This value is expressed in UTC time
    /// </returns>
    public virtual DateTime GetLastWriteTimeUtc(string path)
    {
		_logger.LogError(">>> Entering GetLastWriteTimeUtc(path={Path})", path);
        _logger.LogWarning("<<< Exiting GetLastWriteTimeUtc(path={Path})", path);
        return File.GetLastWriteTimeUtc(path);
    }

    /// <summary>
    /// Creates or opens a file at the specified path for read/write access
    /// </summary>
    /// <param name="path">The path and name of the file to create</param>
    /// <returns>A <see cref="FileStream"/> that provides read/write access to the file specified in <paramref name="path"/></returns>
    public FileStream GetOrCreateFile(string path)
    {
		_logger.LogError(">>> Entering GetOrCreateFile(path={Path})", path);
        if (FileExists(path))
            return File.Open(path, FileMode.Open, FileAccess.ReadWrite);

        var fileInfo = new FileInfo(path);
		_logger.LogError("GetOrCreateFile(path={Path}): fileInfo → {FileInfo}", path, fileInfo);
        CreateDirectory(fileInfo.DirectoryName);

		_logger.LogWarning("<<< Exiting GetOrCreateFile(path={Path})", path);
        return File.Create(path);
    }

    /// <summary>
    /// Retrieves the parent directory of the specified path
    /// </summary>
    /// <param name="directoryPath">The path for which to retrieve the parent directory</param>
    /// <returns>The parent directory, or null if path is the root directory, including the root of a UNC server or share name</returns>
    public virtual string GetParentDirectory(string directoryPath)
    {
		_logger.LogError(">>> Entering GetParentDirectory(directoryPath={DirectoryPath})", directoryPath);
        _logger.LogWarning("<<< Exiting GetParentDirectory(directoryPath={DirectoryPath})", directoryPath);
        return Directory.GetParent(directoryPath).FullName;
    }

    /// <summary>
    /// Gets a virtual path from a physical disk path.
    /// </summary>
    /// <param name="path">The physical disk path</param>
    /// <returns>The virtual path. E.g. "~/bin"</returns>
    public virtual string GetVirtualPath(string path)
    {
		_logger.LogError(">>> Entering GetVirtualPath(path={Path})", path);
        if (string.IsNullOrEmpty(path))
            return path;

        if (!IsDirectory(path) && FileExists(path))
            path = new FileInfo(path).DirectoryName;
			_logger.LogError("GetVirtualPath(path={Path}): path → {Path1}", path, path);

        path = path?.Replace(WebRootPath, string.Empty).Replace('\\', '/').Trim('/').TrimStart('~', '/');

		_logger.LogWarning("<<< Exiting GetVirtualPath(path={Path})", path);
        return $"~/{path ?? string.Empty}";
    }

    /// <summary>
    /// Checks if the path is directory
    /// </summary>
    /// <param name="path">Path for check</param>
    /// <returns>True, if the path is a directory, otherwise false</returns>
    public virtual bool IsDirectory(string path)
    {
		_logger.LogError(">>> Entering IsDirectory(path={Path})", path);
        _logger.LogWarning("<<< Exiting IsDirectory(path={Path})", path);
        return DirectoryExists(path);
    }

    /// <summary>
    /// Maps a virtual path to a physical disk path.
    /// </summary>
    /// <param name="path">The path to map. E.g. "~/bin"</param>
    /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
    public virtual string MapPath(string path)
    {
		_logger.LogError(">>> Entering MapPath(path={Path})", path);
        path = path.Replace("~/", string.Empty).TrimStart('/');

		_logger.LogError("MapPath(path={Path}): path → {Path1}", path, path);
        //if virtual path has slash on the end, it should be after transform the virtual path to physical path too
        var pathEnd = path.EndsWith('/') ? Path.DirectorySeparatorChar.ToString() : string.Empty;
		_logger.LogError("MapPath(path={Path}): pathEnd → {PathEnd}", path, pathEnd);

		_logger.LogWarning("<<< Exiting MapPath(path={Path})", path);
        return Combine(Root ?? string.Empty, path) + pathEnd;
    }

    /// <summary>
    /// Reads the contents of the file into a byte array
    /// </summary>
    /// <param name="filePath">The file for reading</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a byte array containing the contents of the file
    /// </returns>
    public virtual async Task<byte[]> ReadAllBytesAsync(string filePath)
    {
		_logger.LogError(">>> Entering ReadAllBytesAsync(filePath={FilePath})", filePath);
        _logger.LogWarning("<<< Exiting ReadAllBytesAsync(filePath={FilePath})", filePath);
        return File.Exists(filePath) ? await File.ReadAllBytesAsync(filePath) : Array.Empty<byte>();
    }

    /// <summary>
    /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
    /// </summary>
    /// <param name="path">The file to open for reading</param>
    /// <param name="encoding">The encoding applied to the contents of the file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a string containing all lines of the file
    /// </returns>
    public virtual async Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
		_logger.LogError(">>> Entering ReadAllTextAsync(path={Path},encoding={Encoding})", path, encoding);
        await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream, encoding);
		_logger.LogError("ReadAllTextAsync(path={Path},encoding={Encoding}): streamReader → {StreamReader}", path, encoding, streamReader);
		_logger.LogError("ReadAllTextAsync(path={Path},encoding={Encoding}): fileStream → {FileStream}", path, encoding, fileStream);

		_logger.LogWarning("<<< Exiting ReadAllTextAsync(path={Path},encoding={Encoding})", path, encoding);
        return await streamReader.ReadToEndAsync();
    }

    /// <summary>
    /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
    /// </summary>
    /// <param name="path">The file to open for reading</param>
    /// <param name="encoding">The encoding applied to the contents of the file</param>
    /// <returns>A string containing all lines of the file</returns>
    public virtual string ReadAllText(string path, Encoding encoding)
    {
		_logger.LogError(">>> Entering ReadAllText(path={Path},encoding={Encoding})", path, encoding);
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream, encoding);
		_logger.LogError("ReadAllText(path={Path},encoding={Encoding}): streamReader → {StreamReader}", path, encoding, streamReader);
		_logger.LogError("ReadAllText(path={Path},encoding={Encoding}): fileStream → {FileStream}", path, encoding, fileStream);

		_logger.LogWarning("<<< Exiting ReadAllText(path={Path},encoding={Encoding})", path, encoding);
        return streamReader.ReadToEnd();
    }

    /// <summary>
    /// Writes the specified byte array to the file
    /// </summary>
    /// <param name="filePath">The file to write to</param>
    /// <param name="bytes">The bytes to write to the file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WriteAllBytesAsync(string filePath, byte[] bytes)
    {
		_logger.LogError(">>> Entering WriteAllBytesAsync(filePath={FilePath},bytes={Bytes})", filePath, bytes);
        await File.WriteAllBytesAsync(filePath, bytes);
		_logger.LogWarning("<<< Exiting WriteAllBytesAsync(filePath={FilePath},bytes={Bytes})", filePath, bytes);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file using the specified encoding,
    /// and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file to write to</param>
    /// <param name="contents">The string to write to the file</param>
    /// <param name="encoding">The encoding to apply to the string</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WriteAllTextAsync(string path, string contents, Encoding encoding)
    {
		_logger.LogError(">>> Entering WriteAllTextAsync(path={Path},contents={Contents},encoding={Encoding})", path, contents, encoding);
        await File.WriteAllTextAsync(path, contents, encoding);
		_logger.LogWarning("<<< Exiting WriteAllTextAsync(path={Path},contents={Contents},encoding={Encoding})", path, contents, encoding);
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file using the specified encoding,
    /// and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file to write to</param>
    /// <param name="contents">The string to write to the file</param>
    /// <param name="encoding">The encoding to apply to the string</param>
    public virtual void WriteAllText(string path, string contents, Encoding encoding)
    {
		_logger.LogError(">>> Entering WriteAllText(path={Path},contents={Contents},encoding={Encoding})", path, contents, encoding);
        File.WriteAllText(path, contents, encoding);
		_logger.LogWarning("<<< Exiting WriteAllText(path={Path},contents={Contents},encoding={Encoding})", path, contents, encoding);
    }

    /// <summary>Locate a file at the given path.</summary>
    /// <param name="subpath">Relative path that identifies the file.</param>
    /// <returns>The file information. Caller must check Exists property.</returns>
    public new virtual IFileInfo GetFileInfo(string subpath)
    {
		_logger.LogError(">>> Entering GetFileInfo(subpath={Subpath})", subpath);
        subpath = subpath.Replace(Root, string.Empty);

		_logger.LogError("GetFileInfo(subpath={Subpath}): subpath → {Subpath1}", subpath, subpath);
		_logger.LogWarning("<<< Exiting GetFileInfo(subpath={Subpath})", subpath);
        return base.GetFileInfo(subpath);
    }

    /// <summary>
    /// Tests if the given path contains a root. A path is considered rooted
    /// if it starts with a backslash ("\") or a valid drive letter and a colon (":")
    /// </summary>
    /// <param name="path">Path to test</param>
    /// <returns>true if path contains a root; otherwise, false</returns>
    public virtual bool IsPathRooted(string path)
    {
		_logger.LogError(">>> Entering IsPathRooted(path={Path})", path);
        _logger.LogWarning("<<< Exiting IsPathRooted(path={Path})", path);
        return !string.IsNullOrEmpty(path) && Path.IsPathRooted(path);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
    /// </summary>
    public string WebRootPath { get; }

    #endregion
}