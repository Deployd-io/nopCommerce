using FluentMigrator.Runner;

namespace Nop.Data.Extensions;

public static class MigrationRunnerBuilderExtensions
{
    public static IMigrationRunnerBuilder SetCommandTimeout(this IMigrationRunnerBuilder builder)
    {
		logger.LogDebug(">>> Entering SetCommandTimeout(builder={Builder})", builder);
        var dataSettings = DataSettingsManager.LoadSettings();

		logger.LogDebug("SetCommandTimeout(builder={Builder}): dataSettings → {DataSettings}", builder, dataSettings);
		logger.LogDebug("<<< Exiting SetCommandTimeout(builder={Builder})", builder);
        return dataSettings is not { SQLCommandTimeout: { } }
            ? builder
            : builder.WithGlobalCommandTimeout(TimeSpan.FromSeconds(dataSettings.SQLCommandTimeout.Value));
    }
}
