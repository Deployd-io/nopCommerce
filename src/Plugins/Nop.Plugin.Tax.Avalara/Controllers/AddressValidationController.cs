using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.Avalara.Controllers;

[AutoValidateAntiforgeryToken]
public class AddressValidationController : BaseController
{
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
        _addressService = addressService;
        _customerService = customerService;
        _workContext = workContext;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> UseValidatedAddress(int addressId, bool isNewAddress)
    {
		logger.LogDebug(">>> Entering UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress})", addressId, isNewAddress);
        //try to get an address by the passed identifier
        var address = await _addressService.GetAddressByIdAsync(addressId);
        if (address != null)
		logger.LogDebug("UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress}): address → {Address}", addressId, isNewAddress, address);
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            //add address to customer collection if it's a new
			logger.LogDebug("UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress}): customer → {Customer}", addressId, isNewAddress, customer);
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
		logger.LogDebug("<<< Exiting UseValidatedAddress(addressId={AddressId},isNewAddress={IsNewAddress})", addressId, isNewAddress);
        return Content(string.Empty);
    }

    #endregion
}
