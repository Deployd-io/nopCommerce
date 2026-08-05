using FluentMigrator.Expressions;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations;

/// <summary>
/// Column type convention
/// </summary>
public class NopColumnsConvention : IColumnsConvention
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Applies a convention to a FluentMigrator.Expressions.IColumnsExpression
    /// </summary>
    /// <param name="expression">The expression this convention should be applied to</param>
    /// <returns>The same or a new expression. The underlying type must stay the same</returns>
    public IColumnsExpression Apply(IColumnsExpression expression)
    {
		_logger.LogDebug(">>> Entering Apply(expression={Expression})", expression);
        var dataSettings = DataSettingsManager.LoadSettings();

		_logger.LogDebug("Apply(expression={Expression}): dataSettings → {DataSettings}", expression, dataSettings);
        if (dataSettings.DataProvider != DataProviderType.PostgreSQL)
            return expression;

        foreach (var columnDefinition in expression.Columns)
        {
            if (columnDefinition.Type != System.Data.DbType.String)
                continue;

            columnDefinition.Type = null;
			_logger.LogDebug("Apply(expression={Expression}): columnDefinition.Type → {Arg1}", expression, columnDefinition.Type);
			_logger.LogDebug("Apply(expression={Expression}): columnDefinition.CustomType → {Arg1}", expression, columnDefinition.CustomType);
            columnDefinition.CustomType = "citext";
        }

		_logger.LogDebug("<<< Exiting Apply(expression={Expression})", expression);
        return expression;
    }

    #endregion
}