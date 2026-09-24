using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Authentication;

/// <summary>
/// Represents service using cookie middleware for the authentication
/// </summary>
public partial class CookieAuthenticationService : IAuthenticationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected Customer _cachedCustomer;

    #endregion

    #region Ctor

    public CookieAuthenticationService(CustomerSettings customerSettings,
        ICustomerService customerService,
        IHttpContextAccessor httpContextAccessor)
    {
		_logger.LogError(">>> Entering CookieAuthenticationService(customerSettings={CustomerSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor})", customerSettings, customerService, httpContextAccessor);
        _customerSettings = customerSettings;
        _customerService = customerService;
		_logger.LogDebug("CookieAuthenticationService(customerSettings={CustomerSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor}): _customerService → {_customerService}", customerSettings, customerService, httpContextAccessor, _customerService);
		_logger.LogDebug("CookieAuthenticationService(customerSettings={CustomerSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor}): _customerSettings → {_customerSettings}", customerSettings, customerService, httpContextAccessor, _customerSettings);
    _logger.LogWarning("<<< Exiting CookieAuthenticationService(customerSettings={CustomerSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor})", customerSettings, customerService, httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sign in
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SignInAsync(Customer customer, bool isPersistent)
    {
		_logger.LogError(">>> Entering SignInAsync(customer={Customer},isPersistent={IsPersistent})", customer, isPersistent);
        ArgumentNullException.ThrowIfNull(customer);

        //create claims for customer's username and email
        var claims = new List<Claim>();
		_logger.LogError("SignInAsync(customer={Customer},isPersistent={IsPersistent}): claims → {Claims}", customer, isPersistent, claims);

        if (!string.IsNullOrEmpty(customer.Username))
            claims.Add(new Claim(ClaimTypes.Name, customer.Username, ClaimValueTypes.String, NopAuthenticationDefaults.ClaimsIssuer));

        if (!string.IsNullOrEmpty(customer.Email))
            claims.Add(new Claim(ClaimTypes.Email, customer.Email, ClaimValueTypes.Email, NopAuthenticationDefaults.ClaimsIssuer));

        //create principal for the current authentication scheme
        var userIdentity = new ClaimsIdentity(claims, NopAuthenticationDefaults.AuthenticationScheme);
		_logger.LogError("SignInAsync(customer={Customer},isPersistent={IsPersistent}): userIdentity → {UserIdentity}", customer, isPersistent, userIdentity);
        var userPrincipal = new ClaimsPrincipal(userIdentity);
		_logger.LogError("SignInAsync(customer={Customer},isPersistent={IsPersistent}): userPrincipal → {UserPrincipal}", customer, isPersistent, userPrincipal);

        //set value indicating whether session is persisted and the time at which the authentication was issued
        var authenticationProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            IssuedUtc = DateTime.UtcNow
        };

        //sign in
        await _httpContextAccessor.HttpContext.SignInAsync(NopAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

        //cache authenticated customer
		_logger.LogWarning("<<< Exiting SignInAsync(customer={Customer},isPersistent={IsPersistent})", customer, isPersistent);
        _cachedCustomer = customer;
		_logger.LogDebug("SignInAsync(customer={Customer},isPersistent={IsPersistent}): _cachedCustomer → {_cachedCustomer}", customer, isPersistent, _cachedCustomer);
    }

    /// <summary>
    /// Sign out
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SignOutAsync()
    {
		_logger.LogError(">>> Entering SignOutAsync()");
        //reset cached customer
        _cachedCustomer = null;
		_logger.LogDebug("SignOutAsync(): _cachedCustomer → {_cachedCustomer}", _cachedCustomer);

        //and sign out from the current authentication scheme
        await _httpContextAccessor.HttpContext.SignOutAsync(NopAuthenticationDefaults.AuthenticationScheme);
		_logger.LogWarning("<<< Exiting SignOutAsync()");
    }

    /// <summary>
    /// Get authenticated customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    public virtual async Task<Customer> GetAuthenticatedCustomerAsync()
    {
		_logger.LogError(">>> Entering GetAuthenticatedCustomerAsync()");
        //whether there is a cached customer
        if (_cachedCustomer != null)
            return _cachedCustomer;

        //try to get authenticated user identity
        var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(NopAuthenticationDefaults.AuthenticationScheme);
		_logger.LogError("GetAuthenticatedCustomerAsync(): authenticateResult → {AuthenticateResult}", authenticateResult);
        if (!authenticateResult.Succeeded)
            return null;

        Customer customer = null;
		_logger.LogError("GetAuthenticatedCustomerAsync(): customer → {Customer}", customer);
        if (_customerSettings.UsernamesEnabled)
        {
            //try to get customer by username
            var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                && claim.Issuer.Equals(NopAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
            if (usernameClaim != null)
                customer = await _customerService.GetCustomerByUsernameAsync(usernameClaim.Value);
        }
        else
        {
            //try to get customer by email
            var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                                                                             && claim.Issuer.Equals(NopAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
            if (emailClaim != null)
                customer = await _customerService.GetCustomerByEmailAsync(emailClaim.Value);
        }

        //whether the found customer is available
        if (customer == null || !customer.Active || customer.RequireReLogin || customer.Deleted || !await _customerService.IsRegisteredAsync(customer))
            return null;
            
        static DateTime trimMilliseconds(DateTime dt)
        {
            _logger.LogError(">>> Entering trimMilliseconds(dt={Dt})", dt);
            return new(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }

        //get the latest password
        var customerPassword = await _customerService.GetCurrentPasswordAsync(customer.Id);
		_logger.LogError("GetAuthenticatedCustomerAsync(): customerPassword → {CustomerPassword}", customerPassword);
        //require a customer to re-login after password changing
        var isPasswordChange = trimMilliseconds(customerPassword.CreatedOnUtc).CompareTo(trimMilliseconds(authenticateResult.Properties.IssuedUtc?.DateTime ?? DateTime.UtcNow)) > 0;
		_logger.LogError("GetAuthenticatedCustomerAsync(): isPasswordChange → {IsPasswordChange}", isPasswordChange);
        if (_customerSettings.RequiredReLoginAfterPasswordChange && isPasswordChange)
            return null;

        //cache authenticated customer
        _cachedCustomer = customer;
		_logger.LogDebug("GetAuthenticatedCustomerAsync(): _cachedCustomer → {_cachedCustomer}", _cachedCustomer);

		_logger.LogWarning("<<< Exiting GetAuthenticatedCustomerAsync()");
        return _cachedCustomer;
    }

    #endregion
}