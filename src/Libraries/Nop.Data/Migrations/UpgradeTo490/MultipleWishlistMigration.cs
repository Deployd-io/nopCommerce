using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025/05/23 08:00:00", "Multiple wishlist")]
public class MultipleWishlistMigration : ForwardOnlyMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!Schema.Table(nameof(CustomWishlist)).Exists())
            Create.TableFor<CustomWishlist>();

        //add new column
        if (!Schema.Table(nameof(ShoppingCartItem)).Column(nameof(ShoppingCartItem.CustomWishlistId)).Exists())
            Alter.Table(nameof(ShoppingCartItem)).AddColumn(nameof(ShoppingCartItem.CustomWishlistId)).AsInt32().Nullable().ForeignKey<CustomWishlist>();
			_logger.LogDebug("<<< Exiting Up()");
    }

    #endregion
}
