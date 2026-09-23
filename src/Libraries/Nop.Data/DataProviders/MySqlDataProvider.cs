using System.Data;
using System.Data.Common;
using System.Text;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.SqlQuery;
using MySqlConnector;
using Nop.Core;
using Nop.Data.Mapping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders;

public partial class MySqlNopDataProvider : BaseDataProvider, INopDataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    //it's quite fast hash (to cheaply distinguish between objects)
    protected const string HASH_ALGORITHM = "SHA1";

    #endregion

    #region Utilities

    /// <summary>
    /// Creates the database connection
    /// </summary>
    protected override DataConnection CreateDataConnection()
    {
		_logger.LogError(">>> Entering CreateDataConnection()");
        var dataContext = CreateDataConnection(LinqToDbDataProvider);

		_logger.LogError("CreateDataConnection(): dataContext → {DataContext}", dataContext);
        dataContext.MappingSchema.SetDataType(typeof(Guid), new SqlDataType(DataType.NChar, typeof(Guid), 36));
        dataContext.MappingSchema.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));

		_logger.LogWarning("<<< Exiting CreateDataConnection()");
        return dataContext;
    }

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected virtual MySqlConnectionStringBuilder GetConnectionStringBuilder()
    {
		_logger.LogError(">>> Entering GetConnectionStringBuilder()");
        _logger.LogWarning("<<< Exiting GetConnectionStringBuilder()");
        return new MySqlConnectionStringBuilder(DataSettings.ConnectionString);
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
        return new MySqlConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the database by using the loaded connection string
    /// </summary>
    /// <param name="triesToConnect"></param>
    public virtual void CreateDatabase(int triesToConnect = 10)
    {
		_logger.LogError(">>> Entering CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();
		_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): builder → {Builder}", triesToConnect, builder);

        //gets database name
        var databaseName = builder.Database;
		_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): databaseName → {DatabaseName}", triesToConnect, databaseName);

        //now create connection string to 'master' database. It always exists.
        builder.Database = null;
		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder.Database → {Arg1}", triesToConnect, builder.Database);

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
			_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): query → {Query}", triesToConnect, query);

            if (!string.IsNullOrWhiteSpace(DataSettings.CharacterSet))
                query = $"{query} CHARACTER SET {DataSettings.CharacterSet}";

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
            using var connection = CreateDbConnection();
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
    /// Get the current identity value
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the integer identity; null if cannot get the result
    /// </returns>
    public virtual async Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering GetTableIdentAsync()");
        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
		_logger.LogError("GetTableIdentAsync(): tableName → {TableName}", tableName);
        var databaseName = GetConnectionStringBuilder().Database;
		_logger.LogError("GetTableIdentAsync(): databaseName → {DatabaseName}", databaseName);
		_logger.LogError("GetTableIdentAsync(): currentConnection → {CurrentConnection}", currentConnection);

        //we're using the DbConnection object until linq2db solve this issue https://github.com/linq2db/linq2db/issues/1987
        //with DataContext we could be used KeepConnectionAlive option
        await using var dbConnection = GetInternalDbConnection(DataSettings.ConnectionString);
		_logger.LogError("GetTableIdentAsync(): dbConnection → {DbConnection}", dbConnection);

        dbConnection.StateChange += (sender, e) =>
        {
            try
            {
                if (e.CurrentState != ConnectionState.Open)
                    return;

                var connection = (IDbConnection)sender;
				_logger.LogError("GetTableIdentAsync(): connection → {Connection}", connection);
				_logger.LogDebug("GetTableIdentAsync(): internalCommand.Connection → {Arg0}", internalCommand.Connection);
				_logger.LogDebug("GetTableIdentAsync(): internalCommand.CommandText → {Arg0}", internalCommand.CommandText);
                using var internalCommand = connection.CreateCommand();
				_logger.LogError("GetTableIdentAsync(): internalCommand → {InternalCommand}", internalCommand);
                internalCommand.Connection = connection;
                internalCommand.CommandText = "SET @@SESSION.information_schema_stats_expiry = 0;";
                internalCommand.ExecuteNonQuery();
            }
            //ignoring for older than 8.0 versions MySQL (#1193 Unknown system variable)
            catch (MySqlException ex) when (ex.Number == 1193)
            {
				_logger.LogError("Exception in GetTableIdentAsync(): {Arg0}", ex.Message);
                //ignore
            }
        };

		_logger.LogWarning("<<< Exiting GetTableIdentAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await using var command = dbConnection.CreateCommand();
		_logger.LogError("GetTableIdentAsync(): command → {Command}", command);
		_logger.LogDebug("GetTableIdentAsync(): command.Connection → {Arg0}", command.Connection);
		_logger.LogDebug("GetTableIdentAsync(): command.CommandText → {Arg0}", command.CommandText);
        command.Connection = dbConnection;
        command.CommandText = $"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'";
		_logger.LogError("GetTableIdentAsync(): TABLE_SCHEMA → {TABLE_SCHEMA}", TABLE_SCHEMA);
        await dbConnection.OpenAsync();

		_logger.LogError("GetTableIdentAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        return Convert.ToInt32((await command.ExecuteScalarAsync()) ?? 1);
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
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var currentIdent = await GetTableIdentAsync<TEntity>();
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
            return;

        using var currentConnection = CreateDataConnection();
		_logger.LogError("SetTableIdentAsync(ident={Ident}): currentConnection → {CurrentConnection}", ident, currentConnection);
		_logger.LogError("SetTableIdentAsync(ident)={Ident}: database query executed in {Arg1} ms", ident, __logStart.ElapsedMilliseconds);
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
		_logger.LogError("SetTableIdentAsync(ident={Ident}): tableName → {TableName}", ident, tableName);

        await currentConnection.ExecuteAsync($"ALTER TABLE `{tableName}` AUTO_INCREMENT = {ident};");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task BackupDatabaseAsync(string fileName)
    {
		_logger.LogError(">>> Entering BackupDatabaseAsync(fileName={FileName})", fileName);
		_logger.LogError("Throwing DataException from BackupDatabaseAsync(fileName={FileName})", fileName);
    _logger.LogWarning("<<< Exiting BackupDatabaseAsync(fileName={FileName})", fileName);
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RestoreDatabaseAsync(string backupFileName)
    {
		_logger.LogError(">>> Entering RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
		_logger.LogError("Throwing DataException from RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
    _logger.LogWarning("<<< Exiting RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
		_logger.LogError(">>> Entering ReIndexTablesAsync()");
        using var currentConnection = CreateDataConnection();
        var tables = currentConnection.Query<string>($"SHOW TABLES FROM `{GetConnectionStringBuilder().Database}`").ToList();
		_logger.LogError("ReIndexTablesAsync(): tables → {Tables}", tables);
		_logger.LogError("ReIndexTablesAsync(): currentConnection → {CurrentConnection}", currentConnection);

        if (tables.Count > 0)
            await currentConnection.ExecuteAsync($"OPTIMIZE TABLE `{string.Join("`, `", tables)}`");
			_logger.LogWarning("<<< Exiting ReIndexTablesAsync()");
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task ShrinkDatabaseAsync()
    {
		_logger.LogError(">>> Entering ShrinkDatabaseAsync()");
		_logger.LogError("Throwing DataException from ShrinkDatabaseAsync()");
    _logger.LogWarning("<<< Exiting ShrinkDatabaseAsync()");
        throw new DataException("This database provider does not support database shrinking.  Instead, use Re-index operation to optimize database space. Optimization is only available when the 'innodb_file_per_table' setting is enabled");
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

        if (nopConnectionString.IntegratedSecurity)
			_logger.LogError("Throwing NopException from BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
            throw new NopException("Data provider supports connection only with login and password");

        var builder = new MySqlConnectionStringBuilder
        {
            Server = nopConnectionString.ServerName,
            //Cast DatabaseName to lowercase to avoid case-sensitivity problems
            Database = nopConnectionString.DatabaseName.ToLowerInvariant(),
            AllowUserVariables = true,
            UserID = nopConnectionString.Username,
            Password = nopConnectionString.Password,
            UseXaTransactions = false
        };

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
        //mySql support only 64 chars for constraint name
        //that is why we use hash function for create unique name
        //see details on this topic: https://dev.mysql.com/doc/refman/8.0/en/identifier-length.html
		_logger.LogWarning("<<< Exiting CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
        return "FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), HASH_ALGORITHM);
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
        return "IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), HASH_ALGORITHM);
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
        var connectionBuilder = GetConnectionStringBuilder();

		_logger.LogError("GetDataBaseCollationAsync(): connectionBuilder → {ConnectionBuilder}", connectionBuilder);
        return GetSqlStringValueAsync($"SELECT DEFAULT_COLLATION_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = '{connectionBuilder.Database}';");
    }

    #endregion

    #region Properties

    /// <summary>
    /// MySql data provider
    /// </summary>
    protected override IDataProvider LinqToDbDataProvider => MySqlTools.GetDataProvider();

    /// <summary>
    /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
    /// </summary>
    public int SupportedLengthOfBinaryHash { get; } = 0;

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public virtual bool BackupSupported => false;

    #endregion
}