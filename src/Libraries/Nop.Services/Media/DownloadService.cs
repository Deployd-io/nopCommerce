using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Media;

/// <summary>
/// Download service
/// </summary>
public partial class DownloadService : IDownloadService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<Download> _downloadRepository;

    #endregion

    #region Ctor

    public DownloadService(IRepository<Download> downloadRepository)
    {
		_logger.LogError(">>> Entering DownloadService(downloadRepository={DownloadRepository})", downloadRepository);
        _downloadRepository = downloadRepository;
		_logger.LogDebug("DownloadService(downloadRepository={DownloadRepository}): _downloadRepository → {_downloadRepository}", downloadRepository, _downloadRepository);
    _logger.LogWarning("<<< Exiting DownloadService(downloadRepository={DownloadRepository})", downloadRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a download
    /// </summary>
    /// <param name="downloadId">Download identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the download
    /// </returns>
    public virtual async Task<Download> GetDownloadByIdAsync(int downloadId)
    {
		_logger.LogError(">>> Entering GetDownloadByIdAsync(downloadId={DownloadId})", downloadId);
        _logger.LogWarning("<<< Exiting GetDownloadByIdAsync(downloadId={DownloadId})", downloadId);
        return await _downloadRepository.GetByIdAsync(downloadId);
    }

    /// <summary>
    /// Gets a download by GUID
    /// </summary>
    /// <param name="downloadGuid">Download GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the download
    /// </returns>
    public virtual async Task<Download> GetDownloadByGuidAsync(Guid downloadGuid)
    {
		_logger.LogError(">>> Entering GetDownloadByGuidAsync(downloadGuid={DownloadGuid})", downloadGuid);
        if (downloadGuid == Guid.Empty)
            return null;

        var query = from o in _downloadRepository.Table
            where o.DownloadGuid == downloadGuid
            select o;

		_logger.LogWarning("<<< Exiting GetDownloadByGuidAsync(downloadGuid={DownloadGuid})", downloadGuid);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Deletes a download
    /// </summary>
    /// <param name="download">Download</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDownloadAsync(Download download)
    {
		_logger.LogError(">>> Entering DeleteDownloadAsync(download={Download})", download);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _downloadRepository.DeleteAsync(download);
		_logger.LogError("DeleteDownloadAsync(download)={Download}: external service call took {Arg1} ms", download, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteDownloadAsync(download={Download})", download);
    }

    /// <summary>
    /// Inserts a download
    /// </summary>
    /// <param name="download">Download</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDownloadAsync(Download download)
    {
		_logger.LogError(">>> Entering InsertDownloadAsync(download={Download})", download);
        await _downloadRepository.InsertAsync(download);
		_logger.LogWarning("<<< Exiting InsertDownloadAsync(download={Download})", download);
    }

    /// <summary>
    /// Gets the download binary array
    /// </summary>
    /// <param name="file">File</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the download binary array
    /// </returns>
    public virtual async Task<byte[]> GetDownloadBitsAsync(IFormFile file)
    {
		_logger.LogError(">>> Entering GetDownloadBitsAsync(file={File})", file);
        await using var fileStream = file.OpenReadStream();
        await using var ms = new MemoryStream();
		_logger.LogError("GetDownloadBitsAsync(file={File}): ms → {Ms}", file, ms);
		_logger.LogError("GetDownloadBitsAsync(file={File}): fileStream → {FileStream}", file, fileStream);
        await fileStream.CopyToAsync(ms);
        var fileBytes = ms.ToArray();
		_logger.LogError("GetDownloadBitsAsync(file={File}): fileBytes → {FileBytes}", file, fileBytes);

		_logger.LogWarning("<<< Exiting GetDownloadBitsAsync(file={File})", file);
        return fileBytes;
    }

    #endregion
}