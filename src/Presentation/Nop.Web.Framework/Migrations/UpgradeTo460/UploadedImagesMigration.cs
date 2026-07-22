using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopMigration("2022-10-18 00:00:00", "Move uploaded images to disk", MigrationProcessType.Update)]
public class UploadedImagesMigration : Migration
{
    #region Fields

    protected readonly IPictureService _pictureService;
    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRoxyFilemanFileProvider _roxyFilemanFileProvider;

    #endregion

    public UploadedImagesMigration(IPictureService pictureService, IRepository<Picture> pictureRepository, IRoxyFilemanFileProvider roxyFilemanFileProvider)
    {
		logger.LogDebug(">>> Entering UploadedImagesMigration(pictureService={PictureService},pictureRepository={PictureRepository},roxyFilemanFileProvider={RoxyFilemanFileProvider})", pictureService, pictureRepository, roxyFilemanFileProvider);
        _pictureService = pictureService;
        _pictureRepository = pictureRepository;
		logger.LogDebug("UploadedImagesMigration(pictureService={PictureService},pictureRepository={PictureRepository},roxyFilemanFileProvider={RoxyFilemanFileProvider}): _pictureRepository → {_pictureRepository}", pictureService, pictureRepository, roxyFilemanFileProvider, _pictureRepository);
		logger.LogDebug("UploadedImagesMigration(pictureService={PictureService},pictureRepository={PictureRepository},roxyFilemanFileProvider={RoxyFilemanFileProvider}): _pictureService → {_pictureService}", pictureService, pictureRepository, roxyFilemanFileProvider, _pictureService);
        _roxyFilemanFileProvider = roxyFilemanFileProvider;
		logger.LogDebug("<<< Exiting UploadedImagesMigration(pictureService={PictureService},pictureRepository={PictureRepository},roxyFilemanFileProvider={RoxyFilemanFileProvider})", pictureService, pictureRepository, roxyFilemanFileProvider);
		logger.LogDebug("UploadedImagesMigration(pictureService={PictureService},pictureRepository={PictureRepository},roxyFilemanFileProvider={RoxyFilemanFileProvider}): _roxyFilemanFileProvider → {_roxyFilemanFileProvider}", pictureService, pictureRepository, roxyFilemanFileProvider, _roxyFilemanFileProvider);
    }

    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!_pictureService.IsStoreInDbAsync().Result)
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        const int pageSize = 400;
        var pageIndex = 0;
        var uploadRoot = $"~{NopRoxyFilemanDefaults.DefaultRootDirectory}/";

		logger.LogDebug("Up(): uploadRoot → {UploadRoot}", uploadRoot);
        try
        {
            while (true)
            {
                var pictures = _pictureService.GetPicturesAsync(uploadRoot,
                    pageIndex, pageSize).Result;
                pageIndex++;

                //all pictures flushed?
                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                        return;

                    var seoFileName = picture.SeoFilename;

                    var lastPart = _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType).Result;

					logger.LogDebug("Up(): lastPart → {LastPart}", lastPart);
                    var thumbFileName = $"{seoFileName}.{lastPart}";
                    var thumbDirectoryName = picture.VirtualPath[uploadRoot.Length..];
					logger.LogDebug("Up(): thumbFileName → {ThumbFileName}", thumbFileName);
                    var thumbFilePath = Path.Combine(thumbDirectoryName, thumbFileName);

					logger.LogDebug("Up(): thumbFilePath → {ThumbFilePath}", thumbFilePath);
                    _roxyFilemanFileProvider.CreateDirectory("/", thumbDirectoryName);

                    if (picture.IsNew)
                    {
                        // delete old file if exist
                        _roxyFilemanFileProvider.DeleteFile(thumbFilePath);
                    }

                    if (_roxyFilemanFileProvider.GetFileInfo(thumbFilePath).Exists)
                    {
                        _pictureRepository.DeleteAsync(picture, false).Wait();
						logger.LogInformation("Up(): delete query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
                        continue;
                    }
                    //the named mutex helps to avoid creating the same files in different threads,
                    //and does not decrease performance significantly, because the code is blocked only for the specific file.
                    //you should be very careful, mutexes cannot be used in with the await operation
                    //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                    using var mutex = new Mutex(false, thumbFileName);

					logger.LogDebug("Up(): mutex → {Mutex}", mutex);
                    mutex.WaitOne();

                    try
                    {
                        //check, if the file was created, while we were waiting for the release of the mutex.
                        if (!_roxyFilemanFileProvider.GetFileInfo(thumbFilePath).Exists)
                        {
                            var pictureBinary = _pictureService.LoadPictureBinaryAsync(picture).Result;

							logger.LogDebug("Up(): pictureBinary → {PictureBinary}", pictureBinary);
                            if (pictureBinary == null || pictureBinary.Length == 0)
                                continue;

                            using var stream = new MemoryStream(pictureBinary);
                            _roxyFilemanFileProvider.SaveFileAsync(thumbDirectoryName, thumbFileName, picture.MimeType, stream).Wait();
							logger.LogDebug("Up(): stream → {Stream}", stream);
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }

                    _pictureRepository.DeleteAsync(picture, false).Wait();
                }
            }
        }
        catch
        {
			logger.LogError("Exception in Up(): {Arg0}", ex.Message);
            // ignored
        }
		logger.LogInformation("Up(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}
