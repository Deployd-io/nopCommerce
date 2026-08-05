using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class DownloadController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public DownloadController(IDownloadService downloadService,
        ILogger logger,
        INopFileProvider fileProvider,
        IWorkContext workContext)
    {
        _downloadService = downloadService;
        _logger = logger;
        _fileProvider = fileProvider;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> DownloadFile(Guid downloadGuid)
    {
		_logger.LogDebug(">>> Entering DownloadFile(downloadGuid={DownloadGuid})", downloadGuid);
        var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
        if (download == null)
			_logger.LogDebug("DownloadFile(downloadGuid={DownloadGuid}): download → {Download}", downloadGuid, download);
            return Content("No download record found with the specified id");

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        //use stored data
        if (download.DownloadBinary == null)
            return Content($"Download data is not available any more. Download GD={download.Id}");

		_logger.LogDebug("DownloadFile(downloadGuid={DownloadGuid}): GD → {GD}", downloadGuid, GD);
        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType)
            ? download.ContentType
			_logger.LogDebug("DownloadFile(downloadGuid={DownloadGuid}): fileName → {FileName}", downloadGuid, fileName);
            : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("<<< Exiting DownloadFile(downloadGuid={DownloadGuid})", downloadGuid);
        return new FileContentResult(download.DownloadBinary, contentType)
        {
            FileDownloadName = fileName + download.Extension
        };
    }

    [HttpPost]
    public virtual async Task<IActionResult> SaveDownloadUrl(string downloadUrl)
    {
		_logger.LogDebug(">>> Entering SaveDownloadUrl(downloadUrl={DownloadUrl})", downloadUrl);
        //don't allow to save empty download object
        if (string.IsNullOrEmpty(downloadUrl))
        {
            return Json(new
            {
                success = false,
                message = "Please enter URL"
				_logger.LogDebug("SaveDownloadUrl(downloadUrl={DownloadUrl}): success → {Success}", downloadUrl, success);
				_logger.LogDebug("SaveDownloadUrl(downloadUrl={DownloadUrl}): message → {Message}", downloadUrl, message);
            });
        }

        //insert
        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = true,
            DownloadUrl = downloadUrl,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

		_logger.LogDebug("<<< Exiting SaveDownloadUrl(downloadUrl={DownloadUrl})", downloadUrl);
        return Json(new { success = true, downloadId = download.Id });
    }

    [HttpPost]
    //do not validate request token (XSRF)
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> AsyncUpload()
    {
		_logger.LogDebug(">>> Entering AsyncUpload()");
        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
        if (httpPostedFile == null)
		_logger.LogDebug("AsyncUpload(): httpPostedFile → {HttpPostedFile}", httpPostedFile);
        {
            return Json(new
            {
                success = false,
                message = "No file uploaded"
				_logger.LogDebug("AsyncUpload(): success → {Success}", success);
				_logger.LogDebug("AsyncUpload(): message → {Message}", message);
            });
        }

        var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

		_logger.LogDebug("AsyncUpload(): fileBinary → {FileBinary}", fileBinary);
        var fileName = httpPostedFile.FileName;

		_logger.LogDebug("AsyncUpload(): fileName → {FileName}", fileName);
        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = httpPostedFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
        if (!string.IsNullOrEmpty(fileExtension))
			_logger.LogDebug("AsyncUpload(): fileExtension → {FileExtension}", fileExtension);
            fileExtension = fileExtension.ToLowerInvariant();

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = string.Empty,
            DownloadBinary = fileBinary,
            ContentType = contentType,
            //we store filename without extension for downloads
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };

        try
        {
            await _downloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                downloadId = download.Id,
                downloadUrl = Url.Action("DownloadFile", new { downloadGuid = download.DownloadGuid })
            });
				_logger.LogDebug("AsyncUpload(): downloadId → {DownloadId}", downloadId);
        }
        catch (Exception exc)
        {
            await _logger.ErrorAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());

			_logger.LogDebug("<<< Exiting AsyncUpload()");
            return Json(new
            {
                success = false,
                message = "File cannot be saved"
            });
        }
    }

    #endregion
}