using Microsoft.Extensions.Logging;
using LinqToDB;

namespace Nop.Data;

/// <summary>
/// Represents temporary storage
/// </summary>
/// <typeparam name="T">Storage record mapping class</typeparam>
public partial class TempSqlDataStorage<T> : TempTable<T>, ITempDataStorage<T> where T : class
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TempSqlDataStorage(string storageName, IQueryable<T> query, IDataContext dataConnection)
        : base(dataConnection, storageName, query, tableOptions: TableOptions.NotSet | TableOptions.CheckExistence)
    {
		_logger.LogError(">>> Entering TempSqlDataStorage(storageName={StorageName},query={Query},dataConnection={DataConnection})", storageName, dataConnection);
		_logger.LogDebug("TempSqlDataStorage(storageName={StorageName},query={Query},dataConnection={DataConnection}): dataConnection.CloseAfterUse → {Arg3}", storageName, dataConnection, dataConnection.CloseAfterUse);
    _logger.LogWarning("<<< Exiting TempSqlDataStorage(storageName={StorageName},query={Query},dataConnection={DataConnection})", storageName, dataConnection);
        dataConnection.CloseAfterUse = true;
    }

    #endregion
}