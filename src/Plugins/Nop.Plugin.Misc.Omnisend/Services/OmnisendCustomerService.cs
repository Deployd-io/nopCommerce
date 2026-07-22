using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.DTO.Events;
using Nop.Services.Common;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the helper class for customer
/// </summary>
public class OmnisendCustomerService
{
    #region Fields

    private readonly IAddressService _addressService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;
    private readonly OmnisendHttpClient _httpClient;

    #endregion

    #region Ctor

    public OmnisendCustomerService(IAddressService addressService,
        IGenericAttributeService genericAttributeService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper,
        OmnisendHttpClient httpClient)
    {
        _addressService = addressService;
        _genericAttributeService = genericAttributeService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task<string> GetCartIdAsync(Customer customer)
    {
		logger.LogDebug(">>> Entering GetCartIdAsync(customer={Customer})", customer);
        var cartId = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute);
			logger.LogDebug("GetCartIdAsync(customer={Customer}): cartId → {CartId}", customer, cartId);

        cartId = string.IsNullOrEmpty(cartId)
            ? await _genericAttributeService.GetAttributeAsync<string>(customer,
                OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute)
            : cartId;

        if (!string.IsNullOrEmpty(cartId))
            return cartId;

        cartId = Guid.NewGuid().ToString();

        await _genericAttributeService.SaveAttributeAsync(customer,
            OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute, cartId);

		logger.LogDebug("<<< Exiting GetCartIdAsync(customer={Customer})", customer);
        return cartId;
    }

    /// <summary>
    /// Gets the customer email address
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="billingAddressId">billing address identifier</param>
    public async Task<string> GetEmailAsync(Customer customer, int? billingAddressId = null)
    {
		logger.LogDebug(">>> Entering GetEmailAsync(customer={Customer},billingAddressId={BillingAddressId})", customer, billingAddressId);
        var email = !string.IsNullOrEmpty(customer.Email)
            ? customer.Email
            : await _genericAttributeService.GetAttributeAsync<string>(customer,
                OmnisendDefaults.CustomerEmailAttribute);

		logger.LogDebug("<<< Exiting GetEmailAsync(customer={Customer},billingAddressId={BillingAddressId})", customer, billingAddressId);
        return !string.IsNullOrEmpty(email)
            ? email
            : (await _addressService.GetAddressByIdAsync((billingAddressId ?? customer.BillingAddressId) ?? 0))
            ?.Email;
    }

    /// <summary>
    /// Create customer event
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="properties">Event properties</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains customer event or null if customer email is not determinate
    /// </returns>
    public async Task<CustomerEvents> CreateCustomerEventAsync(Customer customer, IEventProperty properties)
    {
		logger.LogDebug(">>> Entering CreateCustomerEventAsync(customer={Customer},properties={Properties})", customer, properties);
        var email = await GetEmailAsync(customer);

		logger.LogDebug("CreateCustomerEventAsync(customer={Customer},properties={Properties}): email → {Email}", customer, properties, email);
        if (string.IsNullOrEmpty(email))
            return null;

        var customerEvent = new CustomerEvents { Email = email, Properties = properties };

		logger.LogDebug("<<< Exiting CreateCustomerEventAsync(customer={Customer},properties={Properties})", customer, properties);
        return customerEvent;
    }

    /// <summary>
    /// Gets the contact identifier
    /// </summary>
    /// <param name="email">Email to determinate customer</param>
    public async Task<string> GetContactIdAsync(string email)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}&limit=1";

		logger.LogDebug("GetContactIdAsync(email={Email}): url → {Url}", email, url);
        var res = await _httpClient.PerformRequestAsync(url, httpMethod: HttpMethod.Get);

		logger.LogInformation("GetContactIdAsync(email)={Email}: external service call {Url} took {Arg2} ms", email, url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("GetContactIdAsync(email={Email}): res → {Res}", email, res);
        if (string.IsNullOrEmpty(res))
            return null;

        var contact = JsonConvert
            .DeserializeAnonymousType(res, new { contacts = new[] { new { contactID = string.Empty } } })?.contacts
            .FirstOrDefault();

		logger.LogDebug("<<< Exiting GetContactIdAsync(email={Email})", email);
        return contact?.contactID;
    }

    /// <summary>
    /// Store the cart identifier
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task StoreCartIdAsync(Customer customer)
    {
		logger.LogDebug(">>> Entering StoreCartIdAsync(customer={Customer})", customer);
        var cartId = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute);

        if (string.IsNullOrEmpty(cartId))
            await _genericAttributeService.SaveAttributeAsync(customer,
                OmnisendDefaults.StoredCustomerShoppingCartIdAttribute, await GetCartIdAsync(customer));
				logger.LogDebug("<<< Exiting StoreCartIdAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Delete the current shopping cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task DeleteCurrentCustomerShoppingCartIdAsync(Customer customer)
    {
		logger.LogDebug(">>> Entering DeleteCurrentCustomerShoppingCartIdAsync(customer={Customer})", customer);
        await _genericAttributeService.SaveAttributeAsync<string>(customer,
            OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute, null);
			logger.LogDebug("<<< Exiting DeleteCurrentCustomerShoppingCartIdAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Specifies whether to send the delete shopping cart event
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the True if we need to sand delete events</returns>
    public async Task<bool> IsNeedToSendDeleteShoppingCartEventAsync(Customer customer)
    {
		logger.LogDebug(">>> Entering IsNeedToSendDeleteShoppingCartEventAsync(customer={Customer})", customer);
		logger.LogDebug("<<< Exiting IsNeedToSendDeleteShoppingCartEventAsync(customer={Customer})", customer);
        return string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute));
    }

    /// <summary>
    /// Delete the stored shopping cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task DeleteStoredCustomerShoppingCartIdAsync(Customer customer)
    {
		logger.LogDebug(">>> Entering DeleteStoredCustomerShoppingCartIdAsync(customer={Customer})", customer);
        await _genericAttributeService.SaveAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute, null);
			logger.LogDebug("<<< Exiting DeleteStoredCustomerShoppingCartIdAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Gets the abandoned checkout url
    /// </summary>
    /// <param name="cartId">Cart identifier</param>
    /// <returns>The abandoned checkout url</returns>
    public string GetAbandonedCheckoutUrl(string cartId)
    {
        return _nopUrlHelper.RouteUrl(OmnisendDefaults.AbandonedCheckoutRouteName, new { cartId }, _webHelper.GetCurrentRequestProtocol());
    }

    #endregion
}
