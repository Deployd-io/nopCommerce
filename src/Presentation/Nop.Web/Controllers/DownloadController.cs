using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

public partial class DownloadController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;

    public DownloadController(CustomerSettings customerSettings,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IProductService productService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext})", customerSettings, downloadService, localizationService, orderService, productService, workContext);
        _customerSettings = customerSettings;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _customerSettings → {_customerSettings}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _customerSettings);
        _downloadService = downloadService;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _downloadService → {_downloadService}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _downloadService);
        _localizationService = localizationService;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _localizationService → {_localizationService}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _localizationService);
        _orderService = orderService;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _orderService → {_orderService}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _orderService);
        _productService = productService;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _productService → {_productService}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _productService);
        _workContext = workContext;
		_logger.LogDebug("DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext}): _workContext → {_workContext}", customerSettings, downloadService, localizationService, orderService, productService, workContext, _workContext);
		_logger.LogDebug("<<< Exiting DownloadController(customerSettings={CustomerSettings},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},workContext={WorkContext})", customerSettings, downloadService, localizationService, orderService, productService, workContext);
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> Sample(int productId)
    {
		_logger.LogDebug(">>> Entering Sample(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
			_logger.LogDebug("Sample(productId={ProductId}): product → {Product}", productId, product);
            return InvokeHttp404();

        if (!product.HasSampleDownload)
            return Content("Product doesn't have a sample download.");

        var download = await _downloadService.GetDownloadByIdAsync(product.SampleDownloadId);
		_logger.LogDebug("Sample(productId={ProductId}): download → {Download}", productId, download);
        if (download == null)
            return Content("Sample download is not available any more.");

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        if (download.DownloadBinary == null)
            return Content("Download data is not available any more.");

        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
		_logger.LogDebug("Sample(productId={ProductId}): fileName → {FileName}", productId, fileName);
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("Sample(productId={ProductId}): contentType → {ContentType}", productId, contentType);
		_logger.LogDebug("<<< Exiting Sample(productId={ProductId})", productId);
        return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetDownload(Guid orderItemId, bool agree = false)
    {
		_logger.LogDebug(">>> Entering GetDownload(orderItemId={OrderItemId},agree={Agree})", orderItemId, agree);
        var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
        if (orderItem == null)
			_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): orderItem → {OrderItem}", orderItemId, agree, orderItem);
            return InvokeHttp404();

        var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
		_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): order → {Order}", orderItemId, agree, order);

        if (!await _orderService.IsDownloadAllowedAsync(orderItem))
            return Content("Downloads are not allowed");

        if (_customerSettings.DownloadableProductsValidateUser)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): customer → {Customer}", orderItemId, agree, customer);
            if (customer == null)
                return Challenge();

            if (order.CustomerId != customer.Id)
                return Content("This is not your order");
        }

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
		_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): product → {Product}", orderItemId, agree, product);

        var download = await _downloadService.GetDownloadByIdAsync(product.DownloadId);
		_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): download → {Download}", orderItemId, agree, download);
        if (download == null)
            return Content("Download is not available any more.");

        if (product.HasUserAgreement && !agree)
            return RedirectToRoute(NopRouteNames.Standard.DOWNLOAD_USER_AGREEMENT, new { orderItemId = orderItemId });


        if (!product.UnlimitedDownloads && orderItem.DownloadCount >= product.MaxNumberOfDownloads)
            return Content(string.Format(await _localizationService.GetResourceAsync("DownloadableProducts.ReachedMaximumNumber"), product.MaxNumberOfDownloads));

        if (download.UseDownloadUrl)
        {
            //increase download
            orderItem.DownloadCount++;
            await _orderService.UpdateOrderItemAsync(orderItem);

            //return result
            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            return new RedirectResult(download.DownloadUrl);
        }

        //binary download
        if (download.DownloadBinary == null)
            return Content("Download data is not available any more.");

        //increase download
        orderItem.DownloadCount++;
        await _orderService.UpdateOrderItemAsync(orderItem);

        //return result
        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
		_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): fileName → {FileName}", orderItemId, agree, fileName);
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("GetDownload(orderItemId={OrderItemId},agree={Agree}): contentType → {ContentType}", orderItemId, agree, contentType);
		_logger.LogDebug("<<< Exiting GetDownload(orderItemId={OrderItemId},agree={Agree})", orderItemId, agree);
        return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetLicense(Guid orderItemId)
    {
		_logger.LogDebug(">>> Entering GetLicense(orderItemId={OrderItemId})", orderItemId);
        var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
        if (orderItem == null)
			_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): orderItem → {OrderItem}", orderItemId, orderItem);
            return InvokeHttp404();

        var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
		_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): order → {Order}", orderItemId, order);

        if (!await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
            return Content("Downloads are not allowed");

        if (_customerSettings.DownloadableProductsValidateUser)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): customer → {Customer}", orderItemId, customer);
            if (customer == null || order.CustomerId != customer.Id)
                return Challenge();
        }

        var download = await _downloadService.GetDownloadByIdAsync(orderItem.LicenseDownloadId ?? 0);
		_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): download → {Download}", orderItemId, download);
        if (download == null)
            return Content("Download is not available any more.");

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        //binary download
        if (download.DownloadBinary == null)
            return Content("Download data is not available any more.");

        //return result
        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderItem.ProductId.ToString();
		_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): fileName → {FileName}", orderItemId, fileName);
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("GetLicense(orderItemId={OrderItemId}): contentType → {ContentType}", orderItemId, contentType);
		_logger.LogDebug("<<< Exiting GetLicense(orderItemId={OrderItemId})", orderItemId);
        return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
    }

    public virtual async Task<IActionResult> GetFileUpload(Guid downloadId)
    {
		_logger.LogDebug(">>> Entering GetFileUpload(downloadId={DownloadId})", downloadId);
        var download = await _downloadService.GetDownloadByGuidAsync(downloadId);
        if (download == null)
			_logger.LogDebug("GetFileUpload(downloadId={DownloadId}): download → {Download}", downloadId, download);
            return Content("Download is not available any more.");

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        //binary download
        if (download.DownloadBinary == null)
            return Content("Download data is not available any more.");

        //return result
        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
		_logger.LogDebug("GetFileUpload(downloadId={DownloadId}): fileName → {FileName}", downloadId, fileName);
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("GetFileUpload(downloadId={DownloadId}): contentType → {ContentType}", downloadId, contentType);
		_logger.LogDebug("<<< Exiting GetFileUpload(downloadId={DownloadId})", downloadId);
        return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetOrderNoteFile(int orderNoteId)
    {
		_logger.LogDebug(">>> Entering GetOrderNoteFile(orderNoteId={OrderNoteId})", orderNoteId);
        var orderNote = await _orderService.GetOrderNoteByIdAsync(orderNoteId);
        if (orderNote == null)
			_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): orderNote → {OrderNote}", orderNoteId, orderNote);
            return InvokeHttp404();

        var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId);
		_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): order → {Order}", orderNoteId, order);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): customer → {Customer}", orderNoteId, customer);
        if (customer == null || order.CustomerId != customer.Id)
            return Challenge();

        var download = await _downloadService.GetDownloadByIdAsync(orderNote.DownloadId);
		_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): download → {Download}", orderNoteId, download);
        if (download == null)
            return Content("Download is not available any more.");

        //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
        //In this case, it is not relevant. Url may not be local.
        if (download.UseDownloadUrl)
            return new RedirectResult(download.DownloadUrl);

        //binary download
        if (download.DownloadBinary == null)
            return Content("Download data is not available any more.");

        //return result
        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderNote.Id.ToString();
		_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): fileName → {FileName}", orderNoteId, fileName);
        var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
		_logger.LogDebug("GetOrderNoteFile(orderNoteId={OrderNoteId}): contentType → {ContentType}", orderNoteId, contentType);
		_logger.LogDebug("<<< Exiting GetOrderNoteFile(orderNoteId={OrderNoteId})", orderNoteId);
        return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
    }
}