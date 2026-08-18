using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Nop.Core;
using Nop.Core.ComponentModel;
using Nop.Data;
using Nop.Data.DataProviders;
namespace Nop.Tests;

/// <summary>
/// Represents the SQLite data provider
/// </summary>
public partial class SqLiteNopDataProvider : BaseDataProvider, INopDataProvider
{
    #region Consts

    //it's quite fast hash (to cheaply distinguish between objects)
    private const string HASH_ALGORITHM = "SHA1";
    private static DataConnection _dataContext;

    private static readonly ReaderWriterLockSlim _locker = new();

    #endregion

    #region Methods

    public void CreateDatabase(int triesToConnect = 10)
    {
		_logger.LogDebug(">>> Entering CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
        ExecuteNonQueryAsync("PRAGMA journal_mode=WAL;").Wait();
		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): journal_mode → {Journal_mode}", triesToConnect, journal_mode);
		_logger.LogDebug("<<< Exiting CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
    }

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
		_logger.LogDebug(">>> Entering GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

		_logger.LogDebug("<<< Exiting GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
        return new SqliteConnection(string.IsNullOrEmpty(connectionString)
            ? DataSettingsManager.LoadSettings().ConnectionString
            : connectionString);
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>Inserted entity</returns>
    public override TEntity InsertEntity<TEntity>(TEntity entity)
    {
		_logger.LogDebug(">>> Entering InsertEntity(entity={Entity})", entity);
        using (new ReaderWriteLockDisposable(_locker))
        {
            entity.Id = DataContext.InsertWithInt32Identity(entity);
		_logger.LogDebug("<<< Exiting InsertEntity(entity={Entity})", entity);
            return entity;
        }
    }

    /// <summary>
    /// Insert a new entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>Entity</returns>
    public override Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity)
    {
		_logger.LogDebug(">>> Entering InsertEntityAsync(entity={Entity})", entity);
        InsertEntity(entity);

		_logger.LogDebug("<<< Exiting InsertEntityAsync(entity={Entity})", entity);
        return Task.FromResult(entity);
    }

    /// <summary>
    /// Updates record in table, using values from entity parameter.
    /// Record to update identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task UpdateEntityAsync<TEntity>(TEntity entity)
    {
		_logger.LogDebug(">>> Entering UpdateEntityAsync(entity={Entity})", entity);
        using (new ReaderWriteLockDisposable(_locker))
            DataContext.Update(entity);

		_logger.LogDebug("<<< Exiting UpdateEntityAsync(entity={Entity})", entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogDebug(">>> Entering UpdateEntitiesAsync(entities={Entities})", entities);
        foreach (var entity in entities)
            await UpdateEntityAsync(entity);
			_logger.LogDebug("<<< Exiting UpdateEntitiesAsync(entities={Entities})", entities);
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void UpdateEntities<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogDebug(">>> Entering UpdateEntities(entities={Entities})", entities);
        foreach (var entity in entities)
            UpdateEntity(entity);
			_logger.LogDebug("<<< Exiting UpdateEntities(entities={Entities})", entities);
    }

    /// <summary>
    /// Deletes record in table. Record to delete identified
    /// by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task DeleteEntityAsync<TEntity>(TEntity entity)
    {
		_logger.LogDebug(">>> Entering DeleteEntityAsync(entity={Entity})", entity);
        using (new ReaderWriteLockDisposable(_locker))
            DataContext.Delete(entity);

		_logger.LogDebug("<<< Exiting DeleteEntityAsync(entity={Entity})", entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs delete records in a table
    /// </summary>
    /// <param name="entities">Entities for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities)
    {
		_logger.LogDebug(">>> Entering BulkDeleteEntitiesAsync(entities={Entities})", entities);
        using (new ReaderWriteLockDisposable(_locker))
        {
            foreach (var entity in entities)
                DataContext.Delete(entity);
        }

		_logger.LogDebug("<<< Exiting BulkDeleteEntitiesAsync(entities={Entities})", entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs delete records in a table by a condition
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task<int> BulkDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(DataContext.GetTable<TEntity>()
            .Where(predicate).Delete());
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogDebug(">>> Entering BulkInsertEntitiesAsync(entities={Entities})", entities);
        foreach (var entity in entities)
            InsertEntity(entity);

		_logger.LogDebug("<<< Exiting BulkInsertEntitiesAsync(entities={Entities})", entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the name of a foreign key
    /// </summary>
    /// <param name="foreignTable">Foreign key table</param>
    /// <param name="foreignColumn">Foreign key column name</param>
    /// <param name="primaryTable">Primary table</param>
    /// <param name="primaryColumn">Primary key column name</param>
    /// <returns>Name of a foreign key</returns>
    public string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn)
    {
		_logger.LogDebug(">>> Entering CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
		_logger.LogDebug("<<< Exiting CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
        return "FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), HASH_ALGORITHM);
    }

    /// <summary>
    /// Gets the name of an index
    /// </summary>
    /// <param name="targetTable">Target table name</param>
    /// <param name="targetColumn">Target column name</param>
    /// <returns>Name of an index</returns>
    public string GetIndexName(string targetTable, string targetColumn)
    {
		_logger.LogDebug(">>> Entering GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
		_logger.LogDebug("<<< Exiting GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
        return "IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), HASH_ALGORITHM);
    }

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    public override IQueryable<TEntity> GetTable<TEntity>()
    {
		_logger.LogDebug(">>> Entering GetTable()");
        using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
			_logger.LogDebug("<<< Exiting GetTable()");
            return DataContext.GetTable<TEntity>();
    }

    /// <summary>
    /// Get the current identity value
    /// </summary>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <returns>Integer identity; null if cannot get the result</returns>
    public Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering GetTableIdentAsync()");
        using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
        {
            var tableName = DataContext.GetTable<TEntity>().TableName;

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var result = DataContext.Query<int?>($"select seq from sqlite_sequence where name = \"{tableName}\"")
                .FirstOrDefault();
				_logger.LogInformation("GetTableIdentAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting GetTableIdentAsync()");
            return Task.FromResult<int?>(result ?? 1);
        }
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>Returns true if the database exists.</returns>
    public bool DatabaseExists()
    {
		_logger.LogDebug(">>> Entering DatabaseExists()");
		_logger.LogDebug("<<< Exiting DatabaseExists()");
        return true;
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    public virtual Task BackupDatabaseAsync(string fileName)
    {
		_logger.LogDebug(">>> Entering BackupDatabaseAsync(fileName={FileName})", fileName);
		_logger.LogWarning("Throwing DataException from BackupDatabaseAsync(fileName={FileName})", fileName);
		_logger.LogDebug("<<< Exiting BackupDatabaseAsync(fileName={FileName})", fileName);
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    public virtual Task RestoreDatabaseAsync(string backupFileName)
    {
		_logger.LogDebug(">>> Entering RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
		_logger.LogWarning("Throwing DataException from RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
		_logger.LogDebug("<<< Exiting RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    public Task ReIndexTablesAsync()
    {
		_logger.LogDebug(">>> Entering ReIndexTablesAsync()");
        using (new ReaderWriteLockDisposable(_locker))
            DataContext.Execute("VACUUM;");

		_logger.LogDebug("<<< Exiting ReIndexTablesAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task ShrinkDatabaseAsync()
    {
		_logger.LogDebug(">>> Entering ShrinkDatabaseAsync()");
        using (new ReaderWriteLockDisposable(_locker))
            DataContext.Execute("VACUUM;");

		_logger.LogDebug("<<< Exiting ShrinkDatabaseAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
		_logger.LogDebug(">>> Entering BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
        ArgumentNullException.ThrowIfNull(nopConnectionString);

        if (nopConnectionString.IntegratedSecurity)
			_logger.LogWarning("Throwing NopException from BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
            throw new NopException("Data provider supports connection only with password");

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = CommonHelper.DefaultFileProvider.MapPath($"~/App_Data/{nopConnectionString.DatabaseName}.sqlite"),
            Password = nopConnectionString.Password,
            Mode = SqliteOpenMode.ReadWrite,
            Cache = SqliteCacheMode.Shared
        };

		_logger.LogDebug("<<< Exiting BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
        return builder.ConnectionString;
    }

    /// <summary>
    /// Set table identity (is supported)
    /// </summary>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <param name="ident">Identity value</param>
    public Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering SetTableIdentAsync(ident={Ident})", ident);
        using (new ReaderWriteLockDisposable(_locker))
        {
            var tableName = DataContext.GetTable<TEntity>().TableName;

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("SetTableIdentAsync(ident)={Ident}: update query executed in {Arg1} ms", ident, __logStart.ElapsedMilliseconds);
            DataContext.Execute($"update sqlite_sequence set seq = {ident} where name = \"{tableName}\"");
        }

		_logger.LogDebug("<<< Exiting SetTableIdentAsync(ident={Ident})", ident);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes command using System.Data.CommandType.StoredProcedure command type and
    /// returns results as collection of values of specified type
    /// </summary>
    /// <typeparam name="T">Result record type</typeparam>
    /// <param name="procedureName">Procedure name</param>
    /// <param name="parameters">Command parameters</param>
    /// <returns>Returns collection of query result records</returns>
    public override Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
    {
		_logger.LogDebug(">>> Entering QueryProcAsync(procedureName={ProcedureName},parameters={Parameters})", procedureName, parameters);
        //stored procedure is not support by SqLite
		_logger.LogDebug("<<< Exiting QueryProcAsync(procedureName={ProcedureName},parameters={Parameters})", procedureName, parameters);
        return Task.FromResult<IList<T>>(new List<T>());
    }

    /// <summary>
    /// Executes SQL command and returns results as collection of values of specified type
    /// </summary>
    /// <typeparam name="T">Type of result items</typeparam>
    /// <param name="sql">SQL command text</param>
    /// <param name="parameters">Parameters to execute the SQL command</param>
    /// <returns>Collection of values of specified type</returns>
    public override Task<IList<T>> QueryAsync<T>(string sql, params DataParameter[] parameters)
    {
		_logger.LogDebug(">>> Entering QueryAsync(sql={Sql},parameters={Parameters})", sql, parameters);
        using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
			_logger.LogDebug("<<< Exiting QueryAsync(sql={Sql},parameters={Parameters})", sql, parameters);
            return Task.FromResult<IList<T>>(DataContext.Query<T>(sql, parameters).ToList());
    }

    /// <summary>
    /// Executes command asynchronously and returns number of affected records
    /// </summary>
    /// <param name="sql">Command text</param>
    /// <param name="dataParameters">Command parameters</param>
    /// <returns>Number of records, affected by command execution.</returns>
    public override Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
    {
		_logger.LogDebug(">>> Entering ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters})", sql, dataParameters);
        using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
        {
            using var dataConnection = CreateDataConnection(LinqToDbDataProvider);
            var command = new CommandInfo(dataConnection, sql, dataParameters);

			_logger.LogDebug("<<< Exiting ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters})", sql, dataParameters);
            return command.ExecuteAsync();
        }
    }

    /// <summary>
    /// Creates a new temporary storage and populate it using data from provided query
    /// </summary>
    /// <param name="storeKey">Name of temporary storage</param>
    /// <param name="query">Query to get records to populate created storage with initial data</param>
    /// <typeparam name="TItem">Storage record mapping class</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the iQueryable instance of temporary storage
    /// </returns>
    public override Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storeKey, IQueryable<TItem> query)
    {
		_logger.LogDebug(">>> Entering CreateTempDataStorageAsync(storeKey={StoreKey},query={Query})", storeKey);
		_logger.LogDebug("<<< Exiting CreateTempDataStorageAsync(storeKey={StoreKey},query={Query})", storeKey);
        return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storeKey, query, DataContext));
    }

    public Task<bool> DatabaseExistsAsync()
    {
		_logger.LogDebug(">>> Entering DatabaseExistsAsync()");
		_logger.LogDebug("<<< Exiting DatabaseExistsAsync()");
        return Task.FromResult(DatabaseExists());
    }

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override Task TruncateAsync<TEntity>(bool resetIdentity = false)
    {
		_logger.LogDebug(">>> Entering TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
        using (new ReaderWriteLockDisposable(_locker))
			using Microsoft.Extensions.Logging;
            DataContext.GetTable<TEntity>().Truncate(resetIdentity);

		_logger.LogDebug("<<< Exiting TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the name of the database collation
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains an empty string
    /// </returns>
    public Task<string> GetDataBaseCollationAsync()
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
		_logger.LogDebug(">>> Entering GetDataBaseCollationAsync()");
		_logger.LogDebug("<<< Exiting GetDataBaseCollationAsync()");
        return Task.FromResult(string.Empty);
    }

    #endregion

    #region Properties

    protected DataConnection DataContext => _dataContext ??= CreateDataConnection();

    /// <summary>
    /// Linq2Db data provider
    /// </summary>
    protected override IDataProvider LinqToDbDataProvider { get; } = SQLiteTools.GetDataProvider(ProviderName.SQLiteMS);

    /// <summary>
    /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
    /// </summary>
    public int SupportedLengthOfBinaryHash { get; } = 0;

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public bool BackupSupported { get; } = false;

    #endregion
}