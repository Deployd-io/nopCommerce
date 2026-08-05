using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-11-18 00:00:00", "SchemaMigration for 4.80.0")]
public class SchemaMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        //#7187
        var ptoductTableName = nameof(Product);
        var hasTierPricesColumnName = "HasTierPrices";
		_logger.LogDebug("Up(): ptoductTableName → {PtoductTableName}", ptoductTableName);
		_logger.LogDebug("Up(): hasTierPricesColumnName → {HasTierPricesColumnName}", hasTierPricesColumnName);
        if (Schema.Table(ptoductTableName).Column(hasTierPricesColumnName).Exists())
            Delete.Column(hasTierPricesColumnName).FromTable(ptoductTableName);

        //#7188
        var hasDiscountsAppliedColumnName = "HasDiscountsApplied";
        if (Schema.Table(ptoductTableName).Column(hasDiscountsAppliedColumnName).Exists())
			_logger.LogDebug("Up(): hasDiscountsAppliedColumnName → {HasDiscountsAppliedColumnName}", hasDiscountsAppliedColumnName);
            Delete.Column(hasDiscountsAppliedColumnName).FromTable(ptoductTableName);

        //#7242
        var categoryTableName = nameof(Category);
        var restrictFromVendorsColumnName = nameof(Category.RestrictFromVendors);
		_logger.LogDebug("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
		_logger.LogDebug("Up(): restrictFromVendorsColumnName → {RestrictFromVendorsColumnName}", restrictFromVendorsColumnName);

        if (!Schema.Table(categoryTableName).Column(restrictFromVendorsColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(restrictFromVendorsColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        //#7281
        var customerTableName = nameof(Customer);
        var mustChangePasswordColumnName = nameof(Customer.MustChangePassword);
		_logger.LogDebug("Up(): customerTableName → {CustomerTableName}", customerTableName);
		_logger.LogDebug("Up(): mustChangePasswordColumnName → {MustChangePasswordColumnName}", mustChangePasswordColumnName);

        if (!Schema.Table(customerTableName).Column(mustChangePasswordColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(mustChangePasswordColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        //#7294
        var topicTableName = nameof(Topic);
        var topicSystemNameColumnName = nameof(Topic.SystemName);
		_logger.LogDebug("Up(): topicTableName → {TopicTableName}", topicTableName);
		_logger.LogDebug("Up(): topicSystemNameColumnName → {TopicSystemNameColumnName}", topicSystemNameColumnName);

        if (!Schema.Table(topicTableName).Index("IX_Topic_SystemName").Exists())
        {
            Alter.Table(topicTableName)
                .AlterColumn(topicSystemNameColumnName)
                .AsString(400)
                .Nullable();
        }

        //#7241
        var discountTableName = nameof(Discount);
        var vendorIdDiscountColumnName = nameof(Discount.VendorId);
		_logger.LogDebug("Up(): discountTableName → {DiscountTableName}", discountTableName);
		_logger.LogDebug("Up(): vendorIdDiscountColumnName → {VendorIdDiscountColumnName}", vendorIdDiscountColumnName);

        if (!Schema.Table(discountTableName).Column(vendorIdDiscountColumnName).Exists())
        {
            Alter.Table(discountTableName)
                .AddColumn(vendorIdDiscountColumnName).AsInt32().ForeignKey<Vendor>(onDelete: Rule.SetNull).Nullable();
        }

        //#7243
        var vendorTableName = nameof(Vendor);
        var pmCustomerIdColumnName = nameof(Vendor.PmCustomerId);
		_logger.LogDebug("Up(): vendorTableName → {VendorTableName}", vendorTableName);
		_logger.LogDebug("Up(): pmCustomerIdColumnName → {PmCustomerIdColumnName}", pmCustomerIdColumnName);
        if (!Schema.Table(vendorTableName).Column(pmCustomerIdColumnName).Exists())
            Alter.Table(vendorTableName).AddColumn(pmCustomerIdColumnName).AsInt32().Nullable();
			_logger.LogDebug("<<< Exiting Up()");
    }
}
