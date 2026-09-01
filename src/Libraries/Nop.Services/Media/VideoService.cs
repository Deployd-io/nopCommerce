using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Media;

/// <summary>
/// Video service
/// </summary>
public partial class VideoService : IVideoService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ProductVideo> _productVideoRepository;
    protected readonly IRepository<Video> _videoRepository;

    #endregion

    #region Ctor

    public VideoService(IRepository<ProductVideo> productVideoRepository,
        IRepository<Video> videoRepository)
    {
		_logger.LogDebug(">>> Entering VideoService(productVideoRepository={ProductVideoRepository},videoRepository={VideoRepository})", productVideoRepository, videoRepository);
        _productVideoRepository = productVideoRepository;
		_logger.LogDebug("VideoService(productVideoRepository={ProductVideoRepository},videoRepository={VideoRepository}): _productVideoRepository → {_productVideoRepository}", productVideoRepository, videoRepository, _productVideoRepository);
        _videoRepository = videoRepository;
    }

    #endregion

    #region CRUD methods

    /// <summary>
    /// Gets a video
    /// </summary>
    /// <param name="videoId">Video identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the video
    /// </returns>
    public virtual async Task<Video> GetVideoByIdAsync(int videoId)
    {
		_logger.LogDebug(">>> Entering GetVideoByIdAsync(videoId={VideoId})", videoId);
		_logger.LogInformation("GetVideoByIdAsync(videoId)={VideoId}: find query executing", videoId);
        return await _videoRepository.GetByIdAsync(videoId, cache => default);
    }

    /// <summary>
    /// Gets videos by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the videos
    /// </returns>
    public virtual async Task<IList<Video>> GetVideosByProductIdAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetVideosByProductIdAsync(productId={ProductId})", productId);
        if (productId == 0)
            return new List<Video>();

        var query = from v in _videoRepository.Table
            join pv in _productVideoRepository.Table on v.Id equals pv.VideoId
            orderby pv.DisplayOrder, pv.Id
            where pv.ProductId == productId
            select v;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var videos = await query.ToListAsync();

		_logger.LogDebug("GetVideosByProductIdAsync(productId={ProductId}): videos → {Videos}", productId, videos);
		_logger.LogInformation("GetVideosByProductIdAsync(productId)={ProductId}: database query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetVideosByProductIdAsync(productId={ProductId})", productId);
        return videos;
    }

    /// <summary>
    /// Inserts a video
    /// </summary>
    /// <param name="video">Video</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the video
    /// </returns>
    public virtual async Task<Video> InsertVideoAsync(Video video)
    {
		_logger.LogDebug(">>> Entering InsertVideoAsync(video={Video})", video);
        await _videoRepository.InsertAsync(video);
        return video;
    }

    /// <summary>
    /// Updates the video
    /// </summary>
    /// <param name="video">Video</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the video
    /// </returns>
    public virtual async Task<Video> UpdateVideoAsync(Video video)
    {
		_logger.LogDebug(">>> Entering UpdateVideoAsync(video={Video})", video);
        await _videoRepository.UpdateAsync(video);

        return video;
    }

    /// <summary>
    /// Deletes a video
    /// </summary>
    /// <param name="video">Video</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteVideoAsync(Video video)
    {
		_logger.LogDebug(">>> Entering DeleteVideoAsync(video={Video})", video);
		_logger.LogInformation("DeleteVideoAsync(video)={Video}: external service call took {Arg1} ms", video, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(video);

        await _videoRepository.DeleteAsync(video);
    }

    #endregion
}