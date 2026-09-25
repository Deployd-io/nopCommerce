using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Topics;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-02 00:00:01", "SchemaMigration for 4.90.0")]
public class SchemaMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        //#7387
        var productTableName = nameof(Product);
		_logger.LogError("Up(): productTableName → {ProductTableName}", productTableName);

        var ageVerificationColumnName = nameof(Product.AgeVerification);
		_logger.LogError("Up(): ageVerificationColumnName → {AgeVerificationColumnName}", ageVerificationColumnName);
        if (!Schema.Table(productTableName).Column(ageVerificationColumnName).Exists())
        {
            Alter.Table(productTableName)
                .AddColumn(ageVerificationColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        var minimumAgeToPurchaseColumnName = nameof(Product.MinimumAgeToPurchase);
		_logger.LogError("Up(): minimumAgeToPurchaseColumnName → {MinimumAgeToPurchaseColumnName}", minimumAgeToPurchaseColumnName);
        if (!Schema.Table(productTableName).Column(minimumAgeToPurchaseColumnName).Exists())
        {
            Alter.Table(productTableName)
                .AddColumn(minimumAgeToPurchaseColumnName)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }

        //#7294
        var topicTableName = nameof(Topic);
		_logger.LogError("Up(): topicTableName → {TopicTableName}", topicTableName);
        var topicAvailableEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
		_logger.LogError("Up(): topicAvailableEndDateColumnName → {TopicAvailableEndDateColumnName}", topicAvailableEndDateColumnName);
        var topicAvailableStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
		_logger.LogError("Up(): topicAvailableStartDateColumnName → {TopicAvailableStartDateColumnName}", topicAvailableStartDateColumnName);

        if (!Schema.Table(topicTableName).Column(topicAvailableEndDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableEndDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.Table(topicTableName).Column(topicAvailableStartDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableStartDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        //#873
        var productTagTableName = nameof(ProductTag);
		_logger.LogError("Up(): productTagTableName → {ProductTagTableName}", productTagTableName);

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaDescription)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaDescription)).AsString().Nullable();

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaKeywords)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaKeywords)).AsString(400).Nullable();

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaTitle)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaTitle)).AsString(400).Nullable();

        //#7390
        if (!Schema.Table(nameof(Menu)).Exists())
            Create.TableFor<Menu>();

        if (!Schema.Table(nameof(MenuItem)).Exists())
            Create.TableFor<MenuItem>();

        var footerColumn1ColumnName = "IncludeInFooterColumn1";
		_logger.LogError("Up(): footerColumn1ColumnName → {FooterColumn1ColumnName}", footerColumn1ColumnName);
        if (Schema.Table(topicTableName).Column(footerColumn1ColumnName).Exists())
            Delete.Column(footerColumn1ColumnName).FromTable(topicTableName);

        var footerColumn2ColumnName = "IncludeInFooterColumn2";
		_logger.LogError("Up(): footerColumn2ColumnName → {FooterColumn2ColumnName}", footerColumn2ColumnName);
        if (Schema.Table(topicTableName).Column(footerColumn2ColumnName).Exists())
            Delete.Column(footerColumn2ColumnName).FromTable(topicTableName);

        var footerColumn3ColumnName = "IncludeInFooterColumn3";
		_logger.LogError("Up(): footerColumn3ColumnName → {FooterColumn3ColumnName}", footerColumn3ColumnName);
        if (Schema.Table(topicTableName).Column(footerColumn3ColumnName).Exists())
            Delete.Column(footerColumn3ColumnName).FromTable(topicTableName);

        var includeTopicInTopMenuColumnName = "IncludeInTopMenu";
		_logger.LogError("Up(): includeTopicInTopMenuColumnName → {IncludeTopicInTopMenuColumnName}", includeTopicInTopMenuColumnName);
        if (Schema.Table(topicTableName).Column(includeTopicInTopMenuColumnName).Exists())
            Delete.Column(includeTopicInTopMenuColumnName).FromTable(topicTableName);

        var categoryTableName = nameof(Category);
		_logger.LogError("Up(): categoryTableName → {CategoryTableName}", categoryTableName);
        var includeCategoryInTopMenuColumnName = "IncludeInTopMenu";
		_logger.LogError("Up(): includeCategoryInTopMenuColumnName → {IncludeCategoryInTopMenuColumnName}", includeCategoryInTopMenuColumnName);
        if (Schema.Table(categoryTableName).Column(includeCategoryInTopMenuColumnName).Exists())
            Delete.Column(includeCategoryInTopMenuColumnName).FromTable(categoryTableName);


    }
}
