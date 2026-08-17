using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the address model factory
/// </summary>
public partial class AddressModelFactory : IAddressModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AddressModelFactory(AddressSettings addressSettings,
        IAddressService addressService,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService,
        ICountryService countryService,
        ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        IWorkContext workContext)
    {
        _addressSettings = addressSettings;
        _addressService = addressService;
        _addressAttributeFormatter = addressAttributeFormatter;
        _addressAttributeParser = addressAttributeParser;
        _addressAttributeService = addressAttributeService;
        _countryService = countryService;
        _localizationService = localizationService;
        _stateProvinceService = stateProvinceService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare address attributes
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address entity</param>
    /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of address entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareCustomAddressAttributesAsync(AddressModel model,
        Address address, string overrideAttributesXml = "")
    {
		_logger.LogDebug(">>> Entering PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml})", model, address, overrideAttributesXml);
        var attributes = await _addressAttributeService.GetAllAttributesAsync();
        foreach (var attribute in attributes)
        {
		_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): attributes → {Attributes}", model, address, overrideAttributesXml, attributes);
            var attributeModel = new AddressAttributeModel
            {
                Id = attribute.Id,
                ControlId = string.Format(NopCommonDefaults.AddressAttributeControlName, attribute.Id),
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _addressAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): attributeValues → {AttributeValues}", model, address, overrideAttributesXml, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new AddressAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            var selectedAddressAttributes = !string.IsNullOrEmpty(overrideAttributesXml) ?
                overrideAttributesXml :
                address?.CustomAttributes;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(selectedAddressAttributes))
                    {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;
							_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): item.IsPreSelected → {Arg3}", model, address, overrideAttributesXml, item.IsPreSelected);

                        //select new values
                        var selectedValues = await _addressAttributeParser.ParseAttributeValuesAsync(selectedAddressAttributes);
						_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): selectedValues → {SelectedValues}", model, address, overrideAttributesXml, selectedValues);
                        foreach (var attributeValue in selectedValues)
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(selectedAddressAttributes))
                    {
                        var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
						_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): enteredText → {EnteredText}", model, address, overrideAttributesXml, enteredText);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
							_logger.LogDebug("PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml}): attributeModel.DefaultValue → {Arg3}", model, address, overrideAttributesXml, attributeModel.DefaultValue);
                    }
                }
                    break;
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                default:
                    //not supported attribute control types
                    break;
            }

            model.CustomAddressAttributes.Add(attributeModel);
        }
		_logger.LogDebug("<<< Exiting PrepareCustomAddressAttributesAsync(model={Model},address={Address},overrideAttributesXml={OverrideAttributesXml})", model, address, overrideAttributesXml);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare address model
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address entity</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="addressSettings">Address settings</param>
    /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
    /// <param name="prePopulateWithCustomerFields">Whether to populate model properties with the customer fields (used with the customer entity)</param>
    /// <param name="customer">Customer entity; required if prePopulateWithCustomerFields is true</param>
    /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareAddressModelAsync(AddressModel model,
        Address address, bool excludeProperties,
        AddressSettings addressSettings,
        Func<Task<IList<Country>>> loadCountries = null,
        bool prePopulateWithCustomerFields = false,
        Customer customer = null,
        string overrideAttributesXml = "")
    {
		_logger.LogDebug(">>> Entering PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml})", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(addressSettings);

        var languageId = customer != null ? (customer?.LanguageId ?? 0) : (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): languageId → {LanguageId}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, languageId);

        if (!excludeProperties && address != null)
        {
            model.Id = address.Id;
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.Id → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.Id);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.FirstName → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.FirstName);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.LastName → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.LastName);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.Email → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.Email);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.Company → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.Company);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CountryId → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CountryId);
            model.FirstName = address.FirstName;
            model.LastName = address.LastName;
            model.Email = address.Email;
            model.Company = address.Company;
            model.CountryId = address.CountryId;
            model.CountryName = await _countryService.GetCountryByAddressAsync(address) is Country country ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : null;
            model.StateProvinceId = address.StateProvinceId;
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StateProvinceId → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StateProvinceId);
            model.StateProvinceName = await _stateProvinceService.GetStateProvinceByAddressAsync(address) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name) : null;
            model.County = address.County;
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.County → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.County);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.City → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.City);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.Address1 → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.Address1);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.Address2 → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.Address2);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.ZipPostalCode → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.ZipPostalCode);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.PhoneNumber → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.PhoneNumber);
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.FaxNumber → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.FaxNumber);
            model.City = address.City;
            model.Address1 = address.Address1;
            model.Address2 = address.Address2;
            model.ZipPostalCode = address.ZipPostalCode;
            model.PhoneNumber = address.PhoneNumber;
            model.FaxNumber = address.FaxNumber;
        }

        if (address == null && prePopulateWithCustomerFields)
        {
            if (customer == null)
                throw new Exception("Customer cannot be null when prepopulating an address");

            model.Email = customer.Email;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Company = customer.Company;
            model.Address1 = customer.StreetAddress;
            model.Address2 = customer.StreetAddress2;
            model.ZipPostalCode = customer.ZipPostalCode;
            model.City = customer.City;
            model.County = customer.County;
            model.PhoneNumber = customer.Phone;
            model.FaxNumber = customer.Fax;

            if (_addressSettings.PrePopulateCountryByCustomer)
            {
                model.CountryId = addressSettings.CountryEnabled && customer.CountryId != 0 ? customer.CountryId : null;
                model.StateProvinceId = addressSettings.StateProvinceEnabled && customer.StateProvinceId != 0 ? customer.StateProvinceId : null;
            }
        }

        //countries and states
        if (addressSettings.CountryEnabled && loadCountries != null)
        {
            var countries = await loadCountries();
			_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): countries → {Countries}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, countries);

            if (_addressSettings.PreselectCountryIfOnlyOne && countries.Count == 1)
            {
                model.CountryId = countries[0].Id;
            }
            else
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            }

            if (addressSettings.DefaultCountryId != null)
                model.CountryId = model.CountryId ?? addressSettings.DefaultCountryId;

            foreach (var c in countries)
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (addressSettings.StateProvinceEnabled)
            {
                var states = (await _stateProvinceService
                        .GetStateProvincesByCountryIdAsync(model.CountryId ?? 0, languageId))
                    .ToList();
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                            Value = s.Id.ToString(),
                            Selected = (s.Id == model.StateProvinceId)
                        });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
					_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): anyCountrySelected → {AnyCountrySelected}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, anyCountrySelected);
                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }
            }
        }

        //form fields
        model.CompanyEnabled = addressSettings.CompanyEnabled;
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CompanyEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CompanyEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CompanyRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CompanyRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StreetAddressEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StreetAddressEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StreetAddressRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StreetAddressRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StreetAddress2Enabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StreetAddress2Enabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StreetAddress2Required → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StreetAddress2Required);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.ZipPostalCodeEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.ZipPostalCodeEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.ZipPostalCodeRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.ZipPostalCodeRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CityEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CityEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CityRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CityRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CountyEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CountyEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CountyRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CountyRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.CountryEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.CountryEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.StateProvinceEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.StateProvinceEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.PhoneEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.PhoneEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.PhoneRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.PhoneRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.FaxEnabled → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.FaxEnabled);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.FaxRequired → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.FaxRequired);
		_logger.LogDebug("PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml}): model.DefaultCountryId → {Arg8}", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml, model.DefaultCountryId);
        model.CompanyRequired = addressSettings.CompanyRequired;
        model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
        model.StreetAddressRequired = addressSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = addressSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
        model.CityEnabled = addressSettings.CityEnabled;
        model.CityRequired = addressSettings.CityRequired;
        model.CountyEnabled = addressSettings.CountyEnabled;
        model.CountyRequired = addressSettings.CountyRequired;
        model.CountryEnabled = addressSettings.CountryEnabled;
        model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
        model.PhoneEnabled = addressSettings.PhoneEnabled;
        model.PhoneRequired = addressSettings.PhoneRequired;
        model.FaxEnabled = addressSettings.FaxEnabled;
        model.FaxRequired = addressSettings.FaxRequired;
        model.DefaultCountryId = addressSettings.DefaultCountryId;

        //customer attribute services
        if (_addressAttributeService != null && _addressAttributeParser != null)
        {
            await PrepareCustomAddressAttributesAsync(model, address, overrideAttributesXml);
        }
        if (_addressAttributeFormatter != null && address != null)
        {
            model.FormattedCustomAddressAttributes = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
        }

        (model.AddressLine, model.AddressFields) = await _addressService.FormatAddressAsync(address, languageId);
		_logger.LogDebug("<<< Exiting PrepareAddressModelAsync(model={Model},address={Address},excludeProperties={ExcludeProperties},addressSettings={AddressSettings},loadCountries={LoadCountries},prePopulateWithCustomerFields={PrePopulateWithCustomerFields},customer={Customer},overrideAttributesXml={OverrideAttributesXml})", model, address, excludeProperties, addressSettings, loadCountries, prePopulateWithCustomerFields, customer, overrideAttributesXml);
    }

    #endregion
}