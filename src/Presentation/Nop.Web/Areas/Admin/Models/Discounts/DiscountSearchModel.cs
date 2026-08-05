using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount search model
/// </summary>
public partial record DiscountSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DiscountSearchModel()
    {
		_logger.LogDebug(">>> Entering DiscountSearchModel()");
        AvailableActiveOptions = new List<SelectListItem>();
        AvailableDiscountTypes = new List<SelectListItem>();
		_logger.LogDebug("DiscountSearchModel(): AvailableActiveOptions → {AvailableActiveOptions}", AvailableActiveOptions);
		_logger.LogDebug("DiscountSearchModel(): AvailableDiscountTypes → {AvailableDiscountTypes}", AvailableDiscountTypes);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting DiscountSearchModel()");
		_logger.LogDebug("DiscountSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountCouponCode")]
    public string SearchDiscountCouponCode { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountName")]
    public string SearchDiscountName { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchDiscountType")]
    public int SearchDiscountTypeId { get; set; }

    public IList<SelectListItem> AvailableDiscountTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchStartDate")]
    [UIHint("DateNullable")]
    public DateTime? SearchStartDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchEndDate")]
    [UIHint("DateNullable")]
    public DateTime? SearchEndDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.IsActive")]
    public int IsActiveId { get; set; }
    public IList<SelectListItem> AvailableActiveOptions { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Discounts.List.SearchVendor")]
    public int SearchVendorId { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    #endregion
}