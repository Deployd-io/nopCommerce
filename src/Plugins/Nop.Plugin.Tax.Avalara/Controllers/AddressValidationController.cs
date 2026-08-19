using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

[AutoValidateAntiforgeryToken]
public class AddressValidationController : BaseController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICustomerService _customerService;
    protected readonly IWorkContext _workContext;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public AddressValidationController(IAddressService addressService,
        ICustomerService customerService,
        IWorkContext workContext,
        TaxSettings taxSettings)
    {
		_logger.LogDebug(">>> Entering AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings})", addressService, customerService, workContext, taxSettings);
        _addressService = addressService;
        _customerService = customerService;
		_logger.LogDebug("AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings}): _customerService → {_customerService}", addressService, customerService, workContext, taxSettings, _customerService);
		_logger.LogDebug("AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings}): _addressService → {_addressService}", addressService, customerService, workContext, taxSettings, _addressService);
        _workContext = workContext;
		_logger.LogDebug("AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings}): _workContext → {_workContext}", addressService, customerService, workContext, taxSettings, _workContext);
        _taxSettings = taxSettings;
		_logger.LogDebug("AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", addressService, customerService, workContext, taxSettings, _taxSettings);
		_logger.LogDebug("<<< Exiting AddressValidationController(addressService={AddressService},customerService={CustomerService},workContext={WorkContext},taxSettings={TaxSettings})", addressService, customerService, workContext, taxSettings);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> UseValidatedAddress(int addressId, bool isNewAddress)
    {
		_logger.LogDebug(">>> Entering UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress})", addressId, isNewAddress);
        //try to get an address by the passed identifier
        var address = await _addressService.GetAddressByIdAsync(addressId);
		_logger.LogDebug("UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress}): address → {Address}", addressId, isNewAddress, address);
        if (address != null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress}): customer → {Customer}", addressId, isNewAddress, customer);
            //add address to customer collection if it's a new
            if (isNewAddress)
                await _customerService.InsertCustomerAddressAsync(customer, address);

            //and update appropriate customer address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress)
                (customer).BillingAddressId = address.Id;
            if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress)
                (customer).ShippingAddressId = address.Id;
            await _customerService.UpdateCustomerAsync(customer);
        }

        //nothing to return
		_logger.LogDebug("<<< Exiting UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress})", addressId, isNewAddress);
        return Content(string.Empty);
    }

    #endregion
}