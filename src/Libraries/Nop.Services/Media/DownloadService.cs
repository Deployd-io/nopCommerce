using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;
using Nop.Data;

namespace Nop.Services.Media;

/// <summary>
/// Download service
/// </summary>
public partial class DownloadService : IDownloadService
{
    #region Fields

    protected readonly IRepository<Download> _downloadRepository;

    #endregion

    #region Ctor

    public DownloadService(IRepository<Download> downloadRepository)
    {
		logger.LogDebug(">>> Entering DownloadService(downloadRepository={DownloadRepository})", downloadRepository);
        _downloadRepository = downloadRepository;
		logger.LogDebug("<<< Exiting DownloadService(downloadRepository={DownloadRepository})", downloadRepository);
		logger.LogDebug("DownloadService(downloadRepository={DownloadRepository}): _downloadRepository → {_downloadRepository}", downloadRepository, _downloadRepository);
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
		logger.LogDebug(">>> Entering GetDownloadByIdAsync(downloadId={DownloadId})", downloadId);
		logger.LogDebug("<<< Exiting GetDownloadByIdAsync(downloadId={DownloadId})", downloadId);
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
		logger.LogDebug(">>> Entering GetDownloadByGuidAsync(downloadGuid={DownloadGuid})", downloadGuid);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (downloadGuid == Guid.Empty)
            return null;

        var query = from o in _downloadRepository.Table
            where o.DownloadGuid == downloadGuid
            select o;

		logger.LogDebug("<<< Exiting GetDownloadByGuidAsync(downloadGuid={DownloadGuid})", downloadGuid);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Deletes a download
    /// </summary>
    /// <param name="download">Download</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDownloadAsync(Download download)
    {
		logger.LogDebug(">>> Entering DeleteDownloadAsync(download={Download})", download);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _downloadRepository.DeleteAsync(download);
		logger.LogDebug("<<< Exiting DeleteDownloadAsync(download={Download})", download);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteDownloadAsync(download)={Download}: external service call took {Arg1} ms", download, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteDownloadAsync(download)={Download}: delete query executed in {Arg1} ms", download, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteDownloadAsync(download)={Download}: external service call took {Arg1} ms", download, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteDownloadAsync(download)={Download}: delete query executed in {Arg1} ms", download, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Inserts a download
    /// </summary>
    /// <param name="download">Download</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDownloadAsync(Download download)
    {
		logger.LogDebug(">>> Entering InsertDownloadAsync(download={Download})", download);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _downloadRepository.InsertAsync(download);
		logger.LogDebug("<<< Exiting InsertDownloadAsync(download={Download})", download);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertDownloadAsync(download)={Download}: save query executed in {Arg1} ms", download, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertDownloadAsync(download)={Download}: save query executed in {Arg1} ms", download, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetDownloadBitsAsync(file={File})", file);
        await using var fileStream = file.OpenReadStream();
        await using var ms = new MemoryStream();
		logger.LogDebug("GetDownloadBitsAsync(file={File}): fileStream → {FileStream}", file, fileStream);
		logger.LogDebug("GetDownloadBitsAsync(file={File}): ms → {Ms}", file, ms);
        await fileStream.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

		logger.LogDebug("GetDownloadBitsAsync(file={File}): fileBytes → {FileBytes}", file, fileBytes);
		logger.LogDebug("<<< Exiting GetDownloadBitsAsync(file={File})", file);
        return fileBytes;
    }

    #endregion
}
