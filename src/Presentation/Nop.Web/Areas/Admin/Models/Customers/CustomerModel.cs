using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer model
/// </summary>
public partial record CustomerModel : BaseNopEntityModel, IAclSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerModel()
    {
		_logger.LogDebug(">>> Entering CustomerModel()");
        AvailableTimeZones = new List<SelectListItem>();
        SendEmail = new SendEmailModel() { SendImmediately = true };
		_logger.LogDebug("CustomerModel(): AvailableTimeZones → {AvailableTimeZones}", AvailableTimeZones);
        SendPm = new SendPmModel();
		_logger.LogDebug("CustomerModel(): SendPm → {SendPm}", SendPm);

        SelectedCustomerRoleIds = new List<int>();
		_logger.LogDebug("CustomerModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();

		_logger.LogDebug("CustomerModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("CustomerModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        AvailableStates = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("CustomerModel(): AvailableStates → {AvailableStates}", AvailableStates);
		_logger.LogDebug("CustomerModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        CustomerAttributes = new List<CustomerAttributeModel>();
        AddRewardPoints = new AddRewardPointsToCustomerModel();
		_logger.LogDebug("CustomerModel(): CustomerAttributes → {CustomerAttributes}", CustomerAttributes);
		_logger.LogDebug("CustomerModel(): AddRewardPoints → {AddRewardPoints}", AddRewardPoints);
        CustomerRewardPointsSearchModel = new CustomerRewardPointsSearchModel();
        CustomerAddressSearchModel = new CustomerAddressSearchModel();
		_logger.LogDebug("CustomerModel(): CustomerRewardPointsSearchModel → {CustomerRewardPointsSearchModel}", CustomerRewardPointsSearchModel);
		_logger.LogDebug("CustomerModel(): CustomerAddressSearchModel → {CustomerAddressSearchModel}", CustomerAddressSearchModel);
        CustomerOrderSearchModel = new CustomerOrderSearchModel();
        CustomerShoppingCartSearchModel = new CustomerShoppingCartSearchModel();
		_logger.LogDebug("CustomerModel(): CustomerOrderSearchModel → {CustomerOrderSearchModel}", CustomerOrderSearchModel);
		_logger.LogDebug("CustomerModel(): CustomerShoppingCartSearchModel → {CustomerShoppingCartSearchModel}", CustomerShoppingCartSearchModel);
        CustomerActivityLogSearchModel = new CustomerActivityLogSearchModel();
        CustomerBackInStockSubscriptionSearchModel = new CustomerBackInStockSubscriptionSearchModel();
		_logger.LogDebug("CustomerModel(): CustomerActivityLogSearchModel → {CustomerActivityLogSearchModel}", CustomerActivityLogSearchModel);
		_logger.LogDebug("CustomerModel(): CustomerBackInStockSubscriptionSearchModel → {CustomerBackInStockSubscriptionSearchModel}", CustomerBackInStockSubscriptionSearchModel);
        CustomerAssociatedExternalAuthRecordsSearchModel = new CustomerAssociatedExternalAuthRecordsSearchModel();
		_logger.LogDebug("<<< Exiting CustomerModel()");
		_logger.LogDebug("CustomerModel(): CustomerAssociatedExternalAuthRecordsSearchModel → {CustomerAssociatedExternalAuthRecordsSearchModel}", CustomerAssociatedExternalAuthRecordsSearchModel);
    }

    #endregion

    #region Properties

    public bool UsernamesEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
    public string Username { get; set; }

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Vendor")]
    public int VendorId { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    //form fields & properties
    public bool GenderEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
    public string Gender { get; set; }

    public bool NeutralGenderEnabled { get; set; }

    public bool FirstNameEnabled { get; set; }
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
    public string FirstName { get; set; }

    public bool LastNameEnabled { get; set; }
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
    public string LastName { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
    public string FullName { get; set; }

    public bool DateOfBirthEnabled { get; set; }

    [UIHint("DateNullable")]
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    public bool CompanyEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Company")]
    public string Company { get; set; }

    public bool StreetAddressEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress")]
    public string StreetAddress { get; set; }

    public bool StreetAddress2Enabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress2")]
    public string StreetAddress2 { get; set; }

    public bool ZipPostalCodeEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.ZipPostalCode")]
    public string ZipPostalCode { get; set; }

    public bool CityEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.City")]
    public string City { get; set; }

    public bool CountyEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.County")]
    public string County { get; set; }

    public bool CountryEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Country")]
    public int CountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    public bool StateProvinceEnabled { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.StateProvince")]
    public int StateProvinceId { get; set; }

    public IList<SelectListItem> AvailableStates { get; set; }

    public bool PhoneEnabled { get; set; }

    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Phone")]
    public string Phone { get; set; }

    public bool FaxEnabled { get; set; }

    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Fax")]
    public string Fax { get; set; }

    public List<CustomerAttributeModel> CustomerAttributes { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.RegisteredInStore")]
    public string RegisteredInStore { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
    public string AdminComment { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
    public bool IsTaxExempt { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
    public bool Active { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
    public int AffiliateId { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
    public string AffiliateName { get; set; }

    //time zone
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
    public string TimeZoneId { get; set; }

    public bool AllowCustomersToSetTimeZone { get; set; }

    public IList<SelectListItem> AvailableTimeZones { get; set; }

    //EU VAT
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
    public string VatNumber { get; set; }

    public string VatNumberStatusNote { get; set; }

    public bool DisplayVatNumber { get; set; }

    public bool DisplayRegisteredInStore { get; set; }

    //registration date
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastActivityDate")]
    public DateTime LastActivityDate { get; set; }

    //IP address
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.IPAddress")]
    public string LastIpAddress { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastVisitedPage")]
    public string LastVisitedPage { get; set; }

    //customer roles
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
    public string CustomerRoleNames { get; set; }

    //binding with multi-factor authentication provider
    [NopResourceDisplayName("Admin.Customers.Customers.Fields.MultiFactorAuthenticationProvider")]
    public string MultiFactorAuthenticationProvider { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.MustChangePassword")]
    public bool MustChangePassword { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    //reward points history
    public bool DisplayRewardPointsHistory { get; set; }

    public AddRewardPointsToCustomerModel AddRewardPoints { get; set; }

    public CustomerRewardPointsSearchModel CustomerRewardPointsSearchModel { get; set; }

    //send email model
    public SendEmailModel SendEmail { get; set; }

    //send PM model
    public SendPmModel SendPm { get; set; }

    //send a private message
    public bool AllowSendingOfPrivateMessage { get; set; }

    //send the welcome message
    public bool AllowSendingOfWelcomeMessage { get; set; }

    //re-send the activation message
    public bool AllowReSendingOfActivationMessage { get; set; }

    //GDPR enabled
    public bool GdprEnabled { get; set; }

    public string AvatarUrl { get; set; }

    public CustomerAddressSearchModel CustomerAddressSearchModel { get; set; }

    public CustomerOrderSearchModel CustomerOrderSearchModel { get; set; }

    public CustomerShoppingCartSearchModel CustomerShoppingCartSearchModel { get; set; }

    public CustomerActivityLogSearchModel CustomerActivityLogSearchModel { get; set; }

    public CustomerBackInStockSubscriptionSearchModel CustomerBackInStockSubscriptionSearchModel { get; set; }

    public CustomerAssociatedExternalAuthRecordsSearchModel CustomerAssociatedExternalAuthRecordsSearchModel { get; set; }

    #endregion

    #region Nested classes

    public partial record SendEmailModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }
    }

    public partial record SendPmModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Message")]
        public string Message { get; set; }
    }

    public partial record CustomerAttributeModel : BaseNopEntityModel
    {
        public CustomerAttributeModel()
        {
			_logger.LogDebug(">>> Entering CustomerAttributeModel()");
            Values = new List<CustomerAttributeValueModel>();
			_logger.LogDebug("<<< Exiting CustomerAttributeModel()");
			_logger.LogDebug("CustomerAttributeModel(): Values → {Values}", Values);
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<CustomerAttributeValueModel> Values { get; set; }
    }

    public partial record CustomerAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }

    #endregion
}