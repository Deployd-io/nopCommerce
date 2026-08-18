using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a related product entity builder
/// </summary>
public partial class RelatedProductBuilder : NopEntityBuilder<RelatedProduct>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
	_logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
	_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}