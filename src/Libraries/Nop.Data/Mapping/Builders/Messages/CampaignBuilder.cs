using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents a campaign entity builder
/// </summary>
public partial class CampaignBuilder : NopEntityBuilder<Campaign>
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
            .WithColumn(nameof(Campaign.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Campaign.Subject)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Campaign.Body)).AsString(int.MaxValue).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}