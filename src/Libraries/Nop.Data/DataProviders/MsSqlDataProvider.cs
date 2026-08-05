using System.Data.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Data.SqlClient;
using Nop.Core;
using Nop.Data.Mapping;

namespace Nop.Data.DataProviders;

/// <summary>
/// Represents the MS SQL Server data provider
/// </summary>
public partial class MsSqlNopDataProvider : BaseDataProvider, INopDataProvider
{
    #region Utilities

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected virtual SqlConnectionStringBuilder GetConnectionStringBuilder()
    {
		_logger.LogDebug(">>> Entering GetConnectionStringBuilder()");
		_logger.LogDebug("<<< Exiting GetConnectionStringBuilder()");
        return new SqlConnectionStringBuilder(DataSettings.ConnectionString);
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
        return new SqlConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create the database
    /// </summary>
    /// <param name="triesToConnect">Count of tries to connect to the database after creating; set 0 if no need to connect after creating</param>
    public virtual void CreateDatabase(int triesToConnect = 10)
    {
		_logger.LogDebug(">>> Entering CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();

		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder → {Builder}", triesToConnect, builder);
        //gets database name
        var databaseName = builder.InitialCatalog;

		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): databaseName → {DatabaseName}", triesToConnect, databaseName);
        //now create connection string to 'master' database. It always exists.
        builder.InitialCatalog = "master";
		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder.InitialCatalog → {Arg1}", triesToConnect, builder.InitialCatalog);

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE [{databaseName}]";
            if (!string.IsNullOrWhiteSpace(DataSettings.Collation))
				_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): query → {Query}", triesToConnect, query);
                query = $"{query} COLLATE {DataSettings.Collation}";

            var command = connection.CreateCommand();
            command.CommandText = query;
			_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): command → {Command}", triesToConnect, command);
			_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): command.CommandText → {Arg1}", triesToConnect, command.CommandText);
            command.Connection.Open();

            command.ExecuteNonQuery();
			_logger.LogInformation("CreateDatabase(triesToConnect)={TriesToConnect}: database query executed in {Arg1} ms", triesToConnect, __logStart.ElapsedMilliseconds);
        }

        //try connect
        if (triesToConnect <= 0)
            return;

        //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
        //but we have already started creation of tables and sample data.
        //as a result there is an exception thrown and the installation process cannot continue.
        //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
        for (var i = 0; i <= triesToConnect; i++)
        {
            if (i == triesToConnect)
                throw new Exception("Unable to connect to the new database. Please try one more time");

            if (!DatabaseExists())
                Thread.Sleep(1000);
            else
                break;
        }
		_logger.LogDebug("<<< Exiting CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the returns true if the database exists.
    /// </returns>
    public virtual async Task<bool> DatabaseExistsAsync()
    {
		_logger.LogDebug(">>> Entering DatabaseExistsAsync()");
        try
        {
            await using var connection = GetInternalDbConnection(DataSettings.ConnectionString);

			_logger.LogDebug("DatabaseExistsAsync(): connection → {Connection}", connection);
            //just try to connect
            await connection.OpenAsync();

            return true;
        }
        catch
        {
			_logger.LogError("Exception in DatabaseExistsAsync(): {Arg0}", ex.Message);
			_logger.LogDebug("<<< Exiting DatabaseExistsAsync()");
            return false;
        }
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>Returns true if the database exists.</returns>
    public virtual bool DatabaseExists()
    {
		_logger.LogDebug(">>> Entering DatabaseExists()");
        try
        {
            using var connection = GetInternalDbConnection(DataSettings.ConnectionString);
            //just try to connect
			_logger.LogDebug("DatabaseExists(): connection → {Connection}", connection);
            connection.Open();

            return true;
        }
        catch
        {
			_logger.LogError("Exception in DatabaseExists(): {Arg0}", ex.Message);
			_logger.LogDebug("<<< Exiting DatabaseExists()");
            return false;
        }
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
        var table = (ITable<TEntity>)base.GetTable<TEntity>();

		_logger.LogDebug("GetTable(): table → {Table}", table);
        return DataSettingsManager.UseNoLock() ? table.With("NOLOCK") : table;
    }

    /// <summary>
    /// Get the current identity value
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the integer identity; null if cannot get the result
    /// </returns>
    public virtual Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering GetTableIdentAsync()");
        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
		_logger.LogDebug("GetTableIdentAsync(): currentConnection → {CurrentConnection}", currentConnection);
		_logger.LogDebug("GetTableIdentAsync(): tableName → {TableName}", tableName);

        var result = currentConnection.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
            .FirstOrDefault();

        return Task.FromResult<int?>(result.HasValue ? Convert.ToInt32(result) : 1);
    }

    /// <summary>
    /// Set table identity (is supported)
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="ident">Identity value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
    {
		_logger.LogDebug(">>> Entering SetTableIdentAsync(ident={Ident})", ident);
        using var currentConnection = CreateDataConnection();
        var currentIdent = await GetTableIdentAsync<TEntity>();
		_logger.LogDebug("SetTableIdentAsync(ident={Ident}): currentConnection → {CurrentConnection}", ident, currentConnection);
		_logger.LogDebug("SetTableIdentAsync(ident={Ident}): currentIdent → {CurrentIdent}", ident, currentIdent);
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
			_logger.LogDebug("<<< Exiting SetTableIdentAsync(ident={Ident})", ident);
            return;

        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;

		_logger.LogDebug("SetTableIdentAsync(ident={Ident}): tableName → {TableName}", ident, tableName);
        await currentConnection.ExecuteAsync($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BackupDatabaseAsync(string fileName)
    {
		_logger.LogDebug(">>> Entering BackupDatabaseAsync(fileName={FileName})", fileName);
        using var currentConnection = CreateDataConnection();
        
		_logger.LogDebug("BackupDatabaseAsync(fileName={FileName}): currentConnection → {CurrentConnection}", fileName, currentConnection);
        var commandText = $"BACKUP DATABASE [{GetConnectionStringBuilder().InitialCatalog}] TO DISK = '{fileName}' WITH FORMAT";
        await currentConnection.ExecuteAsync(commandText);
		_logger.LogDebug("<<< Exiting BackupDatabaseAsync(fileName={FileName})", fileName);
		_logger.LogDebug("BackupDatabaseAsync(fileName={FileName}): commandText → {CommandText}", fileName, commandText);
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RestoreDatabaseAsync(string backupFileName)
    {
		_logger.LogDebug(">>> Entering RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
        using var currentConnection = CreateDataConnection();
        var commandText = string.Format(
            "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
            "ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE\n" +
            "BEGIN TRY\n" +
            "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
            "END TRY\n" +
            "BEGIN CATCH\n" +
            "SET @ErrorMessage = ERROR_MESSAGE()\n" +
            "END CATCH\n" +
            "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
            "IF (@ErrorMessage is not NULL)\n" +
            "BEGIN\n" +
            "RAISERROR (@ErrorMessage, 16, 1)\n" +
            "END",
            GetConnectionStringBuilder().InitialCatalog,
            backupFileName);
			_logger.LogDebug("RestoreDatabaseAsync(backupFileName={BackupFileName}): currentConnection → {CurrentConnection}", backupFileName, currentConnection);

        await currentConnection.ExecuteAsync(commandText);
		_logger.LogDebug("<<< Exiting RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
		_logger.LogDebug(">>> Entering ReIndexTablesAsync()");
        using var currentConnection = CreateDataConnection();
        var commandText = $@"
					_logger.LogDebug("ReIndexTablesAsync(): currentConnection → {CurrentConnection}", currentConnection);
                    DECLARE @TableName sysname 
                    DECLARE cur_reindex CURSOR FOR
                    SELECT table_name
                    FROM [{GetConnectionStringBuilder().InitialCatalog}].INFORMATION_SCHEMA.TABLES
                    WHERE table_type = 'base table'
                    OPEN cur_reindex
                    FETCH NEXT FROM cur_reindex INTO @TableName
                    WHILE @@FETCH_STATUS = 0
                        BEGIN
                            exec('ALTER INDEX ALL ON [' + @TableName + '] REBUILD')
                            FETCH NEXT FROM cur_reindex INTO @TableName
                        END
                    CLOSE cur_reindex
                    DEALLOCATE cur_reindex";

        await currentConnection.ExecuteAsync(commandText);
		_logger.LogDebug("<<< Exiting ReIndexTablesAsync()");
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ShrinkDatabaseAsync()
    {
		_logger.LogDebug(">>> Entering ShrinkDatabaseAsync()");
        using var currentConnection = CreateDataConnection();
        await currentConnection.ExecuteAsync($"DBCC SHRINKDATABASE ([{GetConnectionStringBuilder().InitialCatalog}]);");
		_logger.LogDebug("<<< Exiting ShrinkDatabaseAsync()");
		_logger.LogDebug("ShrinkDatabaseAsync(): currentConnection → {CurrentConnection}", currentConnection);
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
		_logger.LogDebug(">>> Entering BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
        ArgumentNullException.ThrowIfNull(nopConnectionString);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = nopConnectionString.ServerName,
            InitialCatalog = nopConnectionString.DatabaseName,
            PersistSecurityInfo = false,
            IntegratedSecurity = nopConnectionString.IntegratedSecurity,
            TrustServerCertificate = true
        };

        if (!nopConnectionString.IntegratedSecurity)
        {
            builder.UserID = nopConnectionString.Username;
			_logger.LogDebug("BuildConnectionString(nopConnectionString={NopConnectionString}): builder.UserID → {Arg1}", nopConnectionString, builder.UserID);
			_logger.LogDebug("BuildConnectionString(nopConnectionString={NopConnectionString}): builder.Password → {Arg1}", nopConnectionString, builder.Password);
            builder.Password = nopConnectionString.Password;
        }

		_logger.LogDebug("<<< Exiting BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
        return builder.ConnectionString;
    }

    /// <summary>
    /// Gets the name of a foreign key
    /// </summary>
    /// <param name="foreignTable">Foreign key table</param>
    /// <param name="foreignColumn">Foreign key column name</param>
    /// <param name="primaryTable">Primary table</param>
    /// <param name="primaryColumn">Primary key column name</param>
    /// <returns>Name of a foreign key</returns>
    public virtual string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn)
    {
		_logger.LogDebug(">>> Entering CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
		_logger.LogDebug("<<< Exiting CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
        return $"FK_{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}";
    }

    /// <summary>
    /// Gets the name of an index
    /// </summary>
    /// <param name="targetTable">Target table name</param>
    /// <param name="targetColumn">Target column name</param>
    /// <returns>Name of an index</returns>
    public virtual string GetIndexName(string targetTable, string targetColumn)
    {
		_logger.LogDebug(">>> Entering GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
		_logger.LogDebug("<<< Exiting GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
        return $"IX_{targetTable}_{targetColumn}";
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
        using var dataContext = CreateDataConnection();
        await dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .MergeAsync();
			_logger.LogDebug("UpdateEntitiesAsync(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
    }
	_logger.LogInformation("UpdateEntitiesAsync(entities)={Entities}: update query executed in {Arg1} ms", entities, __logStart.ElapsedMilliseconds);

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void UpdateEntities<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogDebug(">>> Entering UpdateEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection();
		using Microsoft.Extensions.Logging;
        dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .Merge();
			_logger.LogDebug("UpdateEntities(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
    }
	_logger.LogInformation("UpdateEntities(entities)={Entities}: update query executed in {Arg1} ms", entities, __logStart.ElapsedMilliseconds);
    
    /// <summary>
    /// Gets the name of the database collation
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collation name
    /// </returns>
    public virtual Task<string> GetDataBaseCollationAsync()
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        var builder = GetConnectionStringBuilder();
		_logger.LogDebug(">>> Entering GetDataBaseCollationAsync()");
		_logger.LogDebug("<<< Exiting GetDataBaseCollationAsync()");
        return GetSqlStringValueAsync($"SELECT CONVERT (varchar(256), DATABASEPROPERTYEX('{builder.InitialCatalog}','collation'));");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sql server data provider
    /// </summary>
    protected override IDataProvider LinqToDbDataProvider => SqlServerTools.GetDataProvider(SqlServerVersion.v2012, SqlServerProvider.MicrosoftDataSqlClient);

    /// <summary>
    /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
    /// </summary>
    public int SupportedLengthOfBinaryHash { get; } = 8000;

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public virtual bool BackupSupported => true;

    #endregion
}
