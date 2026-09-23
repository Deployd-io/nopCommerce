using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Tax.Avalara.Models.ItemClassification;
using Nop.Plugin.Tax.Avalara.Models.Log;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Models.Configuration;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel, IAclSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationModel()
    {
		_logger.LogError(">>> Entering ConfigurationModel()");
        TestAddress = new AddressModel();
        Companies = new List<SelectListItem>();
		_logger.LogError("ConfigurationModel(): Companies → {Companies}", Companies);
        TaxOriginAddressTypes = new List<SelectListItem>();
		_logger.LogError("ConfigurationModel(): TaxOriginAddressTypes → {TaxOriginAddressTypes}", TaxOriginAddressTypes);
        TaxTransactionLogSearchModel = new TaxTransactionLogSearchModel();
		_logger.LogError("ConfigurationModel(): TaxTransactionLogSearchModel → {TaxTransactionLogSearchModel}", TaxTransactionLogSearchModel);
        ItemClassificationSearchModel = new ItemClassificationSearchModel();
		_logger.LogError("ConfigurationModel(): ItemClassificationSearchModel → {ItemClassificationSearchModel}", ItemClassificationSearchModel);
        SelectedCustomerRoleIds = new List<int>();
		_logger.LogError("ConfigurationModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogError("ConfigurationModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
        SelectedCountryIds = new List<int>();
		_logger.LogError("ConfigurationModel(): SelectedCountryIds → {SelectedCountryIds}", SelectedCountryIds);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogError("ConfigurationModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
    }

    #endregion

    #region Properties

    #region Common

    public bool IsConfigured { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.AccountId")]
    public string AccountId { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.LicenseKey")]
    [NoTrim]
    [DataType(DataType.Password)]
    public string LicenseKey { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.Company")]
    public string CompanyCode { get; set; }
    public IList<SelectListItem> Companies { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.EnableLogging")]
    public bool EnableLogging { get; set; }

    public AddressModel TestAddress { get; set; }

    public string TestTaxResult { get; set; }

    public TaxTransactionLogSearchModel TaxTransactionLogSearchModel { get; set; }

    public bool HideGeneralBlock { get; set; }

    public bool HideLogBlock { get; set; }

    #endregion

    #region Tax calculation

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.CommitTransactions")]
    public bool CommitTransactions { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.ValidateAddress")]
    public bool ValidateAddress { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.TaxOriginAddressType")]
    public int TaxOriginAddressTypeId { get; set; }
    public IList<SelectListItem> TaxOriginAddressTypes { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.UseTaxRateTables")]
    public bool UseTaxRateTables { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.GetTaxRateByAddressOnly")]
    public bool GetTaxRateByAddressOnly { get; set; }

    #endregion

    #region Certificates

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.EnableCertificates")]
    public bool EnableCertificates { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.AutoValidateCertificate")]
    public bool AutoValidateCertificate { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.AllowEditCustomer")]
    public bool AllowEditCustomer { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage")]
    public bool DisplayNoValidCertificatesMessage { get; set; }

    //ACL (customer roles)
    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    #endregion

    #region Item Classification

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.UseItemClassification")]
    public bool UseItemClassification { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.Countries")]
    public IList<int> SelectedCountryIds { get; set; }
    public IList<SelectListItem> AvailableCountries { get; set; }

    public ItemClassificationSearchModel ItemClassificationSearchModel { get; set; }

    public bool HideItemClassificationBlock { get; set; }

    #endregion

    #endregion
}