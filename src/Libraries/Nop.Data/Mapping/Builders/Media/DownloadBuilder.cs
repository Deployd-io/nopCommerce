using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Media;

/// <summary>
/// Represents a download entity builder
/// </summary>
public partial class DownloadBuilder : NopEntityBuilder<Download>
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
    }

    #endregion
}