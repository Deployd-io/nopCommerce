using System.Collections.Concurrent;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping;

/// <summary>
/// Provides an access to entity mapping information
/// </summary>
public static class NopMappingSchema
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private static ConcurrentDictionary<Type, NopEntityDescriptor> EntityDescriptors { get; } = new();

    #endregion

    /// <summary>
    /// Returns mapped entity descriptor
    /// </summary>
    /// <param name="entityType">Type of entity</param>
    /// <returns>Mapped entity descriptor</returns>
    public static NopEntityDescriptor GetEntityDescriptor(Type entityType)
    {
		_logger.LogError(">>> Entering GetEntityDescriptor(entityType={EntityType})", entityType);
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
            return null;

        return EntityDescriptors.GetOrAdd(entityType, t =>
        {
            var tableName = NameCompatibilityManager.GetTableName(t);
            var expression = new CreateTableExpression { TableName = tableName };
            var builder = new CreateTableExpressionBuilder(expression, new NullMigrationContext());
            builder.RetrieveTableExpressions(t);

            return new NopEntityDescriptor
            {
                EntityName = tableName,
                SchemaName = builder.Expression.SchemaName,
                Fields = builder.Expression.Columns.Select(column => new NopEntityFieldDescriptor
                {
                    Name = column.Name,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsNullable = column.IsNullable,
                    Size = column.Size,
                    Precision = column.Precision,
                    IsIdentity = column.IsIdentity,
                    Type = column.Type ?? System.Data.DbType.String
                }).ToList()
            };
        });
		_logger.LogWarning("<<< Exiting GetEntityDescriptor(entityType={EntityType})", entityType);
    }

    /// <summary>
    /// Get or create mapping schema with specified configuration name
    /// </summary>
    public static MappingSchema GetMappingSchema(string configurationName, IDataProvider mappings)
    {
	_logger.LogError(">>> Entering GetMappingSchema(configurationName={ConfigurationName},mappings=<redacted>)", configurationName);

        if (Singleton<MappingSchema>.Instance is null)
        {
            Singleton<MappingSchema>.Instance = new MappingSchema(configurationName, mappings.MappingSchema);
            Singleton<MappingSchema>.Instance.AddMetadataReader(new FluentMigratorMetadataReader());
        }

		_logger.LogWarning("<<< Exiting GetMappingSchema(configurationName={ConfigurationName},mappings=<redacted>)", configurationName);
        return Singleton<MappingSchema>.Instance;
    }
}
