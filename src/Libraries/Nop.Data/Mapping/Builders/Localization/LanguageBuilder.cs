using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Localization;

/// <summary>
/// Represents a language entity builder
/// </summary>
public partial class LanguageBuilder : NopEntityBuilder<Language>
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
            .WithColumn(nameof(Language.Name)).AsString(100).NotNullable()
            .WithColumn(nameof(Language.LanguageCulture)).AsString(20).NotNullable()
            .WithColumn(nameof(Language.UniqueSeoCode)).AsString(2).Nullable()
            .WithColumn(nameof(Language.FlagImageFileName)).AsString(50).Nullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}