﻿using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using SkiaSharp;
using Microsoft.Extensions.Logging;

 namespace Nop.Services.Media.RoxyFileman; 

 /// <summary>
 /// Looks up and manages uploaded files using the on-disk file system
 /// </summary>
 public partial class RoxyFilemanFileProvider : PhysicalFileProvider, IRoxyFilemanFileProvider
 {
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
     #region Fields

     protected INopFileProvider _nopFileProvider;
     protected readonly MediaSettings _mediaSettings;

     #endregion

     #region Ctor

     public RoxyFilemanFileProvider(INopFileProvider nopFileProvider, 
         MediaSettings mediaSettings) : base(nopFileProvider.Combine(nopFileProvider.GetLocalImagesPath(mediaSettings), NopRoxyFilemanDefaults.DefaultRootDirectory))
     {
		_logger.LogDebug(">>> Entering RoxyFilemanFileProvider(nopFileProvider={NopFileProvider},mediaSettings={MediaSettings})", nopFileProvider, mediaSettings);
         _mediaSettings = mediaSettings;
		_logger.LogDebug("RoxyFilemanFileProvider(nopFileProvider={NopFileProvider},mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", nopFileProvider, mediaSettings, _mediaSettings);
         _nopFileProvider = nopFileProvider;
     }

     #endregion

     #region Utilities

     /// <summary>
     /// Adjust image measures to target size
     /// </summary>
     /// <param name="image">Source image</param>
     /// <param name="maxWidth">Target width</param>
     /// <param name="maxHeight">Target height</param>
     /// <returns>Adjusted width and height</returns>
     protected virtual (int width, int height) ValidateImageMeasures(SKBitmap image, int maxWidth = 0, int maxHeight = 0)
     {
		_logger.LogDebug(">>> Entering ValidateImageMeasures(image={Image},maxWidth={MaxWidth},maxHeight={MaxHeight})", image, maxWidth, maxHeight);
         ArgumentNullException.ThrowIfNull(image);

         float width = Math.Min(image.Width, maxWidth);
		_logger.LogDebug("ValidateImageMeasures(image={Image},maxWidth={MaxWidth},maxHeight={MaxHeight}): width → {Width}", image, maxWidth, maxHeight, width);
         float height = Math.Min(image.Height, maxHeight);
		_logger.LogDebug("ValidateImageMeasures(image={Image},maxWidth={MaxWidth},maxHeight={MaxHeight}): height → {Height}", image, maxWidth, maxHeight, height);

         var targetSize = Math.Max(width, height);
		_logger.LogDebug("ValidateImageMeasures(image={Image},maxWidth={MaxWidth},maxHeight={MaxHeight}): targetSize → {TargetSize}", image, maxWidth, maxHeight, targetSize);

         if (image.Height > image.Width)
         {
             // portrait
             width = image.Width * (targetSize / image.Height);
             height = targetSize;
         }
         else
         {
             // landscape or square
             width = targetSize;
             height = image.Height * (targetSize / image.Width);
         }

		_logger.LogDebug("<<< Exiting ValidateImageMeasures(image={Image},maxWidth={MaxWidth},maxHeight={MaxHeight})", image, maxWidth, maxHeight);
         return ((int)width, (int)height);
     }

     /// <summary>
     /// Get a file type by the specified path string
     /// </summary>
     /// <param name="subpath">The path string from which to get the file type</param>
     /// <returns>File type</returns>
     protected virtual string GetFileType(string subpath)
     {
		_logger.LogDebug(">>> Entering GetFileType(subpath={Subpath})", subpath);
         var fileExtension = Path.GetExtension(subpath)?.ToLowerInvariant();

		_logger.LogDebug("GetFileType(subpath={Subpath}): fileExtension → {FileExtension}", subpath, fileExtension);
         return fileExtension switch
         {
             ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".svg" => "image",
             ".swf" or ".flv" => "flash",
             ".mp4" or ".webm" or ".ogg" or ".mov" or ".m4a" or ".mp3" or ".wav" => "media",
             _ => "file"
         };

         /* These media extensions are not supported by HTML5 or tinyMCE out of the box
          * but may possibly be supported if You find players for them.
          * if (fileExtension == ".3gp" || fileExtension == ".flv" 
          *     || fileExtension == ".rmvb" || fileExtension == ".wmv" || fileExtension == ".divx"
          *     || fileExtension == ".divx" || fileExtension == ".mpg" || fileExtension == ".rmvb"
          *     || fileExtension == ".vob" // video
          *     || fileExtension == ".aif" || fileExtension == ".aiff" || fileExtension == ".amr"
          *     || fileExtension == ".asf" || fileExtension == ".asx" || fileExtension == ".wma"
          *     || fileExtension == ".mid" || fileExtension == ".mp2") // audio
          *     fileType = "media"; */
		_logger.LogDebug("<<< Exiting GetFileType(subpath={Subpath})", subpath);
     }

     /// <summary>
     /// Get the absolute path for the specified path string in the root directory for this instance
     /// </summary>
     /// <param name="path">The file or directory for which to obtain absolute path information</param>
     /// <returns>The fully qualified location of path, such as "C:\MyFile.txt"</returns>
     protected virtual string GetFullPath(string path)
     {
		_logger.LogDebug(">>> Entering GetFullPath(path={Path})", path);
         if (string.IsNullOrEmpty(path))
			_logger.LogWarning("Throwing RoxyFilemanException from GetFullPath(path={Path})", path);
             throw new RoxyFilemanException("NoFilesFound");

         path = path.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		_logger.LogDebug("GetFullPath(path={Path}): path → {Path1}", path, path);

         if (_nopFileProvider.IsPathRooted(path))
             throw new RoxyFilemanException("NoFilesFound");

         var fullPath = Path.GetFullPath(Path.Combine(Root, path));
		_logger.LogDebug("GetFullPath(path={Path}): fullPath → {FullPath}", path, fullPath);

         if (!IsUnderneathRoot(fullPath))
             throw new RoxyFilemanException("NoFilesFound");

         return fullPath;
     }

     /// <summary>
     /// Get image format by mime type
     /// </summary>
     /// <param name="mimeType">Mime type</param>
     /// <returns>SKEncodedImageFormat</returns>
     protected virtual SKEncodedImageFormat GetImageFormatByMimeType(string mimeType)
     {
		_logger.LogDebug(">>> Entering GetImageFormatByMimeType(mimeType={MimeType})", mimeType);
         var format = SKEncodedImageFormat.Jpeg;
         if (string.IsNullOrEmpty(mimeType))
             return format;

         var parts = mimeType.ToLowerInvariant().Split('/');
		_logger.LogDebug("GetImageFormatByMimeType(mimeType={MimeType}): parts → {Parts}", mimeType, parts);
         var lastPart = parts[^1];

         switch (lastPart)
         {
             case "webp":
                 format = SKEncodedImageFormat.Webp;
                 break;
             case "png":
             case "gif":
             case "bmp":
             case "x-icon":
                 format = SKEncodedImageFormat.Png;
                 break;
             default:
                 break;
         }

		_logger.LogDebug("<<< Exiting GetImageFormatByMimeType(mimeType={MimeType})", mimeType);
         return format;
     }

     /// <summary>
     /// Get the unique name of the file (add -copy-(N) to the file name if there is already a file with that name in the directory)
     /// </summary>
     /// <param name="directoryPath">Path to the file directory</param>
     /// <param name="fileName">Original file name</param>
     /// <returns>Unique name of the file</returns>
     protected virtual string GetUniqueFileName(string directoryPath, string fileName)
     {
		_logger.LogDebug(">>> Entering GetUniqueFileName(directoryPath={DirectoryPath},fileName={FileName})", directoryPath, fileName);
         var uniqueFileName = fileName;

		_logger.LogDebug("GetUniqueFileName(directoryPath={DirectoryPath},fileName={FileName}): uniqueFileName → {UniqueFileName}", directoryPath, fileName, uniqueFileName);
         var i = 0;
         while (GetFileInfo(Path.Combine(directoryPath, uniqueFileName)) is IFileInfo fileInfo && fileInfo.Exists)
         {
             uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{Path.GetExtension(fileName)}";
         }

		_logger.LogDebug("<<< Exiting GetUniqueFileName(directoryPath={DirectoryPath},fileName={FileName})", directoryPath, fileName);
         return uniqueFileName;
     }

     /// <summary>
     /// Check the specified path is in the root directory of this instance
     /// </summary>
     /// <param name="fullPath">The absolute path</param>
     /// <returns>True if passed path is in the root; otherwise false</returns>
     protected virtual bool IsUnderneathRoot(string fullPath)
     {
		_logger.LogDebug(">>> Entering IsUnderneathRoot(fullPath={FullPath})", fullPath);
         return fullPath
             .StartsWith(Root, StringComparison.OrdinalIgnoreCase);
     }

     /// <summary>
     /// Scale image to fit the destination sizes
     /// </summary>
     /// <param name="data">Image data</param>
     /// <param name="format">SkiaSharp image format</param>
     /// <param name="maxWidth">Target width</param>
     /// <param name="maxHeight">Target height</param>
     /// <returns>The byte array of resized image</returns>
     protected virtual byte[] ResizeImage(byte[] data, SKEncodedImageFormat format, int maxWidth, int maxHeight)
     {
		_logger.LogDebug(">>> Entering ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight})", data, format, maxWidth, maxHeight);
         using var sourceStream = new SKMemoryStream(data);
         using var inputData = SKData.Create(sourceStream);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): inputData → {InputData}", data, format, maxWidth, maxHeight, inputData);
         using var image = SKBitmap.Decode(inputData);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): image → {Image}", data, format, maxWidth, maxHeight, image);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): sourceStream → {SourceStream}", data, format, maxWidth, maxHeight, sourceStream);

         var (width, height) = ValidateImageMeasures(image, maxWidth, maxHeight);

         var toBitmap = new SKBitmap(width, height, image.ColorType, image.AlphaType);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): toBitmap → {ToBitmap}", data, format, maxWidth, maxHeight, toBitmap);
         var samplingOptions = new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): samplingOptions → {SamplingOptions}", data, format, maxWidth, maxHeight, samplingOptions);

         if (!image.ScalePixels(toBitmap, samplingOptions))
			_logger.LogWarning("Throwing Exception from ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight})", data, format, maxWidth, maxHeight);
             throw new Exception("Image scaling");

         var newImage = SKImage.FromBitmap(toBitmap);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): newImage → {NewImage}", data, format, maxWidth, maxHeight, newImage);
         var imageData = newImage.Encode(format, _mediaSettings.DefaultImageQuality);
		_logger.LogDebug("ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight}): imageData → {ImageData}", data, format, maxWidth, maxHeight, imageData);

         newImage.Dispose();
		_logger.LogDebug("<<< Exiting ResizeImage(data={Data},format={Format},maxWidth={MaxWidth},maxHeight={MaxHeight})", data, format, maxWidth, maxHeight);
         return imageData.ToArray();
     }

     #endregion

     #region Methods

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
		_logger.LogDebug(">>> Entering DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName})", sourceDirName, destDirName);
         if (destDirName.StartsWith(sourceDirName, StringComparison.InvariantCulture))
			_logger.LogWarning("Throwing RoxyFilemanException from DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName})", sourceDirName, destDirName);
             throw new RoxyFilemanException("E_CannotMoveDirToChild");

         var sourceDirInfo = new DirectoryInfo(GetFullPath(sourceDirName));
		_logger.LogDebug("DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName}): sourceDirInfo → {SourceDirInfo}", sourceDirName, destDirName, sourceDirInfo);
         if (!sourceDirInfo.Exists)
             throw new RoxyFilemanException("E_MoveDirInvalisPath");

         if (string.Equals(sourceDirInfo.FullName, Root, StringComparison.InvariantCultureIgnoreCase))
             throw new RoxyFilemanException("E_MoveDir");

         var newFullPath = Path.Combine(GetFullPath(destDirName), sourceDirInfo.Name);
		_logger.LogDebug("DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName}): newFullPath → {NewFullPath}", sourceDirName, destDirName, newFullPath);
         var destinationDirInfo = new DirectoryInfo(newFullPath);
		_logger.LogDebug("DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName}): destinationDirInfo → {DestinationDirInfo}", sourceDirName, destDirName, destinationDirInfo);
         if (destinationDirInfo.Exists)
             throw new RoxyFilemanException("E_DirAlreadyExists");

         try
         {
             sourceDirInfo.MoveTo(destinationDirInfo.FullName);
         }
         catch
         {
			_logger.LogError("Exception in DirectoryMove(sourceDirName={SourceDirName},destDirName={DestDirName}): {Arg2}", sourceDirName, destDirName, ex.Message);
             throw new RoxyFilemanException("E_MoveDir");
         }
     }

     /// <summary>
     /// Locate a file at the given path by directly mapping path segments to physical directories.
     /// </summary>
     /// <param name="subpath">A path under the root directory</param>
     /// <returns>The file information. Caller must check Microsoft.Extensions.FileProviders.IFileInfo.Exists property.</returns>
     public new IFileInfo GetFileInfo(string subpath)
     {
		_logger.LogDebug(">>> Entering GetFileInfo(subpath={Subpath})", subpath);
         if (string.IsNullOrEmpty(subpath))
             return new NotFoundFileInfo(subpath);

         subpath = subpath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		_logger.LogDebug("GetFileInfo(subpath={Subpath}): subpath → {Subpath1}", subpath, subpath);

         //absolute paths not permitted.
         if (_nopFileProvider.IsPathRooted(subpath))
             return new NotFoundFileInfo(subpath);

         return base.GetFileInfo(subpath);
     }

     /// <summary>
     /// Create configuration file for RoxyFileman
     /// </summary>
     /// <param name="pathBase">The base path for the store</param>
     /// <param name="lang">Two-letter language code</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual async Task<RoxyFilemanConfig> GetOrCreateConfigurationAsync(string pathBase, string lang)
     {
		_logger.LogDebug(">>> Entering GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang})", pathBase, lang);
         //check whether the path base has changed, otherwise there is no need to overwrite the configuration file
         if (Singleton<RoxyFilemanConfig>.Instance?.RETURN_URL_PREFIX?.Equals(pathBase) ?? false)
         {
             return Singleton<RoxyFilemanConfig>.Instance;
         }

         var filePath = _nopFileProvider.GetAbsolutePath(NopRoxyFilemanDefaults.ConfigurationFile);
		_logger.LogDebug("GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang}): filePath → {FilePath}", pathBase, lang, filePath);

         //create file if not exists
         _nopFileProvider.CreateFile(filePath);

         //try to read existing configuration
         var existingText = await _nopFileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
		_logger.LogDebug("GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang}): existingText → {ExistingText}", pathBase, lang, existingText);
         var existingConfiguration = JsonConvert.DeserializeObject<RoxyFilemanConfig>(existingText);
		_logger.LogDebug("GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang}): existingConfiguration → {ExistingConfiguration}", pathBase, lang, existingConfiguration);

         //create configuration
         var configuration = new RoxyFilemanConfig
         {
             FILES_ROOT = existingConfiguration?.FILES_ROOT ?? NopRoxyFilemanDefaults.DefaultRootDirectory,
             SESSION_PATH_KEY = existingConfiguration?.SESSION_PATH_KEY ?? string.Empty,
             THUMBS_VIEW_WIDTH = existingConfiguration?.THUMBS_VIEW_WIDTH ?? 140,
             THUMBS_VIEW_HEIGHT = existingConfiguration?.THUMBS_VIEW_HEIGHT ?? 120,
             PREVIEW_THUMB_WIDTH = existingConfiguration?.PREVIEW_THUMB_WIDTH ?? 300,
             PREVIEW_THUMB_HEIGHT = existingConfiguration?.PREVIEW_THUMB_HEIGHT ?? 200,
             MAX_IMAGE_WIDTH = existingConfiguration?.MAX_IMAGE_WIDTH ?? _mediaSettings.MaximumImageSize,
             MAX_IMAGE_HEIGHT = existingConfiguration?.MAX_IMAGE_HEIGHT ?? _mediaSettings.MaximumImageSize,
             DEFAULTVIEW = existingConfiguration?.DEFAULTVIEW ?? "list",
             FORBIDDEN_UPLOADS = existingConfiguration?.FORBIDDEN_UPLOADS ?? string.Join(" ", NopRoxyFilemanDefaults.ForbiddenUploadExtensions),
             ALLOWED_UPLOADS = existingConfiguration?.ALLOWED_UPLOADS ?? string.Empty,
             FILEPERMISSIONS = existingConfiguration?.FILEPERMISSIONS ?? "0644",
             DIRPERMISSIONS = existingConfiguration?.DIRPERMISSIONS ?? "0755",
             LANG = existingConfiguration?.LANG ?? lang,
             DATEFORMAT = existingConfiguration?.DATEFORMAT ?? "dd/MM/yyyy HH:mm",
             OPEN_LAST_DIR = existingConfiguration?.OPEN_LAST_DIR ?? "yes",

             //no need user to configure
             INTEGRATION = "custom",
             RETURN_URL_PREFIX = $"{pathBase}/images/uploaded/",
             DIRLIST = $"{pathBase}/Admin/RoxyFileman/DirectoriesList",
             CREATEDIR = $"{pathBase}/Admin/RoxyFileman/CreateDirectory",
             DELETEDIR = $"{pathBase}/Admin/RoxyFileman/DeleteDirectory",
             MOVEDIR = $"{pathBase}/Admin/RoxyFileman/MoveDirectory",
             COPYDIR = $"{pathBase}/Admin/RoxyFileman/CopyDirectory",
             RENAMEDIR = $"{pathBase}/Admin/RoxyFileman/RenameDirectory",
             FILESLIST = $"{pathBase}/Admin/RoxyFileman/FilesList",
             UPLOAD = $"{pathBase}/Admin/RoxyFileman/UploadFiles",
             DOWNLOAD = $"{pathBase}/Admin/RoxyFileman/DownloadFile",
             DOWNLOADDIR = $"{pathBase}/Admin/RoxyFileman/DownloadDirectory",
             DELETEFILE = $"{pathBase}/Admin/RoxyFileman/DeleteFile",
             MOVEFILE = $"{pathBase}/Admin/RoxyFileman/MoveFile",
             COPYFILE = $"{pathBase}/Admin/RoxyFileman/CopyFile",
             RENAMEFILE = $"{pathBase}/Admin/RoxyFileman/RenameFile",
             GENERATETHUMB = $"{pathBase}/Admin/RoxyFileman/CreateImageThumbnail"
         };

         //save the file
         var text = JsonConvert.SerializeObject(configuration, Formatting.Indented);
		_logger.LogDebug("GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang}): text → {Text}", pathBase, lang, text);
         await File.WriteAllTextAsync(filePath, text, Encoding.UTF8);

         Singleton<RoxyFilemanConfig>.Instance = configuration;

		_logger.LogDebug("<<< Exiting GetOrCreateConfigurationAsync(pathBase={PathBase},lang={Lang})", pathBase, lang);
         return configuration;
     }

     /// <summary>
     /// Get all available directories as a directory tree
     /// </summary>
     /// <param name="type">Type of the file</param>
     /// <param name="isRecursive">A value indicating whether to return a directory tree recursively</param>
     /// <param name="rootDirectoryPath">Path to start directory</param>
     public virtual IEnumerable<RoxyDirectoryInfo> GetDirectories(string type, bool isRecursive = true, string rootDirectoryPath = "")
     {
		_logger.LogDebug(">>> Entering GetDirectories(type={Type},isRecursive={IsRecursive},rootDirectoryPath={RootDirectoryPath})", type, isRecursive, rootDirectoryPath);
         foreach (var item in GetDirectoryContents(rootDirectoryPath))
         {
             if (item.IsDirectory)
             {
                 var dirInfo = new DirectoryInfo(item.PhysicalPath);
				_logger.LogDebug("GetDirectories(type={Type},isRecursive={IsRecursive},rootDirectoryPath={RootDirectoryPath}): dirInfo → {DirInfo}", type, isRecursive, rootDirectoryPath, dirInfo);

                 yield return new RoxyDirectoryInfo(
                     getRelativePath(item.Name),
                     dirInfo.GetFiles().Count(x => isMatchType(x.Name)),
                     dirInfo.GetDirectories().Length);
             }

             if (!isRecursive)
                 break;

             foreach (var subDir in GetDirectories(type, isRecursive, getRelativePath(item.Name)))
                 yield return subDir;
         }

         string getRelativePath(string name) => Path.Combine(rootDirectoryPath, name);
         bool isMatchType(string name) => string.IsNullOrEmpty(type) || GetFileType(name) == type;
		_logger.LogDebug("<<< Exiting GetDirectories(type={Type},isRecursive={IsRecursive},rootDirectoryPath={RootDirectoryPath})", type, isRecursive, rootDirectoryPath);
     }

     /// <summary>
     /// Get files in the passed directory
     /// </summary>
     /// <param name="directoryPath">Path to the files directory</param>
     /// <param name="type">Type of the files</param>
     /// <returns>
     /// The list of <see cref="RoxyFileInfo"/>
     /// </returns>
     public virtual IEnumerable<RoxyFileInfo> GetFiles(string directoryPath = "", string type = "")
     {
		_logger.LogDebug(">>> Entering GetFiles(directoryPath={DirectoryPath},type={Type})", directoryPath, type);
         var files = GetDirectoryContents(directoryPath);

		_logger.LogDebug("GetFiles(directoryPath={DirectoryPath},type={Type}): files → {Files}", directoryPath, type, files);
         return files
             .Where(f => !f.IsDirectory && isMatchType(f.Name))
             .Select(f =>
             {
                 var width = 0;
                 var height = 0;

                 if (GetFileType(f.Name) == "image")
                 {
                     using var skData = SKData.Create(f.PhysicalPath);
                        
                     if (skData != null)
                     {
                         var image = SKBitmap.DecodeBounds(skData);

                         width = image.Width;
                         height = image.Height;
                     }
                 }

                 return new RoxyFileInfo(getRelativePath(f.Name), f.LastModified, f.Length, width, height);
             });

         bool isMatchType(string name) => string.IsNullOrEmpty(type) || GetFileType(name) == type;
         string getRelativePath(string name) => Path.Combine(directoryPath, name);
		_logger.LogDebug("<<< Exiting GetFiles(directoryPath={DirectoryPath},type={Type})", directoryPath, type);
     }

     /// <summary>
     /// Moves a specified file to a new location, providing the option to specify a new file name
     /// </summary>
     /// <param name="sourcePath">The name of the file to move. Can include a relative or absolute path</param>
     /// <param name="destinationPath">The new path and name for the file</param>
     public virtual void FileMove(string sourcePath, string destinationPath)
     {
		_logger.LogDebug(">>> Entering FileMove(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
         var sourceFile = GetFileInfo(sourcePath);

		_logger.LogDebug("FileMove(sourcePath={SourcePath},destinationPath={DestinationPath}): sourceFile → {SourceFile}", sourcePath, destinationPath, sourceFile);
         if (!sourceFile.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from FileMove(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
             throw new RoxyFilemanException("E_MoveFileInvalisPath");

         var destinationFile = GetFileInfo(destinationPath);
		_logger.LogDebug("FileMove(sourcePath={SourcePath},destinationPath={DestinationPath}): destinationFile → {DestinationFile}", sourcePath, destinationPath, destinationFile);
         if (destinationFile.Exists)
             throw new RoxyFilemanException("E_MoveFileAlreadyExists");

         try
         {
             new FileInfo(sourceFile.PhysicalPath)
                 .MoveTo(GetFullPath(destinationPath));
         }
         catch
         {
			_logger.LogError("Exception in FileMove(sourcePath={SourcePath},destinationPath={DestinationPath}): {Arg2}", sourcePath, destinationPath, ex.Message);
             throw new RoxyFilemanException("E_MoveFile");
         }
     }

     /// <summary>
     /// Copy the directory with the embedded files and directories
     /// </summary>
     /// <param name="sourcePath">Path to the source directory</param>
     /// <param name="destinationPath">Path to the destination directory</param>
     public virtual void CopyDirectory(string sourcePath, string destinationPath)
     {
		_logger.LogDebug(">>> Entering CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
         var sourceDirInfo = new DirectoryInfo(GetFullPath(sourcePath));
         if (!sourceDirInfo.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
			_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): sourceDirInfo → {SourceDirInfo}", sourcePath, destinationPath, sourceDirInfo);
             throw new RoxyFilemanException("E_CopyDirInvalidPath");

         var newPath = Path.Combine(GetFullPath(destinationPath), sourceDirInfo.Name);
		_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): newPath → {NewPath}", sourcePath, destinationPath, newPath);
         var destinationDirInfo = new DirectoryInfo(newPath);
		_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): destinationDirInfo → {DestinationDirInfo}", sourcePath, destinationPath, destinationDirInfo);

         if (destinationDirInfo.Exists)
             throw new RoxyFilemanException("E_DirAlreadyExists");

         try
         {
             destinationDirInfo.Create();

             foreach (var file in sourceDirInfo.GetFiles())
             {
                 var newFile = GetFileInfo(Path.Combine(destinationPath, file.Name));
				_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): newFile → {NewFile}", sourcePath, destinationPath, newFile);
                 if (!newFile.Exists)
                     file.CopyTo(Path.Combine(destinationDirInfo.FullName, file.Name));
             }

             foreach (var directory in sourceDirInfo.GetDirectories())
             {
                 var destinationSubPath = Path.Combine(destinationPath, sourceDirInfo.Name, directory.Name);
				_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): destinationSubPath → {DestinationSubPath}", sourcePath, destinationPath, destinationSubPath);
                 var sourceSubPath = Path.Combine(sourcePath, directory.Name);
				_logger.LogDebug("CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): sourceSubPath → {SourceSubPath}", sourcePath, destinationPath, sourceSubPath);
                 CopyDirectory(sourceSubPath, destinationSubPath);
             }
         }
         catch
         {
			_logger.LogError("Exception in CopyDirectory(sourcePath={SourcePath},destinationPath={DestinationPath}): {Arg2}", sourcePath, destinationPath, ex.Message);
             throw new RoxyFilemanException("E_CopyFile");
         }
     }

     /// <summary>
     /// Rename the directory
     /// </summary>
     /// <param name="sourcePath">Path to the source directory</param>
     /// <param name="newName">New name of the directory</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual void RenameDirectory(string sourcePath, string newName)
     {
		_logger.LogDebug(">>> Entering RenameDirectory(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
         try
         {
             var destinationPath = Path.Combine(Path.GetDirectoryName(sourcePath), newName);
			_logger.LogDebug("RenameDirectory(sourcePath={SourcePath},newName={NewName}): destinationPath → {DestinationPath}", sourcePath, newName, destinationPath);
             DirectoryMove(sourcePath, destinationPath);
         }
         catch (Exception ex)
         {
			_logger.LogError("Exception in RenameDirectory(sourcePath={SourcePath},newName={NewName}): {Arg2}", sourcePath, newName, ex.Message);
		_logger.LogDebug("<<< Exiting RenameDirectory(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
             throw new RoxyFilemanException("E_RenameDir", ex);
         }
     }

     /// <summary>
     /// Rename the file
     /// </summary>
     /// <param name="sourcePath">Path to the source file</param>
     /// <param name="newName">New name of the file</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual void RenameFile(string sourcePath, string newName)
     {
		_logger.LogDebug(">>> Entering RenameFile(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
         try
         {
             var destinationPath = Path.Combine(Path.GetDirectoryName(sourcePath), newName);
			_logger.LogDebug("RenameFile(sourcePath={SourcePath},newName={NewName}): destinationPath → {DestinationPath}", sourcePath, newName, destinationPath);
             FileMove(sourcePath, destinationPath);
         }
         catch (Exception ex)
         {
			_logger.LogError("Exception in RenameFile(sourcePath={SourcePath},newName={NewName}): {Arg2}", sourcePath, newName, ex.Message);
		_logger.LogDebug("<<< Exiting RenameFile(sourcePath={SourcePath},newName={NewName})", sourcePath, newName);
             throw new RoxyFilemanException("E_RenameFile", ex);
         }
     }

     /// <summary>
     /// Delete the file
     /// </summary>
     /// <param name="path">Path to the file</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual void DeleteFile(string path)
     {
		_logger.LogDebug(">>> Entering DeleteFile(path={Path})", path);
         var fileToDelete = GetFileInfo(path);

		_logger.LogDebug("DeleteFile(path={Path}): fileToDelete → {FileToDelete}", path, fileToDelete);
         if (!fileToDelete.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from DeleteFile(path={Path})", path);
             throw new RoxyFilemanException("E_DeleteFileInvalidPath");

         try
         {
             File.Delete(GetFullPath(path));
         }
         catch
         {
			_logger.LogError("Exception in DeleteFile(path={Path}): {Arg1}", path, ex.Message);
             throw new RoxyFilemanException("E_DeleteFile");
         }
     }

     /// <summary>
     /// Copy the file
     /// </summary>
     /// <param name="sourcePath">Path to the source file</param>
     /// <param name="destinationPath">Path to the destination file</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual void CopyFile(string sourcePath, string destinationPath)
     {
		_logger.LogDebug(">>> Entering CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
         var sourceFile = GetFileInfo(sourcePath);

		_logger.LogDebug("CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath}): sourceFile → {SourceFile}", sourcePath, destinationPath, sourceFile);
         if (!sourceFile.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath})", sourcePath, destinationPath);
             throw new RoxyFilemanException("E_CopyFileInvalidPath");

         var newFilePath = Path.Combine(destinationPath, sourceFile.Name);
		_logger.LogDebug("CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath}): newFilePath → {NewFilePath}", sourcePath, destinationPath, newFilePath);
         var destinationFile = GetFileInfo(newFilePath);
		_logger.LogDebug("CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath}): destinationFile → {DestinationFile}", sourcePath, destinationPath, destinationFile);

         if (destinationFile.Exists)
             newFilePath = Path.Combine(destinationPath, GetUniqueFileName(destinationPath, sourceFile.Name));

         try
         {
             File.Copy(sourceFile.PhysicalPath, GetFullPath(newFilePath));
         }
         catch
         {
			_logger.LogError("Exception in CopyFile(sourcePath={SourcePath},destinationPath={DestinationPath}): {Arg2}", sourcePath, destinationPath, ex.Message);
             throw new RoxyFilemanException("E_CopyFile");
         }
     }

     /// <summary>
     /// Create the new directory
     /// </summary>
     /// <param name="parentDirectoryPath">Path to the parent directory</param>
     /// <param name="name">Name of the new directory</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual void CreateDirectory(string parentDirectoryPath, string name)
     {
		_logger.LogDebug(">>> Entering CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name})", parentDirectoryPath, name);
         //validate path and get absolute form
         var fullPath = GetFullPath(Path.Combine(parentDirectoryPath, name));
		_logger.LogDebug("CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name}): fullPath → {FullPath}", parentDirectoryPath, name, fullPath);

         var newDirectory = new DirectoryInfo(fullPath);
		_logger.LogDebug("CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name}): newDirectory → {NewDirectory}", parentDirectoryPath, name, newDirectory);
         if (!newDirectory.Exists)
             newDirectory.Create();
			_logger.LogDebug("<<< Exiting CreateDirectory(parentDirectoryPath={ParentDirectoryPath},name={Name})", parentDirectoryPath, name);
     }

     /// <summary>
     /// Delete the directory
     /// </summary>
     /// <param name="path">Path to the directory</param>
     public virtual void DeleteDirectory(string path)
     {
		_logger.LogDebug(">>> Entering DeleteDirectory(path={Path})", path);
         var sourceDirInfo = new DirectoryInfo(GetFullPath(path));
         if (!sourceDirInfo.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from DeleteDirectory(path={Path})", path);
			_logger.LogDebug("DeleteDirectory(path={Path}): sourceDirInfo → {SourceDirInfo}", path, sourceDirInfo);
             throw new RoxyFilemanException("E_DeleteDirInvalidPath");

         if (string.Equals(sourceDirInfo.FullName, Root, StringComparison.InvariantCultureIgnoreCase))
             throw new RoxyFilemanException("E_CannotDeleteRoot");

         if (sourceDirInfo.GetFiles().Length > 0 || sourceDirInfo.GetDirectories().Length > 0)
             throw new RoxyFilemanException("E_DeleteNonEmpty");

         try
         {
             sourceDirInfo.Delete();
         }
         catch
         {
			_logger.LogError("Exception in DeleteDirectory(path={Path}): {Arg1}", path, ex.Message);
             throw new RoxyFilemanException("E_CannotDeleteDir");
         }
     }

     /// <summary>
     /// Save file in the root directory for this instance
     /// </summary>
     /// <param name="directoryPath">Directory path in the root</param>
     /// <param name="fileName">The file name and extension</param>
     /// <param name="contentType">Mime type</param>
     /// <param name="fileStream">The stream to read</param>
     /// <returns>A task that represents the asynchronous operation</returns>
     public virtual async Task SaveFileAsync(string directoryPath, string fileName, string contentType, Stream fileStream)
     {
		_logger.LogDebug(">>> Entering SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream})", directoryPath, fileName, contentType, fileStream);
         var uniqueFileName = GetUniqueFileName(directoryPath, Path.GetFileName(fileName));
         var destinationFile = Path.Combine(directoryPath, uniqueFileName);
		_logger.LogDebug("SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream}): destinationFile → {DestinationFile}", directoryPath, fileName, contentType, fileStream, destinationFile);
		_logger.LogDebug("SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream}): uniqueFileName → {UniqueFileName}", directoryPath, fileName, contentType, fileStream, uniqueFileName);

         await using var stream = new FileStream(GetFullPath(destinationFile), FileMode.Create);
		_logger.LogDebug("SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream}): stream → {Stream}", directoryPath, fileName, contentType, fileStream, stream);

         if (GetFileType(Path.GetExtension(uniqueFileName)) == "image")
         {
             using var memoryStream = new MemoryStream();
			_logger.LogDebug("SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream}): memoryStream → {MemoryStream}", directoryPath, fileName, contentType, fileStream, memoryStream);
             await fileStream.CopyToAsync(memoryStream);

             var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

             var imageData = ResizeImage(memoryStream.ToArray(),
                 GetImageFormatByMimeType(contentType),
                 roxyConfig?.MAX_IMAGE_WIDTH ?? _mediaSettings.MaximumImageSize,
                 roxyConfig?.MAX_IMAGE_HEIGHT ?? _mediaSettings.MaximumImageSize);

             await stream.WriteAsync(imageData);
         }
         else
         {
             await fileStream.CopyToAsync(stream);
         }

         await stream.FlushAsync();
		_logger.LogDebug("<<< Exiting SaveFileAsync(directoryPath={DirectoryPath},fileName={FileName},contentType={ContentType},fileStream={FileStream})", directoryPath, fileName, contentType, fileStream);
     }

     /// <summary>
     /// Get the thumbnail of the image
     /// </summary>
     /// <param name="sourcePath">File path</param>
     /// <param name="contentType">Mime type</param>
     /// <returns>Byte array of the specified image</returns>
     public virtual byte[] CreateImageThumbnail(string sourcePath, string contentType)
     {
		_logger.LogDebug(">>> Entering CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType})", sourcePath, contentType);
         var imageInfo = GetFileInfo(sourcePath);

		_logger.LogDebug("CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType}): imageInfo → {ImageInfo}", sourcePath, contentType, imageInfo);
         if (!imageInfo.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType})", sourcePath, contentType);
             throw new RoxyFilemanException("Image not found");

         var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

         using var imageStream = imageInfo.CreateReadStream();
		_logger.LogDebug("CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType}): imageStream → {ImageStream}", sourcePath, contentType, imageStream);
         using var ms = new MemoryStream();
		_logger.LogDebug("CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType}): ms → {Ms}", sourcePath, contentType, ms);

         imageStream.CopyTo(ms);

		_logger.LogDebug("<<< Exiting CreateImageThumbnail(sourcePath={SourcePath},contentType={ContentType})", sourcePath, contentType);
         return ResizeImage(
             ms.ToArray(),
             GetImageFormatByMimeType(contentType),
             roxyConfig.THUMBS_VIEW_WIDTH,
             roxyConfig.THUMBS_VIEW_HEIGHT);
     }

     /// <summary>
     /// Create a zip archive of the specified directory.
     /// </summary>
     /// <param name="directoryPath">The directory path with files to compress</param>
     /// <returns>The byte array</returns>
     public virtual byte[] CreateZipArchiveFromDirectory(string directoryPath)
     {
		_logger.LogDebug(">>> Entering CreateZipArchiveFromDirectory(directoryPath={DirectoryPath})", directoryPath);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
         var sourceDirInfo = new DirectoryInfo(GetFullPath(directoryPath));
         if (!sourceDirInfo.Exists)
			_logger.LogWarning("Throwing RoxyFilemanException from CreateZipArchiveFromDirectory(directoryPath={DirectoryPath})", directoryPath);
			_logger.LogDebug("CreateZipArchiveFromDirectory(directoryPath={DirectoryPath}): sourceDirInfo → {SourceDirInfo}", directoryPath, sourceDirInfo);
             throw new RoxyFilemanException("E_CreateArchive");

         using var memoryStream = new MemoryStream();
		_logger.LogDebug("CreateZipArchiveFromDirectory(directoryPath={DirectoryPath}): memoryStream → {MemoryStream}", directoryPath, memoryStream);
         using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
         {
             foreach (var file in sourceDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
             {
                 var fileRelativePath = file.FullName.Replace(sourceDirInfo.FullName, string.Empty)
                     .TrimStart('\\');

                 using var fileStream = file.OpenRead();
                 using var fileStreamInZip = archive.CreateEntry(fileRelativePath).Open();
                 fileStream.CopyTo(fileStreamInZip);
             }
         }

         //ToArray() should be outside of the archive using
		_logger.LogInformation("CreateZipArchiveFromDirectory(directoryPath)={DirectoryPath}: external service call took {Arg1} ms", directoryPath, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CreateZipArchiveFromDirectory(directoryPath={DirectoryPath})", directoryPath);
         return memoryStream.ToArray();
     }

     #endregion
 }