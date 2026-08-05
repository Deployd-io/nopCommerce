using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Web.Areas.Admin.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the address model factory implementation
/// </summary>
public partial class AddressModelFactory : IAddressModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;

    #endregion

    #region Ctor

    public AddressModelFactory(IAddressAttributeModelFactory addressAttributeModelFactory,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        IBaseAdminModelFactory baseAdminModelFactory)
    {
        _addressAttributeModelFactory = addressAttributeModelFactory;
        _addressAttributeFormatter = addressAttributeFormatter;
        _baseAdminModelFactory = baseAdminModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare address model
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
    {
		_logger.LogDebug(">>> Entering PrepareAddressModelAsync(model={Model},address={Address})", model, address);
        ArgumentNullException.ThrowIfNull(model);

        //prepare available countries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

        //prepare available states
        await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);

        //prepare custom address attributes
        await _addressAttributeModelFactory.PrepareCustomAddressAttributesAsync(model.CustomAddressAttributes, address);

        if (address == null)
            return;

        model.FormattedCustomAddressAttributes = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
		_logger.LogDebug("<<< Exiting PrepareAddressModelAsync(model={Model},address={Address})", model, address);
    }

    #endregion
}