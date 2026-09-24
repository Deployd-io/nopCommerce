using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using FluentMigrator;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Tools;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Microsoft.Extensions.Logging;

namespace Nop.Data.DataProviders;

public abstract partial class BaseDataProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected abstract DbConnection GetInternalDbConnection(string connectionString);

    /// <summary>
    /// Creates the database connection
    /// </summary>
    protected virtual DataConnection CreateDataConnection()
    {
		_logger.LogError(">>> Entering CreateDataConnection()");
		_logger.LogWarning("<<< Exiting CreateDataConnection()");
		_logger.LogWarning("<<< Exiting CreateDataConnection(dataProvider={DataProvider})", dataProvider);
        return CreateDataConnection(LinqToDbDataProvider);
    }

    /// <summary>
    /// Creates the database connection
    /// </summary>
    /// <param name="dataProvider">Data provider</param>
    /// <returns>Database connection</returns>
    protected virtual DataConnection CreateDataConnection(IDataProvider dataProvider)
    {
        ArgumentNullException.ThrowIfNull(dataProvider);

        var dataConnection = new DataConnection(dataProvider, CreateDbConnection(), NopMappingSchema.GetMappingSchema(ConfigurationName, LinqToDbDataProvider))
        {
            CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
        };

        return dataConnection;
    }

    /// <summary>
    /// Creates a connection to a database
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected virtual DbConnection CreateDbConnection(string connectionString = null)
    {
		_logger.LogError(">>> Entering CreateDbConnection(connectionString={ConnectionString})", connectionString);
        _logger.LogWarning("<<< Exiting CreateDbConnection(connectionString={ConnectionString})", connectionString);
        return GetInternalDbConnection(!string.IsNullOrEmpty(connectionString) ? connectionString : DataSettings.ConnectionString);
    }

    /// <summary>
    /// Gets scalar value from the database
    /// </summary>
    /// <param name="sql">The text command to run</param>
    /// <param name="parameters">Database parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The first column of the first row in the first result set.
    /// </returns>
    protected virtual async Task<string> GetSqlStringValueAsync(string sql, params DataParameter[] parameters)
    {
		_logger.LogError(">>> Entering GetSqlStringValueAsync(sql={Sql},parameters={Parameters})", sql, parameters);
        ArgumentException.ThrowIfNullOrEmpty(sql);

        await using var dbConnection = CreateDbConnection();
		_logger.LogError("GetSqlStringValueAsync(sql={Sql},parameters={Parameters}): dbConnection → {DbConnection}", sql, parameters, dbConnection);
        await using var command = dbConnection.CreateCommand();
		_logger.LogError("GetSqlStringValueAsync(sql={Sql},parameters={Parameters}): command → {Command}", sql, parameters, command);
		_logger.LogDebug("GetSqlStringValueAsync(sql={Sql},parameters={Parameters}): command.Connection → {Arg2}", sql, parameters, command.Connection);
		_logger.LogDebug("GetSqlStringValueAsync(sql={Sql},parameters={Parameters}): command.CommandText → {Arg2}", sql, parameters, command.CommandText);
        command.Connection = dbConnection;
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        await dbConnection.OpenAsync();

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var value = await command.ExecuteScalarAsync();

		_logger.LogError("GetSqlStringValueAsync(sql={Sql},parameters={Parameters}): value → {Value}", sql, parameters, value);
		_logger.LogError("GetSqlStringValueAsync(parameters)={Parameters}: database query executed in {Arg1} ms", parameters, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetSqlStringValueAsync(sql={Sql},parameters={Parameters})", sql, parameters);
        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets a data hash from database side
    /// </summary>
    /// <param name="binaryData">Array for a hashing function</param>
    /// <returns>Data hash</returns>
    /// <remarks>
    /// For SQL Server 2014 (12.x) and earlier, allowed input values are limited to 8000 bytes.
    /// https://docs.microsoft.com/en-us/sql/t-sql/functions/hashbytes-transact-sql
    /// </remarks>
    [Sql.Expression("CONVERT(VARCHAR(128), HASHBYTES('SHA2_512', SUBSTRING({0}, 0, 8000)), 2)", ServerSideOnly = true, Configuration = ProviderName.SqlServer)]
    [Sql.Expression("SHA2({0}, 512)", ServerSideOnly = true, Configuration = ProviderName.MySql)]
    [Sql.Expression("encode(digest({0}, 'sha512'), 'hex')", ServerSideOnly = true, Configuration = ProviderName.PostgreSQL)]
    protected static string SqlSha2(object binaryData)
    {
		_logger.LogError(">>> Entering SqlSha2(binaryData={BinaryData})", binaryData);
		_logger.LogError("Throwing InvalidOperationException from SqlSha2(binaryData={BinaryData})", binaryData);
    _logger.LogWarning("<<< Exiting SqlSha2(binaryData={BinaryData})", binaryData);
        throw new InvalidOperationException("This function should be used only in database code");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize database
    /// </summary>
    public virtual void InitializeDatabase()
    {
		_logger.LogError(">>> Entering InitializeDatabase()");
        var migrationManager = EngineContext.Current.Resolve<IMigrationManager>();

		_logger.LogError("InitializeDatabase(): migrationManager → {MigrationManager}", migrationManager);
        var targetAssembly = typeof(NopDbStartup).Assembly;
		_logger.LogError("InitializeDatabase(): targetAssembly → {TargetAssembly}", targetAssembly);
        migrationManager.ApplyUpMigrations(targetAssembly);

        var typeFinder = Singleton<ITypeFinder>.Instance;
        var mAssemblies = typeFinder.FindClassesOfType<MigrationBase>()
            .Select(t => t.Assembly)
            .Where(assembly => !assembly.FullName?.Contains("FluentMigrator.Runner") ?? false)
            .Distinct()
            .ToArray();

        //mark update migrations as applied
        foreach (var assembly in mAssemblies)
            migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update, true);
			_logger.LogWarning("<<< Exiting InitializeDatabase()");
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
    public virtual Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storeKey, IQueryable<TItem> query)
        where TItem : class
    {
		_logger.LogError(">>> Entering CreateTempDataStorageAsync(storeKey={StoreKey},query={Query})", storeKey);
        _logger.LogWarning("<<< Exiting CreateTempDataStorageAsync(storeKey={StoreKey},query={Query})", storeKey);
        return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storeKey, query, CreateDataConnection()));
    }



    /// <summary>
    /// Get hash values of a stored entity field
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="keySelector">A key selector which should project to a dictionary key</param>
    /// <param name="fieldSelector">A field selector to apply a transform to a hash value</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Dictionary</returns>
    public virtual async Task<IDictionary<int, string>> GetFieldHashesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, int>> keySelector,
        Expression<Func<TEntity, object>> fieldSelector) where TEntity : BaseEntity
    {
        if (keySelector.Body is not MemberExpression keyMember ||
            keyMember.Member is not PropertyInfo keyPropInfo)
        {
            throw new ArgumentException($"Expression '{keySelector}' refers to method or field, not a property.");
        }

        if (fieldSelector.Body is not MemberExpression member ||
            member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException($"Expression '{fieldSelector}' refers to a method or field, not a property.");
        }

        var hashes = GetTable<TEntity>()
            .Where(predicate)
            .Select(x => new
            {
                Id = Sql.Property<int>(x, keyPropInfo.Name),
                Hash = SqlSha2(Sql.Property<object>(x, propInfo.Name))
            });

        return await AsyncIQueryableExtensions.ToDictionaryAsync(hashes, p => p.Id, p => p.Hash);
    }

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    public virtual IQueryable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering GetTable()");
        var options = new DataOptions()
            .UseConnectionString(LinqToDbDataProvider, DataSettings.ConnectionString)
            .UseMappingSchema(NopMappingSchema.GetMappingSchema(ConfigurationName, LinqToDbDataProvider));

		_logger.LogWarning("<<< Exiting GetTable()");
        return new DataContext(options)
        {
            CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
        }
        .GetTable<TEntity>();
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
    public virtual async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering InsertEntityAsync(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
        entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
		_logger.LogError("InsertEntityAsync(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
		_logger.LogWarning("<<< Exiting InsertEntityAsync(entity={Entity})", entity);
        return entity;
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>Inserted entity</returns>
    public virtual TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering InsertEntity(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
        entity.Id = dataContext.InsertWithInt32Identity(entity);
		_logger.LogError("InsertEntity(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
		_logger.LogWarning("<<< Exiting InsertEntity(entity={Entity})", entity);
        return entity;
    }

    /// <summary>
    /// Updates record in table, using values from entity parameter.
    /// Record to update identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering UpdateEntityAsync(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
		_logger.LogWarning("<<< Exiting UpdateEntityAsync(entity={Entity})", entity);
		_logger.LogError("UpdateEntityAsync(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        await dataContext.UpdateAsync(entity);
    }

    /// <summary>
    /// Updates record in table, using values from entity parameter.
    /// Record to update identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual void UpdateEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering UpdateEntity(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
		_logger.LogWarning("<<< Exiting UpdateEntity(entity={Entity})", entity);
		_logger.LogError("UpdateEntity(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        dataContext.Update(entity);
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering UpdateEntitiesAsync(entities={Entities})", entities);
        //we don't use the Merge API on this level, because this API not support all databases.
        //you may see all supported databases by the following link: https://linq2db.github.io/articles/sql/merge/Merge-API.html#supported-databases
        foreach (var entity in entities)
            await UpdateEntityAsync(entity);
			_logger.LogWarning("<<< Exiting UpdateEntitiesAsync(entities={Entities})", entities);
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual void UpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering UpdateEntities(entities={Entities})", entities);
        //we don't use the Merge API on this level, because this API not support all databases.
        //you may see all supported databases by the following link: https://linq2db.github.io/articles/sql/merge/Merge-API.html#supported-databases
        foreach (var entity in entities)
            UpdateEntity(entity);
			_logger.LogWarning("<<< Exiting UpdateEntities(entities={Entities})", entities);
    }

    /// <summary>
    /// Deletes record in table. Record to delete identified
    /// by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering DeleteEntityAsync(entity={Entity})", entity);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        using var dataContext = CreateDataConnection();
		_logger.LogError("DeleteEntityAsync(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        await dataContext.DeleteAsync(entity);
		_logger.LogError("DeleteEntityAsync(entity)={Entity}: external service call took {Arg1} ms", entity, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteEntityAsync(entity={Entity})", entity);
    }

    /// <summary>
    /// Deletes record in table. Record to delete identified
    /// by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual void DeleteEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering DeleteEntity(entity={Entity})", entity);
        using var dataContext = CreateDataConnection();
		_logger.LogWarning("<<< Exiting DeleteEntity(entity={Entity})", entity);
		_logger.LogError("DeleteEntity(entity={Entity}): dataContext → {DataContext}", entity, dataContext);
        dataContext.Delete(entity);
    }

    /// <summary>
    /// Performs delete records in a table
    /// </summary>
    /// <param name="entities">Entities for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering BulkDeleteEntitiesAsync(entities={Entities})", entities);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        using var dataContext = CreateDataConnection();
		_logger.LogError("BulkDeleteEntitiesAsync(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
        if (entities.All(entity => entity.Id == 0))
        {
            foreach (var entity in entities)
                await dataContext.DeleteAsync(entity);
        }
        else
        {
            await dataContext.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .DeleteAsync();
				_logger.LogWarning("<<< Exiting BulkDeleteEntitiesAsync(entities={Entities})", entities);
				_logger.LogError("BulkDeleteEntitiesAsync(entities)={Entities}: external service call took {Arg1} ms", entities, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting BulkDeleteEntitiesAsync(predicate={Predicate})", predicate);
        }
    }

    /// <summary>
    /// Performs delete records in a table
    /// </summary>
    /// <param name="entities">Entities for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual void BulkDeleteEntities<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering BulkDeleteEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection();
		_logger.LogWarning("<<< Exiting BulkDeleteEntities(entities={Entities})", entities);
        if (entities.All(entity => entity.Id == 0))
            foreach (var entity in entities)
                dataContext.Delete(entity);
        else
            dataContext.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .Delete();
				_logger.LogError("BulkDeleteEntities(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
    }

    /// <summary>
    /// Performs delete records in a table by a condition
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted records
    /// </returns>
    public virtual async Task<int> BulkDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
		_logger.LogError("BulkDeleteEntitiesAsync(predicate={Predicate}): dataContext → {DataContext}", predicate, dataContext);
        return await dataContext.GetTable<TEntity>()
            .Where(predicate)
            .DeleteAsync();
    }

    /// <summary>
    /// Performs delete records in a table by a condition
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// The number of deleted records
    /// </returns>
    public virtual int BulkDeleteEntities<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
        return dataContext.GetTable<TEntity>()
            .Where(predicate)
            .Delete();
			_logger.LogError("BulkDeleteEntities(predicate={Predicate}): dataContext → {DataContext}", predicate, dataContext);
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering BulkInsertEntitiesAsync(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
		_logger.LogWarning("<<< Exiting BulkInsertEntitiesAsync(entities={Entities})", entities);
		_logger.LogError("BulkInsertEntitiesAsync(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
        await dataContext.BulkCopyAsync(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: false));
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering BulkInsertEntities(entities={Entities})", entities);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
		_logger.LogWarning("<<< Exiting BulkInsertEntities(entities={Entities})", entities);
		_logger.LogError("BulkInsertEntities(entities={Entities}): dataContext → {DataContext}", entities, dataContext);
        dataContext.BulkCopy(new BulkCopyOptions() { KeepIdentity = true }, entities.RetrieveIdentity(dataContext, useSequenceName: false));
    }

    /// <summary>
    /// Executes command asynchronously and returns number of affected records
    /// </summary>
    /// <param name="sql">Command text</param>
    /// <param name="dataParameters">Command parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of records, affected by command execution.
    /// </returns>
    public virtual async Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
    {
		_logger.LogError(">>> Entering ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters})", sql, dataParameters);
        using var dataConnection = CreateDataConnection(LinqToDbDataProvider);
        var command = new CommandInfo(dataConnection, sql, dataParameters);
		_logger.LogError("ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters}): command → {Command}", sql, dataParameters, command);
		_logger.LogError("ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters}): dataConnection → {DataConnection}", sql, dataParameters, dataConnection);

		_logger.LogWarning("<<< Exiting ExecuteNonQueryAsync(sql={Sql},dataParameters={DataParameters})", sql, dataParameters);
        return await command.ExecuteAsync();
    }

    /// <summary>
    /// Executes command using System.Data.CommandType.StoredProcedure command type and
    /// returns results as collection of values of specified type
    /// </summary>
    /// <typeparam name="T">Result record type</typeparam>
    /// <param name="procedureName">Procedure name</param>
    /// <param name="parameters">Command parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the returns collection of query result records
    /// </returns>
    public virtual Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
    {
		_logger.LogError(">>> Entering QueryProcAsync(procedureName={ProcedureName},parameters={Parameters})", procedureName, parameters);
        using var dataConnection = CreateDataConnection(LinqToDbDataProvider);
        var command = new CommandInfo(dataConnection, procedureName, parameters);
		_logger.LogError("QueryProcAsync(procedureName={ProcedureName},parameters={Parameters}): command → {Command}", procedureName, parameters, command);
		_logger.LogError("QueryProcAsync(procedureName={ProcedureName},parameters={Parameters}): dataConnection → {DataConnection}", procedureName, parameters, dataConnection);

        var rez = command.QueryProc<T>()?.ToList();
		_logger.LogError("QueryProcAsync(procedureName={ProcedureName},parameters={Parameters}): rez → {Rez}", procedureName, parameters, rez);
		_logger.LogWarning("<<< Exiting QueryProcAsync(procedureName={ProcedureName},parameters={Parameters})", procedureName, parameters);
        return Task.FromResult<IList<T>>(rez ?? new List<T>());
    }

    /// <summary>
    /// Executes SQL command and returns results as collection of values of specified type
    /// </summary>
    /// <typeparam name="T">Type of result items</typeparam>
    /// <param name="sql">SQL command text</param>
    /// <param name="parameters">Parameters to execute the SQL command</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of values of specified type
    /// </returns>
    public virtual Task<IList<T>> QueryAsync<T>(string sql, params DataParameter[] parameters)
    {
        using var dataContext = CreateDataConnection();
        return Task.FromResult<IList<T>>(dataContext.Query<T>(sql, parameters)?.ToList() ?? new List<T>());
    }

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public virtual async Task TruncateAsync<TEntity>(bool resetIdentity = false) where TEntity : BaseEntity
    {
		_logger.LogError(">>> Entering TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
		_logger.LogWarning("<<< Exiting TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
		_logger.LogError("TruncateAsync(resetIdentity={ResetIdentity}): dataContext → {DataContext}", resetIdentity, dataContext);
        await dataContext.GetTable<TEntity>().TruncateAsync(resetIdentity);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Linq2Db data provider
    /// </summary>
    protected abstract IDataProvider LinqToDbDataProvider { get; }

    /// <summary>
    /// Gets the current data settings
    /// </summary>
    protected DataConfig DataSettings => DataSettingsManager.LoadSettings();

    /// <summary>
    /// Name of database provider
    /// </summary>
    public string ConfigurationName => LinqToDbDataProvider.Name;

    #endregion
}