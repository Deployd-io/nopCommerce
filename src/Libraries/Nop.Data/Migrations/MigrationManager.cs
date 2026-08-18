using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations;

/// <summary>
/// Represents the migration manager
/// </summary>
public partial class MigrationManager : IMigrationManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IFilteringMigrationSource _filteringMigrationSource;
    protected readonly IMigrationRunner _migrationRunner;
    protected readonly IMigrationRunnerConventions _migrationRunnerConventions;
    protected readonly Lazy<IVersionLoader> _versionLoader;

    #endregion

    #region Ctor

    public MigrationManager(
        IFilteringMigrationSource filteringMigrationSource,
        IMigrationRunner migrationRunner,
        IMigrationRunnerConventions migrationRunnerConventions,
        Lazy<IVersionLoader> versionLoader)
    {
		_logger.LogDebug(">>> Entering MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader})", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader);
        _versionLoader = versionLoader;
		_logger.LogDebug("MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader}): _versionLoader → {_versionLoader}", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader, _versionLoader);

        _filteringMigrationSource = filteringMigrationSource;
		_logger.LogDebug("MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader}): _filteringMigrationSource → {_filteringMigrationSource}", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader, _filteringMigrationSource);
        _migrationRunner = migrationRunner;
		_logger.LogDebug("MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader}): _migrationRunner → {_migrationRunner}", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader, _migrationRunner);
        _migrationRunnerConventions = migrationRunnerConventions;
		_logger.LogDebug("MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader}): _migrationRunnerConventions → {_migrationRunnerConventions}", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader, _migrationRunnerConventions);
		_logger.LogDebug("<<< Exiting MigrationManager(filteringMigrationSource={FilteringMigrationSource},migrationRunner={MigrationRunner},migrationRunnerConventions={MigrationRunnerConventions},versionLoader={VersionLoader})", filteringMigrationSource, migrationRunner, migrationRunnerConventions, versionLoader);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Up or Down process (depends on isApplied parameter)
    /// </summary>
    /// <param name="assembly">Assembly to find migrations</param>
    /// <param name="exactlyProcessType">Indicate whether to found exactly specified migration process type; if it set to true, you should set the migrationProcessType parameter to different of NoMatter</param>
    /// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
    /// <param name="isApplied">A value indicating whether to select those migrations that have been applied</param>
    /// <param name="isSchemaMigration">Indicate whether to found only schema migration; if set to true will by returns only that migrations which has the IsSchemaMigration property set to true</param>
    /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
    protected virtual IEnumerable<IMigrationInfo> GetMigrations(Assembly assembly, bool exactlyProcessType,
        MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter, bool isApplied = false, bool isSchemaMigration = false)
    {
		_logger.LogDebug(">>> Entering GetMigrations(assembly={Assembly},exactlyProcessType={ExactlyProcessType},migrationProcessType={MigrationProcessType},isApplied={IsApplied},isSchemaMigration={IsSchemaMigration})", assembly, exactlyProcessType, migrationProcessType, isApplied, isSchemaMigration);
        if (exactlyProcessType && migrationProcessType == MigrationProcessType.NoMatter)
			_logger.LogWarning("Throwing ArgumentException from GetMigrations(assembly={Assembly},exactlyProcessType={ExactlyProcessType},migrationProcessType={MigrationProcessType},isApplied={IsApplied},isSchemaMigration={IsSchemaMigration})", assembly, exactlyProcessType, migrationProcessType, isApplied, isSchemaMigration);
            throw new ArgumentException("Migration process type can't be NoMatter if exactlyProcessType set to true", nameof(migrationProcessType));

        //load all version data stored in the version table
        //we do this for avoid a problem with state of migration
        _versionLoader.Value.LoadVersionInfo();

        var migrations = _filteringMigrationSource
            .GetMigrations(t =>
            {
                var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();
				_logger.LogDebug("GetMigrations(assembly={Assembly},exactlyProcessType={ExactlyProcessType},migrationProcessType={MigrationProcessType},isApplied={IsApplied},isSchemaMigration={IsSchemaMigration}): migrationAttribute → {MigrationAttribute}", assembly, exactlyProcessType, migrationProcessType, isApplied, isSchemaMigration, migrationAttribute);

                if (migrationAttribute is null)
                    return false;

                if (isSchemaMigration && !migrationAttribute.IsSchemaMigration)
                    return false;

                if (isApplied == !_versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                    return false;

                if (exactlyProcessType && migrationProcessType != migrationAttribute.TargetMigrationProcess)
                    return false;

                if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                    migrationProcessType != MigrationProcessType.NoMatter &&
                    migrationProcessType != migrationAttribute.TargetMigrationProcess)
                    return false;

		_logger.LogDebug("<<< Exiting GetMigrations(assembly={Assembly},exactlyProcessType={ExactlyProcessType},migrationProcessType={MigrationProcessType},isApplied={IsApplied},isSchemaMigration={IsSchemaMigration})", assembly, exactlyProcessType, migrationProcessType, isApplied, isSchemaMigration);
                return assembly == null || t.Assembly == assembly;
            }) ?? Enumerable.Empty<IMigration>();

        return migrations.Select(_migrationRunnerConventions.GetMigrationInfoForMigration);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes an Up for all found unapplied migrations
    /// </summary>
    /// <param name="assembly">Assembly to find migrations</param>
    /// <param name="migrationProcessType">Type of migration process</param>
    /// <param name="commitVersionOnly">Commit only version information</param>
    public virtual void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation, bool commitVersionOnly = false)
    {
		_logger.LogDebug(">>> Entering ApplyUpMigrations(assembly={Assembly},migrationProcessType={MigrationProcessType},commitVersionOnly={CommitVersionOnly})", assembly, migrationProcessType, commitVersionOnly);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, commitVersionOnly && migrationProcessType != MigrationProcessType.NoMatter, migrationProcessType).OrderBy(migration => migration.Version))
            ApplyUpMigration(migrationInfo, commitVersionOnly);
			_logger.LogDebug("<<< Exiting ApplyUpMigrations(assembly={Assembly},migrationProcessType={MigrationProcessType},commitVersionOnly={CommitVersionOnly})", assembly, migrationProcessType, commitVersionOnly);
    }

    /// <summary>
    /// Executes an Up for schema unapplied migrations
    /// </summary>
    /// <param name="assembly">Assembly to find migrations</param>
    public virtual void ApplyUpSchemaMigrations(Assembly assembly)
    {
		_logger.LogDebug(">>> Entering ApplyUpSchemaMigrations(assembly={Assembly})", assembly);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, isSchemaMigration: true).OrderBy(migration => migration.Version))
            ApplyUpMigration(migrationInfo);
			_logger.LogDebug("<<< Exiting ApplyUpSchemaMigrations(assembly={Assembly})", assembly);
    }

    /// <summary>
    /// Executes a Down for all found (and applied) migrations
    /// </summary>
    /// <param name="assembly">Assembly to find the migration</param>
    public virtual void ApplyDownMigrations(Assembly assembly)
    {
		_logger.LogDebug(">>> Entering ApplyDownMigrations(assembly={Assembly})", assembly);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, true).OrderByDescending(migration => migration.Version))
            ApplyDownMigration(migrationInfo);
			_logger.LogDebug("<<< Exiting ApplyDownMigrations(assembly={Assembly})", assembly);
    }

    /// <summary>
    /// Executes down expressions for the passed migration
    /// </summary>
    /// <param name="migrationInfo">Migration to rollback</param>
    public virtual void ApplyDownMigration(IMigrationInfo migrationInfo)
    {
		_logger.LogDebug(">>> Entering ApplyDownMigration(migrationInfo={MigrationInfo})", migrationInfo);
        ArgumentNullException.ThrowIfNull(migrationInfo);

        _migrationRunner.Down(migrationInfo.Migration);
        _versionLoader.Value.DeleteVersion(migrationInfo.Version);
		_logger.LogDebug("<<< Exiting ApplyDownMigration(migrationInfo={MigrationInfo})", migrationInfo);
    }

    /// <summary>
    /// Executes up expressions for the passed migration
    /// </summary>
    /// <param name="migrationInfo">Migration to apply</param>
    /// <param name="commitVersionOnly">Commit only version information</param>
    public virtual void ApplyUpMigration(IMigrationInfo migrationInfo, bool commitVersionOnly = false)
    {
		_logger.LogDebug(">>> Entering ApplyUpMigration(migrationInfo={MigrationInfo},commitVersionOnly={CommitVersionOnly})", migrationInfo, commitVersionOnly);
        ArgumentNullException.ThrowIfNull(migrationInfo);

        if (!commitVersionOnly)
            _migrationRunner.Up(migrationInfo.Migration);
#if DEBUG
        if (!migrationInfo.IsNeedToApplyInDbOnDebugMode())
            return;
#endif
        _versionLoader.Value
            .UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
    }

    #endregion
}