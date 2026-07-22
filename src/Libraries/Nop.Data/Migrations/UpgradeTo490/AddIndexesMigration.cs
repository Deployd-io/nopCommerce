using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-28 00:00:01", "AddIndexesMigration for 4.90.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        //#7296
        var topicTableName = nameof(Topic);
        var topicEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
		logger.LogDebug("Up(): topicTableName → {TopicTableName}", topicTableName);
		logger.LogDebug("Up(): topicEndDateColumnName → {TopicEndDateColumnName}", topicEndDateColumnName);
        var topicStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
        var topicAvailableDatesIndexName = "IX_Topic_Availability";
		logger.LogDebug("Up(): topicStartDateColumnName → {TopicStartDateColumnName}", topicStartDateColumnName);
		logger.LogDebug("Up(): topicAvailableDatesIndexName → {TopicAvailableDatesIndexName}", topicAvailableDatesIndexName);
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
		logger.LogDebug("Up(): productTableName → {ProductTableName}", productTableName);
		logger.LogDebug("Up(): nameColumnName → {NameColumnName}", nameColumnName);
        var skuColumnName = nameof(Product.Sku);
        var manufacturerPartNumberColumnName = nameof(Product.ManufacturerPartNumber);
		logger.LogDebug("Up(): skuColumnName → {SkuColumnName}", skuColumnName);
		logger.LogDebug("Up(): manufacturerPartNumberColumnName → {ManufacturerPartNumberColumnName}", manufacturerPartNumberColumnName);
        var idColumnName = nameof(Product.Id);
        var deletedColumnName = nameof(Product.Deleted);
		logger.LogDebug("Up(): idColumnName → {IdColumnName}", idColumnName);
		logger.LogDebug("Up(): deletedColumnName → {DeletedColumnName}", deletedColumnName);
        var productSearchIndexName = "IX_Product_Search";
        if (!Schema.Table(productTableName).Index(productSearchIndexName).Exists())
			logger.LogDebug("Up(): productSearchIndexName → {ProductSearchIndexName}", productSearchIndexName);
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
		logger.LogDebug("Up(): productAttributeCombinationTableName → {ProductAttributeCombinationTableName}", productAttributeCombinationTableName);
        var productIdColumnName = nameof(ProductAttributeCombination.ProductId);
        var productAttributeCombinationSkuIndexName = "IX_ProductAttributeCombination_Sku";
		logger.LogDebug("Up(): productIdColumnName → {ProductIdColumnName}", productIdColumnName);
		logger.LogDebug("Up(): productAttributeCombinationSkuIndexName → {ProductAttributeCombinationSkuIndexName}", productAttributeCombinationSkuIndexName);
        if (!Schema.Table(productAttributeCombinationTableName).Index(productAttributeCombinationSkuIndexName).Exists())
            Create.Index(productAttributeCombinationSkuIndexName)
                .OnTable(productAttributeCombinationTableName)
                .OnColumn(skuColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var categoryTableName = nameof(Category);
        nameColumnName = nameof(Category.Name);
		logger.LogDebug("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
        deletedColumnName = nameof(Category.Deleted);
        idColumnName = nameof(Category.Id);
        var categoryNameDeletedIndexName = "IX_Category_Name_Deleted";
        if (!Schema.Table(categoryTableName).Index(categoryNameDeletedIndexName).Exists())
			logger.LogDebug("Up(): categoryNameDeletedIndexName → {CategoryNameDeletedIndexName}", categoryNameDeletedIndexName);
            Create.Index(categoryNameDeletedIndexName)
                .OnTable(categoryTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var manufacturerTableName = nameof(Manufacturer);
        nameColumnName = nameof(Manufacturer.Name);
		logger.LogDebug("Up(): manufacturerTableName → {ManufacturerTableName}", manufacturerTableName);
        deletedColumnName = nameof(Manufacturer.Deleted);
        idColumnName = nameof(Manufacturer.Id);
        var manufacturerNameDeletedIndexName = "IX_Manufacturer_Name_Deleted";
        if (!Schema.Table(manufacturerTableName).Index(manufacturerNameDeletedIndexName).Exists())
			logger.LogDebug("Up(): manufacturerNameDeletedIndexName → {ManufacturerNameDeletedIndexName}", manufacturerNameDeletedIndexName);
            Create.Index(manufacturerNameDeletedIndexName)
                .OnTable(manufacturerTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productCategoryTableName = NameCompatibilityManager.GetTableName(typeof(ProductCategory));
        var categoryIdColumnName = nameof(ProductCategory.CategoryId);
		logger.LogDebug("Up(): productCategoryTableName → {ProductCategoryTableName}", productCategoryTableName);
		logger.LogDebug("Up(): categoryIdColumnName → {CategoryIdColumnName}", categoryIdColumnName);
        productIdColumnName = nameof(ProductCategory.ProductId);
        var productCategoryMappingIndexName = "IX_ProductCategoryMapping_CategoryId";
		logger.LogDebug("Up(): productCategoryMappingIndexName → {ProductCategoryMappingIndexName}", productCategoryMappingIndexName);
        if (!Schema.Table(productCategoryTableName).Index(productCategoryMappingIndexName).Exists())
            Create.Index(productCategoryMappingIndexName)
                .OnTable(productCategoryTableName)
                .OnColumn(categoryIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var productManufacturerTableName = NameCompatibilityManager.GetTableName(typeof(ProductManufacturer));
        var manufacturerIdColumnName = nameof(ProductManufacturer.ManufacturerId);
		logger.LogDebug("Up(): productManufacturerTableName → {ProductManufacturerTableName}", productManufacturerTableName);
		logger.LogDebug("Up(): manufacturerIdColumnName → {ManufacturerIdColumnName}", manufacturerIdColumnName);
        productIdColumnName = nameof(ProductManufacturer.ProductId);
        var productManufacturerMappingIndexName = "IX_ProductManufacturerMapping_ManufacturerId";
		logger.LogDebug("Up(): productManufacturerMappingIndexName → {ProductManufacturerMappingIndexName}", productManufacturerMappingIndexName);
        if (!Schema.Table(productManufacturerTableName).Index(productManufacturerMappingIndexName).Exists())
            Create.Index(productManufacturerMappingIndexName)
                .OnTable(productManufacturerTableName)
                .OnColumn(manufacturerIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
				logger.LogDebug("<<< Exiting Up()");
    }
}
