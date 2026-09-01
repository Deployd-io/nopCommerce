using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a filtering values search model
/// </summary>
public partial record FilterLevelValueSearchModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FilterLevelValueSearchModel()
    {
		_logger.LogDebug(">>> Entering FilterLevelValueSearchModel()");
        AvailableFilterLevel1Values = new List<SelectListItem>();
		_logger.LogDebug("FilterLevelValueSearchModel(): AvailableFilterLevel1Values → {AvailableFilterLevel1Values}", AvailableFilterLevel1Values);
        AvailableFilterLevel2Values = new List<SelectListItem>();
		_logger.LogDebug("FilterLevelValueSearchModel(): AvailableFilterLevel2Values → {AvailableFilterLevel2Values}", AvailableFilterLevel2Values);
        AvailableFilterLevel3Values = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public string fl1id { get; set; }
    public bool HideSearchFilterValue1 { get; set; }

    public string fl2id { get; set; }
    public bool HideSearchFilterValue2 { get; set; }

    public string fl3id { get; set; }
    public bool HideSearchFilterValue3 { get; set; }
    public IList<SelectListItem> AvailableFilterLevel1Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel2Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel3Values { get; set; }

    #endregion
}
