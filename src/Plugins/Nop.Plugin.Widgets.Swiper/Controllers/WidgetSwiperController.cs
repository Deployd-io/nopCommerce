using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.Swiper.Domain;
using Nop.Plugin.Widgets.Swiper.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.Swiper.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class WidgetSwiperController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public WidgetSwiperController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPictureService pictureService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
		_logger.LogDebug(">>> Entering WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext})", localizationService, notificationService, pictureService, settingService, storeContext);
        _localizationService = localizationService;
		_logger.LogDebug("WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext}): _localizationService → {_localizationService}", localizationService, notificationService, pictureService, settingService, storeContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext}): _notificationService → {_notificationService}", localizationService, notificationService, pictureService, settingService, storeContext, _notificationService);
        _pictureService = pictureService;
		_logger.LogDebug("WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext}): _pictureService → {_pictureService}", localizationService, notificationService, pictureService, settingService, storeContext, _pictureService);
        _settingService = settingService;
		_logger.LogDebug("WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext}): _settingService → {_settingService}", localizationService, notificationService, pictureService, settingService, storeContext, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("WidgetSwiperController(localizationService={LocalizationService},notificationService={NotificationService},pictureService={PictureService},settingService={SettingService},storeContext={StoreContext}): _storeContext → {_storeContext}", localizationService, notificationService, pictureService, settingService, storeContext, _storeContext);
    }

    #endregion

    #region Utilities

    protected virtual async Task<List<Slide>> GetSlidesForStoreAsync(int storeId)
    {
		_logger.LogDebug(">>> Entering GetSlidesForStoreAsync(storeId={StoreId})", storeId);
        var key = $"{nameof(SwiperSettings)}.{nameof(SwiperSettings.Slides)}";

		_logger.LogWarning("GetSlidesForStoreAsync(storeId={StoreId}): key → {Key}", storeId, key);
        //load settings for a chosen store scope
        var slidesSetting = await _settingService.GetSettingByKeyAsync(key, string.Empty, storeId: storeId, loadSharedValueIfNotFound: false);
		_logger.LogDebug("GetSlidesForStoreAsync(storeId={StoreId}): slidesSetting → {SlidesSetting}", storeId, slidesSetting);

        if (string.IsNullOrEmpty(slidesSetting))
            return new List<Slide>();

        return JsonConvert.DeserializeObject<List<Slide>>(slidesSetting);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("Configure(): storeScope → {StoreScope}", storeScope);
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);
		_logger.LogDebug("Configure(): sliderSettings → {SliderSettings}", sliderSettings);

        var model = new ConfigurationModel
        {
            ShowNavigation = sliderSettings.ShowNavigation,
            ShowPagination = sliderSettings.ShowPagination,
            Autoplay = sliderSettings.Autoplay,
            AutoplayDelay = sliderSettings.AutoplayDelay,
            LazyLoading = sliderSettings.LazyLoading,
            ActiveStoreScopeConfiguration = storeScope
        };

        if (storeScope > 0)
        {
            model.ShowNavigation_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.ShowNavigation, storeScope);
            model.Autoplay_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.Autoplay, storeScope);
            model.AutoplayDelay_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.AutoplayDelay, storeScope);
            model.LazyLoading_OverrideForStore = await _settingService.SettingExistsAsync(sliderSettings, x => x.LazyLoading, storeScope);
        }

		_logger.LogDebug("<<< Exiting Configure()");
		_logger.LogDebug("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Widgets.Swiper/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);

        sliderSettings.ShowNavigation = model.ShowNavigation;
        sliderSettings.ShowPagination = model.ShowPagination;
        sliderSettings.Autoplay = model.Autoplay;
        sliderSettings.AutoplayDelay = model.AutoplayDelay;
        sliderSettings.LazyLoading = model.LazyLoading;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.ShowNavigation, model.ShowNavigation_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.ShowPagination, model.ShowPagination_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Autoplay, model.Autoplay_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.AutoplayDelay, model.AutoplayDelay_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.LazyLoading, model.LazyLoading_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [IgnoreAntiforgeryToken]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("add-slide")]
    public virtual async Task<IActionResult> SlideAdd(SlidePictureModel model)
    {
		_logger.LogDebug(">>> Entering SlideAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("SlideAdd(model={Model}): storeScope → {StoreScope}", model, storeScope);
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);
		_logger.LogDebug("SlideAdd(model={Model}): sliderSettings → {SliderSettings}", model, sliderSettings);

        var slides = await GetSlidesForStoreAsync(storeScope);
		_logger.LogDebug("SlideAdd(model={Model}): slides → {Slides}", model, slides);

        slides.Add(new()
        {
            PictureId = model.PictureId,
            AltText = model.AltText,
            TitleText = model.TitleText,
            LinkUrl = model.LinkUrl
			_logger.LogDebug("SlideAdd(model={Model}): PictureId → {PictureId}", model, PictureId);
			_logger.LogDebug("SlideAdd(model={Model}): AltText → {AltText}", model, AltText);
			_logger.LogDebug("SlideAdd(model={Model}): TitleText → {TitleText}", model, TitleText);
			_logger.LogDebug("SlideAdd(model={Model}): LinkUrl → {LinkUrl}", model, LinkUrl);
        });

        sliderSettings.Slides = JsonConvert.SerializeObject(slides);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Slides, true, storeScope);

		_logger.LogDebug("<<< Exiting SlideAdd(model={Model})", model);
        return RedirectToAction(nameof(Configure));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> SlideList(SlidesSearchModel slidesSearchModel)
    {
		_logger.LogDebug(">>> Entering SlideList(slidesSearchModel={SlidesSearchModel})", slidesSearchModel);
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var slides = await GetSlidesForStoreAsync(storeScope);
		_logger.LogDebug("SlideList(slidesSearchModel={SlidesSearchModel}): slides → {Slides}", slidesSearchModel, slides);
		_logger.LogDebug("SlideList(slidesSearchModel={SlidesSearchModel}): storeScope → {StoreScope}", slidesSearchModel, storeScope);

        if (slides is null)
            return Json(new SlideListModel());

        var model = await new SlideListModel().PrepareToGridAsync(slidesSearchModel, slides.ToPagedList(slidesSearchModel), () =>
        {
            return slides
                .Where(s => s.PictureId != 0)
                .SelectAwait(async item =>
                {
                    var picture = (await _pictureService.GetPictureByIdAsync(item.PictureId))
                        ?? throw new Exception("Picture cannot be loaded");

                    return new PublicSlideModel
                    {
                        PictureId = item.PictureId,
                        PictureUrl = (await _pictureService.GetPictureUrlAsync(picture, 200)).Url,
                        TitleText = item.TitleText,
                        AltText = item.AltText,
                        LinkUrl = item.LinkUrl
                    };
                });
        });

		_logger.LogDebug("<<< Exiting SlideList(slidesSearchModel={SlidesSearchModel})", slidesSearchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> SlideDelete(int pictureId)
    {
		_logger.LogDebug(">>> Entering SlideDelete(pictureId={PictureId})", pictureId);
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("SlideDelete(pictureId={PictureId}): storeScope → {StoreScope}", pictureId, storeScope);

        var slides = await GetSlidesForStoreAsync(storeScope);
		_logger.LogDebug("SlideDelete(pictureId={PictureId}): slides → {Slides}", pictureId, slides);

        if (!slides?.Any() == true)
            return Json(slides);

        if (slides.RemoveAll(s => s.PictureId == pictureId) == 0)
            return new NullJsonResult();

        var pic = await _pictureService.GetPictureByIdAsync(pictureId);
		_logger.LogDebug("SlideDelete(pictureId={PictureId}): pic → {Pic}", pictureId, pic);
        if (pic is null)
            return new NullJsonResult();

        await _pictureService.DeletePictureAsync(pic);

        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);
		_logger.LogDebug("SlideDelete(pictureId={PictureId}): sliderSettings → {SliderSettings}", pictureId, sliderSettings);

        if (!slides?.Any() == true)
        {
            await _settingService.DeleteSettingAsync(sliderSettings, setting => setting.Slides, storeScope);
        }
        else
        {
            sliderSettings.Slides = JsonConvert.SerializeObject(slides);
            await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Slides, true, storeScope);
        }

		_logger.LogDebug("<<< Exiting SlideDelete(pictureId={PictureId})", pictureId);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> SlideEdit(SlidePictureModel model)
    {
		_logger.LogDebug(">>> Entering SlideEdit(model={Model})", model);
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("SlideEdit(model={Model}): storeScope → {StoreScope}", model, storeScope);

        var slides = await GetSlidesForStoreAsync(storeScope);
		_logger.LogDebug("SlideEdit(model={Model}): slides → {Slides}", model, slides);
        if (!slides?.Any() == true)
            return Content("No slides");

        //try to get a picture with the specified id
        var slide = slides.FirstOrDefault(s => s.PictureId == model.PictureId)
            ?? throw new ArgumentException("No slides found with the specified picture id");

        slide.TitleText = model.TitleText;
		_logger.LogDebug("SlideEdit(model={Model}): slide.TitleText → {Arg1}", model, slide.TitleText);
		_logger.LogDebug("SlideEdit(model={Model}): slide.AltText → {Arg1}", model, slide.AltText);
		_logger.LogDebug("SlideEdit(model={Model}): slide.LinkUrl → {Arg1}", model, slide.LinkUrl);
        slide.AltText = model.AltText;
        slide.LinkUrl = model.LinkUrl;

        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(storeScope);
		_logger.LogDebug("SlideEdit(model={Model}): sliderSettings → {SliderSettings}", model, sliderSettings);
        sliderSettings.Slides = JsonConvert.SerializeObject(slides);
        await _settingService.SaveSettingOverridablePerStoreAsync(sliderSettings, x => x.Slides, true, storeScope);

		_logger.LogDebug("<<< Exiting SlideEdit(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion
}