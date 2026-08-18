using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Models.DataTables;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the home models factory implementation
/// </summary>
public partial class HomeModelFactory : IHomeModelFactory
{
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IOrderModelFactory _orderModelFactory;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public HomeModelFactory(AdminAreaSettings adminAreaSettings,
        ICommonModelFactory commonModelFactory,
        ILocalizationService localizationService,
        ILogger logger,
        IOrderModelFactory orderModelFactory,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext,
        NopHttpClient nopHttpClient)
    {
        _adminAreaSettings = adminAreaSettings;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _adminAreaSettings → {_adminAreaSettings}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _adminAreaSettings);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _commonModelFactory → {_commonModelFactory}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _commonModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _localizationService → {_localizationService}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _localizationService);
        _logger = logger;
		_logger.LogDebug(">>> Entering HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient})", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient);
        _orderModelFactory = orderModelFactory;
        _settingService = settingService;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _orderModelFactory → {_orderModelFactory}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _orderModelFactory);
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _settingService → {_settingService}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _settingService);
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _staticCacheManager → {_staticCacheManager}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _staticCacheManager);
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _workContext → {_workContext}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _workContext);
        _nopHttpClient = nopHttpClient;
		_logger.LogDebug("HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient}): _nopHttpClient → {_nopHttpClient}", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient, _nopHttpClient);
		_logger.LogDebug("<<< Exiting HomeModelFactory(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},localizationService={LocalizationService},logger={Logger},orderModelFactory={OrderModelFactory},settingService={SettingService},staticCacheManager={StaticCacheManager},workContext={WorkContext},nopHttpClient={NopHttpClient})", adminAreaSettings, commonModelFactory, localizationService, logger, orderModelFactory, settingService, staticCacheManager, workContext, nopHttpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare dashboard model
    /// </summary>
    /// <param name="model">Dashboard model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model)
    {
		_logger.LogDebug(">>> Entering PrepareDashboardModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

		_logger.LogDebug("<<< Exiting PrepareDashboardModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare popular search term report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PreparePopularSearchTermReportModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PreparePopularSearchTermReportModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var searchModel = new PopularSearchTermSearchModel();
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): searchModel → {SearchModel}", model, searchModel);
        searchModel = await _commonModelFactory.PreparePopularSearchTermSearchModelAsync(searchModel);

        model.Name = "search-term-report-grid";
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("PopularSearchTermsReport", "Common", null);
        model.Length = searchModel.Length;
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): model.LengthMenu → {Arg1}", model, model.LengthMenu);
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): model.RefreshButton → {Arg1}", model, model.RefreshButton);
		_logger.LogDebug("PreparePopularSearchTermReportModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.LengthMenu = searchModel.AvailablePageSizes;
        model.RefreshButton = false;
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(PopularSearchTermModel.Keyword))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SearchTermReport.Keyword")
            },
            new(nameof(PopularSearchTermModel.Count))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SearchTermReport.Count")
            }
        };

		_logger.LogDebug("<<< Exiting PreparePopularSearchTermReportModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare bestsellers brief by amount report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PrepareBestsellersBriefReportByAmountModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellersBriefReportByAmountModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var searchModel = new BestsellerBriefSearchModel();
		_logger.LogDebug("PrepareBestsellersBriefReportByAmountModelAsync(model={Model}): searchModel → {SearchModel}", model, searchModel);
        searchModel = await _orderModelFactory.PrepareBestsellerBriefSearchModelAsync(searchModel);

        model.Name = "bestsellers-byamount-grid";
		_logger.LogDebug("PrepareBestsellersBriefReportByAmountModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("BestsellersBriefReportByAmountList", "Order", new RouteValueDictionary { [nameof(searchModel.OrderBy)] = OrderByEnum.OrderByTotalAmount });
        model.Length = searchModel.PageSize;
		_logger.LogDebug("PrepareBestsellersBriefReportByAmountModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PrepareBestsellersBriefReportByAmountModelAsync(model={Model}): model.RefreshButton → {Arg1}", model, model.RefreshButton);
		_logger.LogDebug("PrepareBestsellersBriefReportByAmountModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.RefreshButton = false;
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(BestsellerModel.ProductName))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.Name")
            },
            new(nameof(BestsellerModel.TotalQuantity))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")
            },
            new(nameof(BestsellerModel.TotalAmount))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")
            },
            new(nameof(BestsellerModel.ProductId))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Common.View"),
                Width = "80",
                ClassName = NopColumnClassDefaults.Button,
                Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
            }
        };

		_logger.LogDebug("<<< Exiting PrepareBestsellersBriefReportByAmountModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare bestsellers brief by quantity report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PrepareBestsellersBriefReportByQuantityModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellersBriefReportByQuantityModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var searchModel = new BestsellerBriefSearchModel();
		_logger.LogDebug("PrepareBestsellersBriefReportByQuantityModelAsync(model={Model}): searchModel → {SearchModel}", model, searchModel);
        searchModel = await _orderModelFactory.PrepareBestsellerBriefSearchModelAsync(searchModel);

        model.Name = "bestsellers-byquantity-grid";
		_logger.LogDebug("PrepareBestsellersBriefReportByQuantityModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("BestsellersBriefReportByQuantityList", "Order", new RouteValueDictionary { [nameof(searchModel.OrderBy)] = OrderByEnum.OrderByQuantity });
        model.Length = searchModel.PageSize;
		_logger.LogDebug("PrepareBestsellersBriefReportByQuantityModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PrepareBestsellersBriefReportByQuantityModelAsync(model={Model}): model.RefreshButton → {Arg1}", model, model.RefreshButton);
		_logger.LogDebug("PrepareBestsellersBriefReportByQuantityModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.RefreshButton = false;        
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(BestsellerModel.ProductName))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.Name")
            },
            new(nameof(BestsellerModel.TotalQuantity))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")
            },
            new(nameof(BestsellerModel.TotalAmount))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")
            },
            new(nameof(BestsellerModel.ProductId))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Common.View"),
                Width = "80",
                ClassName = NopColumnClassDefaults.Button,
                Render = new RenderButtonView(new DataUrl("~/Admin/Product/Edit/"))
            }
        };

		_logger.LogDebug("<<< Exiting PrepareBestsellersBriefReportByQuantityModelAsync(model={Model})", model);
        return model;
    }


    /// <summary>
    /// Prepare latest orders model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PrepareLatestOrdersModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PrepareLatestOrdersModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.Name = "orders-grid";
		_logger.LogDebug("PrepareLatestOrdersModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("OrderList", "Order", null);
        model.Length = 5;
		_logger.LogDebug("PrepareLatestOrdersModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PrepareLatestOrdersModelAsync(model={Model}): model.RefreshButton → {Arg1}", model, model.RefreshButton);
		_logger.LogDebug("PrepareLatestOrdersModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.RefreshButton = false;        
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(OrderModel.CustomOrderNumber))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Orders.Fields.CustomOrderNumber"),
                Width = "80"
            },
            new(nameof(OrderModel.OrderStatus))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Orders.Fields.OrderStatus"),
                Width = "100",
                Render = new RenderCustom("renderColumnOrderStatus")
            },
            new(nameof(OrderModel.CustomerEmail))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Orders.Fields.Customer"),
                Width = "250",
                Render = new RenderCustom("renderColumnCustomerEmail")
            },
            new(nameof(OrderModel.CreatedOn))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Orders.Fields.CreatedOn"),
                Width = "100",
                Render = new RenderDate()
            },
            new(nameof(OrderModel.Id))
            {
                Title = await _localizationService.GetResourceAsync("Admin.Common.View"),
                Width = "50",
                ClassName = NopColumnClassDefaults.Button,
                Render = new RenderButtonView(new DataUrl("~/Admin/Order/Edit/"))
            }
        };

		_logger.LogDebug("<<< Exiting PrepareLatestOrdersModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare incomplete orders report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PrepareOrderIncompleteModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PrepareOrderIncompleteModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.Name = "incomplete-order-report-grid";
		_logger.LogDebug("PrepareOrderIncompleteModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("OrderIncompleteReportList", "Order", null);
        model.Length = int.MaxValue;
		_logger.LogDebug("PrepareOrderIncompleteModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PrepareOrderIncompleteModelAsync(model={Model}): model.Paging → {Arg1}", model, model.Paging);
		_logger.LogDebug("PrepareOrderIncompleteModelAsync(model={Model}): model.Info → {Arg1}", model, model.Info);
		_logger.LogDebug("PrepareOrderIncompleteModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.Paging = false;
        model.Info = false;
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(OrderIncompleteReportModel.Item))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.Item")
            },
            new(nameof(OrderIncompleteReportModel.Total))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.Total"),
                Width = "50"
            },
            new(nameof(OrderIncompleteReportModel.Count))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.Count"),
                Width = "120",
                ClassName =  NopColumnClassDefaults.Button,
                Render = new RenderCustom("renderColumnOrderIncompleteReportCount")
            }
        };

		_logger.LogDebug("<<< Exiting PrepareOrderIncompleteModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare order average report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PrepareOrderAverageModelAsync(DataTablesModel model)
    {
		_logger.LogDebug(">>> Entering PrepareOrderAverageModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.Name = "average-order-report-grid";
		_logger.LogDebug("PrepareOrderAverageModelAsync(model={Model}): model.Name → {Arg1}", model, model.Name);
        model.UrlRead = new DataUrl("OrderAverageReportList", "Order", null);
        model.Length = int.MaxValue;
		_logger.LogDebug("PrepareOrderAverageModelAsync(model={Model}): model.Length → {Arg1}", model, model.Length);
		_logger.LogDebug("PrepareOrderAverageModelAsync(model={Model}): model.Paging → {Arg1}", model, model.Paging);
		_logger.LogDebug("PrepareOrderAverageModelAsync(model={Model}): model.Info → {Arg1}", model, model.Info);
		_logger.LogDebug("PrepareOrderAverageModelAsync(model={Model}): model.ColumnCollection → {Arg1}", model, model.ColumnCollection);
        model.Paging = false;
        model.Info = false;
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(OrderAverageReportModel.OrderStatus))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.OrderStatus")
            },
            new(nameof(OrderAverageReportModel.SumTodayOrders))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.SumTodayOrders")
            },
            new(nameof(OrderAverageReportModel.SumThisWeekOrders))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.SumThisWeekOrders")
            },
            new(nameof(OrderAverageReportModel.SumThisMonthOrders))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.SumThisMonthOrders")
            },
            new(nameof(OrderAverageReportModel.SumThisYearOrders))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.SumThisYearOrders")
            },
            new(nameof(OrderAverageReportModel.SumAllTimeOrders))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SalesReport.Average.SumAllTimeOrders")
            }
        };

		_logger.LogDebug("<<< Exiting PrepareOrderAverageModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare nopCommerce news model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the nopCommerce news model
    /// </returns>
    public virtual async Task<NopCommerceNewsModel> PrepareNopCommerceNewsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareNopCommerceNewsModelAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = new NopCommerceNewsModel
        {
            HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
        };

        try
        {
            //try to get news RSS feed
            var rssData = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.OfficialNewsModelKey), async () =>
            {
                try
                {
                    return await _nopHttpClient.GetNewsRssAsync();
                }
                catch (AggregateException exception)
                {
					_logger.LogError("Exception in PrepareNopCommerceNewsModelAsync(): {Arg0}", exception.Message);
					_logger.LogWarning("Throwing InnerException from PrepareNopCommerceNewsModelAsync()");
                    //rethrow actual excepion
                    throw exception.InnerException;
                }
            });

            for (var i = 0; i < rssData.Items.Count; i++)
            {
                var item = rssData.Items.ElementAt(i);
				_logger.LogDebug("PrepareNopCommerceNewsModelAsync(): item → {Item}", item);
                var newsItem = new NopCommerceNewsDetailsModel
                {
                    Title = item.TitleText,
                    Summary = XmlHelper.XmlDecode(item.Content?.Value ?? string.Empty),
                    Url = item.Url.OriginalString,
                    PublishDate = item.PublishDate
                };
                model.Items.Add(newsItem);

                //has new items?
                if (i != 0)
                    continue;

                var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
				_logger.LogDebug("PrepareNopCommerceNewsModelAsync(): firstRequest → {FirstRequest}", firstRequest);
                if (_adminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
                    continue;

                _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
				_logger.LogDebug("PrepareNopCommerceNewsModelAsync(): _adminAreaSettings.LastNewsTitleAdminArea → {Arg0}", _adminAreaSettings.LastNewsTitleAdminArea);
                await _settingService.SaveSettingAsync(_adminAreaSettings);

                //new item
                if (!firstRequest)
                    model.HasNewItems = true;
					_logger.LogDebug("PrepareNopCommerceNewsModelAsync(): model.HasNewItems → {Arg0}", model.HasNewItems);
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("No access to the news. Website www.nopcommerce.com is not available.", ex);
        }

		_logger.LogInformation("PrepareNopCommerceNewsModelAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareNopCommerceNewsModelAsync()");
        return model;
    }

    #endregion
}