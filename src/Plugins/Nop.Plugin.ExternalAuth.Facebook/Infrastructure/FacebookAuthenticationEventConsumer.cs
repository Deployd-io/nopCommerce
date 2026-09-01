using System.Security.Claims;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;
using Nop.Services.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure;

/// <summary>
/// Facebook authentication event consumer (used for saving customer fields on registration)
/// </summary>
public class FacebookAuthenticationEventConsumer : IConsumer<CustomerAutoRegisteredByExternalMethodEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public FacebookAuthenticationEventConsumer(ICustomerService customerService)
    {
		_logger.LogDebug(">>> Entering FacebookAuthenticationEventConsumer(customerService={CustomerService})", customerService);
        _customerService = customerService;
		_logger.LogDebug("FacebookAuthenticationEventConsumer(customerService={CustomerService}): _customerService → {_customerService}", customerService, _customerService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        if (eventMessage?.Customer == null || eventMessage.AuthenticationParameters == null)
            return;

        //handle event only for this authentication method
        if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(FacebookAuthenticationDefaults.SystemName))
            return;

        var customer = eventMessage.Customer;
        //store some of the customer fields
        var firstName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value;
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): firstName → {FirstName}", eventMessage, firstName);
        if (!string.IsNullOrEmpty(firstName))
            customer.FirstName = firstName;
			_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): customer.FirstName → {Arg1}", eventMessage, customer.FirstName);

        var lastName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value;
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): lastName → {LastName}", eventMessage, lastName);
        if (!string.IsNullOrEmpty(lastName))
            customer.LastName = lastName;
			_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): customer.LastName → {Arg1}", eventMessage, customer.LastName);

        await _customerService.UpdateCustomerAsync(customer);
		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}