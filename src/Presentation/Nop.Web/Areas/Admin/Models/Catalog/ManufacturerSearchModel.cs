using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a manufacturer search model
/// </summary>
public partial record ManufacturerSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ManufacturerSearchModel()
    {
		_logger.LogError(">>> Entering ManufacturerSearchModel()");
        AvailableStores = new List<SelectListItem>();
        AvailablePublishedOptions = new List<SelectListItem>();
		_logger.LogError("ManufacturerSearchModel(): AvailablePublishedOptions → {AvailablePublishedOptions}", AvailablePublishedOptions);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
    public string SearchManufacturerName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchPublished")]
    public int SearchPublishedId { get; set; }

    public IList<SelectListItem> AvailablePublishedOptions { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}