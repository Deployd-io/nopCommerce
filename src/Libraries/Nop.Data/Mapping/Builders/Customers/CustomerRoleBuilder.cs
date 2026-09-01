using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Customers;

/// <summary>
/// Represents a customer role entity builder
/// </summary>
public partial class CustomerRoleBuilder : NopEntityBuilder<CustomerRole>
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
            .WithColumn(nameof(CustomerRole.Name)).AsString(255).NotNullable()
            .WithColumn(nameof(CustomerRole.SystemName)).AsString(255).Nullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}