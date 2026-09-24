using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Seo;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Seo;

/// <summary>
/// Represents a url record entity builder
/// </summary>
public partial class UrlRecordBuilder : NopEntityBuilder<UrlRecord>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogError(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(UrlRecord.EntityName)).AsString(400).NotNullable()
            .WithColumn(nameof(UrlRecord.Slug)).AsString(400).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}