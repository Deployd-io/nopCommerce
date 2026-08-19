using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-07-20 00:00:10", "SchemaMigration for 4.60.0")]
public class SchemaMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        // add column
        var customerTableName = nameof(Customer);
		_logger.LogDebug("Up(): customerTableName → {CustomerTableName}", customerTableName);

        var firstNameCustomerColumnName = nameof(Customer.FirstName);
		_logger.LogDebug("Up(): firstNameCustomerColumnName → {FirstNameCustomerColumnName}", firstNameCustomerColumnName);
        var lastNameCustomerColumnName = nameof(Customer.LastName);
		_logger.LogDebug("Up(): lastNameCustomerColumnName → {LastNameCustomerColumnName}", lastNameCustomerColumnName);
        var genderCustomerColumnName = nameof(Customer.Gender);
		_logger.LogDebug("Up(): genderCustomerColumnName → {GenderCustomerColumnName}", genderCustomerColumnName);
        var dobCustomerColumnName = nameof(Customer.DateOfBirth);
		_logger.LogDebug("Up(): dobCustomerColumnName → {DobCustomerColumnName}", dobCustomerColumnName);
        var companyCustomerColumnName = nameof(Customer.Company);
		_logger.LogDebug("Up(): companyCustomerColumnName → {CompanyCustomerColumnName}", companyCustomerColumnName);
        var address1CustomerColumnName = nameof(Customer.StreetAddress);
		_logger.LogDebug("Up(): address1CustomerColumnName → {Address1CustomerColumnName}", address1CustomerColumnName);
        var address2CustomerColumnName = nameof(Customer.StreetAddress2);
		_logger.LogDebug("Up(): address2CustomerColumnName → {Address2CustomerColumnName}", address2CustomerColumnName);
        var zipCustomerColumnName = nameof(Customer.ZipPostalCode);
		_logger.LogDebug("Up(): zipCustomerColumnName → {ZipCustomerColumnName}", zipCustomerColumnName);
        var cityCustomerColumnName = nameof(Customer.City);
		_logger.LogDebug("Up(): cityCustomerColumnName → {CityCustomerColumnName}", cityCustomerColumnName);
        var countyCustomerColumnName = nameof(Customer.County);
		_logger.LogDebug("Up(): countyCustomerColumnName → {CountyCustomerColumnName}", countyCustomerColumnName);
        var countryIdCustomerColumnName = nameof(Customer.CountryId);
		_logger.LogDebug("Up(): countryIdCustomerColumnName → {CountryIdCustomerColumnName}", countryIdCustomerColumnName);
        var stateIdCustomerColumnName = nameof(Customer.StateProvinceId);
		_logger.LogDebug("Up(): stateIdCustomerColumnName → {StateIdCustomerColumnName}", stateIdCustomerColumnName);
        var phoneCustomerColumnName = nameof(Customer.Phone);
		_logger.LogDebug("Up(): phoneCustomerColumnName → {PhoneCustomerColumnName}", phoneCustomerColumnName);
        var faxCustomerColumnName = nameof(Customer.Fax);
		_logger.LogDebug("Up(): faxCustomerColumnName → {FaxCustomerColumnName}", faxCustomerColumnName);
        var vatNumberCustomerColumnName = nameof(Customer.VatNumber);
		_logger.LogDebug("Up(): vatNumberCustomerColumnName → {VatNumberCustomerColumnName}", vatNumberCustomerColumnName);
        var vatNumberStatusIdCustomerColumnName = nameof(Customer.VatNumberStatusId);
		_logger.LogDebug("Up(): vatNumberStatusIdCustomerColumnName → {VatNumberStatusIdCustomerColumnName}", vatNumberStatusIdCustomerColumnName);
        var timeZoneIdCustomerColumnName = nameof(Customer.TimeZoneId);
		_logger.LogDebug("Up(): timeZoneIdCustomerColumnName → {TimeZoneIdCustomerColumnName}", timeZoneIdCustomerColumnName);
        var attributeXmlCustomerColumnName = nameof(Customer.CustomCustomerAttributesXML);
		_logger.LogDebug("Up(): attributeXmlCustomerColumnName → {AttributeXmlCustomerColumnName}", attributeXmlCustomerColumnName);
        var currencyIdCustomerColumnName = nameof(Customer.CurrencyId);
		_logger.LogDebug("Up(): currencyIdCustomerColumnName → {CurrencyIdCustomerColumnName}", currencyIdCustomerColumnName);
        var languageIdCustomerColumnName = nameof(Customer.LanguageId);
		_logger.LogDebug("Up(): languageIdCustomerColumnName → {LanguageIdCustomerColumnName}", languageIdCustomerColumnName);
        var taxDisplayTypeIdCustomerColumnName = nameof(Customer.TaxDisplayTypeId);
		_logger.LogDebug("Up(): taxDisplayTypeIdCustomerColumnName → {TaxDisplayTypeIdCustomerColumnName}", taxDisplayTypeIdCustomerColumnName);

        if (!Schema.Table(customerTableName).Column(firstNameCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(firstNameCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(lastNameCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(lastNameCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(genderCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(genderCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(dobCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(dobCustomerColumnName).AsDateTime2().Nullable();
        }
        if (!Schema.Table(customerTableName).Column(companyCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(companyCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(address1CustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(address1CustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(address2CustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(address2CustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(zipCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(zipCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(cityCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(cityCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(countyCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(countyCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(countryIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(countryIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
        }
        if (!Schema.Table(customerTableName).Column(stateIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(stateIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
        }
        if (!Schema.Table(customerTableName).Column(phoneCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(phoneCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(faxCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(faxCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(vatNumberCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(vatNumberCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(vatNumberStatusIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(vatNumberStatusIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo((int)VatNumberStatus.Unknown);
        }
        if (!Schema.Table(customerTableName).Column(timeZoneIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(timeZoneIdCustomerColumnName).AsString(1000).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(attributeXmlCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(attributeXmlCustomerColumnName).AsString(int.MaxValue).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(currencyIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(currencyIdCustomerColumnName).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(languageIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(languageIdCustomerColumnName).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable();
        }
        if (!Schema.Table(customerTableName).Column(taxDisplayTypeIdCustomerColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(taxDisplayTypeIdCustomerColumnName).AsInt32().Nullable();
        }

        //5705
        var discountTableName = nameof(Discount);
		_logger.LogDebug("Up(): discountTableName → {DiscountTableName}", discountTableName);
        var isActiveDiscountColumnName = nameof(Discount.IsActive);
		_logger.LogDebug("Up(): isActiveDiscountColumnName → {IsActiveDiscountColumnName}", isActiveDiscountColumnName);

        if (!Schema.Table(discountTableName).Column(isActiveDiscountColumnName).Exists())
        {
            Alter.Table(discountTableName)
                .AddColumn(isActiveDiscountColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
        }
		_logger.LogDebug("<<< Exiting Up()");
    }
}