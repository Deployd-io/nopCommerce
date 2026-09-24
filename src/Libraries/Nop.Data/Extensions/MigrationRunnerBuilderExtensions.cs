using Microsoft.Extensions.Logging;
using FluentMigrator.Runner;

namespace Nop.Data.Extensions;

public static class MigrationRunnerBuilderExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static IMigrationRunnerBuilder SetCommandTimeout(this IMigrationRunnerBuilder builder)
    {
		_logger.LogError(">>> Entering SetCommandTimeout(builder={Builder})", builder);
        var dataSettings = DataSettingsManager.LoadSettings();

		_logger.LogError("SetCommandTimeout(builder={Builder}): dataSettings → {DataSettings}", builder, dataSettings);
		_logger.LogWarning("<<< Exiting SetCommandTimeout(builder={Builder})", builder);
        return dataSettings is not { SQLCommandTimeout: { } }
            ? builder
            : builder.WithGlobalCommandTimeout(TimeSpan.FromSeconds(dataSettings.SQLCommandTimeout.Value));
    }
}
