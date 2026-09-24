using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Media;

/// <summary>
/// Extensions
/// </summary>
public static partial class Extensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Get absolute path for local images
    /// </summary>
    /// <param name="fileProvider">Nop file provider</param>
    /// <param name="mediaSettings">Media settings</param>
    /// <param name="path"></param>
    /// <returns>Get local images path</returns>
    public static string GetLocalImagesPath(this INopFileProvider fileProvider, MediaSettings mediaSettings, string path = null)
    {
		_logger.LogError(">>> Entering GetLocalImagesPath(fileProvider={FileProvider},mediaSettings={MediaSettings},path={Path})", fileProvider, mediaSettings, path);
        if (string.IsNullOrEmpty(path))
        {
            var imagePath = mediaSettings.PicturePath;

            path = string.IsNullOrEmpty(imagePath) ? NopMediaDefaults.DefaultImagesPath : imagePath;
			_logger.LogError("GetLocalImagesPath(fileProvider={FileProvider},mediaSettings={MediaSettings},path={Path}): path → {Path1}", fileProvider, mediaSettings, path, path);
        }

        if (!fileProvider.IsPathRooted(path))
            path = fileProvider.GetAbsolutePath(path);

		_logger.LogWarning("<<< Exiting GetLocalImagesPath(fileProvider={FileProvider},mediaSettings={MediaSettings},path={Path})", fileProvider, mediaSettings, path);
        return path;
    }
}