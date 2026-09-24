using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo440;

[NopSchemaMigration("2020/03/08 11:26:08:9037680", "Specification attribute grouping")]
public class SpecificationAttributeGroupingMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttributeGroup))).Exists())
            Create.TableFor<SpecificationAttributeGroup>();

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute))).Column(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).Exists())
        {
            //add new column
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute)))
                .AddColumn(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).AsInt32().Nullable().ForeignKey<SpecificationAttributeGroup>();
        }
		_logger.LogWarning("<<< Exiting Up()");
    }

    #endregion
}