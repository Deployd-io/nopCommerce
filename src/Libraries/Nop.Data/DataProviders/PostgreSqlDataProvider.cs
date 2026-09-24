using System.Data;
using System.Data.Common;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.SqlQuery;
using LinqToDB.Tools;
using Nop.Core;
using Nop.Data.DataProviders.LinqToDB;
using Nop.Data.Mapping;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders;

public partial class PostgreSqlDataProvider : BaseDataProvider, INopDataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static readonly Lazy<IDataProvider> _dataProvider = new(() => new LinqToDBPostgreSQLDataProvider(), true);

    #endregion

    #region Utilities

    /// <summary>
    /// Creates the database connection by the current data configuration
    /// </summary>
    protected override DataConnection CreateDataConnection()
    {
		_logger.LogError(">>> Entering CreateDataConnection()");
        var dataContext = CreateDataConnection(LinqToDbDataProvider);
        dataContext.MappingSchema.SetDataType(
            typeof(string),
            new SqlDataType(new DbDataType(typeof(string), "citext")));
			_logger.LogError("CreateDataConnection(): dataContext → {DataContext}", dataContext);

		_logger.LogWarning("<<< Exiting CreateDataConnection()");
        return dataContext;
    }

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected virtual NpgsqlConnectionStringBuilder GetConnectionStringBuilder()
    {
		_logger.LogError(">>> Entering GetConnectionStringBuilder()");
        _logger.LogWarning("<<< Exiting GetConnectionStringBuilder()");
        return new NpgsqlConnectionStringBuilder(DataSettings.ConnectionString);
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
        return new NpgsqlConnection(connectionString);
    }

    /// <summary>
    /// Get the name of the sequence associated with a identity column
    /// </summary>
    /// <param name="dataConnection">A database connection object</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Returns the name of the sequence, or NULL if no sequence is associated with the column</returns>
    protected virtual string GetSequenceName<TEntity>(DataConnection dataConnection) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering GetSequenceName(dataConnection={DataConnection})", dataConnection);
        ArgumentNullException.ThrowIfNull(dataConnection);

        var descriptor = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)) 
                         ?? throw new NopException($"Mapped entity descriptor is not found: {typeof(TEntity).Name}");

        var tableName = descriptor.EntityName;
        var columnName = descriptor.Fields.FirstOrDefault(x => x.IsIdentity && x.IsPrimaryKey)?.Name;
		_logger.LogError("GetSequenceName(dataConnection={DataConnection}): columnName → {ColumnName}", dataConnection, columnName);

        if (string.IsNullOrEmpty(columnName))
			_logger.LogError("Throwing NopException from GetSequenceName(dataConnection={DataConnection})", dataConnection);
            throw new NopException("A table's primary key does not have an identity constraint");

        return dataConnection.Query<string>($"SELECT pg_get_serial_sequence('\"{tableName}\"', '{columnName}');")
            .FirstOrDefault();
			_logger.LogWarning("<<< Exiting GetSequenceName(dataConnection={DataConnection})", dataConnection);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogError(">>> Entering BulkInsertEntitiesAsync(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
		_logger.LogWarning("<<< Exiting BulkInsertEntitiesAsync(entities={Entities})", entities);
		_logger.LogError("BulkInsertEntitiesAsync(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
        await dataContext.BulkCopyAsync(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: true));
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities)
    {
		_logger.LogError(">>> Entering BulkInsertEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
		_logger.LogWarning("<<< Exiting BulkInsertEntities(entities={Entities})", entities);
		_logger.LogError("BulkInsertEntities(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
        dataContext.BulkCopy(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: true));
    }

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

        //now create connection string to 'postgres' - default administrative connection database.
        builder.Database = "postgres";
		_logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder.Database → {Arg1}", triesToConnect, builder.Database);

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE \"{databaseName}\" WITH OWNER = '{builder.Username}'";
			_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): query → {Query}", triesToConnect, query);

            if (!string.IsNullOrWhiteSpace(DataSettings.CharacterSet))
                query = $"{query} ENCODING '{DataSettings.CharacterSet}'";

            if (!string.IsNullOrWhiteSpace(DataSettings.Collation))
                query = $"{query} LC_COLLATE = '{DataSettings.Collation}' TEMPLATE template0";

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
            {
                Thread.Sleep(1000);
            }
            else
            {
                builder.Database = databaseName;
                using var connection = GetInternalDbConnection(builder.ConnectionString) as NpgsqlConnection;
				_logger.LogError("CreateDatabase(triesToConnect={TriesToConnect}): connection → {Connection}", triesToConnect, connection);
                var command = connection.CreateCommand();
                command.CommandText = "CREATE EXTENSION IF NOT EXISTS citext; CREATE EXTENSION IF NOT EXISTS pgcrypto;";
                command.Connection.Open();
                command.ExecuteNonQuery();
                connection.ReloadTypes();

                break;
            }
        }
		_logger.LogWarning("<<< Exiting CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
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

		_logger.LogError("GetTableIdentAsync(): currentConnection → {CurrentConnection}", currentConnection);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var seqName = GetSequenceName<TEntity>(currentConnection);
		_logger.LogError("GetTableIdentAsync(): seqName → {SeqName}", seqName);

        var result = currentConnection.Query<int>($"SELECT COALESCE(last_value + CASE WHEN is_called THEN 1 ELSE 0 END, 1) as Value FROM {seqName};")
            .FirstOrDefault();
			_logger.LogError("GetTableIdentAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
			_logger.LogError("GetTableIdentAsync(): result → {Result}", result);

		_logger.LogWarning("<<< Exiting GetTableIdentAsync()");
        return Task.FromResult<int?>(result);
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
        var currentIdent = await GetTableIdentAsync<TEntity>();
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
            return;

        using var currentConnection = CreateDataConnection();

        var seqName = GetSequenceName<TEntity>(currentConnection);
		_logger.LogError("SetTableIdentAsync(ident={Ident}): seqName → {SeqName}", ident, seqName);

        await currentConnection.ExecuteAsync($"select setval('{seqName}', {ident}, false);");
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
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>Inserted entity</returns>
    public override TEntity InsertEntity<TEntity>(TEntity entity)
    {
		_logger.LogError(">>> Entering InsertEntity(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
		_logger.LogError("InsertEntity(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        try
        {
            entity.Id = dataContext.InsertWithInt32Identity(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
			_logger.LogError("Exception in InsertEntity(entity={Entity}): {Arg1}", entity, ex.Message);
            dataContext.Insert(entity);
        }

		_logger.LogWarning("<<< Exiting InsertEntity(entity={Entity})", entity);
        return entity;
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the inserted entity
    /// </returns>
    public override async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity)
    {
		_logger.LogError(">>> Entering InsertEntityAsync(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
		_logger.LogError("InsertEntityAsync(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        try
        {
            entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
			_logger.LogError("Exception in InsertEntityAsync(entity={Entity}): {Arg1}", entity, ex.Message);
            await dataContext.InsertAsync(entity);
        }

		_logger.LogWarning("<<< Exiting InsertEntityAsync(entity={Entity})", entity);
        return entity;
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
		_logger.LogWarning("<<< Exiting ReIndexTablesAsync()");
		_logger.LogError("ReIndexTablesAsync(): currentConnection → {CurrentConnection}", currentConnection);
        await currentConnection.ExecuteAsync($"REINDEX DATABASE \"{GetConnectionStringBuilder().Database}\";");
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ShrinkDatabaseAsync()
    {
		_logger.LogError(">>> Entering ShrinkDatabaseAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        using var currentConnection = CreateDataConnection();
        var tables = currentConnection.Query<string>($"SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema = 'public'").ToList();
		_logger.LogError("ShrinkDatabaseAsync(): tables → {Tables}", tables);
		_logger.LogError("ShrinkDatabaseAsync(): currentConnection → {CurrentConnection}", currentConnection);

        foreach (var table in tables)
            await currentConnection.ExecuteAsync($"VACUUM FULL \"{table}\";");
			_logger.LogError("ShrinkDatabaseAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting ShrinkDatabaseAsync()");
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

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = nopConnectionString.ServerName,
            //Cast DatabaseName to lowercase to avoid case-sensitivity problems
            Database = nopConnectionString.DatabaseName.ToLowerInvariant(),
            Username = nopConnectionString.Username,
            Password = nopConnectionString.Password,
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
        return GetSqlStringValueAsync($"SELECT datcollate AS collation FROM pg_database WHERE datname = '{builder.Database}';");
    }

    #endregion

    #region Properties

    protected override IDataProvider LinqToDbDataProvider => _dataProvider.Value;

    public int SupportedLengthOfBinaryHash => 0;

    public bool BackupSupported => false;

    #endregion
}