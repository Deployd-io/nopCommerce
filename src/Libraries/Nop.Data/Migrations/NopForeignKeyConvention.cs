using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations;

/// <summary>
/// Convention for the default naming of a foreign key
/// </summary>
public class NopForeignKeyConvention : IForeignKeyConvention
{
    #region Fields

    protected readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public NopForeignKeyConvention(INopDataProvider dataProvider)
    {
		logger.LogDebug(">>> Entering NopForeignKeyConvention(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		logger.LogDebug("<<< Exiting NopForeignKeyConvention(dataProvider={DataProvider})", dataProvider);
		logger.LogDebug("NopForeignKeyConvention(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
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
		logger.LogDebug(">>> Entering GetForeignKeyName(foreignKey={ForeignKey})", foreignKey);
        var foreignColumns = string.Join('_', foreignKey.ForeignColumns);
        var primaryColumns = string.Join('_', foreignKey.PrimaryColumns);
		logger.LogDebug("GetForeignKeyName(foreignKey={ForeignKey}): foreignColumns → {ForeignColumns}", foreignKey, foreignColumns);
		logger.LogDebug("GetForeignKeyName(foreignKey={ForeignKey}): primaryColumns → {PrimaryColumns}", foreignKey, primaryColumns);

        var keyName = _dataProvider.CreateForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns);

		logger.LogDebug("GetForeignKeyName(foreignKey={ForeignKey}): keyName → {KeyName}", foreignKey, keyName);
		logger.LogDebug("<<< Exiting GetForeignKeyName(foreignKey={ForeignKey})", foreignKey);
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
		logger.LogDebug(">>> Entering Apply(expression={Expression})", expression);
        if (string.IsNullOrEmpty(expression.ForeignKey.Name))
            expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);

		logger.LogDebug("<<< Exiting Apply(expression={Expression})", expression);
        return expression;
    }

    #endregion
}