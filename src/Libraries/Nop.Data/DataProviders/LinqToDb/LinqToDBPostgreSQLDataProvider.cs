using System.Data.Common;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders.LinqToDB;

/// <summary>
/// Represents a data provider for PostgreSQL
/// </summary>
public partial class LinqToDBPostgreSQLDataProvider : PostgreSQLDataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LinqToDBPostgreSQLDataProvider() : base(ProviderName.PostgreSQL92, PostgreSQLVersion.v92) { }

    public override void SetParameter(DataConnection dataConnection, DbParameter parameter, string name, DbDataType dataType, object value)
    {
	_logger.LogError(">>> Entering SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value})", dataConnection, parameter, name, dataType, value);

        if (value is string && dataType.SystemType == typeof(string))
        {
            dataType = dataType.WithDbType("citext");
			_logger.LogError("SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value}): dataType → {DataType1}", dataConnection, parameter, name, dataType, value, dataType);
        }

        base.SetParameter(dataConnection, parameter, name, dataType, value);
		_logger.LogWarning("<<< Exiting SetParameter(dataConnection={DataConnection},parameter={Parameter},name={Name},dataType={DataType},value={Value})", dataConnection, parameter, name, dataType, value);
    }
}