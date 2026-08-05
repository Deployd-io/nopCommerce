using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Orders;

/// <summary>
/// Represents a return request entity builder
/// </summary>
public partial class ReturnRequestBuilder : NopEntityBuilder<ReturnRequest>
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
            .WithColumn(nameof(ReturnRequest.ReasonForReturn)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ReturnRequest.RequestedAction)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ReturnRequest.CustomerId)).AsInt32().ForeignKey<Customer>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}