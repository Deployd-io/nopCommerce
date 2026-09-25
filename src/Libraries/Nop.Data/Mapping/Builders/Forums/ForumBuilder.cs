using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum buil entity builder
/// </summary>
public partial class ForumBuilder : NopEntityBuilder<Forum>
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
            .WithColumn(nameof(Forum.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(Forum.ForumGroupId)).AsInt32().ForeignKey<ForumGroup>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}