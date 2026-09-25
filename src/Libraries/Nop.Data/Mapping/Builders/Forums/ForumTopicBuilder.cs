using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum topic entity builder
/// </summary>
public partial class ForumTopicBuilder : NopEntityBuilder<ForumTopic>
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
            .WithColumn(nameof(ForumTopic.Subject)).AsString(450).NotNullable()
            .WithColumn(nameof(ForumTopic.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
            .WithColumn(nameof(ForumTopic.ForumId)).AsInt32().ForeignKey<Forum>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}