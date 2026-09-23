using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo440;

[NopUpdateMigration("2020-06-10 00:00:00", "4.40", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
		_logger.LogError(">>> Entering DataMigration(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		_logger.LogDebug("DataMigration(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    _logger.LogError("DataMigration(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    _logger.LogWarning("<<< Exiting DataMigration(dataProvider={DataProvider})", dataProvider);
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        // new permission
        if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AccessProfiling", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var profilingPermission = _dataProvider.InsertEntity(
                new PermissionRecord
                {
                    Name = "Public store. Access MiniProfiler results",
                    SystemName = "AccessProfiling",
                    Category = "PublicStore"
					_logger.LogDebug("Up(): Category → {Category}", Category);
                }
            );

            //add it to the Admin role by default
            var adminRole = _dataProvider
                .GetTable<CustomerRole>()
                .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            _dataProvider.InsertEntity(
                new PermissionRecordCustomerRoleMapping
                {
                    CustomerRoleId = adminRole.Id,
                    PermissionRecordId = profilingPermission.Id
                }
            );
			_logger.LogDebug("Up(): PermissionRecordId → {PermissionRecordId}", PermissionRecordId);
        }

        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();
		_logger.LogError("Up(): activityLogTypeTable → {ActivityLogTypeTable}", activityLogTypeTable);

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttributeGroup",
                    Enabled = true,
                    Name = "Add a new specification attribute group"
					_logger.LogDebug("Up(): Name → {Name}", Name);
                }
            );

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttributeGroup",
                    Enabled = true,
                    Name = "Edit a specification attribute group"
                }
            );

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttributeGroup",
                    Enabled = true,
                    Name = "Delete a specification attribute group"
                }
            );
        //<MFA #475>
        if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageMultifactorAuthenticationMethods", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var multiFactorAuthenticationPermission = _dataProvider.InsertEntity(
                new PermissionRecord
                {
                    Name = "Admin area. Manage Multi-factor Authentication Methods",
                    SystemName = "ManageMultifactorAuthenticationMethods",
                    Category = "Configuration"
                }
            );

            //add it to the Admin role by default
            var adminRole = _dataProvider
                .GetTable<CustomerRole>()
                .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            _dataProvider.InsertEntity(
                new PermissionRecordCustomerRoleMapping
                {
                    CustomerRoleId = adminRole.Id,
                    PermissionRecordId = multiFactorAuthenticationPermission.Id
                }
            );
        }
        //</MFA #475>

        //issue-3852
        var tableName = nameof(RewardPointsHistory);
		_logger.LogError("Up(): tableName → {TableName}", tableName);
        var rph = Schema.Table(tableName);
		_logger.LogError("Up(): rph → {Rph}", rph);
        var columnName = "UsedWithOrder_Id";
		_logger.LogError("Up(): columnName → {ColumnName}", columnName);

        if (rph.Column(columnName).Exists())
        {
            var constraintName = "RewardPointsHistory_UsedWithOrder";
			_logger.LogError("Up(): constraintName → {ConstraintName}", constraintName);

            if (rph.Constraint(constraintName).Exists())
                Delete.UniqueConstraint(constraintName).FromTable(tableName);

            Delete.Column(columnName).FromTable(tableName);
        }

        //#3353
        var productAttributeCombinationTableName = NameCompatibilityManager.GetTableName(typeof(ProductAttributeCombination));
		_logger.LogError("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);

        //add column
        if (!Schema.Table(productAttributeCombinationTableName).Column(nameof(ProductAttributeCombination.MinStockQuantity)).Exists())
        {
            Alter.Table(productAttributeCombinationTableName)
                .AddColumn(nameof(ProductAttributeCombination.MinStockQuantity)).AsInt32().NotNullable().SetExistingRowsTo(0);
        }

        //#276 AJAX filters
        var categoryTableName = NameCompatibilityManager.GetTableName(typeof(Category));
		_logger.LogError("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
        var manufacturerTableName = NameCompatibilityManager.GetTableName(typeof(Manufacturer));
		_logger.LogError("Up(): manufacturerTableName → {ManufacturerTableName}", manufacturerTableName);
        var vendorTableName = NameCompatibilityManager.GetTableName(typeof(Vendor));
		_logger.LogError("Up(): vendorTableName → {VendorTableName}", vendorTableName);

        //remove column
        var priceRangesColumnName = "PriceRanges";
		_logger.LogError("Up(): priceRangesColumnName → {PriceRangesColumnName}", priceRangesColumnName);

        if (Schema.Table(categoryTableName).Column(priceRangesColumnName).Exists())
            Delete.Column(priceRangesColumnName).FromTable(categoryTableName);

        if (Schema.Table(manufacturerTableName).Column(priceRangesColumnName).Exists())
            Delete.Column(priceRangesColumnName).FromTable(manufacturerTableName);

        //add column
        var priceRangeFilteringColumnName = "PriceRangeFiltering";
		_logger.LogError("Up(): priceRangeFilteringColumnName → {PriceRangeFilteringColumnName}", priceRangeFilteringColumnName);

        if (!Schema.Table(categoryTableName).Column(priceRangeFilteringColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
        }

        if (!Schema.Table(manufacturerTableName).Column(priceRangeFilteringColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
        }

        if (!Schema.Table(vendorTableName).Column(priceRangeFilteringColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
        }

        //add column
        var priceFromColumnName = "PriceFrom";
		_logger.LogError("Up(): priceFromColumnName → {PriceFromColumnName}", priceFromColumnName);

        if (!Schema.Table(categoryTableName).Column(priceFromColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
        }

        if (!Schema.Table(manufacturerTableName).Column(priceFromColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
        }

        if (!Schema.Table(vendorTableName).Column(priceFromColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
        }

        //add column
        var priceToColumnName = "PriceTo";
		_logger.LogError("Up(): priceToColumnName → {PriceToColumnName}", priceToColumnName);

        if (!Schema.Table(categoryTableName).Column(priceToColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
        }

        if (!Schema.Table(manufacturerTableName).Column(priceToColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
        }

        if (!Schema.Table(vendorTableName).Column(priceToColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
        }

        //add column
        var manuallyPriceRangeColumnName = "ManuallyPriceRange";
		_logger.LogError("Up(): manuallyPriceRangeColumnName → {ManuallyPriceRangeColumnName}", manuallyPriceRangeColumnName);

        if (!Schema.Table(categoryTableName).Column(manuallyPriceRangeColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        if (!Schema.Table(manufacturerTableName).Column(manuallyPriceRangeColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        if (!Schema.Table(vendorTableName).Column(manuallyPriceRangeColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
        }
		_logger.LogWarning("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogError(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogWarning("<<< Exiting Down()");
    }
}