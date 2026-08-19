using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.News;

/// <summary>
/// Represents a news item entity builder
/// </summary>
public partial class NewsItemBuilder : NopEntityBuilder<NewsItem>
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
            .WithColumn(nameof(NewsItem.Title)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.Short)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.Full)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(NewsItem.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(NewsItem.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(NewsItem.LanguageId)).AsInt32().ForeignKey<Language>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}