using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Data;

/// <summary>
/// Represents the payment token entity builder
/// </summary>
public class PayPalTokenBuilder : NopEntityBuilder<PayPalToken>
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
            .WithColumn(nameof(PayPalToken.CustomerId)).AsInt32().NotNullable().ForeignKey<Customer>()
            .WithColumn(nameof(PayPalToken.VaultId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.VaultCustomerId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.TransactionId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.Title)).AsString(200).Nullable()
            .WithColumn(nameof(PayPalToken.Expiration)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.Type)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.ClientId)).AsString(200).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}