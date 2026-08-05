using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-28 00:00:01", "AddIndexesMigration for 4.90.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        //#7296
        var topicTableName = nameof(Topic);
        var topicEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
		_logger.LogDebug("Up(): topicTableName → {TopicTableName}", topicTableName);
		_logger.LogDebug("Up(): topicEndDateColumnName → {TopicEndDateColumnName}", topicEndDateColumnName);
        var topicStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
        var topicAvailableDatesIndexName = "IX_Topic_Availability";
		_logger.LogDebug("Up(): topicStartDateColumnName → {TopicStartDateColumnName}", topicStartDateColumnName);
		_logger.LogDebug("Up(): topicAvailableDatesIndexName → {TopicAvailableDatesIndexName}", topicAvailableDatesIndexName);
        if (!Schema.Table(topicTableName).Index(topicAvailableDatesIndexName).Exists())
            Create.Index(topicAvailableDatesIndexName)
                .OnTable(topicTableName)
                .OnColumn(topicEndDateColumnName).Descending()
                .OnColumn(topicStartDateColumnName).Descending()
                .WithOptions().NonClustered();

        //#7676
        const string databaseType = "sqlserver";

        var productTableName = nameof(Product);
        var nameColumnName = nameof(Product.Name);
		_logger.LogDebug("Up(): productTableName → {ProductTableName}", productTableName);
		_logger.LogDebug("Up(): nameColumnName → {NameColumnName}", nameColumnName);
        var skuColumnName = nameof(Product.Sku);
        var manufacturerPartNumberColumnName = nameof(Product.ManufacturerPartNumber);
		_logger.LogDebug("Up(): skuColumnName → {SkuColumnName}", skuColumnName);
		_logger.LogDebug("Up(): manufacturerPartNumberColumnName → {ManufacturerPartNumberColumnName}", manufacturerPartNumberColumnName);
        var idColumnName = nameof(Product.Id);
        var deletedColumnName = nameof(Product.Deleted);
		_logger.LogDebug("Up(): idColumnName → {IdColumnName}", idColumnName);
		_logger.LogDebug("Up(): deletedColumnName → {DeletedColumnName}", deletedColumnName);
        var productSearchIndexName = "IX_Product_Search";
        if (!Schema.Table(productTableName).Index(productSearchIndexName).Exists())
			_logger.LogDebug("Up(): productSearchIndexName → {ProductSearchIndexName}", productSearchIndexName);
            IfDatabase(databaseType).Create.Index(productSearchIndexName)
                .OnTable(productTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(skuColumnName).Ascending()
                .OnColumn(manufacturerPartNumberColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
        skuColumnName = nameof(ProductAttributeCombination.Sku);
		_logger.LogDebug("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);
        var productIdColumnName = nameof(ProductAttributeCombination.ProductId);
		_logger.LogDebug("Up(): productIdColumnName → {ProductIdColumnName}", productIdColumnName);
        var productAttributeCombinationSkuIndexName = "IX_ProductAttributeCombination_Sku";
        if (!Schema.Table(productAttributeCombinationTableName).Index(productAttributeCombinationSkuIndexName).Exists())
			_logger.LogDebug("Up(): productAttributeCombinationSkuIndexName → {ProductAttributeCombinationSkuIndexName}", productAttributeCombinationSkuIndexName);
            Create.Index(productAttributeCombinationSkuIndexName)
                .OnTable(productAttributeCombinationTableName)
                .OnColumn(skuColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var categoryTableName = nameof(Category);
        nameColumnName = nameof(Category.Name);
		_logger.LogDebug("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
        deletedColumnName = nameof(Category.Deleted);
        idColumnName = nameof(Category.Id);
        var categoryNameDeletedIndexName = "IX_Category_Name_Deleted";
		_logger.LogDebug("Up(): categoryNameDeletedIndexName → {CategoryNameDeletedIndexName}", categoryNameDeletedIndexName);
        if (!Schema.Table(categoryTableName).Index(categoryNameDeletedIndexName).Exists())
            Create.Index(categoryNameDeletedIndexName)
                .OnTable(categoryTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var manufacturerTableName = nameof(Manufacturer);
        nameColumnName = nameof(Manufacturer.Name);
		_logger.LogDebug("Up(): manufacturerTableName → {ManufacturerTableName}", manufacturerTableName);
        deletedColumnName = nameof(Manufacturer.Deleted);
        idColumnName = nameof(Manufacturer.Id);
        var manufacturerNameDeletedIndexName = "IX_Manufacturer_Name_Deleted";
		_logger.LogDebug("Up(): manufacturerNameDeletedIndexName → {ManufacturerNameDeletedIndexName}", manufacturerNameDeletedIndexName);
        if (!Schema.Table(manufacturerTableName).Index(manufacturerNameDeletedIndexName).Exists())
            Create.Index(manufacturerNameDeletedIndexName)
                .OnTable(manufacturerTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productCategoryTableName = NameCompatibilityManager.GetTableName(typeof(ProductCategory));
        var categoryIdColumnName = nameof(ProductCategory.CategoryId);
		_logger.LogDebug("Up(): productCategoryTableName → {ProductCategoryTableName}", productCategoryTableName);
		_logger.LogDebug("Up(): categoryIdColumnName → {CategoryIdColumnName}", categoryIdColumnName);
        productIdColumnName = nameof(ProductCategory.ProductId);
        var productCategoryMappingIndexName = "IX_ProductCategoryMapping_CategoryId";
		_logger.LogDebug("Up(): productCategoryMappingIndexName → {ProductCategoryMappingIndexName}", productCategoryMappingIndexName);
        if (!Schema.Table(productCategoryTableName).Index(productCategoryMappingIndexName).Exists())
            Create.Index(productCategoryMappingIndexName)
                .OnTable(productCategoryTableName)
                .OnColumn(categoryIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var productManufacturerTableName = NameCompatibilityManager.GetTableName(typeof(ProductManufacturer));
        var manufacturerIdColumnName = nameof(ProductManufacturer.ManufacturerId);
		_logger.LogDebug("Up(): productManufacturerTableName → {ProductManufacturerTableName}", productManufacturerTableName);
		_logger.LogDebug("Up(): manufacturerIdColumnName → {ManufacturerIdColumnName}", manufacturerIdColumnName);
        productIdColumnName = nameof(ProductManufacturer.ProductId);
        var productManufacturerMappingIndexName = "IX_ProductManufacturerMapping_ManufacturerId";
		_logger.LogDebug("Up(): productManufacturerMappingIndexName → {ProductManufacturerMappingIndexName}", productManufacturerMappingIndexName);
        if (!Schema.Table(productManufacturerTableName).Index(productManufacturerMappingIndexName).Exists())
            Create.Index(productManufacturerMappingIndexName)
                .OnTable(productManufacturerTableName)
                .OnColumn(manufacturerIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
				_logger.LogDebug("<<< Exiting Up()");
    }
}