using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record SearchFilterLevelValueModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SearchFilterLevelValueModel()
    {
		_logger.LogDebug(">>> Entering SearchFilterLevelValueModel()");
        AvailableFilterLevel1Values = new List<SelectListItem>();
		_logger.LogDebug("SearchFilterLevelValueModel(): AvailableFilterLevel1Values → {AvailableFilterLevel1Values}", AvailableFilterLevel1Values);
        AvailableFilterLevel2Values = new List<SelectListItem>();
		_logger.LogDebug("SearchFilterLevelValueModel(): AvailableFilterLevel2Values → {AvailableFilterLevel2Values}", AvailableFilterLevel2Values);
        AvailableFilterLevel3Values = new List<SelectListItem>();
    }

    /// <summary>
    /// Filter level value 1 identifier
    /// </summary>
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel1")]
    public string fl1id { get; set; }

    /// <summary>
    ///  Filter level value 2 identifier
    /// </summary>
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel2")]
    public string fl2id { get; set; }

    /// <summary>
    ///  Filter level value 3 identifier
    /// </summary>
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel3")]
    public string fl3id { get; set; }

    public bool HideSearchFilterValue1 { get; set; }
    public bool HideSearchFilterValue2 { get; set; }
    public bool HideSearchFilterValue3 { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }

    public IList<SelectListItem> AvailableFilterLevel1Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel2Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel3Values { get; set; }
}
