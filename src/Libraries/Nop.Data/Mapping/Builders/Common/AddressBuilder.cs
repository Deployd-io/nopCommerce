using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a address entity builder
/// </summary>
public partial class AddressBuilder : NopEntityBuilder<Address>
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
            .WithColumn(nameof(Address.CountryId)).AsInt32().Nullable().ForeignKey<Country>(onDelete: Rule.None)
            .WithColumn(nameof(Address.StateProvinceId)).AsInt32().Nullable().ForeignKey<StateProvince>(onDelete: Rule.None);
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}