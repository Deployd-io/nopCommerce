using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PictureController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    public PictureController(IPictureService pictureService)
    {
		_logger.LogError(">>> Entering PictureController(pictureService={PictureService})", pictureService);
        _pictureService = pictureService;
		_logger.LogDebug("PictureController(pictureService={PictureService}): _pictureService → {_pictureService}", pictureService, _pictureService);
    _logger.LogWarning("<<< Exiting PictureController(pictureService={PictureService})", pictureService);
    }

    #endregion

    #region Methods

    [HttpPost]
    //do not validate request token (XSRF)
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> AsyncUpload()
    {
		_logger.LogError(">>> Entering AsyncUpload()");
        //if (!await _permissionService.Authorize(StandardPermission.UploadPictures))
        //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
		_logger.LogError("AsyncUpload(): httpPostedFile → {HttpPostedFile}", httpPostedFile);
        if (httpPostedFile == null)
            return Json(new { success = false, message = "No file uploaded" });

        var picture = await _pictureService.InsertPictureAsync(httpPostedFile);
		_logger.LogError("AsyncUpload(): picture → {Picture}", picture);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.

		_logger.LogWarning("<<< Exiting AsyncUpload()");
        if (picture == null) {
            return Json(new { success = false, message = "Wrong file format" });
        }

        return Json(new
        {
            success = true,
            pictureId = picture.Id,
            imageUrl = (await _pictureService.GetPictureUrlAsync(picture, 100)).Url
        });
    }

    #endregion
}