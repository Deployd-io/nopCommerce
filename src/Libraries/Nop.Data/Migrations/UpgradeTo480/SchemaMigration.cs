using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-11-18 00:00:00", "SchemaMigration for 4.80.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        //#7187
        var ptoductTableName = nameof(Product);
        var hasTierPricesColumnName = "HasTierPrices";
		logger.LogDebug("Up(): ptoductTableName → {PtoductTableName}", ptoductTableName);
		logger.LogDebug("Up(): hasTierPricesColumnName → {HasTierPricesColumnName}", hasTierPricesColumnName);
        if (Schema.Table(ptoductTableName).Column(hasTierPricesColumnName).Exists())
            Delete.Column(hasTierPricesColumnName).FromTable(ptoductTableName);

        //#7188
        var hasDiscountsAppliedColumnName = "HasDiscountsApplied";
        if (Schema.Table(ptoductTableName).Column(hasDiscountsAppliedColumnName).Exists())
			logger.LogDebug("Up(): hasDiscountsAppliedColumnName → {HasDiscountsAppliedColumnName}", hasDiscountsAppliedColumnName);
            Delete.Column(hasDiscountsAppliedColumnName).FromTable(ptoductTableName);

        //#7242
        var categoryTableName = nameof(Category);
        var restrictFromVendorsColumnName = nameof(Category.RestrictFromVendors);
		logger.LogDebug("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
		logger.LogDebug("Up(): restrictFromVendorsColumnName → {RestrictFromVendorsColumnName}", restrictFromVendorsColumnName);

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
		logger.LogDebug("Up(): customerTableName → {CustomerTableName}", customerTableName);
		logger.LogDebug("Up(): mustChangePasswordColumnName → {MustChangePasswordColumnName}", mustChangePasswordColumnName);

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
		logger.LogDebug("Up(): topicTableName → {TopicTableName}", topicTableName);
		logger.LogDebug("Up(): topicSystemNameColumnName → {TopicSystemNameColumnName}", topicSystemNameColumnName);

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
		logger.LogDebug("Up(): discountTableName → {DiscountTableName}", discountTableName);
		logger.LogDebug("Up(): vendorIdDiscountColumnName → {VendorIdDiscountColumnName}", vendorIdDiscountColumnName);

        if (!Schema.Table(discountTableName).Column(vendorIdDiscountColumnName).Exists())
        {
            Alter.Table(discountTableName)
                .AddColumn(vendorIdDiscountColumnName).AsInt32().ForeignKey<Vendor>(onDelete: Rule.SetNull).Nullable();
        }

        //#7243
        var vendorTableName = nameof(Vendor);
        var pmCustomerIdColumnName = nameof(Vendor.PmCustomerId);
		logger.LogDebug("Up(): vendorTableName → {VendorTableName}", vendorTableName);
		logger.LogDebug("Up(): pmCustomerIdColumnName → {PmCustomerIdColumnName}", pmCustomerIdColumnName);
        if (!Schema.Table(vendorTableName).Column(pmCustomerIdColumnName).Exists())
            Alter.Table(vendorTableName).AddColumn(pmCustomerIdColumnName).AsInt32().Nullable();
			logger.LogDebug("<<< Exiting Up()");
    }
}
