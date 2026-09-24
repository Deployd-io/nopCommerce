using System.Data.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Data.SqlClient;
using Nop.Core;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders;

/// <summary>
/// Represents the MS SQL Server data provider
/// </summary>
public partial class MsSqlNopDataProvider : BaseDataProvider, INopDataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected virtual SqlConnectionStringBuilder GetConnectionStringBuilder()
    {
		_logger.LogError(">>> Entering GetConnectionStringBuilder()");
        _logger.LogWarning("<<< Exiting GetConnectionStringBuilder()");
        return new SqlConnectionStringBuilder(DataSettings.ConnectionString);
    }

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
		_logger.LogError(">>> Entering GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

		_logger.LogWarning("<<< Exiting GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
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
		_logger.LogError(">>> Entering CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();
		_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): builder → {Builder}", triesToConnect, builder);

        //gets database name
        var databaseName = builder.InitialCatalog;
		_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): databaseName → {DatabaseName}", triesToConnect, databaseName);

        //now create connection string to 'master' database. It always exists.
        builder.InitialCatalog = "master";
		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder.InitialCatalog → {Arg1}", triesToConnect, builder.InitialCatalog);

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE [{databaseName}]";
			_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): query → {Query}", triesToConnect, query);
            if (!string.IsNullOrWhiteSpace(DataSettings.Collation))
                query = $"{query} COLLATE {DataSettings.Collation}";

            var command = connection.CreateCommand();
			_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): command → {Command}", triesToConnect, command);
			_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): command.CommandText → {Arg1}", triesToConnect, command.CommandText);
            command.CommandText = query;
            command.Connection.Open();

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("CreateDatabase(triesToConnect)={TriesToConnect}: database query executed in {Arg1} ms", triesToConnect, __logStart.ElapsedMilliseconds);
            command.ExecuteNonQuery();
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
				_logger.LogError("Throwing Exception from CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
                throw new Exception("Unable to connect to the new database. Please try one more time");

            if (!DatabaseExists())
                Thread.Sleep(1000);
            else
                break;
        }
		_logger.LogWarning("<<< Exiting CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
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
		_logger.LogError(">>> Entering DatabaseExistsAsync()");
        try
        {
            await using var connection = GetInternalDbConnection(DataSettings.ConnectionString);
			_logger.LogError("DatabaseExistsAsync(): connection → {Connection}", connection);

            //just try to connect
            await connection.OpenAsync();

            return true;
        }
        catch
        {
			_logger.LogError("Exception in DatabaseExistsAsync(): {Arg0}", ex.Message);
			_logger.LogWarning("<<< Exiting DatabaseExistsAsync()");
            return false;
        }
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>Returns true if the database exists.</returns>
    public virtual bool DatabaseExists()
    {
		_logger.LogError(">>> Entering DatabaseExists()");
        try
        {
            using var connection = GetInternalDbConnection(DataSettings.ConnectionString);
			_logger.LogError("DatabaseExists(): connection → {Connection}", connection);
            //just try to connect
            connection.Open();

            return true;
        }
        catch
        {
			_logger.LogError("Exception in DatabaseExists(): {Arg0}", ex.Message);
			_logger.LogWarning("<<< Exiting DatabaseExists()");
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
		_logger.LogError(">>> Entering GetTable()");
        var table = (ITable<TEntity>)base.GetTable<TEntity>();

		_logger.LogError("GetTable(): table → {Table}", table);
		_logger.LogWarning("<<< Exiting GetTable()");
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
		_logger.LogError(">>> Entering GetTableIdentAsync()");
        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
		_logger.LogError("GetTableIdentAsync(): tableName → {TableName}", tableName);
		_logger.LogError("GetTableIdentAsync(): currentConnection → {CurrentConnection}", currentConnection);

        var result = currentConnection.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
            .FirstOrDefault();

		_logger.LogWarning("<<< Exiting GetTableIdentAsync()");
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
		_logger.LogError(">>> Entering SetTableIdentAsync(ident={Ident})", ident);
        using var currentConnection = CreateDataConnection();
        var currentIdent = await GetTableIdentAsync<TEntity>();
		_logger.LogError("SetTableIdentAsync(ident={Ident}): currentIdent → {CurrentIdent}", ident, currentIdent);
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
            return;

        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
		_logger.LogError("SetTableIdentAsync(ident={Ident}): tableName → {TableName}", ident, tableName);

        await currentConnection.ExecuteAsync($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BackupDatabaseAsync(string fileName)
    {
		_logger.LogError(">>> Entering BackupDatabaseAsync(fileName={FileName})", fileName);
        using var currentConnection = CreateDataConnection();
        
		_logger.LogError("BackupDatabaseAsync(fileName={FileName}): currentConnection → {CurrentConnection}", fileName, currentConnection);
        var commandText = $"BACKUP DATABASE [{GetConnectionStringBuilder().InitialCatalog}] TO DISK = '{fileName}' WITH FORMAT";
		_logger.LogError("BackupDatabaseAsync(fileName={FileName}): commandText → {CommandText}", fileName, commandText);
        await currentConnection.ExecuteAsync(commandText);
		_logger.LogWarning("<<< Exiting BackupDatabaseAsync(fileName={FileName})", fileName);
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RestoreDatabaseAsync(string backupFileName)
    {
		_logger.LogError(">>> Entering RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
			_logger.LogError("RestoreDatabaseAsync(backupFileName)={BackupFileName}: database query executed in {Arg1} ms", backupFileName, __logStart.ElapsedMilliseconds);
			_logger.LogError("RestoreDatabaseAsync(backupFileName={BackupFileName}): currentConnection → {CurrentConnection}", backupFileName, currentConnection);

        await currentConnection.ExecuteAsync(commandText);
		_logger.LogWarning("<<< Exiting RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
		_logger.LogError(">>> Entering ReIndexTablesAsync()");
        using var currentConnection = CreateDataConnection();
        var commandText = $@"
					_logger.LogError("ReIndexTablesAsync(): currentConnection → {CurrentConnection}", currentConnection);
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
		_logger.LogWarning("<<< Exiting ReIndexTablesAsync()");
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ShrinkDatabaseAsync()
    {
		_logger.LogError(">>> Entering ShrinkDatabaseAsync()");
        using var currentConnection = CreateDataConnection();
		_logger.LogWarning("<<< Exiting ShrinkDatabaseAsync()");
		_logger.LogError("ShrinkDatabaseAsync(): currentConnection → {CurrentConnection}", currentConnection);
        await currentConnection.ExecuteAsync($"DBCC SHRINKDATABASE ([{GetConnectionStringBuilder().InitialCatalog}]);");
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
		_logger.LogError(">>> Entering BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
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

		_logger.LogWarning("<<< Exiting BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
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
		_logger.LogError(">>> Entering CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
        _logger.LogWarning("<<< Exiting CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
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
		_logger.LogError(">>> Entering GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
        _logger.LogWarning("<<< Exiting GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
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
		_logger.LogError(">>> Entering UpdateEntitiesAsync(entities={Entities})", entities);
        using var dataContext = CreateDataConnection();
        await dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .MergeAsync();
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void UpdateEntities<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogError(">>> Entering UpdateEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection();
        dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .Merge();
    }
    
    /// <summary>
    /// Gets the name of the database collation
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collation name
    /// </returns>
    public virtual Task<string> GetDataBaseCollationAsync()
    {
		_logger.LogError(">>> Entering GetDataBaseCollationAsync()");
        var builder = GetConnectionStringBuilder();
		_logger.LogError("GetDataBaseCollationAsync(): builder → {Builder}", builder);
		_logger.LogWarning("<<< Exiting GetDataBaseCollationAsync()");
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
