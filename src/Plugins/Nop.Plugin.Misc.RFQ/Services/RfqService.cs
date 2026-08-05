using System.Globalization;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Services;

/// <summary>
/// Represent the RFQ service
/// </summary>
public class RfqService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IProductService _productService;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Quote> _quoteRepository;
    private readonly IRepository<QuoteItem> _quoteItemRepository;
    private readonly IRepository<RequestQuote> _requestQuoteRepository;
    private readonly IRepository<RequestQuoteItem> _requestQuoteItemRepository;
    private readonly IRepository<ShoppingCartItem> _sciRepository;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly ITaxService _taxService;
    private readonly IWorkContext _workContext;
    private readonly RfqMessageService _rfqMessageService;

    #endregion

    #region Ctor

    public RfqService(ICustomerService customerService,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IProductService productService,
        IRepository<Customer> customerRepository,
        IRepository<Quote> quoteRepository,
        IRepository<QuoteItem> quoteItemRepository,
        IRepository<RequestQuote> requestQuoteRepository,
        IRepository<RequestQuoteItem> requestQuoteItemRepository,
        IRepository<ShoppingCartItem> sciRepository,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWorkContext workContext,
        RfqMessageService rfqMessageService)
    {
        _customerService = customerService;
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _productService = productService;
        _customerRepository = customerRepository;
        _quoteRepository = quoteRepository;
        _quoteItemRepository = quoteItemRepository;
        _requestQuoteRepository = requestQuoteRepository;
        _requestQuoteItemRepository = requestQuoteItemRepository;
        _sciRepository = sciRepository;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _taxService = taxService;
        _workContext = workContext;
        _rfqMessageService = rfqMessageService;
    }

    #endregion

    #region Utilities

    private void LogNote(IAdminNote noteItem, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;

        note = DateTime.UtcNow.ToString(RfqDefaults.DateTimeStringFormat) + $": {note}";

		_logger.LogDebug("LogNote(noteItem={NoteItem},note={Note}): note → {Note1}", noteItem, note, note);
        if (!string.IsNullOrEmpty(noteItem.AdminNotes))
            note += "\r\n";
        else
            noteItem.AdminNotes = string.Empty;
			_logger.LogDebug("LogNote(noteItem={NoteItem},note={Note}): noteItem.AdminNotes → {Arg2}", noteItem, note, noteItem.AdminNotes);

        noteItem.AdminNotes = noteItem.AdminNotes.Insert(0, note);
		_logger.LogDebug("<<< Exiting LogNote(noteItem={NoteItem},note={Note})", noteItem, note);
    }

    private async Task UpdateQuantityWithLogAsync(RequestQuoteItem requestQuoteItem, int newQuantity)
    {
        var note = string.Format(
            await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.QuantityChanged"), requestQuoteItem.RequestedQty,
            newQuantity, await GetCurrentCustomerEmailAsync());

        LogNote(requestQuoteItem, note);

        requestQuoteItem.RequestedQty = newQuantity;
    }

    private async Task UpdateUnitPriceWithLogAsync(RequestQuoteItem requestQuoteItem, decimal newUnitPrice)
    {
        var note = string.Format(
            await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.UnitPriceChanged"), requestQuoteItem.RequestedUnitPrice,
            newUnitPrice, await GetCurrentCustomerEmailAsync());

        LogNote(requestQuoteItem, note);

        requestQuoteItem.RequestedUnitPrice = newUnitPrice;
    }

    private async Task UpdateCustomerNotesWithLogAsync(RequestQuote requestQuote, string newCustomerNotes)
    {
        var note = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.CustomerNoteChanged");

        LogNote(requestQuote, note);

        requestQuote.CustomerNotes = newCustomerNotes;
		_logger.LogDebug(">>> Entering GetCurrentCustomerEmailAsync()");
		_logger.LogDebug("<<< Exiting GetCurrentCustomerEmailAsync()");
    }

    private async Task SetIfExistsAsync(RequestQuoteItem requestQuoteItem, IFormCollection form, string formKey)
    {
        var key = $"{formKey}{requestQuoteItem.Id}";

        if (!form.ContainsKey(key))
            return;

        var formValue = form[key];

        switch (formKey)
        {
            case RfqDefaults.QUANTITY_FORM_KEY:
                if (int.TryParse(formValue, out var quantity) && requestQuoteItem.RequestedQty != quantity)
                    await UpdateQuantityWithLogAsync(requestQuoteItem, quantity);
                break;
            case RfqDefaults.UNIT_PRICE_FORM_KEY:
                if (decimal.TryParse(formValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && requestQuoteItem.RequestedUnitPrice != price)
                    await UpdateUnitPriceWithLogAsync(requestQuoteItem, price);

                break;
        }
		_logger.LogDebug("<<< Exiting SetIfExistsAsync(requestQuoteItem={RequestQuoteItem},form={Form},formKey={FormKey})", requestQuoteItem, form, formKey);
    }

    private async Task<string> GetCurrentCustomerEmailAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("GetCurrentCustomerEmailAsync(): customer → {Customer}", customer);
        return customer.Email;
    }

    private async Task<Quote> InsertQuoteAsync(Quote quote)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        LogNote(quote, string.Format(await _localizationService.GetResourceAsync(quote.RequestQuoteId.HasValue ? "Plugins.Misc.RFQ.AdminQuote.QuoteCreatedByRequest" : "Plugins.Misc.RFQ.AdminQuote.QuoteCreatedManuallyByStoreOwner"), await GetCurrentCustomerEmailAsync()));
        await _quoteRepository.InsertAsync(quote);

        return quote;
    }

    private async Task UpdateRequestQuoteItemAsync(RequestQuoteItem requestQuoteItem)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _requestQuoteItemRepository.UpdateAsync(requestQuoteItem);
		_logger.LogDebug(">>> Entering UpdateRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId},quantity={Quantity},unitPrice={UnitPrice})", requestQuoteItemId, quantity, unitPrice);
		_logger.LogDebug("<<< Exiting UpdateRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId},quantity={Quantity},unitPrice={UnitPrice})", requestQuoteItemId, quantity, unitPrice);
    }

    private async Task UpdateQuoteItemAsync(QuoteItem quoteItem)
    {
		_logger.LogDebug(">>> Entering UpdateQuoteItemAsync(quoteItem={QuoteItem})", quoteItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _quoteItemRepository.UpdateAsync(quoteItem);
		_logger.LogDebug("<<< Exiting UpdateQuoteItemAsync(quoteItem={QuoteItem})", quoteItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("UpdateQuoteItemAsync(quoteItem)={QuoteItem}: update query executed in {Arg1} ms", quoteItem, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("UpdateQuoteItemAsync(quoteItem)={QuoteItem}: update query executed in {Arg1} ms", quoteItem, __logStart.ElapsedMilliseconds);
    private async Task InsertQuoteItemsAsync(IList<QuoteItem> quoteItems)
    {
        if (!quoteItems.Any())
            return;

        await _quoteItemRepository.InsertAsync(quoteItems);
		_logger.LogDebug("<<< Exiting InsertQuoteItemsAsync(quoteItems={QuoteItems})", quoteItems);
    }

    private async Task<Quote> CheckIsQuoteExpiredAsync(Quote quote)
    {
        if (quote == null)
            return null;

        if (!quote.ExpirationDateUtc.HasValue)
            return quote;

        if (quote.Status is QuoteStatus.Expired or QuoteStatus.OrderCreated)
            return quote;

        if (quote.ExpirationDateUtc.Value > DateTime.UtcNow)
            return quote;

        quote.Status = QuoteStatus.Expired;
		_logger.LogDebug("CheckIsQuoteExpiredAsync(quote={Quote}): quote.Status → {Arg1}", quote, quote.Status);
        var note = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.QuoteExpired");

		_logger.LogDebug("CheckIsQuoteExpiredAsync(quote={Quote}): note → {Note}", quote, note);
        LogNote(quote, note);
        await UpdateQuoteAsync(quote);

		_logger.LogDebug("<<< Exiting CheckIsQuoteExpiredAsync(quote={Quote})", quote);
        return quote;
    }

    #endregion

    #region Methods

    #region Request a quote

    /// <summary>
    /// Changes the request quote status and add the note to the admin note field
    /// </summary>
    /// <param name="requestQuote">Request quote to change status</param>
    /// <param name="newStatus">New status to change</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ChangeAndLogRequestQuoteStatusAsync(RequestQuote requestQuote, RequestQuoteStatus newStatus)
    {
        if (requestQuote.Status == newStatus)
            return;

        var note = string.Format(
            await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestQuoteStatusChanged"),
            await _localizationService.GetLocalizedEnumAsync(requestQuote.Status), await _localizationService.GetLocalizedEnumAsync(newStatus),
            await GetCurrentCustomerEmailAsync());

        LogNote(requestQuote, note);
        requestQuote.Status = newStatus;
		_logger.LogDebug("<<< Exiting ChangeAndLogRequestQuoteStatusAsync(requestQuote={RequestQuote},newStatus={NewStatus})", requestQuote, newStatus);
    }

    /// <summary>
    /// Creates request a quote by shopping cart items
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote and its items
    /// </returns>
    public async Task<(RequestQuote requestQuote, List<RequestQuoteItem> requestQuoteItem)> CreateRequestQuoteByShoppingCartAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (!cart.Any())
            return (null, null);

        var requestQuote = new RequestQuote
        {
            CreatedOnUtc = DateTime.UtcNow,
            CustomerId = customer.Id
        };

        var items = await cart.SelectAwait(async item =>
        {
            var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(await _productService.GetProductByIdAsync(item.ProductId),
                (await _shoppingCartService.GetUnitPriceAsync(item, true)).unitPrice);

            var requestQuoteItem = new RequestQuoteItem
            {
                Id = item.Id,
                ProductAttributesXml = item.AttributesXml,
                OriginalProductPrice = shoppingCartUnitPriceWithDiscountBase,
                ProductId = item.ProductId,
                RequestedQty = item.Quantity,
                RequestedUnitPrice = shoppingCartUnitPriceWithDiscountBase,
                RequestQuoteId = requestQuote.Id
            };

            return requestQuoteItem;
        }).ToListAsync();

        return (requestQuote, items);
    }

    /// <summary>
    /// Gets the request quote item by identifier
    /// </summary>
    /// <param name="requestId">Request quote identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request quote item
    /// </returns>
    public async Task<RequestQuote> GetRequestQuoteByIdAsync(int requestId)
    {
		_logger.LogDebug(">>> Entering GetRequestQuoteByIdAsync(requestId={RequestId})", requestId);
		_logger.LogDebug("<<< Exiting GetRequestQuoteByIdAsync(requestId={RequestId})", requestId);
        return await _requestQuoteRepository.GetByIdAsync(requestId);
    }

    /// <summary>
    /// Update request quote
    /// </summary>
    /// <param name="requestQuote">Request a quote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateRequestQuoteAsync(RequestQuote requestQuote)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _requestQuoteRepository.UpdateAsync(requestQuote);
    }

    /// <summary>
    /// Send new request a quote
    /// </summary>
    /// <param name="customerNotes">Customer notes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote identifier
    /// </returns>
    public async Task<int> SendNewRequestAsync(string customerNotes)
    {
		_logger.LogDebug(">>> Entering SendNewRequestAsync(customerNotes={CustomerNotes})", customerNotes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var (request, items) = await CreateRequestQuoteByShoppingCartAsync();

        LogNote(request, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestCreated"), await GetCurrentCustomerEmailAsync()));
        request.Status = RequestQuoteStatus.Submitted;
		_logger.LogDebug("SendNewRequestAsync(customerNotes={CustomerNotes}): request.Status → {Arg1}", customerNotes, request.Status);
		_logger.LogInformation("SendNewRequestAsync(customerNotes)={CustomerNotes}: save query executed in {Arg1} ms", customerNotes, __logStart.ElapsedMilliseconds);
        await _requestQuoteRepository.InsertAsync(request);

        var httpRequest = _httpContextAccessor.HttpContext?.Request;

        if (httpRequest == null)
            return 0;

        var needSave = false;

		_logger.LogDebug("SendNewRequestAsync(customerNotes={CustomerNotes}): needSave → {NeedSave}", customerNotes, needSave);
        if (!string.IsNullOrEmpty(customerNotes) && (!request.CustomerNotes?.Equals(customerNotes) ?? true))
        {
            needSave = true;
            await UpdateCustomerNotesWithLogAsync(request, customerNotes);
        }

        if (needSave)
            await UpdateRequestQuoteAsync(request);

        items ??= await GetRequestQuoteItemsAsync(request.Id);

        if (items.Any() && httpRequest.IsPostRequest() && httpRequest.HasFormContentType)
        {
            var form = await httpRequest.ReadFormAsync();

			_logger.LogDebug("SendNewRequestAsync(customerNotes={CustomerNotes}): form → {Form}", customerNotes, form);
            foreach (var requestQuoteItem in items)
            {
                await SetIfExistsAsync(requestQuoteItem, form, RfqDefaults.QUANTITY_FORM_KEY);
                await SetIfExistsAsync(requestQuoteItem, form, RfqDefaults.UNIT_PRICE_FORM_KEY);

                requestQuoteItem.Id = 0;
                requestQuoteItem.RequestQuoteId = request.Id;

                await InsertRequestQuoteItemAsync(requestQuoteItem);
            }
			_logger.LogDebug(">>> Entering GetRequestQuoteItemsAsync(requestId={RequestId})", requestId);
			_logger.LogDebug("<<< Exiting GetRequestQuoteItemsAsync(requestId={RequestId})", requestId);
        }

        await _rfqMessageService.CustomerSentNewRequestQuoteAsync(request);

		_logger.LogDebug("<<< Exiting SendNewRequestAsync(customerNotes={CustomerNotes})", customerNotes);
        return request.Id;
    }

    /// <summary>
    /// Gets customer requests a quote
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<List<RequestQuote>> GetCustomerRequestsAsync(int customerId)
    {
		_logger.LogDebug(">>> Entering GetCustomerRequestsAsync(customerId={CustomerId})", customerId);
		_logger.LogDebug("<<< Exiting GetCustomerRequestsAsync(customerId={CustomerId})", customerId);
        return await _requestQuoteRepository.Table.Where(p => p.CustomerId == customerId).OrderByDescending(p => p.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Gets customer quotes
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<List<Quote>> GetCustomerQuotesAsync(int customerId)
    {
		_logger.LogDebug(">>> Entering GetCustomerQuotesAsync(customerId={CustomerId})", customerId);
        var statuses = new[] { (int)QuoteStatus.Submitted, (int)QuoteStatus.OrderCreated };

		_logger.LogDebug("GetCustomerQuotesAsync(customerId={CustomerId}): statuses → {Statuses}", customerId, statuses);
		_logger.LogDebug("<<< Exiting GetCustomerQuotesAsync(customerId={CustomerId})", customerId);
        return await _quoteRepository.Table.Where(p => p.CustomerId == customerId && statuses.Contains(p.StatusId))
            .OrderByDescending(p => p.Id).ToListAsync();
    }

    /// <summary>
    /// Search requests a quote
    /// </summary>
    /// <param name="requestQuoteStatusId">Request a quote status identifier</param>
    /// <param name="createdOnFrom">Created on from</param>
    /// <param name="createdOnTo">Created on to</param>
    /// <param name="customerEmail">The customer email</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<IPagedList<RequestQuote>> SearchRequestsQuoteAsync(int requestQuoteStatusId, DateTime? createdOnFrom, DateTime? createdOnTo, string customerEmail, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering SearchRequestsQuoteAsync(requestQuoteStatusId={RequestQuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize})", requestQuoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var requests = await _requestQuoteRepository.GetAllPagedAsync(query =>
        {
            if (createdOnFrom.HasValue)
                query = query.Where(l => createdOnFrom.Value <= l.CreatedOnUtc);

			_logger.LogDebug("SearchRequestsQuoteAsync(requestQuoteStatusId={RequestQuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", requestQuoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize, query);
			_logger.LogInformation("SearchRequestsQuoteAsync(requestQuoteStatusId,createdOnFrom,createdOnTo,customerEmail,pageIndex,pageSize)={RequestQuoteStatusId},{CreatedOnFrom},{CreatedOnTo},{CustomerEmail},{PageIndex},{PageSize}: find query executed in {Arg6} ms", requestQuoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
            if (createdOnTo.HasValue)
                query = query.Where(l => createdOnTo.Value >= l.CreatedOnUtc);

            if (requestQuoteStatusId > 0)
                query = query.Where(l => requestQuoteStatusId == l.StatusId);

            if (!string.IsNullOrEmpty(customerEmail))
            {
                query = from rq in query
                        join customer in _customerRepository.Table on rq.CustomerId equals customer.Id
                        where customer.Email.Contains(customerEmail)
                        select rq;
            }

            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

		_logger.LogDebug("<<< Exiting SearchRequestsQuoteAsync(requestQuoteStatusId={RequestQuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize})", requestQuoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize);
        return requests;
    }

    /// <summary>
    /// Deletes the request quote
    /// </summary>
    /// <param name="requestQuoteId">Request quote identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRequestQuoteAsync(int requestQuoteId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug(">>> Entering DeleteRequestQuoteAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
        var requestQuote = await GetRequestQuoteByIdAsync(requestQuoteId);

		_logger.LogDebug("DeleteRequestQuoteAsync(requestQuoteId={RequestQuoteId}): requestQuote → {RequestQuote}", requestQuoteId, requestQuote);
        await DeleteRequestQuoteAsync(requestQuote);
		_logger.LogDebug("<<< Exiting DeleteRequestQuoteAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
    }

    /// <summary>
    /// Deletes the request quote
    /// </summary>
    /// <param name="requestQuote">Request a quote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRequestQuoteAsync(RequestQuote requestQuote)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (requestQuote == null)
			_logger.LogDebug("<<< Exiting DeleteRequestQuoteAsync(requestQuote={RequestQuote})", requestQuote);
            return;

        if (requestQuote.QuoteId.HasValue)
        {
            var quote = await GetQuoteByIdAsync(requestQuote.QuoteId.Value);
            quote.RequestQuoteId = null;
			_logger.LogDebug("DeleteRequestQuoteAsync(requestQuote={RequestQuote}): quote → {Quote}", requestQuote, quote);
			_logger.LogDebug("DeleteRequestQuoteAsync(requestQuote={RequestQuote}): quote.RequestQuoteId → {Arg1}", requestQuote, quote.RequestQuoteId);
            await UpdateQuoteAsync(quote);
        }

        await _requestQuoteRepository.DeleteAsync(requestQuote);
		_logger.LogInformation("DeleteRequestQuoteAsync(requestQuote)={RequestQuote}: external service call took {Arg1} ms", requestQuote, __logStart.ElapsedMilliseconds);
		_logger.LogInformation("DeleteRequestQuoteAsync(requestQuote)={RequestQuote}: delete query executed in {Arg1} ms", requestQuote, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes the requests a quote by identifiers
    /// </summary>
    /// <param name="ids">Identifiers of request a quote to delete</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRequestsQuoteByIdsAsync(ICollection<int> ids)
    {
		_logger.LogDebug(">>> Entering DeleteRequestsQuoteByIdsAsync(ids={Ids})", ids);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _requestQuoteRepository.DeleteAsync(await _requestQuoteRepository.GetByIdsAsync(ids.ToArray()));
		_logger.LogDebug("<<< Exiting DeleteRequestsQuoteByIdsAsync(ids={Ids})", ids);
    }

	_logger.LogInformation("DeleteRequestsQuoteByIdsAsync(ids)={Ids}: external service call took {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	_logger.LogInformation("DeleteRequestsQuoteByIdsAsync(ids)={Ids}: find query executed in {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
    #endregion

    #region Request quote item

    /// <summary>
    /// Get request quote items
    /// </summary>
    /// <param name="requestId">Request quote identifier to load items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote items
    /// </returns>
    public async Task<List<RequestQuoteItem>> GetRequestQuoteItemsAsync(int requestId)
    {
		_logger.LogDebug(">>> Entering GetRequestQuoteItemsAsync(requestId={RequestId})", requestId);
		_logger.LogDebug("<<< Exiting GetRequestQuoteItemsAsync(requestId={RequestId})", requestId);
        return await _requestQuoteItemRepository.Table
            .Where(p => p.RequestQuoteId == requestId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets request a quote item
    /// </summary>
    /// <param name="requestQuoteItemId">Request an quote item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<RequestQuoteItem> GetRequestQuoteItemByIdAsync(int requestQuoteItemId)
    {
		_logger.LogDebug(">>> Entering GetRequestQuoteItemByIdAsync(requestQuoteItemId={RequestQuoteItemId})", requestQuoteItemId);
		_logger.LogDebug("<<< Exiting GetRequestQuoteItemByIdAsync(requestQuoteItemId={RequestQuoteItemId})", requestQuoteItemId);
        return await _requestQuoteItemRepository.GetByIdAsync(requestQuoteItemId, _ => default);
    }

    /// <summary>
    /// Deletes request a quote item
    /// </summary>
    /// <param name="requestQuoteItemId">Request a quote item identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRequestQuoteItemAsync(int requestQuoteItemId)
    {
		_logger.LogDebug(">>> Entering DeleteRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId})", requestQuoteItemId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var item = await GetRequestQuoteItemByIdAsync(requestQuoteItemId);

        if (item == null)
			_logger.LogDebug("DeleteRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId}): item → {Item}", requestQuoteItemId, item);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("DeleteRequestQuoteItemAsync(requestQuoteItemId)={RequestQuoteItemId}: delete query executed in {Arg1} ms", requestQuoteItemId, __logStart.ElapsedMilliseconds);
            return;

        await _requestQuoteItemRepository.DeleteAsync(item);

        var requestQuote = await GetRequestQuoteByIdAsync(item.RequestQuoteId);
        var note = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestQuoteItemDeleted"), requestQuoteItemId, await GetCurrentCustomerEmailAsync());
		_logger.LogDebug("DeleteRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId}): requestQuote → {RequestQuote}", requestQuoteItemId, requestQuote);
		_logger.LogDebug("DeleteRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId}): note → {Note}", requestQuoteItemId, note);

        LogNote(requestQuote, note);
        await UpdateRequestQuoteAsync(requestQuote);
		_logger.LogInformation("DeleteRequestQuoteItemAsync(requestQuoteItemId)={RequestQuoteItemId}: external service call took {Arg1} ms", requestQuoteItemId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId})", requestQuoteItemId);
    }

    /// <summary>
    /// Updates request a quote item
    /// </summary>
    /// <param name="requestQuoteItemId">Request a quote item identifier</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="unitPrice">Unit price</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateRequestQuoteItemAsync(int requestQuoteItemId, int quantity, decimal unitPrice)
    {
        var item = await GetRequestQuoteItemByIdAsync(requestQuoteItemId);

		_logger.LogDebug("UpdateRequestQuoteItemAsync(requestQuoteItemId={RequestQuoteItemId},quantity={Quantity},unitPrice={UnitPrice}): item → {Item}", requestQuoteItemId, quantity, unitPrice, item);
        if (item == null)
            return;

        if (item.RequestedQty != quantity)
            await UpdateQuantityWithLogAsync(item, quantity);

        if (item.RequestedUnitPrice != unitPrice)
            await UpdateUnitPriceWithLogAsync(item, unitPrice);

        await UpdateRequestQuoteItemAsync(item);
    }

    /// <summary>
    /// Inserts request a quote item
    /// </summary>
    /// <param name="requestQuoteItem">Request a quote item</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the request a quote item</returns>
    public async Task<RequestQuoteItem> InsertRequestQuoteItemAsync(RequestQuoteItem requestQuoteItem)
    {
        LogNote(requestQuoteItem, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestItemCreated"), await GetCurrentCustomerEmailAsync()));

        await _requestQuoteItemRepository.InsertAsync(requestQuoteItem);

        return requestQuoteItem;
    }

    #endregion

    #region Quote

    /// <summary>
    /// Creates the empty quote for specified customer
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new quote 
    /// </returns>
    public async Task<Quote> CreateQuoteAsync(int customerId)
    {
		_logger.LogDebug(">>> Entering CreateQuoteAsync(customerId={CustomerId})", customerId);
        if (customerId <= 0)
            return null;

        var requestQuote = new Quote
        {
            Status = QuoteStatus.CreatedManuallyByStoreOwner,
            CreatedOnUtc = DateTime.UtcNow,
            CustomerId = customerId
        };

		_logger.LogDebug("<<< Exiting CreateQuoteAsync(customerId={CustomerId})", customerId);
        return await InsertQuoteAsync(requestQuote);
    }

    /// <summary>
    /// Deletes the quotes by identifiers
    /// </summary>
    /// <param name="ids">Identifiers of the quote to delete</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteQuotesByIdsAsync(ICollection<int> ids)
    {
		_logger.LogDebug(">>> Entering DeleteQuotesByIdsAsync(ids={Ids})", ids);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _quoteRepository.DeleteAsync(await _quoteRepository.GetByIdsAsync(ids.ToArray()));
		_logger.LogDebug("<<< Exiting DeleteQuotesByIdsAsync(ids={Ids})", ids);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteQuotesByIdsAsync(ids)={Ids}: external service call took {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteQuotesByIdsAsync(ids)={Ids}: find query executed in {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteQuotesByIdsAsync(ids)={Ids}: external service call took {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	_logger.LogInformation("DeleteQuotesByIdsAsync(ids)={Ids}: find query executed in {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Search the quotes
    /// </summary>
    /// <param name="quoteStatusId">Request a quote status identifier</param>
    /// <param name="createdOnFrom">Created on from</param>
    /// <param name="createdOnTo">Created on to</param>
    /// <param name="customerEmail">The customer email</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<IPagedList<Quote>> SearchQuotesAsync(int quoteStatusId, DateTime? createdOnFrom, DateTime? createdOnTo, string customerEmail, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering SearchQuotesAsync(quoteStatusId={QuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize})", quoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var quotes = await _quoteRepository.GetAllPagedAsync(query =>
        {
            if (createdOnFrom.HasValue)
                query = query.Where(l => createdOnFrom.Value <= l.CreatedOnUtc);

			_logger.LogDebug("SearchQuotesAsync(quoteStatusId={QuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", quoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize, query);
			_logger.LogInformation("SearchQuotesAsync(quoteStatusId,createdOnFrom,createdOnTo,customerEmail,pageIndex,pageSize)={QuoteStatusId},{CreatedOnFrom},{CreatedOnTo},{CustomerEmail},{PageIndex},{PageSize}: find query executed in {Arg6} ms", quoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
            if (createdOnTo.HasValue)
                query = query.Where(l => createdOnTo.Value >= l.CreatedOnUtc);

            if (quoteStatusId > 0)
                query = query.Where(l => quoteStatusId == l.StatusId);

            if (!string.IsNullOrEmpty(customerEmail))
            {
                query = from rq in query
                        join customer in _customerRepository.Table on rq.CustomerId equals customer.Id
                        where customer.Email.Contains(customerEmail)
                        select rq;
            }
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

        foreach (var quote in quotes)
            await CheckIsQuoteExpiredAsync(quote);

		_logger.LogDebug("<<< Exiting SearchQuotesAsync(quoteStatusId={QuoteStatusId},createdOnFrom={CreatedOnFrom},createdOnTo={CreatedOnTo},customerEmail={CustomerEmail},pageIndex={PageIndex},pageSize={PageSize})", quoteStatusId, createdOnFrom, createdOnTo, customerEmail, pageIndex, pageSize);
        return quotes;
    }

    /// <summary>
    /// Creates the quote by request a quote
    /// </summary>
    /// <param name="requestQuoteId">Request a quote identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new quote identifier
    /// </returns>
    public async Task<int> CreateQuoteByRequestAsync(int requestQuoteId)
    {
		_logger.LogDebug(">>> Entering CreateQuoteByRequestAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
        var requestQuote = await GetRequestQuoteByIdAsync(requestQuoteId);

		_logger.LogDebug("CreateQuoteByRequestAsync(requestQuoteId={RequestQuoteId}): requestQuote → {RequestQuote}", requestQuoteId, requestQuote);
        if (requestQuote == null)
            return 0;

        var quote = new Quote
        {
            CreatedOnUtc = DateTime.UtcNow,
            AdminNotes = string.Empty,
            RequestQuoteId = requestQuoteId,
            Status = QuoteStatus.CreatedFromRequestQuote,
            CustomerId = requestQuote.CustomerId,
        };

        await InsertQuoteAsync(quote);

        var items = await GetRequestQuoteItemsAsync(requestQuoteId);

		_logger.LogDebug("CreateQuoteByRequestAsync(requestQuoteId={RequestQuoteId}): items → {Items}", requestQuoteId, items);
        await InsertQuoteItemsAsync(items.Select(item => new QuoteItem
        {
            AttributesXml = item.ProductAttributesXml,
            ProductId = item.ProductId,
            OfferedQty = item.RequestedQty,
            QuoteId = quote.Id,
            RequestedQty = item.RequestedQty,
            RequestedUnitPrice = item.RequestedUnitPrice,
            OfferedUnitPrice = item.RequestedUnitPrice,
            RequestQuoteId = item.Id
        }).ToList());

        await ChangeAndLogRequestQuoteStatusAsync(requestQuote, RequestQuoteStatus.QuoteIsCreated);
        requestQuote.QuoteId = quote.Id;
		_logger.LogDebug("CreateQuoteByRequestAsync(requestQuoteId={RequestQuoteId}): requestQuote.QuoteId → {Arg1}", requestQuoteId, requestQuote.QuoteId);
        await UpdateRequestQuoteAsync(requestQuote);

		_logger.LogDebug("<<< Exiting CreateQuoteByRequestAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
        return quote.Id;
    }

    /// <summary>
    /// Gets the quote item by identifier
    /// </summary>
    /// <param name="quoteId">The quote identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote item
    /// </returns>
    public async Task<Quote> GetQuoteByIdAsync(int quoteId)
    {
		_logger.LogDebug(">>> Entering GetQuoteByIdAsync(quoteId={QuoteId})", quoteId);
		_logger.LogDebug("<<< Exiting GetQuoteByIdAsync(quoteId={QuoteId})", quoteId);
        return await CheckIsQuoteExpiredAsync(await _quoteRepository.GetByIdAsync(quoteId));
    }

    /// <summary>
    /// Changes the quote status and add the note to the admin note field
    /// </summary>
    /// <param name="quote">Quote to change status</param>
    /// <param name="newStatus">New status to change</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ChangeAndLogQuoteStatusAsync(Quote quote, QuoteStatus newStatus)
    {
        if (quote.Status == newStatus)
            return;

        var note = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.QuoteStatusChanged"),
            await _localizationService.GetLocalizedEnumAsync(quote.Status), await _localizationService.GetLocalizedEnumAsync(newStatus),
            await GetCurrentCustomerEmailAsync());

        LogNote(quote, note);
        quote.Status = newStatus;
		_logger.LogDebug("<<< Exiting ChangeAndLogQuoteStatusAsync(quote={Quote},newStatus={NewStatus})", quote, newStatus);
    }

    /// <summary>
    /// Update quote
    /// </summary>
    /// <param name="quote">The quote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateQuoteAsync(Quote quote)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _quoteRepository.UpdateAsync(quote);
    }

    /// <summary>
    /// Deletes the quote
    /// </summary>
    /// <param name="quoteId">Quote identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteQuoteAsync(int quoteId)
    {
		_logger.LogDebug(">>> Entering DeleteQuoteAsync(quoteId={QuoteId})", quoteId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var quote = await GetQuoteByIdAsync(quoteId);

        if (quote == null)
			_logger.LogDebug("DeleteQuoteAsync(quoteId={QuoteId}): quote → {Quote}", quoteId, quote);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            return;

        await _quoteRepository.DeleteAsync(quote);
		_logger.LogInformation("DeleteQuoteAsync(quoteId)={QuoteId}: delete query executed in {Arg1} ms", quoteId, __logStart.ElapsedMilliseconds);

        if (quote.RequestQuoteId is null or 0)
            return;

        var requestQuote = await _requestQuoteRepository.GetByIdAsync(quote.RequestQuoteId);

		_logger.LogDebug("DeleteQuoteAsync(quoteId={QuoteId}): requestQuote → {RequestQuote}", quoteId, requestQuote);
		_logger.LogInformation("DeleteQuoteAsync(quoteId)={QuoteId}: find query executed in {Arg1} ms", quoteId, __logStart.ElapsedMilliseconds);
        if (requestQuote == null)
            return;

        requestQuote.QuoteId = null;
		_logger.LogDebug("DeleteQuoteAsync(quoteId={QuoteId}): requestQuote.QuoteId → {Arg1}", quoteId, requestQuote.QuoteId);
		_logger.LogDebug("DeleteQuoteAsync(quoteId={QuoteId}): requestQuote.Status → {Arg1}", quoteId, requestQuote.Status);
        requestQuote.Status = RequestQuoteStatus.Submitted;

        await UpdateRequestQuoteAsync(requestQuote);
		_logger.LogInformation("DeleteQuoteAsync(quoteId)={QuoteId}: external service call took {Arg1} ms", quoteId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteQuoteAsync(quoteId={QuoteId})", quoteId);
    }

    /// <summary>
    /// Creates shopping cart items by quote
    /// </summary>
    /// <param name="quoteId">Quote identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task CreateShoppingCartAsync(int quoteId)
    {
		_logger.LogDebug(">>> Entering CreateShoppingCartAsync(quoteId={QuoteId})", quoteId);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("CreateShoppingCartAsync(quoteId={QuoteId}): customer → {Customer}", quoteId, customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART, customer))
			_logger.LogDebug("<<< Exiting CreateShoppingCartAsync(quoteId={QuoteId})", quoteId);
            throw new NopException("Shopping cart is disabled");

        var store = await _storeContext.GetCurrentStoreAsync();
        var quoteItems = await GetQuoteItemsAsync(quoteId);
		_logger.LogDebug("CreateShoppingCartAsync(quoteId={QuoteId}): store → {Store}", quoteId, store);
		_logger.LogDebug("CreateShoppingCartAsync(quoteId={QuoteId}): quoteItems → {QuoteItems}", quoteId, quoteItems);

        await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);

        //reset checkout info
        await _customerService.ResetCheckoutDataAsync(customer, store.Id);

        foreach (var quoteItem in quoteItems)
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var now = DateTime.UtcNow;
            var shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = store.Id,
                ProductId = quoteItem.ProductId,
                AttributesXml = quoteItem.AttributesXml,
                CustomerEnteredPrice = decimal.Zero,
                Quantity = quoteItem.OfferedQty,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null,
                CreatedOnUtc = now,
                UpdatedOnUtc = now,
                CustomerId = customer.Id
            };

            await _sciRepository.InsertAsync(shoppingCartItem, false);
            quoteItem.ShoppingCartItemId = shoppingCartItem.Id;
			_logger.LogDebug("CreateShoppingCartAsync(quoteId={QuoteId}): quoteItem.ShoppingCartItemId → {Arg1}", quoteId, quoteItem.ShoppingCartItemId);
			_logger.LogInformation("CreateShoppingCartAsync(quoteId)={QuoteId}: save query executed in {Arg1} ms", quoteId, __logStart.ElapsedMilliseconds);
			_logger.LogDebug(">>> Entering GetQuoteItemsAsync(quoteId={QuoteId})", quoteId);
			_logger.LogDebug("<<< Exiting GetQuoteItemsAsync(quoteId={QuoteId})", quoteId);
        }

        await UpdateQuoteItemsAsync(quoteItems);

        //updated "HasShoppingCartItems" property used for performance optimization
        if (!customer.HasShoppingCartItems)
        {
            customer.HasShoppingCartItems = true;
			_logger.LogDebug("CreateShoppingCartAsync(quoteId={QuoteId}): customer.HasShoppingCartItems → {Arg1}", quoteId, customer.HasShoppingCartItems);
            await _customerService.UpdateCustomerAsync(customer);
        }
    }

    #endregion

    #region Quote item

    /// <summary>
    /// Inserts the quote item
    /// </summary>
    /// <param name="quoteItem">Quote item</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the quote item</returns>
    public async Task<QuoteItem> InsertQuoteItemAsync(QuoteItem quoteItem)
    {
		_logger.LogDebug(">>> Entering InsertQuoteItemAsync(quoteItem={QuoteItem})", quoteItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _quoteItemRepository.InsertAsync(quoteItem);

		_logger.LogInformation("InsertQuoteItemAsync(quoteItem)={QuoteItem}: save query executed in {Arg1} ms", quoteItem, __logStart.ElapsedMilliseconds);
        var quote = await GetQuoteByIdAsync(quoteItem.QuoteId);
        var note = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.QuoteItemAdded"),
            quoteItem.Id, await GetCurrentCustomerEmailAsync());
			_logger.LogDebug("InsertQuoteItemAsync(quoteItem={QuoteItem}): quote → {Quote}", quoteItem, quote);

        LogNote(quote, note);
        await UpdateQuoteAsync(quote);

		_logger.LogDebug("<<< Exiting InsertQuoteItemAsync(quoteItem={QuoteItem})", quoteItem);
        return quoteItem;
    }

    /// <summary>
    /// Get quote items
    /// </summary>
    /// <param name="quoteId">Quote identifier to load items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote items
    /// </returns>
    public async Task<List<QuoteItem>> GetQuoteItemsAsync(int quoteId)
    {
		_logger.LogDebug(">>> Entering GetQuoteItemsAsync(quoteId={QuoteId})", quoteId);
		_logger.LogDebug("<<< Exiting GetQuoteItemsAsync(quoteId={QuoteId})", quoteId);
        return await _quoteItemRepository.Table
            .Where(p => p.QuoteId == quoteId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the quote item
    /// </summary>
    /// <param name="quoteItemId">Quote item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item
    /// </returns>
    public async Task<QuoteItem> GetQuoteItemByIdAsync(int quoteItemId)
    {
		_logger.LogDebug(">>> Entering GetQuoteItemByIdAsync(quoteItemId={QuoteItemId})", quoteItemId);
		_logger.LogDebug("<<< Exiting GetQuoteItemByIdAsync(quoteItemId={QuoteItemId})", quoteItemId);
        return await _quoteItemRepository.GetByIdAsync(quoteItemId, _ => default);
    }

    /// <summary>
    /// Deletes the quote item
    /// </summary>
    /// <param name="quoteItemId">Quote item identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteQuoteItemAsync(int quoteItemId)
    {
		_logger.LogDebug(">>> Entering DeleteQuoteItemAsync(quoteItemId={QuoteItemId})", quoteItemId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var item = await GetQuoteItemByIdAsync(quoteItemId);

        if (item == null)
			_logger.LogDebug("DeleteQuoteItemAsync(quoteItemId={QuoteItemId}): item → {Item}", quoteItemId, item);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("DeleteQuoteItemAsync(quoteItemId)={QuoteItemId}: delete query executed in {Arg1} ms", quoteItemId, __logStart.ElapsedMilliseconds);
            return;

        await _quoteItemRepository.DeleteAsync(item);

        var quote = await GetQuoteByIdAsync(item.QuoteId);
        var note = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.QuoteItemDeleted"), quoteItemId, await GetCurrentCustomerEmailAsync());
		_logger.LogDebug("DeleteQuoteItemAsync(quoteItemId={QuoteItemId}): quote → {Quote}", quoteItemId, quote);
		_logger.LogDebug("DeleteQuoteItemAsync(quoteItemId={QuoteItemId}): note → {Note}", quoteItemId, note);

        LogNote(quote, note);
        await UpdateQuoteAsync(quote);
		_logger.LogInformation("DeleteQuoteItemAsync(quoteItemId)={QuoteItemId}: external service call took {Arg1} ms", quoteItemId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteQuoteItemAsync(quoteItemId={QuoteItemId})", quoteItemId);
    }

    /// <summary>
    /// Updates a quote item
    /// </summary>
    /// <param name="quoteItemId">A quote item identifier</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="unitPrice">Unit price</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateQuoteItemAsync(int quoteItemId, int quantity, decimal unitPrice)
    {
        var item = await GetQuoteItemByIdAsync(quoteItemId);

		_logger.LogDebug("UpdateQuoteItemAsync(quoteItemId={QuoteItemId},quantity={Quantity},unitPrice={UnitPrice}): item → {Item}", quoteItemId, quantity, unitPrice, item);
        if (item == null)
            return;

        if (item.OfferedQty == quantity && item.OfferedUnitPrice == unitPrice)
            return;

        item.OfferedQty = quantity;
		_logger.LogDebug("UpdateQuoteItemAsync(quoteItemId={QuoteItemId},quantity={Quantity},unitPrice={UnitPrice}): item.OfferedQty → {Arg3}", quoteItemId, quantity, unitPrice, item.OfferedQty);
		_logger.LogDebug("UpdateQuoteItemAsync(quoteItemId={QuoteItemId},quantity={Quantity},unitPrice={UnitPrice}): item.OfferedUnitPrice → {Arg3}", quoteItemId, quantity, unitPrice, item.OfferedUnitPrice);
        item.OfferedUnitPrice = unitPrice;

        await UpdateQuoteItemAsync(item);
    }

    /// <summary>
    /// Gets the quote item by shopping cart item identifier
    /// </summary>
    /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote item
    /// </returns>
    public async Task<QuoteItem> GetQuoteItemByShoppingCartItemIdAsync(int shoppingCartItemId)
    {
		_logger.LogDebug(">>> Entering GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemId={ShoppingCartItemId})", shoppingCartItemId);
		_logger.LogDebug("<<< Exiting GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemId={ShoppingCartItemId})", shoppingCartItemId);
        return await _quoteItemRepository.Table.FirstOrDefaultAsync(qi => qi.ShoppingCartItemId == shoppingCartItemId);
    }

    /// <summary>
    /// Gets the quote items by shopping cart item identifiers
    /// </summary>
    /// <param name="shoppingCartItemIds">Shopping cart item identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of quote item
    /// </returns>
    public async Task<List<QuoteItem>> GetQuoteItemsByShoppingCartItemIdsAsync(IList<int> shoppingCartItemIds)
    {
		_logger.LogDebug(">>> Entering GetQuoteItemsByShoppingCartItemIdsAsync(shoppingCartItemIds={ShoppingCartItemIds})", shoppingCartItemIds);
		_logger.LogDebug("<<< Exiting GetQuoteItemsByShoppingCartItemIdsAsync(shoppingCartItemIds={ShoppingCartItemIds})", shoppingCartItemIds);
        return await _quoteItemRepository.Table.Where(qi => qi.ShoppingCartItemId.HasValue && shoppingCartItemIds.Contains(qi.ShoppingCartItemId.Value)).ToListAsync();
    }

    /// <summary>
    /// Updates request a quote items
    /// </summary>
    /// <param name="quoteItems">Request a quote items for update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateQuoteItemsAsync(List<QuoteItem> quoteItems)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _quoteItemRepository.UpdateAsync(quoteItems);
    }

    #endregion

    #endregion
}