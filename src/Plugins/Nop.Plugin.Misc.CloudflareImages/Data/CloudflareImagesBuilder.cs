using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.CloudflareImages.Data;

public class CloudflareImagesBuilder : NopEntityBuilder<Domain.CloudflareImages>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(Domain.CloudflareImages.ThumbFileName)).AsString().Indexed()
            .WithColumn(nameof(Domain.CloudflareImages.ImageId)).AsString(100);
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }
}