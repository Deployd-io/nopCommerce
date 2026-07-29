using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Services.Media;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PictureController : BaseAdminController
{
    #region Fields

    protected readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    public PictureController(IPictureService pictureService)
    {
		logger.LogDebug(">>> Entering PictureController(pictureService={PictureService})", pictureService);
        _pictureService = pictureService;
		logger.LogDebug("<<< Exiting PictureController(pictureService={PictureService})", pictureService);
		logger.LogDebug("PictureController(pictureService={PictureService}): _pictureService → {_pictureService}", pictureService, _pictureService);
    }

    #endregion

    #region Methods

    [HttpPost]
    //do not validate request token (XSRF)
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> AsyncUpload()
    {
		logger.LogDebug(">>> Entering AsyncUpload()");
        //if (!await _permissionService.Authorize(StandardPermission.UploadPictures))
        //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
        if (httpPostedFile == null)
			logger.LogDebug("AsyncUpload(): httpPostedFile → {HttpPostedFile}", httpPostedFile);
            return Json(new { success = false, message = "No file uploaded" });

        var picture = await _pictureService.InsertPictureAsync(httpPostedFile);

		logger.LogDebug("AsyncUpload(): picture → {Picture}", picture);
        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.

        if (picture == null)
            return Json(new { success = false, message = "Wrong file format" });

		logger.LogDebug("AsyncUpload(): success → {Success}", success);
		logger.LogDebug("<<< Exiting AsyncUpload()");
        return Json(new
        {
            success = true,
            pictureId = picture.Id,
            imageUrl = (await _pictureService.GetPictureUrlAsync(picture, 100)).Url
        });
    }

    #endregion
}