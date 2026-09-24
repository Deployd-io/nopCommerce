using System.Data;
using FluentMigrator.Runner.Generators.MySql;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders.Fluentmigrator;

public partial class NopMySql5TypeMap : MySql5TypeMap
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected override void SetupMySqlTypeMaps()
    {
		_logger.LogError(">>> Entering SetupMySqlTypeMaps()");
        var dataSettings = DataSettingsManager.LoadSettings();

		_logger.LogError("SetupMySqlTypeMaps(): dataSettings → {DataSettings}", dataSettings);
        if (dataSettings.DataProvider != DataProviderType.MySql)
            return;

        base.SetupMySqlTypeMaps();

        var charSet = !string.IsNullOrEmpty(dataSettings.Collation) && !string.IsNullOrEmpty(dataSettings.CharacterSet)
            ? $"CHARACTER SET {dataSettings.CharacterSet}" : string.Empty;

        var collation = !string.IsNullOrEmpty(dataSettings.Collation) ? $"COLLATE {dataSettings.Collation}" : string.Empty;
		_logger.LogError("SetupMySqlTypeMaps(): collation → {Collation}", collation);

        SetTypeMap(DbType.StringFixedLength, $"TEXT {charSet} {collation}", TextCapacity);
        SetTypeMap(DbType.StringFixedLength, $"MEDIUMTEXT {charSet} {collation}", MediumTextCapacity);
        SetTypeMap(DbType.StringFixedLength, $"LONGTEXT {charSet} {collation}", LongTextCapacity);

        SetTypeMap(DbType.String, $"TEXT {charSet} {collation}", TextCapacity);
        SetTypeMap(DbType.String, $"MEDIUMTEXT {charSet} {collation}", MediumTextCapacity);
        SetTypeMap(DbType.String, $"LONGTEXT {charSet} {collation}", LongTextCapacity);
		_logger.LogWarning("<<< Exiting SetupMySqlTypeMaps()");
    }
}
