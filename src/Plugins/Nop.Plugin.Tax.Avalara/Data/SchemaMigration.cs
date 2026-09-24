using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.Avalara.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Data;

[NopMigration("2020/02/03 09:09:17:6455442", "Tax.Avalara base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        Create.TableFor<TaxTransactionLog>();
        Create.TableFor<ItemClassification>();
		_logger.LogWarning("<<< Exiting Up()");
    }

    #endregion
}