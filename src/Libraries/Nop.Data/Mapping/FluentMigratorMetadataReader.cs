using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Metadata;
using Nop.Core;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping;

/// <summary>
/// LINQ To DB metadata reader for schema created by FluentMigrator
/// </summary>
public partial class FluentMigratorMetadataReader : IMetadataReader
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Utilities

    private static DataType DbTypeToDataType(DbType dbType)
    {
		_logger.LogError(">>> Entering DbTypeToDataType(dbType={DbType})", dbType);
		_logger.LogWarning("<<< Exiting DbTypeToDataType(dbType={DbType})", dbType);
        return dbType switch
        {
            DbType.AnsiString => DataType.VarChar,
            DbType.AnsiStringFixedLength => DataType.VarChar,
            DbType.Binary => DataType.Binary,
            DbType.Boolean => DataType.Boolean,
            DbType.Byte => DataType.Byte,
            DbType.Currency => DataType.Money,
            DbType.Date => DataType.Date,
            DbType.DateTime => DataType.DateTime,
            DbType.DateTime2 => DataType.DateTime2,
            DbType.DateTimeOffset => DataType.DateTimeOffset,
            DbType.Decimal => DataType.Decimal,
            DbType.Double => DataType.Double,
            DbType.Guid => DataType.Guid,
            DbType.Int16 => DataType.Int16,
            DbType.Int32 => DataType.Int32,
            DbType.Int64 => DataType.Int64,
            DbType.Object => DataType.Undefined,
            DbType.SByte => DataType.SByte,
            DbType.Single => DataType.Single,
            DbType.String => DataType.NVarChar,
            DbType.StringFixedLength => DataType.NVarChar,
            DbType.Time => DataType.Time,
            DbType.UInt16 => DataType.UInt16,
            DbType.UInt32 => DataType.UInt32,
            DbType.UInt64 => DataType.UInt64,
            DbType.VarNumeric => DataType.VarNumeric,
            DbType.Xml => DataType.Xml,
            _ => DataType.Undefined
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all mapping attributes on specified type.
    /// </summary>
    /// <param name="type">Attributes owner type.</param>
    /// <returns>Array of mapping attributes.</returns>
    public MappingAttribute[] GetAttributes(Type type)
    {
		_logger.LogError(">>> Entering GetAttributes(type={Type})", type);
        if (!type.IsSubclassOf(typeof(BaseEntity)))
            return [];

        var attribute = Types.GetOrAdd((type, null), _ =>
        {
            var entityDescriptor = NopMappingSchema.GetEntityDescriptor(type);
			_logger.LogError("GetAttributes(type={Type}): entityDescriptor → {EntityDescriptor}", type, entityDescriptor);

            if (entityDescriptor is null)
                return null;

		_logger.LogWarning("<<< Exiting GetAttributes(type={Type})", type);
            return new TableAttribute() { Schema = entityDescriptor.SchemaName, Name = entityDescriptor.EntityName };
        });

		_logger.LogWarning("<<< Exiting GetAttributes(type={Type},memberInfo={MemberInfo})", type, memberInfo);
        return [attribute];
    }

    /// <summary>
    /// Gets all mapping attributes on specified type member.
    /// </summary>
    /// <param name="type">Member type. Could be used by some metadata providers to identify actual member owner type.</param>
    /// <param name="memberInfo">Type member for which mapping attributes should be returned.</param>
    /// <returns>Array of attributes.</returns>
    public MappingAttribute[] GetAttributes(Type type, MemberInfo memberInfo)
    {
        if (!type.IsSubclassOf(typeof(BaseEntity)))
            return [];

        var entityDescriptor = NopMappingSchema.GetEntityDescriptor(type);

        if (entityDescriptor is null)
            return [];

        var attribute = Types.GetOrAdd((type, memberInfo), _ =>
        {

            var entityField = entityDescriptor.Fields.SingleOrDefault(cd => cd.Name.Equals(NameCompatibilityManager.GetColumnName(type, memberInfo.Name), StringComparison.OrdinalIgnoreCase));

            if (entityField is null)
                return null;

            if (!(memberInfo as PropertyInfo)?.CanWrite ?? false)
                return null;

            return new ColumnAttribute
            {
                Name = entityField.Name,
                IsPrimaryKey = entityField.IsPrimaryKey,
                IsColumn = true,
                CanBeNull = entityField.IsNullable ?? false,
                Length = entityField.Size ?? 0,
                Precision = entityField.Precision ?? 0,
                IsIdentity = entityField.IsIdentity,
                DataType = DbTypeToDataType(entityField.Type)
            };
        });

        var result = new List<MappingAttribute>();

        if (attribute is ColumnAttribute column)
        {
            result.Add(column);

            //define sequence name for supported db engines to retrieve identity values (PostgreSQL)
            if (column.IsIdentity)
                result.Add(new SequenceNameAttribute($"{entityDescriptor.EntityName}_{column.Name}_seq"));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Gets the dynamic columns defined on given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>All dynamic columns defined on given type.</returns>
    public MemberInfo[] GetDynamicColumns(Type type)
    {
		_logger.LogError(">>> Entering GetDynamicColumns(type={Type})", type);
        _logger.LogWarning("<<< Exiting GetDynamicColumns(type={Type})", type);
        return [];
    }

    /// <summary>
    /// Should return a unique ID for cache purposes. If the implemented Metadata reader returns instance-specific
    /// data you'll need to calculate a unique value based on content. Otherwise just use a static const
    /// e.g. $".{nameof(YourMetadataReader)}."
    /// </summary>
    /// <returns>The object ID as string</returns>
    public string GetObjectID()
    {
		_logger.LogError(">>> Entering GetObjectID()");
        _logger.LogWarning("<<< Exiting GetObjectID()");
        return $".{nameof(FluentMigratorMetadataReader)}.";
    }

    #endregion

    #region Properties

    protected static ConcurrentDictionary<(Type, MemberInfo), MappingAttribute> Types { get; } = new ConcurrentDictionary<(Type, MemberInfo), MappingAttribute>();

    #endregion
}