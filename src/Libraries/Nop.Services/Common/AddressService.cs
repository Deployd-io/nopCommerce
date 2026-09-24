using System.Net;
using System.Text.RegularExpressions;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Address service
/// </summary>
public partial class AddressService : IAddressService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<Address> _addressRepository;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public AddressService(AddressSettings addressSettings,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService,
        ICountryService countryService,
        ILocalizationService localizationService,
        IRepository<Address> addressRepository,
        IStateProvinceService stateProvinceService)
    {
		_logger.LogError(">>> Entering AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService})", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService);
        _addressSettings = addressSettings;
        _addressAttributeParser = addressAttributeParser;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _addressAttributeParser → {_addressAttributeParser}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _addressAttributeParser);
        _addressAttributeService = addressAttributeService;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _addressAttributeService → {_addressAttributeService}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _addressAttributeService);
        _countryService = countryService;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _countryService → {_countryService}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _countryService);
        _localizationService = localizationService;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _localizationService → {_localizationService}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _localizationService);
        _addressRepository = addressRepository;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _addressRepository → {_addressRepository}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _addressRepository);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("AddressService(addressSettings={AddressSettings},addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},countryService={CountryService},localizationService={LocalizationService},addressRepository={AddressRepository},stateProvinceService={StateProvinceService}): _stateProvinceService → {_stateProvinceService}", addressSettings, addressAttributeParser, addressAttributeService, countryService, localizationService, addressRepository, stateProvinceService, _stateProvinceService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes an address
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAddressAsync(Address address)
    {
		_logger.LogError(">>> Entering DeleteAddressAsync(address={Address})", address);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _addressRepository.DeleteAsync(address);
		_logger.LogError("DeleteAddressAsync(address)={Address}: external service call took {Arg1} ms", address, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteAddressAsync(address={Address})", address);
    }

    /// <summary>
    /// Gets total number of addresses by country identifier
    /// </summary>
    /// <param name="countryId">Country identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of addresses
    /// </returns>
    public virtual async Task<int> GetAddressTotalByCountryIdAsync(int countryId)
    {
		_logger.LogError(">>> Entering GetAddressTotalByCountryIdAsync(countryId={CountryId})", countryId);
        if (countryId == 0)
            return 0;

        var query = from a in _addressRepository.Table
            where a.CountryId == countryId
            select a;

		_logger.LogWarning("<<< Exiting GetAddressTotalByCountryIdAsync(countryId={CountryId})", countryId);
        return await query.CountAsync();
    }

    /// <summary>
    /// Gets total number of addresses by state/province identifier
    /// </summary>
    /// <param name="stateProvinceId">State/province identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of addresses
    /// </returns>
    public virtual async Task<int> GetAddressTotalByStateProvinceIdAsync(int stateProvinceId)
    {
		_logger.LogError(">>> Entering GetAddressTotalByStateProvinceIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
        if (stateProvinceId == 0)
            return 0;

        var query = from a in _addressRepository.Table
            where a.StateProvinceId == stateProvinceId
            select a;

		_logger.LogWarning("<<< Exiting GetAddressTotalByStateProvinceIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
        return await query.CountAsync();
    }

    /// <summary>
    /// Gets an address by address identifier
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address
    /// </returns>
    public virtual async Task<Address> GetAddressByIdAsync(int addressId)
    {
		_logger.LogError(">>> Entering GetAddressByIdAsync(addressId={AddressId})", addressId);
		_logger.LogError("GetAddressByIdAsync(addressId)={AddressId}: find query executing", addressId);
    _logger.LogWarning("<<< Exiting GetAddressByIdAsync(addressId={AddressId})", addressId);
        return await _addressRepository.GetByIdAsync(addressId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Inserts an address
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAddressAsync(Address address)
    {
		_logger.LogError(">>> Entering InsertAddressAsync(address={Address})", address);
        ArgumentNullException.ThrowIfNull(address);

        address.CreatedOnUtc = DateTime.UtcNow;
		_logger.LogDebug("InsertAddressAsync(address={Address}): address.CreatedOnUtc → {Arg1}", address, address.CreatedOnUtc);

        //some validation
        if (address.CountryId == 0)
            address.CountryId = null;
			_logger.LogDebug("InsertAddressAsync(address={Address}): address.CountryId → {Arg1}", address, address.CountryId);
        if (address.StateProvinceId == 0)
            address.StateProvinceId = null;
			_logger.LogDebug("InsertAddressAsync(address={Address}): address.StateProvinceId → {Arg1}", address, address.StateProvinceId);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _addressRepository.InsertAsync(address);
		_logger.LogError("InsertAddressAsync(address)={Address}: save query executed in {Arg1} ms", address, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting InsertAddressAsync(address={Address})", address);
    }

    /// <summary>
    /// Updates the address
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAddressAsync(Address address)
    {
		_logger.LogError(">>> Entering UpdateAddressAsync(address={Address})", address);
        ArgumentNullException.ThrowIfNull(address);

        //some validation
        if (address.CountryId == 0)
            address.CountryId = null;
			_logger.LogDebug("UpdateAddressAsync(address={Address}): address.CountryId → {Arg1}", address, address.CountryId);
        if (address.StateProvinceId == 0)
            address.StateProvinceId = null;
			_logger.LogDebug("UpdateAddressAsync(address={Address}): address.StateProvinceId → {Arg1}", address, address.StateProvinceId);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _addressRepository.UpdateAsync(address);
		_logger.LogError("UpdateAddressAsync(address)={Address}: update query executed in {Arg1} ms", address, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateAddressAsync(address={Address})", address);
    }

    /// <summary>
    /// Gets a value indicating whether address is valid (can be saved)
    /// </summary>
    /// <param name="address">Address to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsAddressValidAsync(Address address)
    {
		_logger.LogError(">>> Entering IsAddressValidAsync(address={Address})", address);
        ArgumentNullException.ThrowIfNull(address);

        if (string.IsNullOrWhiteSpace(address.FirstName))
            return false;

        if (string.IsNullOrWhiteSpace(address.LastName))
            return false;

        if (string.IsNullOrWhiteSpace(address.Email))
            return false;

        if (_addressSettings.CompanyEnabled &&
            _addressSettings.CompanyRequired &&
            string.IsNullOrWhiteSpace(address.Company))
            return false;

        if (_addressSettings.StreetAddressEnabled &&
            _addressSettings.StreetAddressRequired &&
            string.IsNullOrWhiteSpace(address.Address1))
            return false;

        if (_addressSettings.StreetAddress2Enabled &&
            _addressSettings.StreetAddress2Required &&
            string.IsNullOrWhiteSpace(address.Address2))
            return false;

        if (_addressSettings.ZipPostalCodeEnabled &&
            _addressSettings.ZipPostalCodeRequired &&
            string.IsNullOrWhiteSpace(address.ZipPostalCode))
            return false;

        if (_addressSettings.CountryEnabled)
        {
            var country = await _countryService.GetCountryByAddressAsync(address);
            if (country == null)
                return false;

            if (_addressSettings.StateProvinceEnabled)
            {
                var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id);
                if (states.Any())
                {
                    if (address.StateProvinceId == null || address.StateProvinceId.Value == 0)
                        return false;

                    var state = states.FirstOrDefault(x => x.Id == address.StateProvinceId.Value);
                    if (state == null)
                        return false;
                }
            }
        }

        if (_addressSettings.CountyEnabled &&
            _addressSettings.CountyRequired &&
            string.IsNullOrWhiteSpace(address.County))
            return false;

        if (_addressSettings.CityEnabled &&
            _addressSettings.CityRequired &&
            string.IsNullOrWhiteSpace(address.City))
            return false;

        if (_addressSettings.PhoneEnabled &&
            _addressSettings.PhoneRequired &&
            string.IsNullOrWhiteSpace(address.PhoneNumber))
            return false;

        if (_addressSettings.FaxEnabled &&
            _addressSettings.FaxRequired &&
            string.IsNullOrWhiteSpace(address.FaxNumber))
            return false;

        var requiredAttributes = (await _addressAttributeService.GetAllAttributesAsync()).Where(x => x.IsRequired);

        foreach (var requiredAttribute in requiredAttributes)
        {
            var value = _addressAttributeParser.ParseValues(address.CustomAttributes, requiredAttribute.Id);

            if (!value.Any() || string.IsNullOrEmpty(value[0]))
                return false;
        }

		_logger.LogWarning("<<< Exiting IsAddressValidAsync(address={Address})", address);
        return true;
    }

    /// <summary>
    /// Find an address
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="email">Email</param>
    /// <param name="faxNumber">Fax number</param>
    /// <param name="company">Company</param>
    /// <param name="address1">Address 1</param>
    /// <param name="address2">Address 2</param>
    /// <param name="city">City</param>
    /// <param name="county">County</param>
    /// <param name="stateProvinceId">State/province identifier</param>
    /// <param name="zipPostalCode">Zip postal code</param>
    /// <param name="countryId">Country identifier</param>
    /// <param name="customAttributes">Custom address attributes (XML format)</param>
    /// <returns>Address</returns>
    public virtual Address FindAddress(List<Address> source, string firstName, string lastName, string phoneNumber, string email,
        string faxNumber, string company, string address1, string address2, string city, string county, int? stateProvinceId,
        string zipPostalCode, int? countryId, string customAttributes)
    {
		_logger.LogError(">>> Entering FindAddress(source={Source},firstName={FirstName},lastName={LastName},phoneNumber={PhoneNumber},email={Email},faxNumber={FaxNumber},company={Company},address1={Address1},address2={Address2},city={City},county={County},stateProvinceId={StateProvinceId},zipPostalCode={ZipPostalCode},countryId={CountryId},customAttributes={CustomAttributes})", source, firstName, lastName, phoneNumber, email, faxNumber, company, address1, address2, city, county, stateProvinceId, zipPostalCode, countryId, customAttributes);
								_logger.LogWarning("<<< Exiting FindAddress(source={Source},firstName={FirstName},lastName={LastName},phoneNumber={PhoneNumber},email={Email},faxNumber={FaxNumber},company={Company},address1={Address1},address2={Address2},city={City},county={County},stateProvinceId={StateProvinceId},zipPostalCode={ZipPostalCode},countryId={CountryId},customAttributes={CustomAttributes})", source, firstName, lastName, phoneNumber, email, faxNumber, company, address1, address2, city, county, stateProvinceId, zipPostalCode, countryId, customAttributes);
        return source.Find(a => ((string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(firstName)) || a.FirstName == firstName) &&
                                ((string.IsNullOrEmpty(a.LastName) && string.IsNullOrEmpty(lastName)) || a.LastName == lastName) &&
                                ((string.IsNullOrEmpty(a.PhoneNumber) && string.IsNullOrEmpty(phoneNumber)) || a.PhoneNumber == phoneNumber) &&
                                ((string.IsNullOrEmpty(a.Email) && string.IsNullOrEmpty(email)) || a.Email == email) &&
                                ((string.IsNullOrEmpty(a.FaxNumber) && string.IsNullOrEmpty(faxNumber)) || a.FaxNumber == faxNumber) &&
                                ((string.IsNullOrEmpty(a.Company) && string.IsNullOrEmpty(company)) || a.Company == company) &&
                                ((string.IsNullOrEmpty(a.Address1) && string.IsNullOrEmpty(address1)) || a.Address1 == address1) &&
                                ((string.IsNullOrEmpty(a.Address2) && string.IsNullOrEmpty(address2)) || a.Address2 == address2) &&
                                ((string.IsNullOrEmpty(a.City) && string.IsNullOrEmpty(city)) || a.City == city) &&
                                ((string.IsNullOrEmpty(a.County) && string.IsNullOrEmpty(county)) || a.County == county) &&
                                ((a.StateProvinceId == null && (stateProvinceId == null || stateProvinceId == 0)) || (a.StateProvinceId != null && a.StateProvinceId == stateProvinceId)) &&
                                ((string.IsNullOrEmpty(a.ZipPostalCode) && string.IsNullOrEmpty(zipPostalCode)) || a.ZipPostalCode == zipPostalCode) &&
                                ((a.CountryId == null && countryId == null) || (a.CountryId != null && a.CountryId == countryId)) &&
                                //actually we should parse custom address attribute (in case if "Display order" is changed) and then compare
                                //bu we simplify this process and simply compare their values in XML
                                ((string.IsNullOrEmpty(a.CustomAttributes) && string.IsNullOrEmpty(customAttributes)) || a.CustomAttributes == customAttributes));
    }

    /// <summary>
    /// Clone address
    /// </summary>
    /// <returns>A deep copy of address</returns>
    public virtual Address CloneAddress(Address address)
    {
		_logger.LogError(">>> Entering CloneAddress(address={Address})", address);
        var addr = new Address
        {
            FirstName = address.FirstName,
            LastName = address.LastName,
            Email = address.Email,
            Company = address.Company,
            CountryId = address.CountryId,
            StateProvinceId = address.StateProvinceId,
            County = address.County,
            City = address.City,
            Address1 = address.Address1,
            Address2 = address.Address2,
            ZipPostalCode = address.ZipPostalCode,
            PhoneNumber = address.PhoneNumber,
            FaxNumber = address.FaxNumber,
            CustomAttributes = address.CustomAttributes,
            CreatedOnUtc = address.CreatedOnUtc
        };

		_logger.LogWarning("<<< Exiting CloneAddress(address={Address})", address);
        return addr;
    }

    /// <summary>
    /// Address format
    /// </summary>
    /// <param name="address">Address</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">Encode to HTML</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// Address line, array address fields
    /// </returns>      
    public virtual async Task<(string, KeyValuePair<AddressField, string>[])> FormatAddressAsync(Address address, int languageId = 0, string separator = ", ", bool htmlEncode = false)
    {
        var fieldsList = new KeyValuePair<AddressField, string>[7];

        if (address == null)
            return (string.Empty, fieldsList);

        var format = await _localizationService.GetResourceAsync("Address.LineFormat", languageId, true, "{0}{1}{2}{3}{4}{5}{6}");
        var indexArray = Regex.Matches(format, @"{\d}").Select(x => Convert.ToInt32(Regex.Match(x.Value, @"\d").Value)).ToArray();

        var indexItem = 0;
        foreach (var item in indexArray)
        {
            switch ((AddressField)item)
            {
                case AddressField.Country:
                    var country = await _countryService.GetCountryByAddressAsync(address);
                    var countryName = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name, languageId) : string.Empty;

                    if (_addressSettings.CountryEnabled && !string.IsNullOrWhiteSpace(countryName))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Country, htmlEncode ? WebUtility.HtmlEncode(countryName) : countryName);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Country, string.Empty);

                    break;

                case AddressField.StateProvince:
                    var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(address);
                    var stateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, languageId) : string.Empty;

                    if (_addressSettings.StateProvinceEnabled && !string.IsNullOrWhiteSpace(stateProvinceName))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.StateProvince, htmlEncode ? WebUtility.HtmlEncode(stateProvinceName) : stateProvinceName);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.StateProvince, string.Empty);

                    break;

                case AddressField.City:
                    if (_addressSettings.CityEnabled && !string.IsNullOrWhiteSpace(address.City))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.City, htmlEncode ? WebUtility.HtmlEncode(address.City) : address.City);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.City, string.Empty);
                    break;

                case AddressField.County:
                    if (_addressSettings.CountyEnabled && !string.IsNullOrWhiteSpace(address.County))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.County, htmlEncode ? WebUtility.HtmlEncode(address.County) : address.County);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.County, string.Empty);
                    break;

                case AddressField.Address1:
                    if (_addressSettings.StreetAddressEnabled && !string.IsNullOrWhiteSpace(address.Address1))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Address1, htmlEncode ? WebUtility.HtmlEncode(address.Address1) : address.Address1);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Address1, string.Empty);
                    break;

                case AddressField.Address2:
                    if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrWhiteSpace(address.Address2))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Address2, htmlEncode ? WebUtility.HtmlEncode(address.Address2) : address.Address2);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.Address2, string.Empty);
                    break;

                case AddressField.ZipPostalCode:
                    if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrWhiteSpace(address.ZipPostalCode))
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.ZipPostalCode, htmlEncode ? WebUtility.HtmlEncode(address.ZipPostalCode) : address.ZipPostalCode);
                    else
                        fieldsList[indexItem] = new KeyValuePair<AddressField, string>(AddressField.ZipPostalCode, string.Empty);
                    break;
                default:
                    break;
            }
            indexItem++;
        }

        var formatString = string.Join("", fieldsList.Select(x => !string.IsNullOrEmpty(x.Value) ? $"{x.Value}{separator}" : x.Value).ToArray())
            .TrimEnd(separator.ToArray());

        return (formatString, fieldsList);
    }
    #endregion
}