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
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _settingService = settingService;
        _storeContext = storeContext;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task<string> GetPictureUrlAsync(int pictureId)
    {
		_logger.LogDebug(">>> Entering GetPictureUrlAsync(pictureId={PictureId})", pictureId);
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
		_logger.LogInformation("GetPictureUrlAsync(pictureId)={PictureId}: external service call {Url} took {Arg2} ms", pictureId, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetPictureUrlAsync(pictureId={PictureId})", pictureId);
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var store = await _storeContext.GetCurrentStoreAsync();
        var sliderSettings = await _settingService.LoadSettingAsync<SwiperSettings>(store.Id);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): store → {Store}", widgetZone, additionalData, store);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): sliderSettings → {SliderSettings}", widgetZone, additionalData, sliderSettings);

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
        foreach (var slide in slides)
        {
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): slides → {Slides}", widgetZone, additionalData, slides);
            var picUrl = await GetPictureUrlAsync(slide.PictureId);
            if (string.IsNullOrEmpty(picUrl))
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): picUrl → {PicUrl}", widgetZone, additionalData, picUrl);
                continue;

            model.Slides.Add(new()
            {
                PictureUrl = picUrl,
                TitleText = slide.TitleText,
                LinkUrl = slide.LinkUrl,
                AltText = slide.AltText,
                LazyLoading = sliderSettings.LazyLoading
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): PictureUrl → {PictureUrl}", widgetZone, additionalData, PictureUrl);
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): TitleText → {TitleText}", widgetZone, additionalData, TitleText);
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): LinkUrl → {LinkUrl}", widgetZone, additionalData, LinkUrl);
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): AltText → {AltText}", widgetZone, additionalData, AltText);
				_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): LazyLoading → {LazyLoading}", widgetZone, additionalData, LazyLoading);
            });
        }

        if (!model.Slides.Any())
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return Content("");

        return View("~/Plugins/Widgets.Swiper/Views/PublicInfo.cshtml", model);
    }

    #endregion
}