using FluentMigrator;
using Nop.Core.Domain.FilterLevels;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-07-03 00:00:00", "Filter level (YMM)")]
public class FilterLevelMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!Schema.Table(nameof(FilterLevelValue)).Exists())
            Create.TableFor<FilterLevelValue>();

        if (!Schema.Table(nameof(FilterLevelValueProductMapping)).Exists())
            Create.TableFor<FilterLevelValueProductMapping>();
			_logger.LogDebug("<<< Exiting Up()");
    }
}
