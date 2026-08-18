using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Http;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Customer;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[AutoValidateAntiforgeryToken]
public class RfqCustomerController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly CustomerModelFactory _modelFactory;
    private readonly ICustomerService _customerService;
    private readonly IPermissionService _permissionService;
    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public RfqCustomerController(CustomerModelFactory modelFactory,
        ICustomerService customerService,
        IPermissionService permissionService,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
		_logger.LogDebug(">>> Entering RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings})", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings);
        _modelFactory = modelFactory;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _modelFactory → {_modelFactory}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _modelFactory);
        _customerService = customerService;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _customerService → {_customerService}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _customerService);
        _permissionService = permissionService;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _permissionService → {_permissionService}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _permissionService);
        _productService = productService;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _productService → {_productService}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _productService);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _shoppingCartService → {_shoppingCartService}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _shoppingCartService);
        _storeContext = storeContext;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _storeContext → {_storeContext}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _workContext → {_workContext}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _workContext);
        _rfqService = rfqService;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqService → {_rfqService}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _rfqService);
        _rfqSettings = rfqSettings;
		_logger.LogDebug("RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqSettings → {_rfqSettings}", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings, _rfqSettings);
		_logger.LogDebug("<<< Exiting RfqCustomerController(modelFactory={ModelFactory},customerService={CustomerService},permissionService={PermissionService},productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings})", modelFactory, customerService, permissionService, productService, storeContext, workContext, rfqService, rfqSettings);
    }

    #endregion

    #region Utilities

    private async Task<IActionResult> CheckCustomerPermissionAsync(Customer customer)
    {
		_logger.LogDebug(">>> Entering CheckCustomerPermissionAsync(customer={Customer})", customer);
        if (!_rfqSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (!await _permissionService.AuthorizeAsync(RfqPermissionConfigManager.ACCESS_RFQ, customer))
		_logger.LogDebug("<<< Exiting CheckCustomerPermissionAsync(customer={Customer})", customer);
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

		_logger.LogDebug("<<< Exiting CheckCustomerPermissionAsync(request={Request})", request);
        return null;
    }

    private async Task<IActionResult> CheckCustomerPermissionAsync(RequestQuote request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        if (request == null)
            return null;

        if (request.CustomerId != customer.Id)
            return RedirectToAction("CustomerRequests");

        return null;
    }
    
    #endregion

    #region Methods

    #region Requests

    public async Task<IActionResult> CustomerRequest(int? requestId = null)
    {
		_logger.LogDebug(">>> Entering CustomerRequest(requestId={RequestId})", requestId);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("CustomerRequest(requestId={RequestId}): customer → {Customer}", requestId, customer);
        var result = await CheckCustomerPermissionAsync(customer);
		_logger.LogDebug("CustomerRequest(requestId={RequestId}): result → {Result}", requestId, result);

        if (result != null)
            return result;

        RequestQuoteModel model;

        if (requestId != null)
        {
            try
            {
                model = await _modelFactory.PrepareRequestQuoteModelAsync(requestId.Value);
            }
            catch (ArgumentNullException)
            {
				_logger.LogError("Exception in CustomerRequest(requestId={RequestId}): {Arg1}", requestId, ArgumentNullException.getMessage());
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
            }

            if (model.CustomerId != customer.Id)
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }
        else
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("CustomerRequest(requestId={RequestId}): store → {Store}", requestId, store);
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
			_logger.LogDebug("CustomerRequest(requestId={RequestId}): cart → {Cart}", requestId, cart);

            var products = await _productService.GetProductsByIdsAsync(cart.Select(i => i.ProductId).ToArray());
			_logger.LogDebug("CustomerRequest(requestId={RequestId}): products → {Products}", requestId, products);

            //"enter your price" or rental products in the cart, so we should redirect customer to shopping cart page
            if (products.Any(product => product.CustomerEntersPrice || product.IsRental))
                return RedirectToRoute(NopRouteNames.General.CART);

            var (request, items) = await _rfqService.CreateRequestQuoteByShoppingCartAsync();

            if (request == null)
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

            model = await _modelFactory.PrepareRequestQuoteModelAsync(request, items);
        }

		_logger.LogDebug("<<< Exiting CustomerRequest(requestId={RequestId})", requestId);
        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("send")]
    public async Task<IActionResult> SendRequest(RequestQuoteModel model)
    {
		_logger.LogDebug(">>> Entering SendRequest(model={Model})", model);
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

		_logger.LogDebug("SendRequest(model={Model}): result → {Result}", model, result);
        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var checkResult = await CheckCustomerPermissionAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id));
			_logger.LogDebug("SendRequest(model={Model}): checkResult → {CheckResult}", model, checkResult);

            if (checkResult != null)
                return checkResult;

            model.Id = await _rfqService.SendNewRequestAsync(model.CustomerNotes);

            return RedirectToAction("CustomerRequest", "RfqCustomer", new { requestId = model.Id });
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);
		_logger.LogDebug("SendRequest(model={Model}): model → {Model1}", model, model);

		_logger.LogDebug("<<< Exiting SendRequest(model={Model})", model);
        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("back-to-cart")]
    public IActionResult BackToCart(RequestQuoteModel model)
    {
		_logger.LogDebug(">>> Entering BackToCart(model={Model})", model);
		_logger.LogDebug("<<< Exiting BackToCart(model={Model})", model);
        return RedirectToRoute(NopRouteNames.General.CART);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("cancel")]
    public async Task<IActionResult> CancelRequest(RequestQuoteModel model)
    {
		_logger.LogDebug(">>> Entering CancelRequest(model={Model})", model);
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

		_logger.LogDebug("CancelRequest(model={Model}): result → {Result}", model, result);
        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var request = await _rfqService.GetRequestQuoteByIdAsync(model.Id);
			_logger.LogDebug("CancelRequest(model={Model}): request → {Request}", model, request);

            if (request == null)
                return RedirectToAction("CustomerRequests");

            var checkResult = await CheckCustomerPermissionAsync(request);
			_logger.LogDebug("CancelRequest(model={Model}): checkResult → {CheckResult}", model, checkResult);

            if (checkResult != null)
                return checkResult;

            await _rfqService.ChangeAndLogRequestQuoteStatusAsync(request, RequestQuoteStatus.Canceled);
            await _rfqService.UpdateRequestQuoteAsync(request);

            return await CustomerRequest(model.Id);
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);
		_logger.LogDebug("CancelRequest(model={Model}): model → {Model1}", model, model);

		_logger.LogDebug("<<< Exiting CancelRequest(model={Model})", model);
        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("delete")]
    public async Task<IActionResult> DeleteRequest(RequestQuoteModel model)
    {
		_logger.LogDebug(">>> Entering DeleteRequest(model={Model})", model);
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

		_logger.LogDebug("DeleteRequest(model={Model}): result → {Result}", model, result);
        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var request = await _rfqService.GetRequestQuoteByIdAsync(model.Id);
			_logger.LogDebug("DeleteRequest(model={Model}): request → {Request}", model, request);

            var checkResult = await CheckCustomerPermissionAsync(request);
			_logger.LogDebug("DeleteRequest(model={Model}): checkResult → {CheckResult}", model, checkResult);

            if (checkResult != null)
                return checkResult;

            await _rfqService.DeleteRequestQuoteAsync(request);

            return RedirectToAction("CustomerRequests");
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);
		_logger.LogDebug("DeleteRequest(model={Model}): model → {Model1}", model, model);

		_logger.LogDebug("<<< Exiting DeleteRequest(model={Model})", model);
        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    public async Task<IActionResult> CustomerRequests()
    {
		_logger.LogDebug(">>> Entering CustomerRequests()");
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("CustomerRequests(): customer → {Customer}", customer);
        var result = await CheckCustomerPermissionAsync(customer);
		_logger.LogDebug("CustomerRequests(): result → {Result}", result);

        if (result != null)
            return result;

        var items = await _rfqService.GetCustomerRequestsAsync(customer.Id);
		_logger.LogDebug("CustomerRequests(): items → {Items}", items);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = await items.SelectAwait(async item =>
            await _modelFactory.PrepareRequestQuoteModelAsync(item, new List<RequestQuoteItem>())).ToListAsync();
			_logger.LogInformation("CustomerRequests(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting CustomerRequests()");
        return View("~/Plugins/Misc.RFQ/Views/CustomerRequests.cshtml", model);
    }

    #endregion

    #region Quotes

    public async Task<IActionResult> CustomerQuotes()
    {
		_logger.LogDebug(">>> Entering CustomerQuotes()");
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("CustomerQuotes(): customer → {Customer}", customer);
        var result = await CheckCustomerPermissionAsync(customer);
		_logger.LogDebug("CustomerQuotes(): result → {Result}", result);

        if (result != null)
            return result;

        var items = await _rfqService.GetCustomerQuotesAsync(customer.Id);
		_logger.LogDebug("CustomerQuotes(): items → {Items}", items);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = await items
            .SelectAwait(async item => await _modelFactory.PrepareQuoteModelAsync(item, new List<QuoteItem>()))
            .ToListAsync();
			_logger.LogInformation("CustomerQuotes(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting CustomerQuotes()");
        return View("~/Plugins/Misc.RFQ/Views/CustomerQuotes.cshtml", model);
    }

    public async Task<IActionResult> CustomerQuote(int quoteId)
    {
		_logger.LogDebug(">>> Entering CustomerQuote(quoteId={QuoteId})", quoteId);
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

		_logger.LogDebug("CustomerQuote(quoteId={QuoteId}): result → {Result}", quoteId, result);
        if (result != null)
            return result;

        QuoteModel model;
        
        try
        {
            model = await _modelFactory.PrepareQuoteModelAsync(quoteId);
        }
        catch (ArgumentNullException)
        {
			_logger.LogError("Exception in CustomerQuote(quoteId={QuoteId}): {Arg1}", quoteId, ArgumentNullException.getMessage());
            return RedirectToAction("CustomerQuotes");
        }

        if (model.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
            return RedirectToAction("CustomerQuotes");

        var statuses = new[] { QuoteStatus.Submitted, QuoteStatus.OrderCreated };

        if (!statuses.Contains(model.StatusType))
		_logger.LogDebug("<<< Exiting CustomerQuote(quoteId={QuoteId})", quoteId);
            return RedirectToAction("CustomerQuotes");

		_logger.LogDebug("<<< Exiting CustomerQuote(model={Model})", model);
        return View("~/Plugins/Misc.RFQ/Views/CustomerQuote.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("createOrder")]
    public async Task<IActionResult> CustomerQuote(QuoteModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            if (model.CustomerId != customer.Id)
                return RedirectToAction("CustomerQuotes");

            var quote = await _rfqService.GetQuoteByIdAsync(model.Id);
            if (quote.Status is QuoteStatus.Expired or QuoteStatus.OrderCreated)
                return RedirectToAction("CustomerQuotes");

            await _rfqService.CreateShoppingCartAsync(model.Id);

            return RedirectToRoute(NopRouteNames.General.CART);
        }

        model = await _modelFactory.PrepareQuoteModelAsync(model.Id, model);

        return View("~/Plugins/Misc.RFQ/Views/CustomerQuote.cshtml", model);
    }

    public async Task<IActionResult> ExitQuoteMode()
    {
		_logger.LogDebug(">>> Entering ExitQuoteMode()");
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("ExitQuoteMode(): customer → {Customer}", customer);
        var result = await CheckCustomerPermissionAsync(customer);
		_logger.LogDebug("ExitQuoteMode(): result → {Result}", result);

        if (result != null)
            return result;

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("ExitQuoteMode(): store → {Store}", store);

        await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);

		_logger.LogDebug("<<< Exiting ExitQuoteMode()");
        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
    }

    #endregion

    #endregion
}
