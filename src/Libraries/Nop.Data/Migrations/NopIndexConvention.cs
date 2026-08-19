using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations;

/// <summary>
/// Convention for the default naming of an index
/// </summary>
public class NopIndexConvention : IIndexConvention
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public NopIndexConvention(INopDataProvider dataProvider)
    {
		_logger.LogDebug(">>> Entering NopIndexConvention(dataProvider={DataProvider})", dataProvider);
        _dataProvider = dataProvider;
		_logger.LogDebug("NopIndexConvention(dataProvider={DataProvider}): _dataProvider → {_dataProvider}", dataProvider, _dataProvider);
		_logger.LogDebug("<<< Exiting NopIndexConvention(dataProvider={DataProvider})", dataProvider);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the default name of an index
    /// </summary>
    /// <param name="index">The index definition</param>
    /// <returns>Name of an index</returns>
    protected virtual string GetIndexName(IndexDefinition index)
    {
		_logger.LogDebug(">>> Entering GetIndexName(index={Index})", index);
		_logger.LogDebug("<<< Exiting GetIndexName(index={Index})", index);
        return _dataProvider.GetIndexName(index.TableName, string.Join('_', index.Columns.Select(c => c.Name)));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies a convention to a FluentMigrator.Expressions.IIndexExpression
    /// </summary>
    /// <param name="expression">The expression this convention should be applied to</param>
    /// <returns>The same or a new expression. The underlying type must stay the same.</returns>
    public IIndexExpression Apply(IIndexExpression expression)
    {
		_logger.LogDebug(">>> Entering Apply(expression={Expression})", expression);
        if (string.IsNullOrEmpty(expression.Index.Name))
            expression.Index.Name = GetIndexName(expression.Index);

		_logger.LogDebug("<<< Exiting Apply(expression={Expression})", expression);
        return expression;
    }

    #endregion
}