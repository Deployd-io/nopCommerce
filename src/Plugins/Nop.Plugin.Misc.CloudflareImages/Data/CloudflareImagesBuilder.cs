using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Nop.Plugin.Misc.CloudflareImages.Data;

public class CloudflareImagesBuilder : NopEntityBuilder<Domain.CloudflareImages>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(Domain.CloudflareImages.ThumbFileName)).AsString().Indexed()
            .WithColumn(nameof(Domain.CloudflareImages.ImageId)).AsString(100);
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }
}