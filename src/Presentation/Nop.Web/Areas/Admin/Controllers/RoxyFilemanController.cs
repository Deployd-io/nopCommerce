using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;
//Controller for Roxy fileman (http://www.roxyfileman.com/) for Summernote editor
//the original file was \RoxyFileman-1.4.5-net\fileman\asp_net\main.ashx

//do not validate request token (XSRF)
public partial class RoxyFilemanController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly IRoxyFilemanService _roxyFilemanService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public RoxyFilemanController(IPermissionService permissionService, IRoxyFilemanService roxyFilemanService, IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering RoxyFilemanController(permissionService={PermissionService},roxyFilemanService={RoxyFilemanService},webHelper={WebHelper})", permissionService, roxyFilemanService, webHelper);
        _permissionService = permissionService;
		_logger.LogDebug("RoxyFilemanController(permissionService={PermissionService},roxyFilemanService={RoxyFilemanService},webHelper={WebHelper}): _permissionService → {_permissionService}", permissionService, roxyFilemanService, webHelper, _permissionService);
        _roxyFilemanService = roxyFilemanService;
		_logger.LogDebug("RoxyFilemanController(permissionService={PermissionService},roxyFilemanService={RoxyFilemanService},webHelper={WebHelper}): _roxyFilemanService → {_roxyFilemanService}", permissionService, roxyFilemanService, webHelper, _roxyFilemanService);
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    protected virtual JsonResult JsonOk()
    {
		_logger.LogDebug(">>> Entering JsonOk()");
        return Json(new { res = "ok" });
    }

    protected virtual JsonResult JsonError(string message)
    {
		_logger.LogDebug(">>> Entering JsonError(message={Message})", message);
        return Json(new { res = "error", msg = message });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create configuration file for RoxyFileman
    /// </summary>
    public virtual async Task CreateConfiguration()
    {
		_logger.LogDebug(">>> Entering CreateConfiguration()");
        var currentPathBase = Request.PathBase.ToString();
		_logger.LogDebug("<<< Exiting CreateConfiguration()");
		_logger.LogDebug("CreateConfiguration(): currentPathBase → {CurrentPathBase}", currentPathBase);
        await _roxyFilemanService.ConfigureAsync(currentPathBase);
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DirectoriesList(string type)
    {
		_logger.LogDebug(">>> Entering DirectoriesList(type={Type})", type);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from DirectoriesList(type={Type})", type);
                throw new Exception("You don't have required permission");

            var directories = _roxyFilemanService.GetDirectoryList(type);
			_logger.LogDebug("DirectoriesList(type={Type}): directories → {Directories}", type, directories);
            return Json(directories);
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in DirectoriesList(type={Type}): {Arg1}", type, ex.Message);
			_logger.LogDebug("<<< Exiting DirectoriesList(type={Type})", type);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> FilesList(string d, string type)
    {
		_logger.LogDebug(">>> Entering FilesList(d={D},type={Type})", d, type);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from FilesList(d={D},type={Type})", d, type);
                throw new Exception("You don't have required permission");

            var directories = _roxyFilemanService.GetFiles(d, type);
			_logger.LogDebug("FilesList(d={D},type={Type}): directories → {Directories}", d, type, directories);

            return Json(directories);
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in FilesList(d={D},type={Type}): {Arg2}", d, type, ex.Message);
			_logger.LogDebug("<<< Exiting FilesList(d={D},type={Type})", d, type);
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> DownloadFile(string f)
    {
		_logger.LogDebug(">>> Entering DownloadFile(f={F})", f);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from DownloadFile(f={F})", f);
                throw new Exception("You don't have required permission");

            var (stream, name) = _roxyFilemanService.GetFileStream(f);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(f, out var contentType))
                contentType = MimeTypes.ApplicationOctetStream;
				_logger.LogDebug("DownloadFile(f={F}): contentType → {ContentType}", f, contentType);

            return File(stream, contentType, name);
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in DownloadFile(f={F}): {Arg1}", f, ex.Message);
			_logger.LogDebug("<<< Exiting DownloadFile(f={F})", f);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CopyDirectory(string d, string n)
    {
		_logger.LogDebug(">>> Entering CopyDirectory(d={D},n={N})", d, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from CopyDirectory(d={D},n={N})", d, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CopyDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in CopyDirectory(d={D},n={N}): {Arg2}", d, n, ex.Message);
			_logger.LogDebug("<<< Exiting CopyDirectory(d={D},n={N})", d, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CopyFile(string f, string n)
    {
		_logger.LogDebug(">>> Entering CopyFile(f={F},n={N})", f, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from CopyFile(f={F},n={N})", f, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CopyFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in CopyFile(f={F},n={N}): {Arg2}", f, n, ex.Message);
			_logger.LogDebug("<<< Exiting CopyFile(f={F},n={N})", f, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CreateDirectory(string d, string n)
    {
		_logger.LogDebug(">>> Entering CreateDirectory(d={D},n={N})", d, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from CreateDirectory(d={D},n={N})", d, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CreateDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in CreateDirectory(d={D},n={N}): {Arg2}", d, n, ex.Message);
			_logger.LogDebug("<<< Exiting CreateDirectory(d={D},n={N})", d, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DeleteDirectory(string d)
    {
		_logger.LogDebug(">>> Entering DeleteDirectory(d={D})", d);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from DeleteDirectory(d={D})", d);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.DeleteDirectory(d);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in DeleteDirectory(d={D}): {Arg1}", d, ex.Message);
			_logger.LogDebug("<<< Exiting DeleteDirectory(d={D})", d);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DeleteFile(string f)
    {
		_logger.LogDebug(">>> Entering DeleteFile(f={F})", f);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from DeleteFile(f={F})", f);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.DeleteFile(f);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in DeleteFile(f={F}): {Arg1}", f, ex.Message);
			_logger.LogDebug("<<< Exiting DeleteFile(f={F})", f);
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> DownloadDirectory(string d)
    {
		_logger.LogDebug(">>> Entering DownloadDirectory(d={D})", d);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
			_logger.LogWarning("Throwing Exception from DownloadDirectory(d={D})", d);
            throw new Exception("You don't have required permission");

        var fileContents = _roxyFilemanService.DownloadDirectory(d);
		_logger.LogDebug("DownloadDirectory(d={D}): fileContents → {FileContents}", d, fileContents);

		_logger.LogDebug("<<< Exiting DownloadDirectory(d={D})", d);
        return File(fileContents, MimeTypes.ApplicationZip);
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> MoveDirectory(string d, string n)
    {
		_logger.LogDebug(">>> Entering MoveDirectory(d={D},n={N})", d, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from MoveDirectory(d={D},n={N})", d, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.MoveDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in MoveDirectory(d={D},n={N}): {Arg2}", d, n, ex.Message);
			_logger.LogDebug("<<< Exiting MoveDirectory(d={D},n={N})", d, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> MoveFile(string f, string n)
    {
		_logger.LogDebug(">>> Entering MoveFile(f={F},n={N})", f, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from MoveFile(f={F},n={N})", f, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.MoveFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in MoveFile(f={F},n={N}): {Arg2}", f, n, ex.Message);
			_logger.LogDebug("<<< Exiting MoveFile(f={F},n={N})", f, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RenameDirectory(string d, string n)
    {
		_logger.LogDebug(">>> Entering RenameDirectory(d={D},n={N})", d, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from RenameDirectory(d={D},n={N})", d, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.RenameDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in RenameDirectory(d={D},n={N}): {Arg2}", d, n, ex.Message);
			_logger.LogDebug("<<< Exiting RenameDirectory(d={D},n={N})", d, n);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RenameFile(string f, string n)
    {
		_logger.LogDebug(">>> Entering RenameFile(f={F},n={N})", f, n);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from RenameFile(f={F},n={N})", f, n);
                throw new Exception("You don't have required permission");

            _roxyFilemanService.RenameFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in RenameFile(f={F},n={N}): {Arg2}", f, n, ex.Message);
			_logger.LogDebug("<<< Exiting RenameFile(f={F},n={N})", f, n);
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> CreateImageThumbnail(string f)
    {
		_logger.LogDebug(">>> Entering CreateImageThumbnail(f={F})", f);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from CreateImageThumbnail(f={F})", f);
                throw new Exception("You don't have required permission");

            var provider = new FileExtensionContentTypeProvider();
			_logger.LogDebug("CreateImageThumbnail(f={F}): provider → {Provider}", f, provider);
            if (!provider.TryGetContentType(f, out var contentType))
                contentType = MimeTypes.ImageJpeg;
				_logger.LogDebug("CreateImageThumbnail(f={F}): contentType → {ContentType}", f, contentType);

            var fileContents = _roxyFilemanService.CreateImageThumbnail(f, contentType);
			_logger.LogDebug("CreateImageThumbnail(f={F}): fileContents → {FileContents}", f, fileContents);

            return File(fileContents, contentType);
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in CreateImageThumbnail(f={F}): {Arg1}", f, ex.Message);
			_logger.LogDebug("<<< Exiting CreateImageThumbnail(f={F})", f);
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    [HttpPost]
    public virtual async Task<IActionResult> UploadFiles([FromForm] RoxyFilemanUploadModel uploadModel)
    {
		_logger.LogDebug(">>> Entering UploadFiles(uploadModel={UploadModel})", uploadModel);
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
				_logger.LogWarning("Throwing Exception from UploadFiles(uploadModel={UploadModel})", uploadModel);
                throw new Exception("You don't have required permission");

            var form = await HttpContext.Request.ReadFormAsync();
			_logger.LogDebug("UploadFiles(uploadModel={UploadModel}): form → {Form}", uploadModel, form);

            if (!form.Files.Any())
				_logger.LogWarning("Throwing RoxyFilemanException from UploadFiles(uploadModel={UploadModel})", uploadModel);
                throw new RoxyFilemanException("E_UploadNoFiles");

            await _roxyFilemanService.UploadFilesAsync(uploadModel.D, form.Files);

            return JsonOk();
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in UploadFiles(uploadModel={UploadModel}): {Arg1}", uploadModel, ex.Message);
            if (!_webHelper.IsAjaxRequest(Request))
            {
                var roxyError = new { res = "error", msg = ex.Message };
				_logger.LogDebug("UploadFiles(uploadModel={UploadModel}): roxyError → {RoxyError}", uploadModel, roxyError);
			_logger.LogDebug("<<< Exiting UploadFiles(uploadModel={UploadModel})", uploadModel);
                return Content($"<script>parent.fileUploaded({JsonConvert.SerializeObject(roxyError)});</script>");
            }

            return JsonError(ex.Message);
        }
    }

    #endregion

}