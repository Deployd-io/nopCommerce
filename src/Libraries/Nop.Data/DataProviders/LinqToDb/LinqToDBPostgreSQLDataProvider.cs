using System.Data.Common;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;

namespace Nop.Data.DataProviders.LinqToDB;

/// <summary>
/// Represents a data provider for PostgreSQL
/// </summary>
public partial class LinqToDBPostgreSQLDataProvider : PostgreSQLDataProvider
{
    public LinqToDBPostgreSQLDataProvider() : base(ProviderName.PostgreSQL92, PostgreSQLVersion.v92) { }

    public override void SetParameter(DataConnection dataConnection, DbParameter parameter, string name, DbDataType dataType, object value)
    {
	logger.LogDebug(">>> Entering SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value})", dataConnection, parameter, name, dataType, value);

        if (value is string && dataType.SystemType == typeof(string))
        {
            dataType = dataType.WithDbType("citext");
        }
		logger.LogDebug("SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value}): dataType → {DataType1}", dataConnection, parameter, name, dataType, value, dataType);

        base.SetParameter(dataConnection, parameter, name, dataType, value);
		logger.LogDebug("<<< Exiting SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value})", dataConnection, parameter, name, dataType, value);
    }
}
