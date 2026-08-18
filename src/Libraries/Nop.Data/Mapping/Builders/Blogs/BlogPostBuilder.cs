using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Blogs;

/// <summary>
/// Represents a blog post entity builder
/// </summary>
public partial class BlogPostBuilder : NopEntityBuilder<BlogPost>
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
            .WithColumn(nameof(BlogPost.Title)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(BlogPost.Body)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(BlogPost.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(BlogPost.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(BlogPost.LanguageId)).AsInt32().ForeignKey<Language>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}