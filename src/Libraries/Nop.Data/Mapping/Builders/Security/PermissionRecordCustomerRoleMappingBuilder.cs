using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Security;

/// <summary>
/// Represents a permission record customer role mapping entity builder
/// </summary>
public partial class PermissionRecordCustomerRoleMappingBuilder : NopEntityBuilder<PermissionRecordCustomerRoleMapping>
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
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(PermissionRecordCustomerRoleMapping), nameof(PermissionRecordCustomerRoleMapping.PermissionRecordId)))
            .AsInt32().PrimaryKey().ForeignKey<PermissionRecord>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(PermissionRecordCustomerRoleMapping), nameof(PermissionRecordCustomerRoleMapping.CustomerRoleId)))
            .AsInt32().PrimaryKey().ForeignKey<CustomerRole>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}