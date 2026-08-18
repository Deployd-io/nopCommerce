using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-03-16 00:00:00", "Product video")]
public class VideoMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Video))).Exists())
            Create.TableFor<Video>();

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ProductVideo))).Exists())
            Create.TableFor<ProductVideo>();
			_logger.LogDebug("<<< Exiting Up()");
    }
}