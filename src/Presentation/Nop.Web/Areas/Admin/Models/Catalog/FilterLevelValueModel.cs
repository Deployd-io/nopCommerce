using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filter level value model
/// </summary>
public partial record FilterLevelValueModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FilterLevelValueModel()
    {
		_logger.LogError(">>> Entering FilterLevelValueModel()");
        FilterLevelValueProductSearchModel = new FilterLevelValueProductSearchModel();
		_logger.LogDebug("FilterLevelValueModel(): FilterLevelValueProductSearchModel → {FilterLevelValueProductSearchModel}", FilterLevelValueProductSearchModel);
    _logger.LogError("FilterLevelValueModel(): FilterLevelValueProductSearchModel → {FilterLevelValueProductSearchModel}", FilterLevelValueProductSearchModel);
    _logger.LogWarning("<<< Exiting FilterLevelValueModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel1")]
    public string FilterLevel1Value { get; set; }
    public bool FilterLevel1ValueEnabled { get; set; }
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel2")]
    public string FilterLevel2Value { get; set; }
    public bool FilterLevel2ValueEnabled { get; set; }
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel3")]
    public string FilterLevel3Value { get; set; }
    public bool FilterLevel3ValueEnabled { get; set; }

    public FilterLevelValueProductSearchModel FilterLevelValueProductSearchModel { get; set; }

    #endregion
}
