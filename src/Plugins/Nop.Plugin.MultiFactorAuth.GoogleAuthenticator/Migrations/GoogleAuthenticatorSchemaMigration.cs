using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Migrations;

[NopMigration("2020/07/30 12:00:00", "Nop.Plugin.MultiFactorAuth.GoogleAuthenticator schema", MigrationProcessType.Installation)]
public class GoogleAuthenticatorSchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        Create.TableFor<GoogleAuthenticatorRecord>();
		_logger.LogDebug("<<< Exiting Up()");
    }
}