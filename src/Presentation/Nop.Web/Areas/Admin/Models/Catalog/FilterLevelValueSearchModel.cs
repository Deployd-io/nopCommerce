using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filtering values search model
/// </summary>
public partial record FilterLevelValueSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FilterLevelValueSearchModel()
    {
		_logger.LogDebug(">>> Entering FilterLevelValueSearchModel()");
        AvailableFilterLevel1Values = new List<SelectListItem>();
        AvailableFilterLevel2Values = new List<SelectListItem>();
		_logger.LogDebug("FilterLevelValueSearchModel(): AvailableFilterLevel1Values → {AvailableFilterLevel1Values}", AvailableFilterLevel1Values);
		_logger.LogDebug("FilterLevelValueSearchModel(): AvailableFilterLevel2Values → {AvailableFilterLevel2Values}", AvailableFilterLevel2Values);
        AvailableFilterLevel3Values = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting FilterLevelValueSearchModel()");
		_logger.LogDebug("FilterLevelValueSearchModel(): AvailableFilterLevel3Values → {AvailableFilterLevel3Values}", AvailableFilterLevel3Values);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel1")]
    public string SearchFilterValue1Id { get; set; }
    public bool HideSearchFilterValue1 { get; set; }

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel2")]
    public string SearchFilterValue2Id { get; set; }
    public bool HideSearchFilterValue2 { get; set; }

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel3")]
    public string SearchFilterValue3Id { get; set; }
    public bool HideSearchFilterValue3 { get; set; }
    public IList<SelectListItem> AvailableFilterLevel1Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel2Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel3Values { get; set; }

    public int ProductId { get; set; }

    #endregion
}
