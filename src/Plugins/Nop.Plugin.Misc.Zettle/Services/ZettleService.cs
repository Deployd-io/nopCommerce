using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Plugin.Misc.Zettle.Domain.Api;
using Nop.Plugin.Misc.Zettle.Domain.Api.Image;
using Nop.Plugin.Misc.Zettle.Domain.Api.Inventory;
using Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;
using Nop.Plugin.Misc.Zettle.Domain.Api.Product;
using Nop.Plugin.Misc.Zettle.Domain.Api.Pusher;
using Nop.Plugin.Misc.Zettle.Domain.Api.Secure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Logging;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents the plugin service
/// </summary>
public class ZettleService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDiscountService _discountService;
    protected readonly ILogger _logger;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly ZettleHttpClient _zettleHttpClient;
    protected readonly ZettleRecordService _zettleRecordService;
    protected readonly ZettleSettings _zettleSettings;

    protected Dictionary<string, string> _locations = new();

    #endregion

    #region Ctor

    public ZettleService(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IDiscountService discountService,
        ILogger logger,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        ISettingService settingService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        ZettleHttpClient zettleHttpClient,
        ZettleRecordService zettleRecordService,
        ZettleSettings zettleSettings)
    {
        _currencySettings = currencySettings;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _currencySettings → {_currencySettings}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _currencySettings);
        _currencyService = currencyService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _currencyService → {_currencyService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _currencyService);
        _discountService = discountService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _discountService → {_discountService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _discountService);
        _logger = logger;
		_logger.LogDebug(">>> Entering ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings})", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings);
        _pictureService = pictureService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _pictureService → {_pictureService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _pictureService);
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _productAttributeParser → {_productAttributeParser}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _productAttributeParser);
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _productAttributeService → {_productAttributeService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _productAttributeService);
        _productService = productService;
        _settingService = settingService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _productService → {_productService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _productService);
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _settingService → {_settingService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _settingService);
        _workContext = workContext;
        _mediaSettings = mediaSettings;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _workContext → {_workContext}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _workContext);
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _mediaSettings → {_mediaSettings}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _mediaSettings);
        _zettleHttpClient = zettleHttpClient;
        _zettleRecordService = zettleRecordService;
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _zettleHttpClient → {_zettleHttpClient}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _zettleHttpClient);
		_logger.LogDebug("ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings}): _zettleRecordService → {_zettleRecordService}", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings, _zettleRecordService);
		_logger.LogDebug("<<< Exiting ZettleService(currencySettings={CurrencySettings},currencyService={CurrencyService},discountService={DiscountService},logger={Logger},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},workContext={WorkContext},mediaSettings={MediaSettings},zettleHttpClient={ZettleHttpClient},zettleRecordService={ZettleRecordService},zettleSettings={ZettleSettings})", currencySettings, currencyService, discountService, logger, pictureService, productAttributeParser, productAttributeService, productService, settingService, workContext, mediaSettings, zettleHttpClient, zettleRecordService, zettleSettings);
        _zettleSettings = zettleSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    protected async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //ensure that plugin is configured
            if (!IsConfigured(_zettleSettings))
                throw new NopException("Plugin not configured");

            return (await function(), default);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.Message;
            if (logErrors)
            {
                var logMessage = $"{ZettleDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
            }

            return (default, errorMessage);
        }
    }

    #region Sync

    /// <summary>
    /// Import discounts to Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportDiscountsAsync(StringBuilder log)
    {
		_logger.LogDebug(">>> Entering ImportDiscountsAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //if enabled
        if (!_zettleSettings.DiscountSyncEnabled)
            return;

        log.AppendLine("Add discounts...");

        var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("ImportDiscountsAsync(log={Log}): storeCurrency → {StoreCurrency}", log, storeCurrency);

        //add only assigned to order subtotal discounts
        var existingDiscounts = await _zettleHttpClient.RequestAsync<GetDiscountsRequest, DiscountList>(new());
		_logger.LogDebug("ImportDiscountsAsync(log={Log}): existingDiscounts → {ExistingDiscounts}", log, existingDiscounts);
        var discounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderSubTotal, showHidden: true);
		_logger.LogDebug("ImportDiscountsAsync(log={Log}): discounts → {Discounts}", log, discounts);
        var discountsToAdd = discounts
            .Where(discount => !existingDiscounts.Any(existingDiscount => existingDiscount.ExternalReference == discount.Id.ToString()))
            .ToList();

        foreach (var discount in discountsToAdd)
        {
            var request = new CreateDiscountRequest
            {
                Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                Name = discount.Name,
                Description = discount.Name,
                ExternalReference = discount.Id.ToString()
            };
            if (!discount.UsePercentage)
            {
                request.Amount = new Domain.Api.Product.Discount.DiscountAmount
				_logger.LogDebug("ImportDiscountsAsync(log={Log}): request.Amount → {Arg1}", log, request.Amount);
                {
                    CurrencyId = storeCurrency.CurrencyCode.ToUpper(),
                    Amount = storeCurrency.CurrencyCode.ToUpper() switch
                    {
                        "JPY" or "ISK" => Convert.ToInt32(Math.Round(discount.DiscountAmount, 0)),
                        _ => Convert.ToInt32(Math.Round(discount.DiscountAmount * 100, 0))
                    }
                };
            }
            else
                request.Percentage = discount.DiscountPercentage;
				_logger.LogDebug("ImportDiscountsAsync(log={Log}): request.Percentage → {Arg1}", log, request.Percentage);

            log.AppendLine($"\tAdd discount '{discount.Name}'");

            await _zettleHttpClient.RequestAsync<CreateDiscountRequest, ApiResponse>(request);
        }
		_logger.LogInformation("ImportDiscountsAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ImportDiscountsAsync(log={Log})", log);
    }

    /// <summary>
    /// Delete products from Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportDeletedAsync(StringBuilder log)
    {
		_logger.LogDebug(">>> Entering ImportDeletedAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        log.AppendLine("Delete products...");

        //get records to delete
        var records = await _zettleRecordService
            .GetAllRecordsAsync(active: true, operationTypes: [OperationType.Delete]);
        var idsToDelete = records
            .Where(record => !string.IsNullOrEmpty(record.Uuid) && record.ProductId > 0 && record.CombinationId == 0)
            .Select(record => record.Uuid)
            .Distinct()
            .ToList();
			_logger.LogDebug("ImportDeletedAsync(log={Log}): idsToDelete → {IdsToDelete}", log, idsToDelete);

        if (idsToDelete.Any())
            log.AppendLine($"\tDelete {idsToDelete.Count} products (#{string.Join(", #", idsToDelete)})");

        //if needed, also delete all existing products
        if (_zettleSettings.DeleteBeforeImport)
        {
            var idsToKeep = (await _zettleRecordService.GetAllRecordsAsync(productOnly: true, active: true))
                .Where(record => !string.IsNullOrEmpty(record.Uuid))
                .Select(record => record.Uuid)
                .Distinct()
                .Except(idsToDelete)
                .ToList();

            var products = await _zettleHttpClient.RequestAsync<GetProductsRequest, ProductList>(new());
			_logger.LogDebug("ImportDeletedAsync(log={Log}): products → {Products}", log, products);
            var existingIds = products.Select(product => product.Uuid).ToList();
			_logger.LogDebug("ImportDeletedAsync(log={Log}): existingIds → {ExistingIds}", log, existingIds);

            idsToDelete.AddRange(existingIds.Except(idsToKeep).ToList());

            log.AppendLine($"\tAlso delete all existing library items before importing products");
        }

        idsToDelete = idsToDelete.Distinct().ToList();
        if (idsToDelete.Any())
            await _zettleHttpClient.RequestAsync<DeleteProductsRequest, ApiResponse>(new DeleteProductsRequest { ProductUuids = idsToDelete });

		_logger.LogDebug("<<< Exiting ImportDeletedAsync(log={Log})", log);
		_logger.LogInformation("ImportDeletedAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
        await _zettleRecordService.DeleteRecordsAsync(records.Select(record => record.Id).ToList());
    }

    /// <summary>
    /// Change product images in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportImageChangedAsync(StringBuilder log)
    {
		_logger.LogDebug(">>> Entering ImportImageChangedAsync(log={Log})", log);
        log.AppendLine("Change images...");

        //upload new images
        var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: [OperationType.ImageChanged]))
            .Where(record => record.ImageSyncEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        await UploadImagesAsync(records, true, log);

        //then update appropriate products
        var products = records
            .GroupBy(record => record.ProductId)
            .Select(group => new
            {
                ProductRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            })
            .ToList();
        foreach (var product in products)
        {
            var existingProduct = await _zettleHttpClient
                .RequestAsync<GetProductRequest, Product>(new GetProductRequest { Uuid = product.ProductRecord.Uuid });
            var request = new UpdateProductRequest
            {
                Uuid = existingProduct.Uuid,
                Name = existingProduct.Name,
                ETag = $"\"{existingProduct.ETag}\""
            };
            if (!product.CombinationRecords.Any())
            {
                request.Presentation = new Product.ProductPresentation { ImageUrl = product.ProductRecord?.ImageUrl };
				_logger.LogDebug("ImportImageChangedAsync(log={Log}): request.Presentation → {Arg1}", log, request.Presentation);
                request.Variants =
                [
                    new() { Uuid = product.ProductRecord.VariantUuid }
				_logger.LogDebug("<<< Exiting ImportImageChangedAsync(log={Log})", log);
                ];
            }
            else
            {
                request.Variants = product.CombinationRecords.Select(record => new Product.ProductVariant
                {
                    Uuid = record.VariantUuid,
                    Presentation = new Product.ProductPresentation { ImageUrl = record.ImageUrl }
                }).ToList();
            }

            log.AppendLine($"\tAdd image to product #{product.ProductRecord.ProductId}");

            await _zettleHttpClient.RequestAsync<UpdateProductRequest, ApiResponse>(request);
        }
    }

    /// <summary>
    /// Update inventory tracking balances in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportInventoryTrackingAsync(StringBuilder log)
    {
		_logger.LogDebug(">>> Entering ImportInventoryTrackingAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        log.AppendLine("Update inventory tracking...");

        var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: [OperationType.Update]))
            .Where(record => record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        if (!records.Any())
            return;

        var storeBalance = await _zettleHttpClient.RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());
		_logger.LogDebug("ImportInventoryTrackingAsync(log={Log}): storeBalance → {StoreBalance}", log, storeBalance);

        var products = records
            .GroupBy(record => record.ProductId)
            .Select(group => new
            {
                ProductRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            })
            .Where(product => !storeBalance.TrackedProducts?.Contains(product.ProductRecord.Uuid, StringComparer.InvariantCultureIgnoreCase) ?? true)
            .ToList();
        if (!products.Any())
            return;

        var productChanges = new List<CreateTrackingRequest.ProductBalanceChange>();
		_logger.LogDebug("ImportInventoryTrackingAsync(log={Log}): productChanges → {ProductChanges}", log, productChanges);
        var recordsToUpdate = new List<ZettleRecord>();
		_logger.LogDebug("ImportInventoryTrackingAsync(log={Log}): recordsToUpdate → {RecordsToUpdate}", log, recordsToUpdate);

        foreach (var product in products)
        {
            log.AppendLine($"\tStart inventory tracking for product #{product.ProductRecord.ProductId}");

            //get current quantity if exists
            var productQuantity = storeBalance.Variants
                ?.FirstOrDefault(balance => balance.ProductUuid == product.ProductRecord.Uuid && balance.VariantUuid == product.ProductRecord.VariantUuid)
                ?.Balance ?? 0;
            (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToStart = (product.ProductRecord, productQuantity, null);
            var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
			_logger.LogDebug("ImportInventoryTrackingAsync(log={Log}): combinationRecordsToStart → {CombinationRecordsToStart}", log, combinationRecordsToStart);
            foreach (var combinationRecord in product.CombinationRecords)
            {
                //get current quantity if exists
                var combinationQuantity = storeBalance.Variants
                    ?.FirstOrDefault(balance => balance.ProductUuid == combinationRecord.Uuid && balance.VariantUuid == combinationRecord.VariantUuid)
                    ?.Balance ?? 0;
                combinationRecordsToStart.Add((combinationRecord, combinationQuantity, null));
            }
            var productChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                productRecordToStart, combinationRecordsToStart);
            if (productChange is null)
                continue;

            productChanges.Add(productChange);
            recordsToUpdate.AddRange(product.CombinationRecords.Union([product.ProductRecord]));
        }

        await UpdateInventoryBalanceAsync(productChanges, recordsToUpdate);
		_logger.LogInformation("ImportInventoryTrackingAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ImportInventoryTrackingAsync(log={Log})", log);
    }

    /// <summary>
    /// Create or update products in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details
    /// </returns>
    protected async Task<Import> ImportCreatedOrUpdatedAsync(StringBuilder log)
    {
		_logger.LogDebug(">>> Entering ImportCreatedOrUpdatedAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        log.AppendLine("Create and update products...");

        //check currency match
        var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): storeCurrency → {StoreCurrency}", log, storeCurrency);
        var (accountInfo, _) = await GetAccountInfoAsync();
        var priceSyncAvailable = string.Equals(storeCurrency.CurrencyCode, accountInfo.Currency, StringComparison.InvariantCultureIgnoreCase);
		_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): priceSyncAvailable → {PriceSyncAvailable}", log, priceSyncAvailable);

        //prepare price function
        int preparePrice(decimal price) => storeCurrency.CurrencyCode.ToUpper() switch
        {
            "JPY" or "ISK" => Convert.ToInt32(Math.Round(price, 0)),
            _ => Convert.ToInt32(Math.Round(price * 100, 0))
        };

        Import import = null;
		_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): import → {Import}", log, import);
        var pageIndex = 0;
        while (true)
        {
            //we can add up to 2000 products per request, but when uploading images, this may be too much
            var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                    operationTypes: [OperationType.Create, OperationType.Update],
                pageIndex: pageIndex++,
            pageSize: _zettleSettings.ImportProductsNumber);
            if (!records.Any())
                return import;

            log.AppendLine($"\tPrepare {records.Count} records to import");

            //upload images if needed
            await UploadImagesAsync(records.ToList(), false, log);

            //prepare products to import
            var products = await _zettleRecordService.PrepareToSyncRecords(records.ToList()).SelectAwait(async product =>
            {
                var request = new Product
                {
                    Uuid = product.Uuid,
                    ExternalReference = product.Sku,
                    Name = product.Name,
                    Id = product.Id,
                    Description = product.Description,
                    CreateWithDefaultTax = _zettleSettings.DefaultTaxEnabled,
                    Category = new Product.ProductCategory
                    {
                        Name = product.CategoryName,
                        Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
                    },
                    Metadata = new Product.ProductMetadata
                    {
                        InPos = true,
                        Source = new Product.ProductMetadata.ProductSource
                        {
                            External = true,
                            Name = ZettleDefaults.PartnerIdentifier
                        }
                    }
                };

                //set image
                if (product.ImageSyncEnabled && !string.IsNullOrEmpty(product.ImageUrl))
                    request.Presentation = new Product.ProductPresentation { ImageUrl = product.ImageUrl };
					_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): request.Presentation → {Arg1}", log, request.Presentation);

                var combinationRecords = records
                    .Where(record => record.ProductId == product.Id && record.CombinationId != 0)
                    .ToList();
                if (!combinationRecords.Any())
                {
                    //a single variant
                    var variant = new Product.ProductVariant
                    {
                        Uuid = product.VariantUuid,
                        Name = product.Name,
                        Sku = product.Sku,
                        Description = product.Description
                    };

                    //set the price if available
                    if (product.PriceSyncEnabled && priceSyncAvailable)
                    {
                        variant.Price = new Product.ProductVariant.ProductPrice
						_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): variant.Price → {Arg1}", log, variant.Price);
                        {
                            Amount = preparePrice(product.Price),
                            CurrencyId = accountInfo.Currency
                        };
                        variant.CostPrice = new Product.ProductVariant.ProductPrice
						_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): variant.CostPrice → {Arg1}", log, variant.CostPrice);
                        {
                            Amount = preparePrice(product.ProductCost),
                            CurrencyId = accountInfo.Currency
                        };
                    }
                    request.Variants = [variant];
					_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): request.Variants → {Arg1}", log, request.Variants);
                }
                else
                {
                    //or multi variants
                    var productCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
					_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): productCombinations → {ProductCombinations}", log, productCombinations);
                    var productAttributMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
					_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): productAttributMappings → {ProductAttributMappings}", log, productAttributMappings);
                    var productAttributes = await productAttributMappings.SelectAwait(async mapping =>
                    {
                        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
						_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): productAttribute → {ProductAttribute}", log, productAttribute);
						var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
						_logger.LogInformation("ImportCreatedOrUpdatedAsync(log)={Log}: database query executed in {Arg1} ms", log, __logStartDb.ElapsedMilliseconds);
                        var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(mapping.Id);
						_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): productAttributeValues → {ProductAttributeValues}", log, productAttributeValues);
                        return new { Name = productAttribute.Name, Values = productAttributeValues.Select(value => value.Name).ToList() };
                    }).ToListAsync();

                    request.VariantOptionDefinitions = new Product.ProductVariantDefinitions
					_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): request.VariantOptionDefinitions → {Arg1}", log, request.VariantOptionDefinitions);
                    {
                        Definitions = productAttributes.Select(attribute => new Product.ProductVariantDefinitions.ProductVariantOptionDefinition
                        {
                            Name = attribute.Name,
                            Properties = attribute.Values.Select(value => new Product.ProductVariantDefinitions.ProductVariantOptionDefinition.ProductVariantOptionProperty
                            {
                                Value = value
                            }).ToList()
                        }).ToList()
                    };

                    var combinations = combinationRecords
                        .Join(productCombinations,
                            record => record.CombinationId,
                            combination => combination.Id,
                            (record, combination) => new { Record = record, Combination = combination })
                        .ToList();
                    request.Variants = await combinations.SelectAwait(async combination =>
                    {
                        var variant = new Product.ProductVariant
                        {
                            Uuid = combination.Record.VariantUuid,
                            Name = product.Name,
                            Sku = combination.Combination.Sku,
                            Description = product.Description
                        };

                        //set image
                        if (combination.Record.ImageSyncEnabled && !string.IsNullOrEmpty(combination.Record.ImageUrl))
                            variant.Presentation = new Product.ProductPresentation { ImageUrl = combination.Record.ImageUrl };
							_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): variant.Presentation → {Arg1}", log, variant.Presentation);

                        //set the price if available
                        if (combination.Record.PriceSyncEnabled && priceSyncAvailable)
                        {
                            variant.Price = new Product.ProductVariant.ProductPrice
                            {
                                Amount = preparePrice(combination.Combination.OverriddenPrice ?? product.Price),
                                CurrencyId = accountInfo.Currency
                            };

                            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.Combination.AttributesXml);
							_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): attributeValues → {AttributeValues}", log, attributeValues);
                            var attributesCost = attributeValues
                                .Where(value => value.AttributeValueType == Core.Domain.Catalog.AttributeValueType.Simple)
                                .Sum(value => value.Cost);
                            variant.CostPrice = new Product.ProductVariant.ProductPrice
                            {
                                Amount = preparePrice(product.ProductCost + attributesCost),
                                CurrencyId = accountInfo.Currency
                            };
                        }

                        variant.Options = await (await _productAttributeParser.ParseProductAttributeMappingsAsync(combination.Combination.AttributesXml))
                            .SelectAwait(async mapping =>
                            {
                                var attribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
								_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): attribute → {Attribute}", log, attribute);
                                var values = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.Combination.AttributesXml, mapping.Id);
								_logger.LogDebug("ImportCreatedOrUpdatedAsync(log={Log}): values → {Values}", log, values);
                                return new Product.ProductVariant.ProductVariantOption { Name = attribute.Name, Value = values.FirstOrDefault()?.Name };
                            })
                            .ToListAsync();

                        return variant;
                    }).ToListAsync();
                }
                return request;
            }).ToListAsync();

            log.AppendLine($"\tImport {products.Count} products (#{string.Join(", #", products.Select(product => product.Id).ToList())})");

            import = await _zettleHttpClient.RequestAsync<CreateImportRequest, Import>(new CreateImportRequest { Products = products });

            log.AppendLine($"\t\tImport ({import.Uuid}) created at {import.Created?.ToLongTimeString()}");
			_logger.LogInformation("ImportCreatedOrUpdatedAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
			_logger.LogDebug(">>> Entering UploadImagesAsync(records={Records},update={Update},log={Log})", records, update, log);
			_logger.LogWarning("Throwing NopException from UploadImagesAsync(records={Records},update={Update},log={Log})", records, update, log);
			_logger.LogInformation("UploadImagesAsync(records,update,log)={Records},{Update},{Log}: database query executed in {Arg3} ms", records, update, log, __logStartDb.ElapsedMilliseconds);
			_logger.LogInformation("UploadImagesAsync(records,update,log)={Records},{Update},{Log}: external service call took {Arg3} ms", records, update, log, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting UploadImagesAsync(records={Records},update={Update},log={Log})", records, update, log);
        }
		_logger.LogDebug("<<< Exiting ImportCreatedOrUpdatedAsync(log={Log})", log);
    }

    /// <summary>
    /// Upload images for the passed records
    /// </summary>
    /// <param name="records">Records</param>
    /// <param name="update">Whether to update existing images</param>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UploadImagesAsync(IList<ZettleRecord> records, bool update, StringBuilder log)
    {
		_logger.LogDebug(">>> Entering UploadImagesAsync(records={Records},update={Update},log={Log})", records, update, log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        if (!_mediaSettings.UseAbsoluteImagePath)
            throw new NopException("For the correct image uploading need to use absolute pictures path (MediaSettings.UseAbsoluteImagePath setting)");

        //prepare images to upload
        var recordsWithImages = await records
            .Where(record => record.ImageSyncEnabled && (update || string.IsNullOrEmpty(record.ImageUrl)))
            .SelectAwait(async record =>
            {
                var product = await _productService.GetProductByIdAsync(record.ProductId);
				_logger.LogDebug("UploadImagesAsync(records={Records},update={Update},log={Log}): product → {Product}", records, update, log, product);
                var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(record.CombinationId);
				_logger.LogDebug("UploadImagesAsync(records={Records},update={Update},log={Log}): combination → {Combination}", records, update, log, combination);
                var picture = await _pictureService.GetProductPictureAsync(product, combination?.AttributesXml);
				_logger.LogDebug("UploadImagesAsync(records={Records},update={Update},log={Log}): picture → {Picture}", records, update, log, picture);
                var ext = await _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType);
				_logger.LogDebug("UploadImagesAsync(records={Records},update={Update},log={Log}): ext → {Ext}", records, update, log, ext);
                var (url, _) = await _pictureService.GetPictureUrlAsync(picture);
                return new { Record = record, Url = url, Format = ext };
            })
            .ToListAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        var imagesToUpload = recordsWithImages.Select(record => new CreateImageRequest
        {
            ImageFormat = record.Format?.ToUpper().Replace("JPG", "JPEG"),
            ImageUrl = record.Url
        }).ToList();

        if (!imagesToUpload.Any())
            return;

        log.AppendLine($"\tUpload {recordsWithImages.Count} new images");

        //upload images
        var images = await _zettleHttpClient
            .RequestAsync<CreateImagesRequest, ImageList>(new CreateImagesRequest { ImageUploads = imagesToUpload });

        log.AppendLine($"\t{images.Uploaded?.Count ?? 0} images uploaded successfully and {images.Invalid?.Count ?? 0} failed to upload");

        //set uploaded images URLs to records
        var recordsToUpdate = images.Uploaded?
            .SelectMany(image =>
            {
                var recordsWithUploadedImage = recordsWithImages
                    .Where(record => string.Equals(record.Url, image.Source, StringComparison.InvariantCultureIgnoreCase))
                    .Select(record => record.Record)
                    .ToList();
                foreach (var record in recordsWithUploadedImage)
                {
                    record.ImageUrl = image.ImageUrls?.FirstOrDefault();
                }
                return recordsWithUploadedImage;
            })
            .Distinct()
            .ToList();
        await _zettleRecordService.UpdateRecordsAsync(recordsToUpdate);
		_logger.LogInformation("UploadImagesAsync(records,update,log)={Records},{Update},{Log}: external service call took {Arg3} ms", records, update, log, __logStart.ElapsedMilliseconds);
    }

    #region Inventory

    /// <summary>
    /// Prepare product inventory balance changes
    /// </summary>
    /// <param name="changeType">Inventory balance change type</param>
    /// <param name="productRecord">Product record with initial stock quantity and qunatity adjustment</param>
    /// <param name="combinationRecords">Combination records with initial stock quantity and qunatity adjustment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains list of balance changes
    /// </returns>
    protected async Task<CreateTrackingRequest.ProductBalanceChange> PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType changeType,
        (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecord,
        List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)> combinationRecords)
    {
        //ensure that inventory is tracked for the product
        var product = await _productService.GetProductByIdAsync(productRecord.Record?.ProductId ?? 0);
        if (product is null || product.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.DontManageStock)
            return null;

        var productChange = new CreateTrackingRequest.ProductBalanceChange
        {
            ProductUuid = productRecord.Record.Uuid,
            TrackingStatusChange = changeType == InventoryBalanceChangeType.StartTracking ? "START_TRACKING" : "NO_CHANGE"
        };

        //Zettle Inventory service keeps track of inventory balances by moving product items between so-called locations
        var fromLocation = await (changeType switch
        {
            InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("SUPPLIER"),
            InventoryBalanceChangeType.Purchase or InventoryBalanceChangeType.Void => GetLocationAsync("STORE"),
            _ => GetLocationAsync("SUPPLIER")
        });
        var toLocation = await (changeType switch
        {
            InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("STORE"),
            InventoryBalanceChangeType.Purchase => GetLocationAsync("SOLD"),
            InventoryBalanceChangeType.Void => GetLocationAsync("BIN"),
            _ => GetLocationAsync("BIN")
        });

        if (!combinationRecords.Any())
        {
            //get initial quantity
            var quantity = changeType == InventoryBalanceChangeType.StartTracking
                ? product.StockQuantity - productRecord.StockQuantity
                : productRecord.QuantityAdjustment ?? 0;
            if (quantity != 0)
            {
                productChange.VariantChanges =
                [
                    new()
                    {
                        FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                        ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                        VariantUuid = productRecord.Record.VariantUuid,
                        Change = Math.Abs(quantity)
                    }
                ];
            }
			_logger.LogDebug("<<< Exiting GetLocationAsync(type={Type})", type);
        }
        else
        {
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
            productChange.VariantChanges = await combinationRecords.SelectAwait(async combinationRecord =>
            {
                var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(combinationRecord.Record.CombinationId);

                //get initial quantity
                var quantity = changeType == InventoryBalanceChangeType.StartTracking
                    ? (product.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.ManageStockByAttributes
                        ? (combination?.StockQuantity ?? 0) - combinationRecord.StockQuantity
                        : (product.StockQuantity / combinations.Count) - combinationRecord.StockQuantity)
                    : combinationRecord.QuantityAdjustment ?? 0;
                if (quantity == 0)
                    return null;

                return new CreateTrackingRequest.VariantBalanceChange
                {
                    FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                    ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                    VariantUuid = combinationRecord.Record.VariantUuid,
                    Change = Math.Abs(quantity)
                };
            }).Where(variantChange => variantChange is not null).ToListAsync();
        }

        return productChange;
    }

    /// <summary>
    /// Update inventory balance
    /// </summary>
    /// <param name="productChanges">List of product changes</param>
    /// <param name="records">Records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UpdateInventoryBalanceAsync(List<CreateTrackingRequest.ProductBalanceChange> productChanges, List<ZettleRecord> records)
    {
		_logger.LogDebug(">>> Entering UpdateInventoryBalanceAsync(productChanges={ProductChanges},records={Records})", productChanges, records);
        if (!productChanges.Any())
            return;

        var inventoryRequest = new CreateTrackingRequest
        {
            ReturnLocationUuid = await GetLocationAsync("STORE"),
            ProductChanges = productChanges,
            ExternalUuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
        };

        //save external UUID to avoid a double change, we will check it when receive a webhook event
        foreach (var record in records)
        {
            record.ExternalUuid = inventoryRequest.ExternalUuid;
			_logger.LogDebug("UpdateInventoryBalanceAsync(productChanges={ProductChanges},records={Records}): record.ExternalUuid → {Arg2}", productChanges, records, record.ExternalUuid);
			_logger.LogDebug(">>> Entering GetLocationAsync(type={Type})", type);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetLocationAsync(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        }
        await _zettleRecordService.UpdateRecordsAsync(records);

        //update balances
        await _zettleHttpClient.RequestAsync<CreateTrackingRequest, LocationInventoryBalance>(inventoryRequest);
		_logger.LogInformation("UpdateInventoryBalanceAsync(productChanges,records)={ProductChanges},{Records}: external service call took {Arg2} ms", productChanges, records, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateInventoryBalanceAsync(productChanges={ProductChanges},records={Records})", productChanges, records);
    }

    /// <summary>
    /// Get location UUID by the passed type
    /// </summary>
    /// <param name="type">Location type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains location UUID
    /// </returns>
    protected async Task<string> GetLocationAsync(string type)
    {
		_logger.LogDebug(">>> Entering GetLocationAsync(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!_locations.TryGetValue(type, out var _))
        {
            var locationList = await _zettleHttpClient.RequestAsync<GetLocationsRequest, LocationList>(new());
			_logger.LogDebug("GetLocationAsync(type={Type}): locationList → {LocationList}", type, locationList);
            _locations = locationList.ToDictionary(location => location.Type?.ToUpper(), location => location.Uuid);
        }

		_logger.LogInformation("GetLocationAsync(type)={Type}: external service call took {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetLocationAsync(type={Type})", type);
        return _locations[type];
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(ZettleSettings settings)
    {
		_logger.LogDebug(">>> Entering IsConfigured(settings={Settings})", settings);
        //Client ID and API Key are required to request services
		_logger.LogDebug("<<< Exiting IsConfigured(settings={Settings})", settings);
        return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.ApiKey);
    }

    #region Account

    /// <summary>
    /// Get the authenticated user info
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains user details; error message if exists
    /// </returns>
    public async Task<(UserInfo Result, string Error)> GetUserInfoAsync()
    {
        return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetUserInfoRequest, UserInfo>(new()), false);
    }

    /// <summary>
    /// Get the merchant account info
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains account details; error message if exists
    /// </returns>
    public async Task<(AccountInfo Result, string Error)> GetAccountInfoAsync()
    {
        return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetAccountInfoRequest, AccountInfo>(new()), false);
    }

    /// <summary>
    /// Get the default tax rate
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the default tax rate; error message if exists
    /// </returns>
    public async Task<(decimal? Result, string Error)> GetDefaultTaxRateAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            var taxRates = await _zettleHttpClient.RequestAsync<GetTaxRatesRequest, TaxRateList>(new());
            return taxRates.TaxRates?.FirstOrDefault(rate => rate.IsDefault == true)?.Percentage;
        });
    }

    /// <summary>
    /// Disconnect the app from an associated Zettle organisation
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains disconnect result; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> DisconnectAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            await _zettleHttpClient.RequestAsync<DeleteAppRequest, ApiResponse>(new());
            return true;
        }, false);
    }

    #endregion

    #region Webhooks

    /// <summary>
    /// Create webhook that receive events for the subscribed event types
    /// </summary>
    /// <param name="webhookUrl">Webhook URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook; error message if exists
    /// </returns>
    public async Task<(Subscription Result, string Error)> CreateWebhookAsync(string webhookUrl)
    {
        return await HandleFunctionAsync(async () =>
        {
            //check whether the webhook already exists
            var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
            var existingWebhook = webhooks
                ?.FirstOrDefault(webhook => webhook.Destination?.Equals(webhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (existingWebhook is not null)
                return existingWebhook;

            //or try to create the new one if doesn't
            var (accountInfo, _) = await GetAccountInfoAsync();
            var request = new CreateSubscriptionRequest
            {
                Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                TransportName = "WEBHOOK",
                EventNames = ZettleDefaults.WebhookEventNames,
                Destination = webhookUrl,
                ContactEmail = accountInfo?.ContactEmail
            };

            return await _zettleHttpClient.RequestAsync<CreateSubscriptionRequest, Subscription>(request);
        });
    }

    /// <summary>
    /// Delete webhook
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteWebhookAsync()
    {
        await HandleFunctionAsync(async () =>
        {
			_logger.LogDebug(">>> Entering DeleteWebhookAsync()");
            var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
			_logger.LogDebug("DeleteWebhookAsync(): webhooks → {Webhooks}", webhooks);
            var existingWebhook = webhooks
                ?.FirstOrDefault(webhook => webhook.Destination?.Equals(_zettleSettings.WebhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (existingWebhook is null)
                return false;

            var request = new DeleteSubscriptionsRequest { Uuid = existingWebhook.Uuid };
            await _zettleHttpClient.RequestAsync<DeleteSubscriptionsRequest, ApiResponse>(request);

			_logger.LogDebug("<<< Exiting DeleteWebhookAsync()");
			_logger.LogInformation("DeleteWebhookAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
            return true;
        }, false);
		_logger.LogInformation("DeleteWebhookAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Handle webhook request
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        await HandleFunctionAsync(async () =>
        {
			_logger.LogDebug(">>> Entering HandleWebhookAsync(request={Request})", request);
            using var streamReader = new StreamReader(request.Body);
			_logger.LogDebug("HandleWebhookAsync(request={Request}): streamReader → {StreamReader}", request, streamReader);
            var requestContent = await streamReader.ReadToEndAsync();
			_logger.LogDebug("HandleWebhookAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);
            if (string.IsNullOrEmpty(requestContent))
				_logger.LogWarning("Throwing NopException from HandleWebhookAsync(request={Request})", request);
                throw new NopException("Webhook request content is empty");

            //get webhook message
            var message = JsonConvert.DeserializeObject<Message>(requestContent);
			_logger.LogDebug("HandleWebhookAsync(request={Request}): message → {Message}", request, message);

            //test message is sent during webhook initialization
            if (message.EventName == "TestMessage")
                return true;

            if (string.IsNullOrEmpty(_zettleSettings.WebhookKey))
                throw new NopException("Webhook is not set");

            //ensure that request is signed
            if (!request.Headers.TryGetValue(ZettleDefaults.SignatureHeader, out var signatures))
                throw new NopException("Webhook request not signed by a signature header");

            var messageBytes = Encoding.UTF8.GetBytes($"{message.Timestamp}.{message.Payload}");
			_logger.LogDebug("HandleWebhookAsync(request={Request}): messageBytes → {MessageBytes}", request, messageBytes);
            var keyBytes = Encoding.UTF8.GetBytes(_zettleSettings.WebhookKey);
			_logger.LogDebug("HandleWebhookAsync(request={Request}): keyBytes → {KeyBytes}", request, keyBytes);
            using var cryptographer = new HMACSHA256(keyBytes);
			_logger.LogDebug("HandleWebhookAsync(request={Request}): cryptographer → {Cryptographer}", request, cryptographer);
            var hashBytes = cryptographer.ComputeHash(messageBytes);
			_logger.LogDebug("HandleWebhookAsync(request={Request}): hashBytes → {HashBytes}", request, hashBytes);
            var encryptedString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
			_logger.LogDebug("HandleWebhookAsync(request={Request}): encryptedString → {EncryptedString}", request, encryptedString);
            if (!signatures.Any(signature => signature.Equals(encryptedString, StringComparison.InvariantCultureIgnoreCase)))
                throw new NopException("Webhook request isn't valid");

            switch (message.EventName)
            {
                case "InventoryBalanceChanged":
                {
                    var balanceInfo = JsonConvert.DeserializeObject<InventoryBalanceUpdate>(message.Payload);

                    for (var i = 0; i < (balanceInfo.BalanceBefore ?? new()).Count; i++)
                    {
						_logger.LogDebug("HandleWebhookAsync(request={Request}): balanceInfo → {BalanceInfo}", request, balanceInfo);
                        var balanceBefore = balanceInfo.BalanceBefore?.ElementAtOrDefault(i);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): balanceBefore → {BalanceBefore}", request, balanceBefore);
                        var balanceAfter = balanceInfo.BalanceAfter?.ElementAtOrDefault(i);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): balanceAfter → {BalanceAfter}", request, balanceAfter);

                        if (string.IsNullOrEmpty(balanceBefore?.ProductUuid) || string.IsNullOrEmpty(balanceAfter?.ProductUuid))
                            continue;

                        if (balanceBefore.ProductUuid != balanceAfter.ProductUuid || balanceBefore.VariantUuid != balanceAfter.VariantUuid)
                            continue;

                        if (!balanceBefore.Balance.HasValue || !balanceAfter.Balance.HasValue)
                            continue;

                        var records = await _zettleRecordService.GetAllRecordsAsync(productUuid: balanceAfter.ProductUuid);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): records → {Records}", request, records);
                        var productRecord = records.FirstOrDefault(record => string.Equals(record.VariantUuid, balanceAfter.VariantUuid, StringComparison.InvariantCultureIgnoreCase));
						_logger.LogDebug("HandleWebhookAsync(request={Request}): productRecord → {ProductRecord}", request, productRecord);
                        if (productRecord is null || !productRecord.Active || !productRecord.InventoryTrackingEnabled)
                            continue;

                        //whether the change is initiated by the plugin (inventory balance has already been changed)
                        if (productRecord.ExternalUuid == balanceInfo.ExternalUuid)
                        {
                            //keep external UUID for a day in case of errors when processing webhook requests
                            var balanceChangeDate = balanceInfo.UpdateDetails.Timestamp ?? DateTime.UtcNow;
                            if (balanceChangeDate < DateTime.UtcNow.AddDays(-1))
                            {
                                productRecord.ExternalUuid = null;
								_logger.LogDebug("HandleWebhookAsync(request={Request}): productRecord.ExternalUuid → {Arg1}", request, productRecord.ExternalUuid);
                                await _zettleRecordService.UpdateRecordAsync(productRecord);
                            }
                            continue;
                        }

                        //adjust inventory
                        var product = await _productService.GetProductByIdAsync(productRecord.ProductId);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): product → {Product}", request, product);
                        var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(productRecord.CombinationId);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): combination → {Combination}", request, combination);
                        var quantityToChange = balanceAfter.Balance.Value - balanceBefore.Balance.Value;
                        var logMessage = $"{ZettleDefaults.SystemName} update. Inventory balance changed at {balanceAfter.Created?.ToLongTimeString()}";
						_logger.LogDebug("HandleWebhookAsync(request={Request}): logMessage → {LogMessage}", request, logMessage);
                        await _productService.AdjustInventoryAsync(product, quantityToChange, combination?.AttributesXml, logMessage);
                    }

                    break;
                }
                case "InventoryTrackingStopped":
                {
                    var inventoryTrackingInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { ProductUuid = string.Empty });
                    if (string.IsNullOrEmpty(inventoryTrackingInfo.ProductUuid))
                        break;

                    //stop tracking
                    var records = (await _zettleRecordService.GetAllRecordsAsync(productUuid: inventoryTrackingInfo.ProductUuid)).ToList();
                    foreach (var record in records)
                    {
						_logger.LogDebug("HandleWebhookAsync(request={Request}): inventoryTrackingInfo → {InventoryTrackingInfo}", request, inventoryTrackingInfo);
                        record.InventoryTrackingEnabled = false;
                        record.UpdatedOnUtc = DateTime.UtcNow;
						_logger.LogDebug("HandleWebhookAsync(request={Request}): record.InventoryTrackingEnabled → {Arg1}", request, record.InventoryTrackingEnabled);
                    }
                    await _zettleRecordService.UpdateRecordsAsync(records);

                    break;
                }
				_logger.LogDebug("HandleWebhookAsync(request={Request}): record.UpdatedOnUtc → {Arg1}", request, record.UpdatedOnUtc);

                case "ProductCreated":
                {
                    //use this event only to start inventory tracking for product
                    var productInfo = JsonConvert.DeserializeObject<Product>(message.Payload);
                    var records = await _zettleRecordService.GetAllRecordsAsync(productUuid: productInfo.Uuid);
                    var productRecord = records.FirstOrDefault(record => record.CombinationId == 0);
                    if (productRecord is null || !productRecord.Active || !productRecord.InventoryTrackingEnabled)
                        break;

                    var storeBalance = await _zettleHttpClient
                        .RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());
                    var trackingStarted = storeBalance.TrackedProducts
                        ?.Contains(productRecord.Uuid, StringComparer.InvariantCultureIgnoreCase);
                    if (trackingStarted ?? true)
                        break;

                    var combinationRecords = records.Where(record => record.CombinationId != 0).ToList();
                    var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
                    foreach (var combinationRecord in combinationRecords)
                    {
						_logger.LogDebug("HandleWebhookAsync(request={Request}): productInfo → {ProductInfo}", request, productInfo);
                        combinationRecordsToStart.Add((combinationRecord, 0, null));
                    }
                    (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToStart = (productRecord, 0, null);
                    var productChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                        productRecordToStart, combinationRecordsToStart);
                    if (productChange is null)
                        break;

                    await UpdateInventoryBalanceAsync([productChange], combinationRecords.Union([productRecord]).ToList());

                    break;
                }

				_logger.LogDebug("HandleWebhookAsync(request={Request}): combinationRecords → {CombinationRecords}", request, combinationRecords);
				_logger.LogDebug("HandleWebhookAsync(request={Request}): combinationRecordsToStart → {CombinationRecordsToStart}", request, combinationRecordsToStart);
                case "ApplicationConnectionRemoved":
                {
                    var applicationInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { Type = string.Empty });
                    if (string.IsNullOrEmpty(applicationInfo.Type))
                        break;

                    var warning = applicationInfo.Type;
                    if (applicationInfo.Type.Equals("ApplicationConnectionRemoved", StringComparison.InvariantCultureIgnoreCase) ||
                        applicationInfo.Type.Equals("PersonalAssertionDeleted", StringComparison.InvariantCultureIgnoreCase))
                    {
						_logger.LogDebug("HandleWebhookAsync(request={Request}): applicationInfo → {ApplicationInfo}", request, applicationInfo);
                        warning = "The application was disconnected from PayPal Zettle organization. You need to reconfigure the plugin.";
						_logger.LogDebug("HandleWebhookAsync(request={Request}): warning → {Warning}", request, warning);

                        _zettleSettings.ClientId = string.Empty;
						_logger.LogDebug("HandleWebhookAsync(request={Request}): _zettleSettings.ClientId → {Arg1}", request, _zettleSettings.ClientId);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): _zettleSettings.ApiKey → {Arg1}", request, _zettleSettings.ApiKey);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): _zettleSettings.WebhookUrl → {Arg1}", request, _zettleSettings.WebhookUrl);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): _zettleSettings.WebhookKey → {Arg1}", request, _zettleSettings.WebhookKey);
						_logger.LogDebug("HandleWebhookAsync(request={Request}): _zettleSettings.ImportId → {Arg1}", request, _zettleSettings.ImportId);
                        _zettleSettings.ApiKey = string.Empty;
                        _zettleSettings.WebhookUrl = string.Empty;
                        _zettleSettings.WebhookKey = string.Empty;
                        _zettleSettings.ImportId = string.Empty;
                        await _settingService.SaveSettingAsync(_zettleSettings);
                    }
                    await _logger.WarningAsync($"{ZettleDefaults.SystemName}. {warning}");

                    break;
                }

                default:
                    throw new NopException($"Unknown webhook resource type '{message.EventName}'");
            }

			_logger.LogDebug("<<< Exiting HandleWebhookAsync(request={Request})", request);
			_logger.LogInformation("HandleWebhookAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
            return true;
        });
    }

    #endregion

    #region Sync

    /// <summary>
    /// Get last import details
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details; error message if exists
    /// </returns>
    public async Task<(Import Result, string Error)> GetImportAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            return await _zettleHttpClient.RequestAsync<GetImportRequest, Import>(new() { ImportUuid = _zettleSettings.ImportId });
        }, false);
    }

    /// <summary>
    /// Start products import
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details; error message if exists
    /// </returns>
    public async Task<(Import Result, string Error)> ImportAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            var log = new StringBuilder($"{ZettleDefaults.SystemName} information.{Environment.NewLine}");
            log.AppendLine($"Synchronization started at {DateTime.UtcNow.ToLongTimeString()} UTC");

            await ImportDiscountsAsync(log);

            await ImportDeletedAsync(log);

            await ImportImageChangedAsync(log);

            await ImportInventoryTrackingAsync(log);

            var import = await ImportCreatedOrUpdatedAsync(log);

            if (!string.IsNullOrEmpty(import?.Uuid))
            {
                //save import id for future use
                await _settingService.SetSettingAsync($"{nameof(ZettleSettings)}.{nameof(ZettleSettings.ImportId)}", import?.Uuid);

                //refresh records
                var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                    operationTypes: [OperationType.Create, OperationType.Update, OperationType.ImageChanged]);
                foreach (var record in records)
                {
                    record.OperationType = OperationType.None;
                    record.UpdatedOnUtc = DateTime.UtcNow;
                }
                await _zettleRecordService.UpdateRecordsAsync(records.ToList());
            }

            log.AppendLine($"Synchronization finished at {DateTime.UtcNow.ToLongTimeString()} UTC");

            if (_zettleSettings.LogSyncMessages)
                await _logger.InformationAsync(log.ToString());

            return import;
        });
    }

    #endregion

    #region Inventory

    /// <summary>
    /// Change inventory balance
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="combinationId">Combination identifier</param>
    /// <param name="quantityAdjustment">Stock quantity adjustment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ChangeInventoryBalanceAsync(int productId, int combinationId, int quantityAdjustment)
    {
		_logger.LogDebug(">>> Entering ChangeInventoryBalanceAsync(productId={ProductId},combinationId={CombinationId},quantityAdjustment={QuantityAdjustment})", productId, combinationId, quantityAdjustment);
        var records = (await _zettleRecordService.GetAllRecordsAsync(active: true))
            .Where(record => record.ProductId == productId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        if (!records.Any())
            return;

        var productRecord = records.FirstOrDefault(record => record.CombinationId == 0);
		_logger.LogDebug("ChangeInventoryBalanceAsync(productId={ProductId},combinationId={CombinationId},quantityAdjustment={QuantityAdjustment}): productRecord → {ProductRecord}", productId, combinationId, quantityAdjustment, productRecord);
        var combinationRecords = combinationId > 0
            ? records.Where(record => record.CombinationId == combinationId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            : new List<ZettleRecord>();

        //we cannot know the exact reason of the change, so we will use Purchase for negative adjustments and Re-stock for positive ones
        var changeType = quantityAdjustment < 0 ? InventoryBalanceChangeType.Purchase : InventoryBalanceChangeType.Restock;
        var combinationRecordsToUpdate = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
		_logger.LogDebug("ChangeInventoryBalanceAsync(productId={ProductId},combinationId={CombinationId},quantityAdjustment={QuantityAdjustment}): combinationRecordsToUpdate → {CombinationRecordsToUpdate}", productId, combinationId, quantityAdjustment, combinationRecordsToUpdate);
        foreach (var combinationRecord in combinationRecords)
        {
            combinationRecordsToUpdate.Add((combinationRecord, 0, Math.Abs(quantityAdjustment)));
        }
        (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToUpdate = (productRecord, 0, Math.Abs(quantityAdjustment));
        var productChange = await PrepareInventoryBalanceChangeAsync(changeType, productRecordToUpdate, combinationRecordsToUpdate);
		_logger.LogDebug("ChangeInventoryBalanceAsync(productId={ProductId},combinationId={CombinationId},quantityAdjustment={QuantityAdjustment}): productChange → {ProductChange}", productId, combinationId, quantityAdjustment, productChange);
        if (productChange is null)
            return;

        await UpdateInventoryBalanceAsync([productChange], combinationRecords.Union([productRecord]).ToList());
		_logger.LogDebug("<<< Exiting ChangeInventoryBalanceAsync(productId={ProductId},combinationId={CombinationId},quantityAdjustment={QuantityAdjustment})", productId, combinationId, quantityAdjustment);
    }

    #endregion

    #endregion
}