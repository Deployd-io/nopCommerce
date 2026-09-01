using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;
using Nop.Data.DataProviders;
using Microsoft.Extensions.Logging;

namespace Nop.Data;

/// <summary>
/// Represents the data provider manager
/// </summary>
public partial class DataProviderManager : IDataProviderManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Gets data provider by specific type
    /// </summary>
    /// <param name="dataProviderType">Data provider type</param>
    /// <returns></returns>
    public static INopDataProvider GetDataProvider(DataProviderType dataProviderType)
    {
		_logger.LogDebug(">>> Entering GetDataProvider(dataProviderType={DataProviderType})", dataProviderType);
		_logger.LogDebug("<<< Exiting GetDataProvider(dataProviderType={DataProviderType})", dataProviderType);
        return dataProviderType switch
        {
            DataProviderType.SqlServer => new MsSqlNopDataProvider(),
            DataProviderType.MySql => new MySqlNopDataProvider(),
            DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
            _ => throw new NopException($"Not supported data provider name: '{dataProviderType}'"),
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets data provider
    /// </summary>
    public INopDataProvider DataProvider
    {
        get
        {
            var dataProviderType = Singleton<DataConfig>.Instance.DataProvider;

            return GetDataProvider(dataProviderType);
        }
    }

    #endregion
}