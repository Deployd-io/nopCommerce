using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Swiper.Domain;
using Nop.Plugin.Widgets.Swiper.Infrastructure.Cache;
using Nop.Plugin.Widgets.Swiper.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.Swiper.Components;

public class WidgetSwiperViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public WidgetSwiperViewComponent(IPictureService pictureService,
    IStaticCacheManager staticCacheManager,
    ISettingService settingService,
    IStoreContext storeContext,
    IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper})", pictureService, staticCacheManager, settingService, storeContext, webHelper);
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _staticCacheManager → {_staticCacheManager}", pictureService, staticCacheManager, settingService, storeContext, webHelper, _staticCacheManager);
		_logger.LogDebug("WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _pictureService → {_pictureService}", pictureService, staticCacheManager, settingService, storeContext, webHelper, _pictureService);
    _logger.LogWarning("<<< Exiting WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper})", pictureService, staticCacheManager, settingService, storeContext, webHelper);
        _settingService = settingService;
		_logger.LogDebug("WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _settingService → {_settingService}", pictureService, staticCacheManager, settingService, storeContext, webHelper, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("WidgetSwiperViewComponent(pictureService={PictureService},staticCacheManager={StaticCacheManager},settingService={SettingService},storeContext={StoreContext},webHelper={WebHelper}): _storeContext → {_storeContext}", pictureService, staticCacheManager, settingService, storeContext, webHelper, _storeContext);
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task<string> GetPictureUrlAsync(int pictureId)
    {
		_logger.LogError(">>> Entering GetPictureUrlAsync(pictureId={PictureId})", pictureId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (pictureId == 0)
            return string.Empty;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PictureUrlModelKey,
            pictureId, _webHelper.IsCurrentConnectionSecured());

        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            //little hack here. nulls aren't cacheable so set it to ""
            var url = await _pictureService.GetPictureUrlAsync(pictureId, showDefaultPicture: false) ?? "";
            return url;
        });
		_logger.LogError("GetPictureUrlAsync(pictureId)={PictureId}: external service call {Url} took {Arg2} ms", pictureId, url, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetPictureUrlAsync(pictureId={PictureId})", pictureId);
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogError(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var store = await _storeContext.GetCurrentStoreAsync();
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(store.Id);
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): sliderSettings → {SliderSettings}", widgetZone, additionalData, sliderSettings);
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): store → {Store}", widgetZone, additionalData, store);

        if (string.IsNullOrEmpty(sliderSettings.Slides))
            return Content("");

        var model = new PublicInfoModel
        {
            ShowNavigation = sliderSettings.ShowNavigation,
            ShowPagination = sliderSettings.ShowPagination,
            Autoplay = sliderSettings.Autoplay,
            AutoplayDelay = sliderSettings.AutoplayDelay,
        };

        var slides = JsonConvert.DeserializeObject<List<Slide>>(sliderSettings.Slides);
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): slides → {Slides}", widgetZone, additionalData, slides);
        foreach (var slide in slides)
        {
            var picUrl = await GetPictureUrlAsync(slide.PictureId);
			_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): picUrl → {PicUrl}", widgetZone, additionalData, picUrl);
            if (string.IsNullOrEmpty(picUrl))
                continue;

            model.Slides.Add(new()
            {
                PictureUrl = picUrl,
                TitleText = slide.TitleText,
                LinkUrl = slide.LinkUrl,
                AltText = slide.AltText,
                LazyLoading = sliderSettings.LazyLoading
            });
			_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): LazyLoading → {LazyLoading}", widgetZone, additionalData, LazyLoading);
        }

        if (!model.Slides.Any())
            return Content("");

        return View("~/Plugins/Widgets.Swiper/Views/PublicInfo.cshtml", model);
    }

    #endregion
}