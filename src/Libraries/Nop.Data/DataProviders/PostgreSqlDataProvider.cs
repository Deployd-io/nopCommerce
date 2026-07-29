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

namespace Nop.Data.DataProviders;

public partial class PostgreSqlDataProvider : BaseDataProvider, INopDataProvider
{
    #region Fields

    protected static readonly Lazy<IDataProvider> _dataProvider = new(() => new LinqToDBPostgreSQLDataProvider(), true);

    #endregion

    #region Utilities

    /// <summary>
    /// Creates the database connection by the current data configuration
    /// </summary>
    protected override DataConnection CreateDataConnection()
    {
		logger.LogDebug(">>> Entering CreateDataConnection()");
        var dataContext = CreateDataConnection(LinqToDbDataProvider);
        dataContext.MappingSchema.SetDataType(
            typeof(string),
            new SqlDataType(new DbDataType(typeof(string), "citext")));
			logger.LogDebug("CreateDataConnection(): dataContext → {DataContext}", dataContext);

		logger.LogDebug("<<< Exiting CreateDataConnection()");
        return dataContext;
    }

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected virtual NpgsqlConnectionStringBuilder GetConnectionStringBuilder()
    {
		logger.LogDebug(">>> Entering GetConnectionStringBuilder()");
		logger.LogDebug("<<< Exiting GetConnectionStringBuilder()");
        return new NpgsqlConnectionStringBuilder(DataSettings.ConnectionString);
    }

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
		logger.LogDebug(">>> Entering GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

		logger.LogDebug("<<< Exiting GetInternalDbConnection(connectionString={ConnectionString})", connectionString);
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
		logger.LogDebug(">>> Entering GetSequenceName(dataConnection={DataConnection})", dataConnection);
        ArgumentNullException.ThrowIfNull(dataConnection);

        var descriptor = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)) 
                         ?? throw new NopException($"Mapped entity descriptor is not found: {typeof(TEntity).Name}");

        var tableName = descriptor.EntityName;
        var columnName = descriptor.Fields.FirstOrDefault(x => x.IsIdentity && x.IsPrimaryKey)?.Name;

		logger.LogDebug("GetSequenceName(dataConnection={DataConnection}): columnName → {ColumnName}", dataConnection, columnName);
        if (string.IsNullOrEmpty(columnName))
            throw new NopException("A table's primary key does not have an identity constraint");

        return dataConnection.Query<string>($"SELECT pg_get_serial_sequence('\"{tableName}\"', '{columnName}');")
            .FirstOrDefault();
		logger.LogDebug("<<< Exiting GetSequenceName(dataConnection={DataConnection})", dataConnection);
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
		logger.LogDebug(">>> Entering BulkInsertEntitiesAsync(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
        await dataContext.BulkCopyAsync(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: true));
		logger.LogDebug("<<< Exiting BulkInsertEntitiesAsync(entities={Entities})", entities);
		logger.LogDebug("BulkInsertEntitiesAsync(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities)
    {
		logger.LogDebug(">>> Entering BulkInsertEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
        dataContext.BulkCopy(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: true));
		logger.LogDebug("<<< Exiting BulkInsertEntities(entities={Entities})", entities);
		logger.LogDebug("BulkInsertEntities(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
    }

    /// <summary>
    /// Creates the database by using the loaded connection string
    /// </summary>
    /// <param name="triesToConnect"></param>
    public virtual void CreateDatabase(int triesToConnect = 10)
    {
		logger.LogDebug(">>> Entering CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();

		logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder → {Builder}", triesToConnect, builder);
        //gets database name
        var databaseName = builder.Database;

		logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): databaseName → {DatabaseName}", triesToConnect, databaseName);
        //now create connection string to 'postgres' - default administrative connection database.
        builder.Database = "postgres";
		logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): builder.Database → {Arg1}", triesToConnect, builder.Database);

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
			logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): connection → {Connection}", triesToConnect, connection);
            var query = $"CREATE DATABASE \"{databaseName}\" WITH OWNER = '{builder.Username}'";

			logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): query → {Query}", triesToConnect, query);
            if (!string.IsNullOrWhiteSpace(DataSettings.CharacterSet))
                query = $"{query} ENCODING '{DataSettings.CharacterSet}'";

            if (!string.IsNullOrWhiteSpace(DataSettings.Collation))
                query = $"{query} LC_COLLATE = '{DataSettings.Collation}' TEMPLATE template0";

            var command = connection.CreateCommand();
            command.CommandText = query;
			logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): command → {Command}", triesToConnect, command);
			logger.LogDebug("CreateDatabase(triesToConnect={TriesToConnect}): command.CommandText → {Arg1}", triesToConnect, command.CommandText);
            command.Connection.Open();

            command.ExecuteNonQuery();
			logger.LogInformation("CreateDatabase(triesToConnect)={TriesToConnect}: database query executed in {Arg1} ms", triesToConnect, __logStart.ElapsedMilliseconds);
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
            {
                Thread.Sleep(1000);
            }
            else
            {
                builder.Database = databaseName;
                using var connection = GetInternalDbConnection(builder.ConnectionString) as NpgsqlConnection;
                var command = connection.CreateCommand();
                command.CommandText = "CREATE EXTENSION IF NOT EXISTS citext; CREATE EXTENSION IF NOT EXISTS pgcrypto;";
                command.Connection.Open();
                command.ExecuteNonQuery();
                connection.ReloadTypes();

                break;
            }
        }
		logger.LogDebug("<<< Exiting CreateDatabase(triesToConnect={TriesToConnect})", triesToConnect);
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>Returns true if the database exists.</returns>
    public virtual bool DatabaseExists()
    {
		logger.LogDebug(">>> Entering DatabaseExists()");
        try
        {
            using var connection = GetInternalDbConnection(DataSettings.ConnectionString);

			logger.LogDebug("DatabaseExists(): connection → {Connection}", connection);
            //just try to connect
            connection.Open();

            return true;
        }
        catch
        {
			logger.LogError("Exception in DatabaseExists(): {Arg0}", ex.Message);
			logger.LogDebug("<<< Exiting DatabaseExists()");
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
		logger.LogDebug(">>> Entering DatabaseExistsAsync()");
        try
        {
            await using var connection = GetInternalDbConnection(DataSettings.ConnectionString);

			logger.LogDebug("DatabaseExistsAsync(): connection → {Connection}", connection);
            //just try to connect
            await connection.OpenAsync();

            return true;
        }
        catch
        {
			logger.LogError("Exception in DatabaseExistsAsync(): {Arg0}", ex.Message);
			logger.LogDebug("<<< Exiting DatabaseExistsAsync()");
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
		logger.LogDebug(">>> Entering GetTableIdentAsync()");
        using var currentConnection = CreateDataConnection();

		logger.LogDebug("GetTableIdentAsync(): currentConnection → {CurrentConnection}", currentConnection);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var seqName = GetSequenceName<TEntity>(currentConnection);

		logger.LogDebug("GetTableIdentAsync(): seqName → {SeqName}", seqName);
        var result = currentConnection.Query<int>($"SELECT COALESCE(last_value + CASE WHEN is_called THEN 1 ELSE 0 END, 1) as Value FROM {seqName};")
            .FirstOrDefault();
			logger.LogDebug("GetTableIdentAsync(): result → {Result}", result);
			logger.LogInformation("GetTableIdentAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

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
		logger.LogDebug(">>> Entering SetTableIdentAsync(ident={Ident})", ident);
        var currentIdent = await GetTableIdentAsync<TEntity>();
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
			logger.LogDebug("SetTableIdentAsync(ident={Ident}): currentIdent → {CurrentIdent}", ident, currentIdent);
            return;

        using var currentConnection = CreateDataConnection();

		logger.LogDebug("SetTableIdentAsync(ident={Ident}): currentConnection → {CurrentConnection}", ident, currentConnection);
        var seqName = GetSequenceName<TEntity>(currentConnection);

		logger.LogDebug("SetTableIdentAsync(ident={Ident}): seqName → {SeqName}", ident, seqName);
        await currentConnection.ExecuteAsync($"select setval('{seqName}', {ident}, false);");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task BackupDatabaseAsync(string fileName)
    {
		logger.LogDebug(">>> Entering BackupDatabaseAsync(fileName={FileName})", fileName);
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
		logger.LogDebug(">>> Entering InsertEntity(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
        try
		logger.LogDebug("InsertEntity(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        {
            entity.Id = dataContext.InsertWithInt32Identity(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
			logger.LogError("Exception in InsertEntity(entity={Entity}): {Arg1}", entity, ex.Message);
            dataContext.Insert(entity);
        }

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
		logger.LogDebug(">>> Entering InsertEntityAsync(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
        try
		logger.LogDebug("InsertEntityAsync(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        {
            entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
			logger.LogError("Exception in InsertEntityAsync(entity={Entity}): {Arg1}", entity, ex.Message);
            await dataContext.InsertAsync(entity);
        }

        return entity;
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RestoreDatabaseAsync(string backupFileName)
    {
		logger.LogDebug(">>> Entering RestoreDatabaseAsync(backupFileName={BackupFileName})", backupFileName);
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
		logger.LogDebug(">>> Entering ReIndexTablesAsync()");
        using var currentConnection = CreateDataConnection();
        await currentConnection.ExecuteAsync($"REINDEX DATABASE \"{GetConnectionStringBuilder().Database}\";");
		logger.LogDebug("<<< Exiting ReIndexTablesAsync()");
		logger.LogDebug("ReIndexTablesAsync(): currentConnection → {CurrentConnection}", currentConnection);
    }

    /// <summary>
    /// Shrinks database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ShrinkDatabaseAsync()
    {
		logger.LogDebug(">>> Entering ShrinkDatabaseAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        using var currentConnection = CreateDataConnection();
        var tables = currentConnection.Query<string>($"SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema = 'public'").ToList();

		logger.LogDebug("ShrinkDatabaseAsync(): currentConnection → {CurrentConnection}", currentConnection);
		logger.LogDebug("ShrinkDatabaseAsync(): tables → {Tables}", tables);
		logger.LogInformation("ShrinkDatabaseAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        foreach (var table in tables)
            await currentConnection.ExecuteAsync($"VACUUM FULL \"{table}\";");
			logger.LogDebug("<<< Exiting ShrinkDatabaseAsync()");
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
        ArgumentNullException.ThrowIfNull(nopConnectionString);
		logger.LogDebug(">>> Entering BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);

        if (nopConnectionString.IntegratedSecurity)
            throw new NopException("Data provider supports connection only with login and password");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = nopConnectionString.ServerName,
            //Cast DatabaseName to lowercase to avoid case-sensitivity problems
            Database = nopConnectionString.DatabaseName.ToLowerInvariant(),
            Username = nopConnectionString.Username,
            Password = nopConnectionString.Password,
        };

		logger.LogDebug("<<< Exiting BuildConnectionString(nopConnectionString={NopConnectionString})", nopConnectionString);
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
		logger.LogDebug(">>> Entering CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
		logger.LogDebug("<<< Exiting CreateForeignKeyName(foreignTable={ForeignTable},foreignColumn={ForeignColumn},primaryTable={PrimaryTable},primaryColumn={PrimaryColumn})", foreignTable, foreignColumn, primaryTable, primaryColumn);
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
		logger.LogDebug(">>> Entering GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
		logger.LogDebug("<<< Exiting GetIndexName(targetTable={TargetTable},targetColumn={TargetColumn})", targetTable, targetColumn);
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
		logger.LogDebug(">>> Entering GetDataBaseCollationAsync()");
        var builder = GetConnectionStringBuilder();
		logger.LogDebug("GetDataBaseCollationAsync(): builder → {Builder}", builder);
		logger.LogDebug("<<< Exiting GetDataBaseCollationAsync()");
        return GetSqlStringValueAsync($"SELECT datcollate AS collation FROM pg_database WHERE datname = '{builder.Database}';");
    }

    #endregion

    #region Properties

    protected override IDataProvider LinqToDbDataProvider => _dataProvider.Value;

    public int SupportedLengthOfBinaryHash => 0;

    public bool BackupSupported => false;

    #endregion
}