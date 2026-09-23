using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Media;

/// <summary>
/// Represents a video entity builder
/// </summary>
public partial class VideoBuilder : NopEntityBuilder<Video>
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
            .WithColumn(nameof(Video.VideoUrl)).AsString(1000).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}