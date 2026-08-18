using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a generic attribute entity builder
/// </summary>
public partial class GenericAttributeBuilder : NopEntityBuilder<GenericAttribute>
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
        table
            .WithColumn(nameof(GenericAttribute.KeyGroup)).AsString(400).NotNullable()
            .WithColumn(nameof(GenericAttribute.Key)).AsString(400).NotNullable()
            .WithColumn(nameof(GenericAttribute.Value)).AsString(int.MaxValue).NotNullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}