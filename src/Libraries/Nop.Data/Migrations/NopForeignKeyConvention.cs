using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations;

/// <summary>
/// Convention for the default naming of a foreign key
/// </summary>
public class NopForeignKeyConvention : IForeignKeyConvention
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public NopForeignKeyConvention(INopDataProvider dataProvider)
    {
		_logger.LogError(">>> Entering NopForeignKeyConvention(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		_logger.LogDebug("NopForeignKeyConvention(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    _logger.LogError("NopForeignKeyConvention(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
    _logger.LogWarning("<<< Exiting NopForeignKeyConvention(dataProvider={DataProvider})", dataProvider);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the default name of a foreign key
    /// </summary>
    /// <param name="foreignKey">The foreign key definition</param>
    /// <returns>Name of a foreign key</returns>
    protected virtual string GetForeignKeyName(ForeignKeyDefinition foreignKey)
    {
		_logger.LogError(">>> Entering GetForeignKeyName(foreignKey={ForeignKey})", foreignKey);
        var foreignColumns = string.Join('_', foreignKey.ForeignColumns);
        var primaryColumns = string.Join('_', foreignKey.PrimaryColumns);
		_logger.LogError("GetForeignKeyName(foreignKey={ForeignKey}): primaryColumns → {PrimaryColumns}", foreignKey, primaryColumns);
		_logger.LogError("GetForeignKeyName(foreignKey={ForeignKey}): foreignColumns → {ForeignColumns}", foreignKey, foreignColumns);

        var keyName = _dataProvider.CreateForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns);
		_logger.LogError("GetForeignKeyName(foreignKey={ForeignKey}): keyName → {KeyName}", foreignKey, keyName);

		_logger.LogWarning("<<< Exiting GetForeignKeyName(foreignKey={ForeignKey})", foreignKey);
        return keyName;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies a convention to a FluentMigrator.Expressions.IForeignKeyExpression
    /// </summary>
    /// <param name="expression">The expression this convention should be applied to</param>
    /// <returns>The same or a new expression. The underlying type must stay the same</returns>
    public IForeignKeyExpression Apply(IForeignKeyExpression expression)
    {
		_logger.LogError(">>> Entering Apply(expression={Expression})", expression);
        if (string.IsNullOrEmpty(expression.ForeignKey.Name))
            expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);

		_logger.LogWarning("<<< Exiting Apply(expression={Expression})", expression);
        return expression;
    }

    #endregion
}