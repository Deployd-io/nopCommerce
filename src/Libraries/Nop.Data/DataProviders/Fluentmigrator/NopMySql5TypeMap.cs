using System.Data;
using FluentMigrator.Runner.Generators.MySql;

namespace Nop.Data.DataProviders.Fluentmigrator;

public partial class NopMySql5TypeMap : MySql5TypeMap
{
    protected override void SetupMySqlTypeMaps()
    {
		logger.LogDebug(">>> Entering SetupMySqlTypeMaps()");
        var dataSettings = DataSettingsManager.LoadSettings();

		logger.LogDebug("SetupMySqlTypeMaps(): dataSettings → {DataSettings}", dataSettings);
        if (dataSettings.DataProvider != DataProviderType.MySql)
            return;

        base.SetupMySqlTypeMaps();

        var charSet = !string.IsNullOrEmpty(dataSettings.Collation) && !string.IsNullOrEmpty(dataSettings.CharacterSet)
            ? $"CHARACTER SET {dataSettings.CharacterSet}" : string.Empty;

        var collation = !string.IsNullOrEmpty(dataSettings.Collation) ? $"COLLATE {dataSettings.Collation}" : string.Empty;

		logger.LogDebug("SetupMySqlTypeMaps(): collation → {Collation}", collation);
        SetTypeMap(DbType.StringFixedLength, $"TEXT {charSet} {collation}", TextCapacity);
        SetTypeMap(DbType.StringFixedLength, $"MEDIUMTEXT {charSet} {collation}", MediumTextCapacity);
        SetTypeMap(DbType.StringFixedLength, $"LONGTEXT {charSet} {collation}", LongTextCapacity);

        SetTypeMap(DbType.String, $"TEXT {charSet} {collation}", TextCapacity);
        SetTypeMap(DbType.String, $"MEDIUMTEXT {charSet} {collation}", MediumTextCapacity);
        SetTypeMap(DbType.String, $"LONGTEXT {charSet} {collation}", LongTextCapacity);
		logger.LogDebug("<<< Exiting SetupMySqlTypeMaps()");
    }
}
