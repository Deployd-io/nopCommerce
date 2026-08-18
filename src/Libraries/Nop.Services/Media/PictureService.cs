using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Seo;
using SkiaSharp;
using Svg.Skia;
using Picture = Nop.Core.Domain.Media.Picture;

namespace Nop.Services.Media;

/// <summary>
/// Picture service
/// </summary>
public partial class PictureService : IPictureService
{
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRepository<PictureBinary> _pictureBinaryRepository;
    protected readonly IRepository<ProductPicture> _productPictureRepository;
    protected readonly ISettingService _settingService;
    protected readonly IThumbService _thumbService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public PictureService(IDownloadService downloadService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        INopFileProvider fileProvider,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IRepository<Picture> pictureRepository,
        IRepository<PictureBinary> pictureBinaryRepository,
        IRepository<ProductPicture> productPictureRepository,
        ISettingService settingService,
        IThumbService thumbService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
    {
        _downloadService = downloadService;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _downloadService → {_downloadService}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _downloadService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _httpContextAccessor → {_httpContextAccessor}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _httpContextAccessor);
        _logger = logger;
		_logger.LogDebug(">>> Entering PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings})", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings);
        _fileProvider = fileProvider;
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _fileProvider → {_fileProvider}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _fileProvider);
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _productAttributeParser → {_productAttributeParser}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _productAttributeParser);
        _productAttributeService = productAttributeService;
        _pictureRepository = pictureRepository;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _productAttributeService → {_productAttributeService}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _productAttributeService);
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _pictureRepository → {_pictureRepository}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _pictureRepository);
        _pictureBinaryRepository = pictureBinaryRepository;
        _productPictureRepository = productPictureRepository;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _pictureBinaryRepository → {_pictureBinaryRepository}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _pictureBinaryRepository);
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _productPictureRepository → {_productPictureRepository}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _productPictureRepository);
        _settingService = settingService;
        _thumbService = thumbService;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _settingService → {_settingService}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _settingService);
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _thumbService → {_thumbService}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _thumbService);
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _urlRecordService → {_urlRecordService}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _urlRecordService);
		_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _webHelper → {_webHelper}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _webHelper);
		_logger.LogDebug("<<< Exiting PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings})", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings);
        _mediaSettings = mediaSettings;
    }
	_logger.LogDebug("PictureService(downloadService={DownloadService},httpContextAccessor={HttpContextAccessor},logger={Logger},fileProvider={FileProvider},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},pictureRepository={PictureRepository},pictureBinaryRepository={PictureBinaryRepository},productPictureRepository={ProductPictureRepository},settingService={SettingService},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", downloadService, httpContextAccessor, logger, fileProvider, productAttributeParser, productAttributeService, pictureRepository, pictureBinaryRepository, productPictureRepository, settingService, thumbService, urlRecordService, webHelper, mediaSettings, _mediaSettings);

    #endregion

    #region Utilities

    /// <summary>
    /// Loads a picture from file
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="mimeType">MIME type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<byte[]> LoadPictureFromFileAsync(int pictureId, string mimeType)
    {
		_logger.LogDebug(">>> Entering LoadPictureFromFileAsync(pictureId={PictureId},mimeType={MimeType})", pictureId, mimeType);
        var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
		_logger.LogDebug("LoadPictureFromFileAsync(pictureId={PictureId},mimeType={MimeType}): fileName → {FileName}", pictureId, mimeType, fileName);
        var filePath = await GetPictureLocalPathAsync(fileName);
		_logger.LogDebug("LoadPictureFromFileAsync(pictureId={PictureId},mimeType={MimeType}): filePath → {FilePath}", pictureId, mimeType, filePath);
		_logger.LogDebug("LoadPictureFromFileAsync(pictureId={PictureId},mimeType={MimeType}): lastPart → {LastPart}", pictureId, mimeType, lastPart);

		_logger.LogDebug("<<< Exiting LoadPictureFromFileAsync(pictureId={PictureId},mimeType={MimeType})", pictureId, mimeType);
		_logger.LogDebug("<<< Exiting GetFileExtensionFromMimeTypeAsync(mimeType={MimeType})", mimeType);
        return await _fileProvider.ReadAllBytesAsync(filePath);
    }

    /// <summary>
    /// Save picture on file system
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SavePictureInFileAsync(int pictureId, byte[] pictureBinary, string mimeType)
    {
		_logger.LogDebug(">>> Entering SavePictureInFileAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType})", pictureId, pictureBinary, mimeType);
        var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
		_logger.LogDebug("SavePictureInFileAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType}): fileName → {FileName}", pictureId, pictureBinary, mimeType, fileName);
		_logger.LogDebug("SavePictureInFileAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType}): lastPart → {LastPart}", pictureId, pictureBinary, mimeType, lastPart);
        await _fileProvider.WriteAllBytesAsync(await GetPictureLocalPathAsync(fileName), pictureBinary);
    }

    /// <summary>
    /// Delete a picture on file system
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeletePictureOnFileSystemAsync(Picture picture)
    {
		_logger.LogDebug(">>> Entering DeletePictureOnFileSystemAsync(picture={Picture})", picture);
        ArgumentNullException.ThrowIfNull(picture);

        var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);
		_logger.LogDebug("DeletePictureOnFileSystemAsync(picture={Picture}): lastPart → {LastPart}", picture, lastPart);
        var fileName = $"{picture.Id:0000000}_0.{lastPart}";
		_logger.LogDebug("DeletePictureOnFileSystemAsync(picture={Picture}): fileName → {FileName}", picture, fileName);
        var filePath = await GetPictureLocalPathAsync(fileName);
		_logger.LogDebug("DeletePictureOnFileSystemAsync(picture={Picture}): filePath → {FilePath}", picture, filePath);
        _fileProvider.DeleteFile(filePath);
		_logger.LogDebug("<<< Exiting DeletePictureOnFileSystemAsync(picture={Picture})", picture);
		_logger.LogDebug(">>> Entering GetFileExtensionFromMimeTypeAsync(mimeType={MimeType})", mimeType);
    }

    /// <summary>
    /// Get images path URL 
    /// </summary>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    protected virtual Task<string> GetImagesPathUrlAsync(string storeLocation = null)
    {
		_logger.LogDebug(">>> Entering GetImagesPathUrlAsync(storeLocation={StoreLocation})", storeLocation);
        var pathBase = _httpContextAccessor.HttpContext?.Request?.PathBase.Value ?? string.Empty;
        var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storeLocation : $"{pathBase}/";
		_logger.LogDebug("GetImagesPathUrlAsync(storeLocation={StoreLocation}): imagesPathUrl → {ImagesPathUrl}", storeLocation, imagesPathUrl);
        imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl) ? _webHelper.GetStoreLocation() : imagesPathUrl;
        imagesPathUrl += "images/";

		_logger.LogDebug("<<< Exiting GetImagesPathUrlAsync(storeLocation={StoreLocation})", storeLocation);
		_logger.LogDebug(">>> Entering GetPictureLocalPathAsync(fileName={FileName})", fileName);
		_logger.LogDebug("<<< Exiting GetPictureLocalPathAsync(fileName={FileName})", fileName);
        return Task.FromResult(imagesPathUrl);
    }

    /// <summary>
    /// Get picture local path. Used when images stored on file system (not in the database)
    /// </summary>
    /// <param name="fileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture path
    /// </returns>
    protected virtual Task<string> GetPictureLocalPathAsync(string fileName)
    {
        return Task.FromResult(_fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), fileName));
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <param name="fromDb">Load from database; otherwise, from file system</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<byte[]> LoadPictureBinaryAsync(Picture picture, bool fromDb)
    {
		_logger.LogDebug(">>> Entering LoadPictureBinaryAsync(picture={Picture},fromDb={FromDb})", picture, fromDb);
        ArgumentNullException.ThrowIfNull(picture);

        var result = fromDb
            ? (await GetPictureBinaryByPictureIdAsync(picture.Id))?.BinaryData ?? Array.Empty<byte>()
            : await LoadPictureFromFileAsync(picture.Id, picture.MimeType);

		_logger.LogDebug("<<< Exiting LoadPictureBinaryAsync(picture={Picture},fromDb={FromDb})", picture, fromDb);
		_logger.LogDebug("<<< Exiting LoadPictureBinaryAsync(picture={Picture})", picture);
        return result;
    }

    /// <summary>
    /// Updates the picture binary data
    /// </summary>
    /// <param name="picture">The picture object</param>
    /// <param name="binaryData">The picture binary data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<PictureBinary> UpdatePictureBinaryAsync(Picture picture, byte[] binaryData)
    {
		_logger.LogDebug(">>> Entering UpdatePictureBinaryAsync(picture={Picture},binaryData={BinaryData})", picture, binaryData);
        ArgumentNullException.ThrowIfNull(picture);

        var pictureBinary = await GetPictureBinaryByPictureIdAsync(picture.Id);
		_logger.LogDebug("UpdatePictureBinaryAsync(picture={Picture},binaryData={BinaryData}): pictureBinary → {PictureBinary}", picture, binaryData, pictureBinary);

        var isNew = pictureBinary == null;

        if (isNew)
            pictureBinary = new PictureBinary
            {
                PictureId = picture.Id
            };

        pictureBinary.BinaryData = binaryData;
		_logger.LogDebug("UpdatePictureBinaryAsync(picture={Picture},binaryData={BinaryData}): pictureBinary.BinaryData → {Arg2}", picture, binaryData, pictureBinary.BinaryData);

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (isNew) {
			_logger.LogInformation("UpdatePictureBinaryAsync(picture,binaryData)={Picture},{BinaryData}: save query executed in {Arg2} ms", picture, binaryData, __logStart.ElapsedMilliseconds);
        }
			_logger.LogInformation("UpdatePictureBinaryAsync(picture,binaryData)={Picture},{BinaryData}: update query executed in {Arg2} ms", picture, binaryData, __logStart.ElapsedMilliseconds);
            await _pictureBinaryRepository.InsertAsync(pictureBinary);
        else
            await _pictureBinaryRepository.UpdateAsync(pictureBinary);

		_logger.LogDebug("<<< Exiting UpdatePictureBinaryAsync(picture={Picture},binaryData={BinaryData})", picture, binaryData);
		_logger.LogDebug(">>> Entering GetPictureBinaryByPictureIdAsync(pictureId={PictureId})", pictureId);
		_logger.LogDebug("<<< Exiting GetPictureBinaryByPictureIdAsync(pictureId={PictureId})", pictureId);
        return pictureBinary;
    }

    /// <summary>
    /// Get image format by mime type
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>SKEncodedImageFormat</returns>
    protected virtual SKEncodedImageFormat GetImageFormatByMimeType(string mimeType)
    {
		_logger.LogDebug(">>> Entering GetImageFormatByMimeType(mimeType={MimeType})", mimeType);
        var format = SKEncodedImageFormat.Jpeg;
        if (string.IsNullOrEmpty(mimeType))
			_logger.LogDebug("GetImageFormatByMimeType(mimeType={MimeType}): format → {Format}", mimeType, format);
            return format;

        var parts = mimeType.ToLowerInvariant().Split('/');
		_logger.LogDebug("GetImageFormatByMimeType(mimeType={MimeType}): parts → {Parts}", mimeType, parts);
        var lastPart = parts[^1];

        switch (lastPart)
        {
            case "webp":
                format = SKEncodedImageFormat.Webp;
                break;
            case "png":
            case "gif":
            case "bmp":
            case "x-icon":
                format = SKEncodedImageFormat.Png;
                break;
            default:
                break;
        }

		_logger.LogDebug("<<< Exiting GetImageFormatByMimeType(mimeType={MimeType})", mimeType);
        return format;
    }

    /// <summary>
    /// Gets the MIME type from the file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected virtual string GetMimeTypeFromFileName(string fileName)
    {
		_logger.LogDebug(">>> Entering GetMimeTypeFromFileName(fileName={FileName})", fileName);
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
		_logger.LogDebug("GetMimeTypeFromFileName(fileName={FileName}): provider → {Provider}", fileName, provider);
        {
            contentType = "application/octet-stream";
        }
		_logger.LogDebug("<<< Exiting GetMimeTypeFromFileName(fileName={FileName})", fileName);
        return contentType;
    }

    /// <summary>
    /// Resize image by targetSize
    /// </summary>
    /// <param name="image">Source image</param>
    /// <param name="format">Destination format</param>
    /// <param name="targetSize">Target size</param>
    /// <param name="encodedOrigin">Encoded origin</param>
    /// <returns>Image as array of byte[]</returns>
    protected virtual byte[] ImageResize(SKBitmap image, SKEncodedImageFormat format, int targetSize, SKEncodedOrigin? encodedOrigin = null)
    {
		_logger.LogDebug(">>> Entering ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin})", image, format, targetSize, encodedOrigin);
        ArgumentNullException.ThrowIfNull(image);

        if (encodedOrigin.HasValue)
            image = AutoOrient(image, encodedOrigin.Value);
			_logger.LogDebug("ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): image → {Image1}", image, format, targetSize, encodedOrigin, image);

        float width, height;
        if (image.Height > image.Width)
        {
            // portrait
            width = image.Width * (targetSize / (float)image.Height);
            height = targetSize;
        }
        else
		_logger.LogDebug("ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): height → {Height}", image, format, targetSize, encodedOrigin, height);
        {
            // landscape or square
            width = targetSize;
            height = image.Height * (targetSize / (float)image.Width);
        }

        if ((int)width == 0 || (int)height == 0)
        {
            width = image.Width;
            height = image.Height;
        }
        try
        {
            var samplingOption = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
			_logger.LogDebug("ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): samplingOption → {SamplingOption}", image, format, targetSize, encodedOrigin, samplingOption);
            using var resizedBitmap = image.Resize(new SKImageInfo((int)width, (int)height), samplingOption);
			_logger.LogDebug("ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): resizedBitmap → {ResizedBitmap}", image, format, targetSize, encodedOrigin, resizedBitmap);
            using var cropImage = SKImage.FromBitmap(resizedBitmap);
			_logger.LogDebug("ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): cropImage → {CropImage}", image, format, targetSize, encodedOrigin, cropImage);

            //In order to exclude saving pictures in low quality at the time of installation, we will set the value of this parameter to 80 (as by default)
            return cropImage.Encode(format, _mediaSettings.DefaultImageQuality > 0 ? _mediaSettings.DefaultImageQuality : 80).ToArray();
        }
        catch
        {
			_logger.LogError("Exception in ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin}): {Arg4}", image, format, targetSize, encodedOrigin, ex.Message);
			_logger.LogDebug("<<< Exiting ImageResize(image={Image},format={Format},targetSize={TargetSize},encodedOrigin={EncodedOrigin})", image, format, targetSize, encodedOrigin);
            return image.Bytes;
        }

    }

    /// <summary>
    /// Gets pictures
    /// </summary>
    /// <param name="pictureIds">Picture identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of pictures
    /// </returns>
    protected virtual async Task<IList<Picture>> GetPicturesByIdsAsync(int[] pictureIds)
    {
		_logger.LogDebug(">>> Entering GetPicturesByIdsAsync(pictureIds={PictureIds})", pictureIds);
		_logger.LogDebug("<<< Exiting GetPicturesByIdsAsync(pictureIds={PictureIds})", pictureIds);
        return await _pictureRepository.GetByIdsAsync(pictureIds, cache => default);
    }

    /// <summary>
    /// Automatically oriented image if needed
    /// </summary>
    /// <param name="bitmap">Image to reoriented</param>
    /// <param name="origin">Base image origin</param>
    /// <returns>Reoriented image</returns>
    protected virtual SKBitmap AutoOrient(SKBitmap bitmap, SKEncodedOrigin origin)
    {
		_logger.LogDebug(">>> Entering AutoOrient(bitmap={Bitmap},origin={Origin})", bitmap, origin);
        SKBitmap rotated;
        switch (origin)
        {
            case SKEncodedOrigin.BottomRight:
                using (var surface = new SKCanvas(bitmap))
                {
                    surface.RotateDegrees(180, bitmap.Width / 2f, bitmap.Height / 2f);
                    surface.DrawBitmap(bitmap.Copy(), 0, 0);
                }
                return bitmap;
            case SKEncodedOrigin.RightTop:
                rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                using (var surface = new SKCanvas(rotated))
                {
				_logger.LogDebug("AutoOrient(bitmap={Bitmap},origin={Origin}): rotated → {Rotated}", bitmap, origin, rotated);
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90);
                    surface.DrawBitmap(bitmap, 0, 0);
                }
                return rotated;
            case SKEncodedOrigin.LeftBottom:
                rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                using (var surface = new SKCanvas(rotated))
                {
                    surface.Translate(0, rotated.Height);
                    surface.RotateDegrees(270);
                    surface.DrawBitmap(bitmap, 0, 0);
                }
                return rotated;
            default:
				_logger.LogDebug("<<< Exiting AutoOrient(bitmap={Bitmap},origin={Origin})", bitmap, origin);
                return bitmap;
        }
    }

    #endregion

    #region Getting picture local path/URL methods

    /// <summary>
    /// Returns the file extension from mime type.
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file extension
    /// </returns>
    public virtual Task<string> GetFileExtensionFromMimeTypeAsync(string mimeType)
    {
        if (mimeType == null)
            return Task.FromResult<string>(null);

        var parts = mimeType.Split('/');
		_logger.LogDebug("GetFileExtensionFromMimeTypeAsync(mimeType={MimeType}): parts → {Parts}", mimeType, parts);
        var lastPart = parts[^1];
        lastPart = lastPart switch
        {
            "pjpeg" => "jpg",
            "jpeg" => "jpeg",
            "bmp" => "bmp",
            "gif" => "gif",
            "x-png" or "png" => "png",
            "tiff" => "tiff",
            "x-icon" => "ico",
            "webp" => "webp",
            "svg+xml" => "svg",
            _ => "",
        };
        return Task.FromResult(lastPart);
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    public virtual async Task<byte[]> LoadPictureBinaryAsync(Picture picture)
    {
        return await LoadPictureBinaryAsync(picture, await IsStoreInDbAsync());
    }

    /// <summary>
    /// Get picture SEO friendly name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<string> GetPictureSeNameAsync(string name)
    {
		_logger.LogDebug(">>> Entering GetPictureSeNameAsync(name={Name})", name);
		_logger.LogDebug("<<< Exiting GetPictureSeNameAsync(name={Name})", name);
        return await _urlRecordService.GetSeNameAsync(name, true, false);
    }

    /// <summary>
    /// Gets the default picture URL
    /// </summary>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<string> GetDefaultPictureUrlAsync(int targetSize = 0,
        PictureType defaultPictureType = PictureType.Entity,
        string storeLocation = null)
    {
		_logger.LogDebug(">>> Entering GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation})", targetSize, defaultPictureType, storeLocation);
        //get overridden default image if exists
        if (defaultPictureType == PictureType.Entity && _mediaSettings.ProductDefaultImageId > 0)
            return await GetPictureUrlAsync(_mediaSettings.ProductDefaultImageId, targetSize, false, storeLocation, PictureType.Entity);

        var defaultImageFileName = defaultPictureType switch
        {
            PictureType.Avatar => await _settingService.GetSettingByKeyAsync("Media.Customer.DefaultAvatarImageName", NopMediaDefaults.DefaultAvatarFileName),
            _ => await _settingService.GetSettingByKeyAsync("Media.DefaultImageName", NopMediaDefaults.DefaultImageFileName),
        };
        var filePath = await GetPictureLocalPathAsync(defaultImageFileName);
		_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): filePath → {FilePath}", targetSize, defaultPictureType, storeLocation, filePath);
        if (!_fileProvider.FileExists(filePath))
        {
            return string.Empty;
        }

        if (targetSize == 0)
            return await GetImagesPathUrlAsync(storeLocation) + defaultImageFileName;

        var fileExtension = _fileProvider.GetFileExtension(filePath);
		_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): fileExtension → {FileExtension}", targetSize, defaultPictureType, storeLocation, fileExtension);
        var thumbFileName = $"{_fileProvider.GetFileNameWithoutExtension(filePath)}_{targetSize}{fileExtension}";
		_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): thumbFileName → {ThumbFileName}", targetSize, defaultPictureType, storeLocation, thumbFileName);
        var thumbFilePath = await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
		_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): thumbFilePath → {ThumbFilePath}", targetSize, defaultPictureType, storeLocation, thumbFilePath);
        if (!await _thumbService.GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
        {
            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
			_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): mutex → {Mutex}", targetSize, defaultPictureType, storeLocation, mutex);
            mutex.WaitOne();
            try
            {
                using var image = SKBitmap.Decode(filePath);
				_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): image → {Image}", targetSize, defaultPictureType, storeLocation, image);
                using var codec = SKCodec.Create(filePath);
				_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): codec → {Codec}", targetSize, defaultPictureType, storeLocation, codec);
                var format = codec.EncodedFormat;
                var pictureBinary = ImageResize(image, format, targetSize);
				_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): pictureBinary → {PictureBinary}", targetSize, defaultPictureType, storeLocation, pictureBinary);
                var mimeType = GetMimeTypeFromFileName(thumbFileName);
				_logger.LogDebug("GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation}): mimeType → {MimeType}", targetSize, defaultPictureType, storeLocation, mimeType);
                _thumbService.SaveThumbAsync(thumbFilePath, thumbFileName, mimeType, pictureBinary).Wait();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

		_logger.LogDebug("<<< Exiting GetDefaultPictureUrlAsync(targetSize={TargetSize},defaultPictureType={DefaultPictureType},storeLocation={StoreLocation})", targetSize, defaultPictureType, storeLocation);
        return await _thumbService.GetThumbUrlAsync(thumbFileName, storeLocation);
    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<string> GetPictureUrlAsync(int pictureId,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity)
    {
		_logger.LogDebug(">>> Entering GetPictureUrlAsync(pictureId={PictureId},targetSize={TargetSize},showDefaultPicture={ShowDefaultPicture},storeLocation={StoreLocation},defaultPictureType={DefaultPictureType})", pictureId, targetSize, showDefaultPicture, storeLocation, defaultPictureType);
        var picture = await GetPictureByIdAsync(pictureId);
		_logger.LogDebug("GetPictureUrlAsync(pictureId={PictureId},targetSize={TargetSize},showDefaultPicture={ShowDefaultPicture},storeLocation={StoreLocation},defaultPictureType={DefaultPictureType}): picture → {Picture}", pictureId, targetSize, showDefaultPicture, storeLocation, defaultPictureType, picture);
		_logger.LogDebug("<<< Exiting GetPictureUrlAsync(pictureId={PictureId},targetSize={TargetSize},showDefaultPicture={ShowDefaultPicture},storeLocation={StoreLocation},defaultPictureType={DefaultPictureType})", pictureId, targetSize, showDefaultPicture, storeLocation, defaultPictureType);
        return (await GetPictureUrlAsync(picture, targetSize, showDefaultPicture, storeLocation, defaultPictureType)).Url;
    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="picture">Reference instance of Picture</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<(string Url, Picture Picture)> GetPictureUrlAsync(Picture picture,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity)
    {
        if (picture == null)
            return showDefaultPicture ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), null) : (string.Empty, (Picture)null);

        byte[] pictureBinary = null;
        if (picture.IsNew)
        {
            await _thumbService.DeletePictureThumbsAsync(picture);
            pictureBinary = await LoadPictureBinaryAsync(picture);

            if ((pictureBinary?.Length ?? 0) == 0)
                return showDefaultPicture ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), picture) : (string.Empty, picture);

            //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
            picture = await UpdatePictureAsync(picture.Id,
                pictureBinary,
                picture.MimeType,
                picture.SeoFilename,
                picture.AltAttribute,
                picture.TitleAttribute,
                false,
                false);
        }

        var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

        var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);

        var thumbFileName = !string.IsNullOrEmpty(seoFileName)
            ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
            : $"{picture.Id:0000000}.{lastPart}";

        //there is no need to resize the svg image as the browser will take care of it
        if (targetSize == 0 || picture.MimeType == MimeTypes.ImageSvg)
        {
            var thumbFilePath = await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
            if (await _thumbService.GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                return (await _thumbService.GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
                _thumbService.SaveThumbAsync(thumbFilePath, thumbFileName, picture.MimeType, pictureBinary).Wait();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        else
        {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                : $"{picture.Id:0000000}_{targetSize}.{lastPart}";

            var thumbFilePath = await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
            if (await _thumbService.GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                return (await _thumbService.GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            if (pictureBinary == null)
                return (await _thumbService.GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();

            try
            {
                var format = GetImageFormatByMimeType(picture.MimeType);

                if (_mediaSettings.AutoOrientImage)
                {
                    using var stream = new MemoryStream(pictureBinary);
                    using var inputStream = new SKManagedStream(stream);
                    using var codec = SKCodec.Create(inputStream);
                    using var image = SKBitmap.Decode(codec);
                    pictureBinary = ImageResize(image, format, targetSize, codec.EncodedOrigin);
                }
                else
                {
                    using var image = SKBitmap.Decode(pictureBinary);
                    pictureBinary = ImageResize(image, format, targetSize);
                }

                _thumbService.SaveThumbAsync(thumbFilePath, thumbFileName, picture.MimeType, pictureBinary).Wait();
            }
            catch
            {
                // ignored
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

		_logger.LogDebug(">>> Entering GetPictureByIdAsync(pictureId={PictureId})", pictureId);
		_logger.LogDebug("<<< Exiting GetPictureByIdAsync(pictureId={PictureId})", pictureId);
        return (await _thumbService.GetThumbUrlAsync(thumbFileName, storeLocation), picture);
    }

    #endregion

    #region Convertation methods

    /// <summary>
    /// Convert image from SVG format to PNG
    /// </summary>
    /// <param name="stream">Stream for SVG file</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the byte array</returns>
    public virtual Task<byte[]> ConvertSvgToPngAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ConvertSvgToPngAsync(stream={Stream})", stream);
        try
        {
            using var svg = new SKSvg();
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): svg → {Svg}", stream, svg);
            svg.Load(stream);

            using var bitmap = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): bitmap → {Bitmap}", stream, bitmap);
            var canvas = new SKCanvas(bitmap);
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): canvas → {Canvas}", stream, canvas);
            canvas.DrawPicture(svg.Picture);
            canvas.Flush();
            canvas.Save();

            using var image = SKImage.FromBitmap(bitmap);
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): image → {Image}", stream, image);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): data → {Data}", stream, data);

            // save the data to a stream
            using var memStream = new MemoryStream();
			_logger.LogDebug("ConvertSvgToPngAsync(stream={Stream}): memStream → {MemStream}", stream, memStream);
            data.SaveTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult(memStream.ToArray());
        }
        catch
        {
		_logger.LogError("Exception in ConvertSvgToPngAsync(stream={Stream}): {Arg1}", stream, ex.Message);
        }

		_logger.LogDebug("<<< Exiting ConvertSvgToPngAsync(stream={Stream})", stream);
        return null;
    }

    #endregion

    #region CRUD methods

    /// <summary>
    /// Gets a picture
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> GetPictureByIdAsync(int pictureId)
    {
        return await _pictureRepository.GetByIdAsync(pictureId, cache => default);
    }

    /// <summary>
    /// Deletes a picture
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePictureAsync(Picture picture)
    {
		_logger.LogDebug(">>> Entering DeletePictureAsync(picture={Picture})", picture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(picture);

        //delete thumbs
        await _thumbService.DeletePictureThumbsAsync(picture);

        //delete from file system
        if (!await IsStoreInDbAsync())
            await DeletePictureOnFileSystemAsync(picture);

        //delete from database
        await _pictureRepository.DeleteAsync(picture);
		_logger.LogInformation("DeletePictureAsync(picture)={Picture}: external service call took {Arg1} ms", picture, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeletePictureAsync(picture={Picture})", picture);
    }

    /// <summary>
    /// Gets a collection of pictures
    /// </summary>
    /// <param name="virtualPath">Virtual path</param>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Items on each page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of pictures
    /// </returns>
    public virtual async Task<IPagedList<Picture>> GetPicturesAsync(string virtualPath = "", int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetPicturesAsync(virtualPath={VirtualPath},pageIndex={PageIndex},pageSize={PageSize})", virtualPath, pageIndex, pageSize);
        var query = _pictureRepository.Table;

		_logger.LogDebug("GetPicturesAsync(virtualPath={VirtualPath},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", virtualPath, pageIndex, pageSize, query);
        if (!string.IsNullOrEmpty(virtualPath))
            query = virtualPath.EndsWith('/') ? query.Where(p => p.VirtualPath.StartsWith(virtualPath) || p.VirtualPath == virtualPath.TrimEnd('/')) : query.Where(p => p.VirtualPath == virtualPath);

        query = query.OrderByDescending(p => p.Id);

		_logger.LogDebug("<<< Exiting GetPicturesAsync(virtualPath={VirtualPath},pageIndex={PageIndex},pageSize={PageSize})", virtualPath, pageIndex, pageSize);
		_logger.LogDebug(">>> Entering IsStoreInDbAsync()");
		_logger.LogDebug("<<< Exiting IsStoreInDbAsync()");
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets pictures by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pictures
    /// </returns>
    public virtual async Task<IList<Picture>> GetPicturesByProductIdAsync(int productId, int recordsToReturn = 0)
    {
		_logger.LogDebug(">>> Entering GetPicturesByProductIdAsync(productId={ProductId},recordsToReturn={RecordsToReturn})", productId, recordsToReturn);
        if (productId == 0)
            return new List<Picture>();

        var query = from p in _pictureRepository.Table
					_logger.LogDebug("GetPicturesByProductIdAsync(productId={ProductId},recordsToReturn={RecordsToReturn}): query → {Query}", productId, recordsToReturn, query);
                    join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
                    orderby pp.DisplayOrder, pp.Id
                    where pp.ProductId == productId
                    select p;

        if (recordsToReturn > 0)
            query = query.Take(recordsToReturn);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var pics = await query.ToListAsync();

		_logger.LogDebug("GetPicturesByProductIdAsync(productId={ProductId},recordsToReturn={RecordsToReturn}): pics → {Pics}", productId, recordsToReturn, pics);
		_logger.LogInformation("GetPicturesByProductIdAsync(productId,recordsToReturn)={ProductId},{RecordsToReturn}: database query executed in {Arg2} ms", productId, recordsToReturn, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetPicturesByProductIdAsync(productId={ProductId},recordsToReturn={RecordsToReturn})", productId, recordsToReturn);
        return pics;
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="pictureBinary">The picture binary</param>
    /// <param name="mimeType">The picture MIME type</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
    /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
    /// <param name="isNew">A value indicating whether the picture is new</param>
    /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> InsertPictureAsync(byte[] pictureBinary, string mimeType, string seoFilename,
        string altAttribute = null, string titleAttribute = null,
        bool isNew = true, bool validateBinary = true)
    {
		_logger.LogDebug(">>> Entering InsertPictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary})", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary);
        mimeType = CommonHelper.EnsureNotNull(mimeType);
        mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);
		_logger.LogDebug("InsertPictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): mimeType → {MimeType1}", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, mimeType);

        seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);
		_logger.LogDebug("InsertPictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): seoFilename → {SeoFilename1}", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, seoFilename);

        if (validateBinary)
            pictureBinary = await ValidatePictureAsync(pictureBinary, mimeType, seoFilename);
			_logger.LogDebug("InsertPictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): pictureBinary → {PictureBinary1}", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, pictureBinary);

        var picture = new Picture
        {
            MimeType = mimeType,
            SeoFilename = seoFilename,
            AltAttribute = altAttribute,
            TitleAttribute = titleAttribute,
            IsNew = isNew
			_logger.LogDebug(">>> Entering ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName})", pictureBinary, mimeType, fileName);
			_logger.LogDebug("ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName}): codec → {Codec}", pictureBinary, mimeType, fileName, codec);
			_logger.LogDebug("ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName}): image → {Image}", pictureBinary, mimeType, fileName, image);
			_logger.LogDebug("ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName}): pictureBinary → {PictureBinary1}", pictureBinary, mimeType, fileName, pictureBinary);
			_logger.LogDebug("<<< Exiting ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName})", pictureBinary, mimeType, fileName);
        };
        await _pictureRepository.InsertAsync(picture);
		_logger.LogInformation("InsertPictureAsync(pictureBinary,mimeType,seoFilename,altAttribute,titleAttribute,isNew,validateBinary)={PictureBinary},{MimeType},{SeoFilename},{AltAttribute},{TitleAttribute},{IsNew},{ValidateBinary}: save query executed in {Arg7} ms", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, __logStart.ElapsedMilliseconds);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? pictureBinary : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, pictureBinary, mimeType);

		_logger.LogDebug("<<< Exiting InsertPictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary})", pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary);
        return picture;
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="formFile">Form file</param>
    /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
    /// <param name="virtualPath">Virtual path</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> InsertPictureAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "")
    {
		_logger.LogDebug(">>> Entering InsertPictureAsync(formFile={FormFile},defaultFileName={DefaultFileName},virtualPath={VirtualPath})", formFile, defaultFileName, virtualPath);
        var imgExt = new List<string>
        {
            ".bmp",
            ".gif",
            ".webp",
            ".jpeg",
            ".jpg",
            ".jpe",
            ".jfif",
            ".pjpeg",
            ".pjp",
            ".png",
            ".tiff",
            ".tif",
            ".svg"
        } as IReadOnlyCollection<string>;

        var fileName = formFile.FileName;
		_logger.LogDebug("InsertPictureAsync(formFile={FormFile},defaultFileName={DefaultFileName},virtualPath={VirtualPath}): fileName → {FileName}", formFile, defaultFileName, virtualPath, fileName);
        if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(defaultFileName))
            fileName = defaultFileName;

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = formFile.ContentType;
		_logger.LogDebug("InsertPictureAsync(formFile={FormFile},defaultFileName={DefaultFileName},virtualPath={VirtualPath}): contentType → {ContentType}", formFile, defaultFileName, virtualPath, contentType);

        var fileExtension = _fileProvider.GetFileExtension(fileName);
		_logger.LogDebug("InsertPictureAsync(formFile={FormFile},defaultFileName={DefaultFileName},virtualPath={VirtualPath}): fileExtension → {FileExtension}", formFile, defaultFileName, virtualPath, fileExtension);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        if (imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
            return null;

        //contentType is not always available 
        //that's why we manually update it here
        //https://mimetype.io/all-types/
        if (string.IsNullOrEmpty(contentType))
            contentType = GetPictureContentTypeByFileExtension(fileExtension);

        if (contentType == MimeTypes.ImageSvg && !_mediaSettings.AllowSvgUploads)
            return null;

        var picture = await InsertPictureAsync(await _downloadService.GetDownloadBitsAsync(formFile),
            contentType,
            _fileProvider.GetFileNameWithoutExtension(fileName),
            validateBinary: contentType != MimeTypes.ImageSvg);

        if (string.IsNullOrEmpty(virtualPath))
            return picture;

        picture.VirtualPath = _fileProvider.GetVirtualPath(virtualPath);
        await UpdatePictureAsync(picture);

        return picture;
    }

    /// <summary>
    /// Updates the picture
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <param name="pictureBinary">The picture binary</param>
    /// <param name="mimeType">The picture MIME type</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
    /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
    /// <param name="isNew">A value indicating whether the picture is new</param>
    /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> UpdatePictureAsync(int pictureId, byte[] pictureBinary, string mimeType,
        string seoFilename, string altAttribute = null, string titleAttribute = null,
        bool isNew = true, bool validateBinary = true)
    {
        mimeType = CommonHelper.EnsureNotNull(mimeType);
        mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

        seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): seoFilename → {SeoFilename1}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, seoFilename);

        if (validateBinary)
            pictureBinary = await ValidatePictureAsync(pictureBinary, mimeType, seoFilename);
			_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): pictureBinary → {PictureBinary1}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, pictureBinary);

        var picture = await GetPictureByIdAsync(pictureId);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture → {Picture}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture);
        if (picture == null)
            return null;

        //delete old thumbs if a picture has been changed
        if (seoFilename != picture.SeoFilename)
            await _thumbService.DeletePictureThumbsAsync(picture);

        picture.MimeType = mimeType;
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture.MimeType → {Arg8}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture.MimeType);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture.SeoFilename → {Arg8}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture.SeoFilename);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture.AltAttribute → {Arg8}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture.AltAttribute);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture.TitleAttribute → {Arg8}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture.TitleAttribute);
		_logger.LogDebug("UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary}): picture.IsNew → {Arg8}", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, picture.IsNew);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        picture.SeoFilename = seoFilename;
        picture.AltAttribute = altAttribute;
        picture.TitleAttribute = titleAttribute;
        picture.IsNew = isNew;

        await _pictureRepository.UpdateAsync(picture);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? pictureBinary : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, pictureBinary, mimeType);

        return picture;
    }

    /// <summary>
    /// Updates the picture
    /// </summary>
    /// <param name="picture">The picture to update</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> UpdatePictureAsync(Picture picture)
    {
		_logger.LogDebug(">>> Entering UpdatePictureAsync(picture={Picture})", picture);
        if (picture == null)
            return null;

        var seoFilename = CommonHelper.EnsureMaximumLength(picture.SeoFilename, 100);
		_logger.LogDebug("UpdatePictureAsync(picture={Picture}): seoFilename → {SeoFilename}", picture, seoFilename);

        //delete old thumbs if exists
        await _thumbService.DeletePictureThumbsAsync(picture);

        picture.SeoFilename = seoFilename;
		_logger.LogDebug("UpdatePictureAsync(picture={Picture}): picture.SeoFilename → {Arg1}", picture, picture.SeoFilename);
		_logger.LogInformation("UpdatePictureAsync(picture)={Picture}: update query executed in {Arg1} ms", picture, __logStart.ElapsedMilliseconds);

        await _pictureRepository.UpdateAsync(picture);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? (await GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, (await GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData, picture.MimeType);

		_logger.LogDebug("<<< Exiting UpdatePictureAsync(picture={Picture})", picture);
		_logger.LogInformation("UpdatePictureAsync(pictureId,pictureBinary,mimeType,seoFilename,altAttribute,titleAttribute,isNew,validateBinary)={PictureId},{PictureBinary},{MimeType},{SeoFilename},{AltAttribute},{TitleAttribute},{IsNew},{ValidateBinary}: update query executed in {Arg8} ms", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdatePictureAsync(pictureId={PictureId},pictureBinary={PictureBinary},mimeType={MimeType},seoFilename={SeoFilename},altAttribute={AltAttribute},titleAttribute={TitleAttribute},isNew={IsNew},validateBinary={ValidateBinary})", pictureId, pictureBinary, mimeType, seoFilename, altAttribute, titleAttribute, isNew, validateBinary);
        return picture;
    }

    /// <summary>
    /// Get product picture binary by picture identifier
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    public virtual async Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId)
    {
        return await _pictureBinaryRepository.Table
            .FirstOrDefaultAsync(pb => pb.PictureId == pictureId);
    }

    /// <summary>
    /// Updates a SEO filename of a picture
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> SetSeoFilenameAsync(int pictureId, string seoFilename)
    {
		_logger.LogDebug(">>> Entering SetSeoFilenameAsync(pictureId={PictureId},seoFilename={SeoFilename})", pictureId, seoFilename);
        var picture = await GetPictureByIdAsync(pictureId) ?? throw new ArgumentException("No picture found with the specified id");

		_logger.LogDebug("SetSeoFilenameAsync(pictureId={PictureId},seoFilename={SeoFilename}): picture → {Picture}", pictureId, seoFilename, picture);
        //update if it has been changed
        if (seoFilename != picture.SeoFilename)
        {
            //update picture
            picture = await UpdatePictureAsync(picture.Id,
                await LoadPictureBinaryAsync(picture),
                picture.MimeType,
                seoFilename,
                picture.AltAttribute,
                picture.TitleAttribute,
                true,
                false);
        }

		_logger.LogDebug("<<< Exiting SetSeoFilenameAsync(pictureId={PictureId},seoFilename={SeoFilename})", pictureId, seoFilename);
        return picture;
    }

    /// <summary>
    /// Validates input picture dimensions
    /// </summary>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="fileName">Name of file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary or throws an exception
    /// </returns>
    public virtual async Task<byte[]> ValidatePictureAsync(byte[] pictureBinary, string mimeType, string fileName)
    {
		_logger.LogDebug(">>> Entering ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName})", pictureBinary, mimeType, fileName);
        try
        {
            SKBitmap image;

            if (_mediaSettings.AutoOrientImage)
            {
                using var input = new MemoryStream(pictureBinary);
				_logger.LogDebug("ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName}): input → {Input}", pictureBinary, mimeType, fileName, input);
                using var codec = SKCodec.Create(input);
                image = AutoOrient(SKBitmap.Decode(codec), codec.EncodedOrigin);
            }
            else
                image = SKBitmap.Decode(pictureBinary);

            //resize the image in accordance with the maximum size
            if (Math.Max(image.Height, image.Width) <= _mediaSettings.MaximumImageSize)
                return pictureBinary;

            var format = GetImageFormatByMimeType(mimeType);
			_logger.LogDebug("ValidatePictureAsync(pictureBinary={PictureBinary},mimeType={MimeType},fileName={FileName}): format → {Format}", pictureBinary, mimeType, fileName, format);
            pictureBinary = ImageResize(image, format, _mediaSettings.MaximumImageSize);

            return pictureBinary;
        }
        catch (Exception exc)
        {
            await _logger.ErrorAsync($"Cannot decode picture binary (file name: {fileName})", exc);
            return pictureBinary;
        }
    }

    /// <summary>
    /// Get product picture (for shopping cart and order details pages)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes (in XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> GetProductPictureAsync(Product product, string attributesXml)
    {
		_logger.LogDebug(">>> Entering GetProductPictureAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        //first, try to get product attribute combination picture
        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
		_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): combination → {Combination}", product, attributesXml, combination);
        if (combination != null)
        {
            var combinationPicture = (await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id)).FirstOrDefault();
			_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): combinationPicture → {CombinationPicture}", product, attributesXml, combinationPicture);
            if (await GetPictureByIdAsync(combinationPicture?.PictureId ?? 0) is Picture picture)
                return picture;
        }

        //then, let's see whether we have attribute values with pictures
        var values = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
		_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): values → {Values}", product, attributesXml, values);
        foreach (var attributeValue in values)
        {
            var valuePictures = await _productAttributeService.GetProductAttributeValuePicturesAsync(attributeValue.Id);
			_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): valuePictures → {ValuePictures}", product, attributesXml, valuePictures);
            var attributePicture = (await GetPicturesByIdsAsync(valuePictures.Select(vp => vp.PictureId).ToArray())).FirstOrDefault();
			_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): attributePicture → {AttributePicture}", product, attributesXml, attributePicture);

            if (attributePicture != null)
                return attributePicture;
        }

        //now let's load the default product picture
        var productPicture = (await GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
		_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): productPicture → {ProductPicture}", product, attributesXml, productPicture);
        if (productPicture != null)
            return productPicture;

        //finally, let's check whether this product has some parent "grouped" product
        if (product.VisibleIndividually || product.ParentGroupedProductId <= 0)
            return null;

        var parentGroupedProductPicture = (await GetPicturesByProductIdAsync(product.ParentGroupedProductId, 1)).FirstOrDefault();
		_logger.LogDebug("GetProductPictureAsync(product={Product},attributesXml={AttributesXml}): parentGroupedProductPicture → {ParentGroupedProductPicture}", product, attributesXml, parentGroupedProductPicture);
		_logger.LogDebug("<<< Exiting GetProductPictureAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return parentGroupedProductPicture;
    }

    /// <summary>
    /// Gets a value indicating whether the images should be stored in data base.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<bool> IsStoreInDbAsync()
    {
        return await _settingService.GetSettingByKeyAsync("Media.Images.StoreInDB", true);
    }

    /// <summary>
    /// Sets a value indicating whether the images should be stored in data base
    /// </summary>
    /// <param name="isStoreInDb">A value indicating whether the images should be stored in data base</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetIsStoreInDbAsync(bool isStoreInDb)
    {
		_logger.LogDebug(">>> Entering SetIsStoreInDbAsync(isStoreInDb={IsStoreInDb})", isStoreInDb);
        //check whether it's a new value
        if (await IsStoreInDbAsync() == isStoreInDb)
            return;

        //save the new setting value
        await _settingService.SetSettingAsync("Media.Images.StoreInDB", isStoreInDb);

        var pageIndex = 0;
        const int pageSize = 400;
        try
        {
            while (true)
            {
                var pictures = await GetPicturesAsync(pageIndex: pageIndex, pageSize: pageSize);
				_logger.LogDebug("SetIsStoreInDbAsync(isStoreInDb={IsStoreInDb}): pictures → {Pictures}", isStoreInDb, pictures);
                pageIndex++;

                //all pictures converted?
                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (!string.IsNullOrEmpty(picture.VirtualPath))
                        continue;

                    var pictureBinary = await LoadPictureBinaryAsync(picture, !isStoreInDb);
					_logger.LogDebug("SetIsStoreInDbAsync(isStoreInDb={IsStoreInDb}): pictureBinary → {PictureBinary}", isStoreInDb, pictureBinary);

                    //we used the code below before. but it's too slow
                    //let's do it manually (uncommented code) - copy some logic from "UpdatePicture" method
                    /*just update a picture (all required logic is in "UpdatePicture" method)
                    we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    UpdatePicture(picture.Id,
                                  pictureBinary,
                                  picture.MimeType,
                                  picture.SeoFilename,
                                  true,
                                  false);*/
                    if (isStoreInDb)
                        //delete from file system. now it's in the database
                        await DeletePictureOnFileSystemAsync(picture);
                    else
                        //now on file system
                        await SavePictureInFileAsync(picture.Id, pictureBinary, picture.MimeType);
                    //update appropriate properties
                    await UpdatePictureBinaryAsync(picture, isStoreInDb ? pictureBinary : Array.Empty<byte>());
                    picture.IsNew = true;
                }

                //save all at once
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogInformation("SetIsStoreInDbAsync(isStoreInDb)={IsStoreInDb}: update query executed in {Arg1} ms", isStoreInDb, __logStart.ElapsedMilliseconds);
                await _pictureRepository.UpdateAsync(pictures, false);
            }
        }
        catch
        {
			_logger.LogError("Exception in SetIsStoreInDbAsync(isStoreInDb={IsStoreInDb}): {Arg1}", isStoreInDb, exc.Message);
            // ignored
        }
		_logger.LogDebug("<<< Exiting SetIsStoreInDbAsync(isStoreInDb={IsStoreInDb})", isStoreInDb);
    }


    /// <summary>
    /// Change path to store pictures
    /// </summary>
    /// <param name="path">New path</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ChangePicturesPathAsync(string path)
    {
		_logger.LogDebug(">>> Entering ChangePicturesPathAsync(path={Path})", path);
        var newPath = _fileProvider.GetLocalImagesPath(_mediaSettings, path);
        _fileProvider.CreateDirectory(newPath);
		_logger.LogDebug("ChangePicturesPathAsync(path={Path}): newPath → {NewPath}", path, newPath);

        var oldPath = _fileProvider.GetLocalImagesPath(_mediaSettings);
		_logger.LogDebug("ChangePicturesPathAsync(path={Path}): oldPath → {OldPath}", path, oldPath);
        if (newPath.Equals(oldPath))
            return;

        var isSubDir = newPath.StartsWith(oldPath);
		_logger.LogDebug("ChangePicturesPathAsync(path={Path}): isSubDir → {IsSubDir}", path, isSubDir);

        var directoriesToDelete = new List<string>();
		_logger.LogDebug("ChangePicturesPathAsync(path={Path}): directoriesToDelete → {DirectoriesToDelete}", path, directoriesToDelete);
        newPath = newPath.TrimEnd('/').TrimEnd('\\');

        foreach (var originalFile in _fileProvider.EnumerateFiles(oldPath, "*.*", false))
        {
            if (isSubDir && originalFile.StartsWith(newPath))
                continue;

            var fileName = originalFile.Replace(oldPath, string.Empty);
			_logger.LogDebug("ChangePicturesPathAsync(path={Path}): fileName → {FileName}", path, fileName);
            var newFilePath = _fileProvider.Combine(newPath, fileName);
			_logger.LogDebug("ChangePicturesPathAsync(path={Path}): newFilePath → {NewFilePath}", path, newFilePath);
            var newDirPath = _fileProvider.GetParentDirectory(newFilePath).TrimEnd('/').TrimEnd('\\');
			_logger.LogDebug("ChangePicturesPathAsync(path={Path}): newDirPath → {NewDirPath}", path, newDirPath);

            var oldDirPath = _fileProvider.GetParentDirectory(originalFile).TrimEnd('/').TrimEnd('\\');
			_logger.LogDebug("ChangePicturesPathAsync(path={Path}): oldDirPath → {OldDirPath}", path, oldDirPath);

            if (!oldDirPath.Equals(oldPath.TrimEnd('/').TrimEnd('\\')) && !directoriesToDelete.Contains(oldDirPath))
                directoriesToDelete.Add(oldDirPath);

            if (!newDirPath.Equals(newPath))
                _fileProvider.CreateDirectory(newDirPath);

            _fileProvider.FileMove(originalFile, newFilePath);
        }

        if (!newPath.StartsWith(oldPath))
            _fileProvider.DeleteDirectory(oldPath);
        else
        {
            foreach (var dir in directoriesToDelete.Where(_fileProvider.DirectoryExists)) 
                _fileProvider.DeleteDirectory(dir);
        }

        _mediaSettings.PicturePath= path;
		_logger.LogDebug("ChangePicturesPathAsync(path={Path}): _mediaSettings.PicturePath → {Arg1}", path, _mediaSettings.PicturePath);
        await _settingService.SaveSettingAsync(_mediaSettings, settings => settings.PicturePath);
    }

    #endregion

    #region Common methods

    /// <summary>
    /// Get content type for picture by file extension
    /// </summary>
    /// <param name="fileExtension">The file extension</param>
    /// <returns>Picture's content type</returns>
    public string GetPictureContentTypeByFileExtension(string fileExtension)
    {
		_logger.LogDebug(">>> Entering GetPictureContentTypeByFileExtension(fileExtension={FileExtension})", fileExtension);
        string contentType = null;

		_logger.LogDebug("GetPictureContentTypeByFileExtension(fileExtension={FileExtension}): contentType → {ContentType}", fileExtension, contentType);
        switch (fileExtension.ToLower())
        {
            case ".bmp":
                contentType = MimeTypes.ImageBmp;
                break;
            case ".gif":
                contentType = MimeTypes.ImageGif;
                break;
            case ".jpeg":
            case ".jpg":
            case ".jpe":
            case ".jfif":
            case ".pjpeg":
            case ".pjp":
                contentType = MimeTypes.ImageJpeg;
                break;
            case ".webp":
                contentType = MimeTypes.ImageWebp;
                break;
            case ".png":
                contentType = MimeTypes.ImagePng;
                break;
            case ".svg":
                contentType = MimeTypes.ImageSvg;
                break;
            case ".tiff":
            case ".tif":
                contentType = MimeTypes.ImageTiff;
                break;
            default:
                break;
        }

		_logger.LogDebug("<<< Exiting GetPictureContentTypeByFileExtension(fileExtension={FileExtension})", fileExtension);
        return contentType;
    }

    #endregion
}
