using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a never sold products report search model
/// </summary>
public partial record NeverSoldReportSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NeverSoldReportSearchModel()
    {
		_logger.LogError(">>> Entering NeverSoldReportSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogError("NeverSoldReportSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
		_logger.LogError("NeverSoldReportSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogError("NeverSoldReportSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchCategory")]
    public int SearchCategoryId { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchManufacturer")]
    public int SearchManufacturerId { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.SearchVendor")]
    public int SearchVendorId { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    #endregion
}