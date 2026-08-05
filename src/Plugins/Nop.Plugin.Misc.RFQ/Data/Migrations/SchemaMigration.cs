using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.RFQ.Domains;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2024/07/03 10:30:08:1687554", "Nop.Plugin.Misc.RFQ schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        Create.TableFor<RequestQuote>();
        Create.TableFor<RequestQuoteItem>();

        Create.TableFor<Quote>();
        Create.TableFor<QuoteItem>();
		_logger.LogDebug("<<< Exiting Up()");
    }
}