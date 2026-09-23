using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.News;

/// <summary>
/// Represents a news comment entity builder
/// </summary>
public partial class NewsCommentBuilder : NopEntityBuilder<NewsComment>
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
            .WithColumn(nameof(NewsComment.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(NewsComment.NewsItemId)).AsInt32().ForeignKey<NewsItem>()
            .WithColumn(nameof(NewsComment.StoreId)).AsInt32().ForeignKey<Store>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}