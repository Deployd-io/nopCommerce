using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.CloudflareImages.Services;

/// <summary>
/// Picture service for Cloudflare Images
/// </summary>
public partial class CloudflareThumbService : IThumbService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly CloudflareImagesSettings _cloudflareImagesSettings;
    private readonly CloudflareImagesHttpClient _cloudflareImagesHttpClient;
    private readonly INopFileProvider _fileProvider;
    private readonly IRepository<Domain.CloudflareImages> _cloudflareImagesRepository;

    #endregion

    #region Ctor

    public CloudflareThumbService(CloudflareImagesSettings cloudflareImagesSettings,
        CloudflareImagesHttpClient cloudflareImagesHttpClient,
        INopFileProvider fileProvider,
        IRepository<Domain.CloudflareImages> cloudflareImagesRepository)
    {
		_logger.LogError(">>> Entering CloudflareThumbService(cloudflareImagesSettings={CloudflareImagesSettings},cloudflareImagesHttpClient={CloudflareImagesHttpClient},fileProvider={FileProvider},cloudflareImagesRepository={CloudflareImagesRepository})", cloudflareImagesSettings, cloudflareImagesHttpClient, fileProvider, cloudflareImagesRepository);
        _cloudflareImagesSettings = cloudflareImagesSettings;
        _cloudflareImagesHttpClient = cloudflareImagesHttpClient;
		_logger.LogDebug("CloudflareThumbService(cloudflareImagesSettings={CloudflareImagesSettings},cloudflareImagesHttpClient={CloudflareImagesHttpClient},fileProvider={FileProvider},cloudflareImagesRepository={CloudflareImagesRepository}): _cloudflareImagesHttpClient → {_cloudflareImagesHttpClient}", cloudflareImagesSettings, cloudflareImagesHttpClient, fileProvider, cloudflareImagesRepository, _cloudflareImagesHttpClient);
		_logger.LogDebug("CloudflareThumbService(cloudflareImagesSettings={CloudflareImagesSettings},cloudflareImagesHttpClient={CloudflareImagesHttpClient},fileProvider={FileProvider},cloudflareImagesRepository={CloudflareImagesRepository}): _cloudflareImagesSettings → {_cloudflareImagesSettings}", cloudflareImagesSettings, cloudflareImagesHttpClient, fileProvider, cloudflareImagesRepository, _cloudflareImagesSettings);
    _logger.LogWarning("<<< Exiting CloudflareThumbService(cloudflareImagesSettings={CloudflareImagesSettings},cloudflareImagesHttpClient={CloudflareImagesHttpClient},fileProvider={FileProvider},cloudflareImagesRepository={CloudflareImagesRepository})", cloudflareImagesSettings, cloudflareImagesHttpClient, fileProvider, cloudflareImagesRepository);
        _fileProvider = fileProvider;
		_logger.LogDebug("CloudflareThumbService(cloudflareImagesSettings={CloudflareImagesSettings},cloudflareImagesHttpClient={CloudflareImagesHttpClient},fileProvider={FileProvider},cloudflareImagesRepository={CloudflareImagesRepository}): _fileProvider → {_fileProvider}", cloudflareImagesSettings, cloudflareImagesHttpClient, fileProvider, cloudflareImagesRepository, _fileProvider);
        _cloudflareImagesRepository = cloudflareImagesRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a picture thumb local path
    /// </summary>
    /// <param name="pictureUrl">Picture URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public async Task<string> GetThumbLocalPathAsync(string pictureUrl)
    {
		_logger.LogError(">>> Entering GetThumbLocalPathAsync(pictureUrl={PictureUrl})", pictureUrl);
		_logger.LogWarning("<<< Exiting GetThumbLocalPathAsync(pictureUrl={PictureUrl})", pictureUrl);
        if (string.IsNullOrEmpty(pictureUrl)) {
            return string.Empty;
        }

        return await GetThumbLocalPathByFileNameAsync(_fileProvider.GetFileName(pictureUrl));
    }

    /// <summary>
    /// Get a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result
    /// </returns>
    public async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
    {
		_logger.LogError(">>> Entering GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName})", thumbFilePath, thumbFileName);
		_logger.LogError("GeneratedThumbExistsAsync(thumbFilePath,thumbFileName)={ThumbFilePath},{ThumbFileName}: database query executing", thumbFilePath, thumbFileName);
    _logger.LogWarning("<<< Exiting GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName})", thumbFilePath, thumbFileName);
		_logger.LogError(">>> Entering GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName})", thumbFileName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetThumbLocalPathByFileNameAsync(thumbFileName)={ThumbFileName}: database query executed in {Arg1} ms", thumbFileName, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName})", thumbFileName);
        return await _cloudflareImagesRepository.Table.AnyAsync(i => i.ThumbFileName.Equals(thumbFileName));
    }

    /// <summary>
    /// Save a picture thumb
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="binary">Picture binary</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
    {
		_logger.LogError(">>> Entering SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary})", thumbFilePath, thumbFileName, mimeType, binary);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var dataContent = new MultipartFormDataContent
        {
            { new StreamContent(new MemoryStream(binary)), "file", thumbFileName }
        };

        var response = await _cloudflareImagesHttpClient.SaveThumbAsync(dataContent);
		_logger.LogError("SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary}): response → {Response}", thumbFilePath, thumbFileName, mimeType, binary, response);

			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogWarning("<<< Exiting SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary})", thumbFilePath, thumbFileName, mimeType, binary);
        if (response is not { Success: true }) {
            return;
        }

        await _cloudflareImagesRepository.InsertAsync(new Domain.CloudflareImages
        {
			_logger.LogError("SaveThumbAsync(thumbFilePath,thumbFileName,mimeType,binary)={ThumbFilePath},{ThumbFileName},{MimeType},{Binary}: external service call took {Arg4} ms", thumbFilePath, thumbFileName, mimeType, binary, __logStart.ElapsedMilliseconds);
            ImageId = response.Result.Id,
            ThumbFileName = thumbFileName
        }, false);
		_logger.LogError("SaveThumbAsync(thumbFilePath,thumbFileName,mimeType,binary)={ThumbFilePath},{ThumbFileName},{MimeType},{Binary}: save query executed in {Arg4} ms", thumbFilePath, thumbFileName, mimeType, binary, __logStartDb.ElapsedMilliseconds);
    }

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public async Task<string> GetThumbLocalPathByFileNameAsync(string thumbFileName)
    {
		_logger.LogError(">>> Entering GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName})", thumbFileName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var image = await _cloudflareImagesRepository.Table.FirstOrDefaultAsync(i => i.ThumbFileName.Equals(thumbFileName));

			_logger.LogError("GetThumbLocalPathByFileNameAsync(thumbFileName)={ThumbFileName}: database query executed in {Arg1} ms", thumbFileName, __logStart.ElapsedMilliseconds);
			_logger.LogError("GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName}): image → {Image}", thumbFileName, image);
        if (image == null) {
            return null;
        }

        return _cloudflareImagesSettings.DeliveryUrl
            .Replace(CloudflareImagesDefaults.ImageIdPattern, image.ImageId)
            .Replace(CloudflareImagesDefaults.VariantNamePattern, "public");
    }

    /// <summary>
    /// Get picture (thumb) URL 
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
		_logger.LogError(">>> Entering GetThumbUrlAsync(thumbFileName={ThumbFileName},storeLocation={StoreLocation})", thumbFileName, storeLocation);
        _logger.LogWarning("<<< Exiting GetThumbUrlAsync(thumbFileName={ThumbFileName},storeLocation={StoreLocation})", thumbFileName, storeLocation);
        return await GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    /// <summary>
    /// Delete picture thumbs
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePictureThumbsAsync(Picture picture)
    {
		_logger.LogError(">>> Entering DeletePictureThumbsAsync(picture={Picture})", picture);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var items = await _cloudflareImagesRepository.Table
            .Where(i => i.ThumbFileName.Contains($"_{picture.SeoFilename}.") || i.ThumbFileName.Contains($"_{picture.SeoFilename}_"))
            .ToListAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("DeletePictureThumbsAsync(picture)={Picture}: database query executed in {Arg1} ms", picture, __logStartDb.ElapsedMilliseconds);

        foreach (var item in items)
            await _cloudflareImagesHttpClient.DeleteThumbAsync(item.ImageId);

        await _cloudflareImagesRepository.DeleteAsync(items, false);
		_logger.LogError("DeletePictureThumbsAsync(picture)={Picture}: external service call took {Arg1} ms", picture, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeletePictureThumbsAsync(picture={Picture})", picture);
    }

    #endregion
}