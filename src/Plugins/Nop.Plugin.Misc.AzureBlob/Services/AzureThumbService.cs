using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.AzureBlob.Services;

/// <summary>
/// Picture service for Windows Azure
/// </summary>
public class AzureThumbService : IThumbService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private static BlobContainerClient _blobContainerClient;
    private static BlobServiceClient _blobServiceClient;
    private static bool _azureBlobStorageAppendContainerName;
    private static bool _isInitialized;
    private static string _azureBlobStorageConnectionString;
    private static string _azureBlobStorageContainerName;
    private static string _azureBlobStorageEndPoint;

    private readonly AzureBlobSettings _azureBlobSettings;
    private readonly INopFileProvider _fileProvider;
    private readonly IStaticCacheManager _staticCacheManager;

    private readonly object _locker = new();

    #endregion

    #region Ctor

    public AzureThumbService(AzureBlobSettings azureBlobSettings,
        INopFileProvider fileProvider,
        IStaticCacheManager staticCacheManager)
    {
		_logger.LogDebug(">>> Entering AzureThumbService(azureBlobSettings={AzureBlobSettings},fileProvider={FileProvider},staticCacheManager={StaticCacheManager})", azureBlobSettings, fileProvider, staticCacheManager);
        _azureBlobSettings = azureBlobSettings;
		_logger.LogDebug("AzureThumbService(azureBlobSettings={AzureBlobSettings},fileProvider={FileProvider},staticCacheManager={StaticCacheManager}): _azureBlobSettings → {_azureBlobSettings}", azureBlobSettings, fileProvider, staticCacheManager, _azureBlobSettings);
        _fileProvider = fileProvider;
		_logger.LogDebug("AzureThumbService(azureBlobSettings={AzureBlobSettings},fileProvider={FileProvider},staticCacheManager={StaticCacheManager}): _fileProvider → {_fileProvider}", azureBlobSettings, fileProvider, staticCacheManager, _fileProvider);
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize cloud container
    /// </summary>
    private void Init()
    {
		_logger.LogDebug(">>> Entering Init()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (_isInitialized)
            return;

        if (string.IsNullOrEmpty(_azureBlobSettings.ConnectionString))
			_logger.LogWarning("Throwing Exception from Init()");
            throw new Exception("Azure connection string for Blob is not specified");

        if (string.IsNullOrEmpty(_azureBlobSettings.ContainerName))
            throw new Exception("Azure container name for Blob is not specified");

        if (string.IsNullOrEmpty(_azureBlobSettings.EndPoint))
            throw new Exception("Azure end point for Blob is not specified");

        lock (_locker)
        {
            if (_isInitialized)
                return;

            _azureBlobStorageAppendContainerName = _azureBlobSettings.AppendContainerName;
			_logger.LogDebug("Init(): _azureBlobStorageAppendContainerName → {_azureBlobStorageAppendContainerName}", _azureBlobStorageAppendContainerName);
            _azureBlobStorageConnectionString = _azureBlobSettings.ConnectionString;
			_logger.LogDebug("Init(): _azureBlobStorageConnectionString → {_azureBlobStorageConnectionString}", _azureBlobStorageConnectionString);
            _azureBlobStorageContainerName = _azureBlobSettings.ContainerName.Trim().ToLowerInvariant();
            _azureBlobStorageEndPoint = _azureBlobSettings.EndPoint.Trim().ToLowerInvariant().TrimEnd('/');

			_logger.LogDebug("Init(): _azureBlobStorageContainerName → {_azureBlobStorageContainerName}", _azureBlobStorageContainerName);
            _blobServiceClient = new BlobServiceClient(_azureBlobStorageConnectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureBlobStorageContainerName);

			_logger.LogDebug("Init(): _azureBlobStorageEndPoint → {_azureBlobStorageEndPoint}", _azureBlobStorageEndPoint);
			_logger.LogDebug("Init(): _blobServiceClient → {_blobServiceClient}", _blobServiceClient);
			_logger.LogDebug("Init(): _blobContainerClient → {_blobContainerClient}", _blobContainerClient);
			_logger.LogInformation("Init(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
            _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob).GetAwaiter().GetResult();

            _isInitialized = true;
			_logger.LogDebug("Init(): _isInitialized → {_isInitialized}", _isInitialized);
        }
		_logger.LogDebug("<<< Exiting Init()");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public Task<string> GetThumbLocalPathByFileNameAsync(string thumbFileName)
    {
		_logger.LogDebug(">>> Entering GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName})", thumbFileName);
        Init();

        var path = _azureBlobStorageAppendContainerName ? $"{_azureBlobStorageContainerName}/" : string.Empty;
		_logger.LogDebug("GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName}): path → {Path}", thumbFileName, path);

		_logger.LogDebug("<<< Exiting GetThumbLocalPathByFileNameAsync(thumbFileName={ThumbFileName})", thumbFileName);
        return Task.FromResult($"{_azureBlobStorageEndPoint}/{path}{thumbFileName}");
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
		_logger.LogDebug(">>> Entering GetThumbUrlAsync(thumbFileName={ThumbFileName},storeLocation={StoreLocation})", thumbFileName, storeLocation);
        return await GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    /// <summary>
    /// Delete picture thumbs
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePictureThumbsAsync(Picture picture)
    {
		_logger.LogDebug(">>> Entering DeletePictureThumbsAsync(picture={Picture})", picture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        Init();

        //create a string containing the Blob name prefix
        var prefix = $"{picture.Id:0000000}";
		_logger.LogDebug("DeletePictureThumbsAsync(picture={Picture}): prefix → {Prefix}", picture, prefix);

        var tasks = await _blobContainerClient
            .GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix)
            .Select(blob => _blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots))
            .Select(dummy => (Task)dummy)
            .ToListAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("DeletePictureThumbsAsync(picture)={Picture}: database query executed in {Arg1} ms", picture, __logStartDb.ElapsedMilliseconds);
			_logger.LogInformation("DeletePictureThumbsAsync(picture)={Picture}: external service call took {Arg1} ms", picture, __logStart.ElapsedMilliseconds);
        await Task.WhenAll(tasks);

		_logger.LogDebug("<<< Exiting DeletePictureThumbsAsync(picture={Picture})", picture);
        await _staticCacheManager.RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
    }

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
		_logger.LogDebug(">>> Entering GetThumbLocalPathAsync(pictureUrl={PictureUrl})", pictureUrl);
        if (string.IsNullOrEmpty(pictureUrl))
            return string.Empty;

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
		_logger.LogDebug(">>> Entering GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName})", thumbFilePath, thumbFileName);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        Init();

        try
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(AzureBlobDefaults.ThumbExistsCacheKey, thumbFileName);
			_logger.LogWarning("GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName}): key → {Key}", thumbFilePath, thumbFileName, key);

            return await _staticCacheManager.GetAsync(key, async () => await _blobContainerClient.GetBlobClient(thumbFileName).ExistsAsync());
        }
        catch
        {
			_logger.LogError("Exception in GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName}): {Arg2}", thumbFilePath, thumbFileName, ex.Message);
			_logger.LogInformation("GeneratedThumbExistsAsync(thumbFilePath,thumbFileName)={ThumbFilePath},{ThumbFileName}: external service call took {Arg2} ms", thumbFilePath, thumbFileName, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting GeneratedThumbExistsAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName})", thumbFilePath, thumbFileName);
            return false;
        }
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
		_logger.LogDebug(">>> Entering SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary})", thumbFilePath, thumbFileName, mimeType, binary);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        Init();

        var blobClient = _blobContainerClient.GetBlobClient(thumbFileName);
		_logger.LogDebug("SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary}): blobClient → {BlobClient}", thumbFilePath, thumbFileName, mimeType, binary, blobClient);
        await using var ms = new MemoryStream(binary);
		_logger.LogDebug("SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary}): ms → {Ms}", thumbFilePath, thumbFileName, mimeType, binary, ms);

        //set mime type
        BlobHttpHeaders headers = null;
		_logger.LogDebug("SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary}): headers → {Headers}", thumbFilePath, thumbFileName, mimeType, binary, headers);
        if (!string.IsNullOrWhiteSpace(mimeType))
            headers = new BlobHttpHeaders { ContentType = mimeType };

        //set cache control
        if (!string.IsNullOrWhiteSpace(_azureBlobSettings.AzureCacheControlHeader))
        {
            headers ??= new BlobHttpHeaders();
            headers.CacheControl = _azureBlobSettings.AzureCacheControlHeader;
			_logger.LogDebug("SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary}): headers.CacheControl → {Arg4}", thumbFilePath, thumbFileName, mimeType, binary, headers.CacheControl);
        }

        if (headers is null)
            //we must explicitly indicate through the parameter that the object needs to be overwritten if it already exists
            //see more: https://github.com/Azure/azure-sdk-for-net/issues/9470
            await blobClient.UploadAsync(ms, overwrite: true);
        else
            await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = headers });

		_logger.LogDebug("<<< Exiting SaveThumbAsync(thumbFilePath={ThumbFilePath},thumbFileName={ThumbFileName},mimeType={MimeType},binary={Binary})", thumbFilePath, thumbFileName, mimeType, binary);
		_logger.LogInformation("SaveThumbAsync(thumbFilePath,thumbFileName,mimeType,binary)={ThumbFilePath},{ThumbFileName},{MimeType},{Binary}: external service call took {Arg4} ms", thumbFilePath, thumbFileName, mimeType, binary, __logStart.ElapsedMilliseconds);
        await _staticCacheManager.RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
    }

    #endregion
}