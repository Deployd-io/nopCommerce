using Microsoft.Extensions.Logging;
using Nop.Web.Framework.UI.Paging;

namespace Nop.Web.Models.Blogs;

public partial record BlogPagingFilteringModel : BasePageableModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    public virtual DateTime? GetParsedMonth()
    {
		_logger.LogError(">>> Entering GetParsedMonth()");
        DateTime? result = null;
        if (!string.IsNullOrEmpty(Month))
        {
            var tempDate = Month.Split(['-']);
			_logger.LogError("GetParsedMonth(): tempDate → {TempDate}", tempDate);
            if (tempDate.Length == 2)
            {
                result = new DateTime(Convert.ToInt32(tempDate[0]), Convert.ToInt32(tempDate[1]), 1);
				_logger.LogError("GetParsedMonth(): result → {Result}", result);
            }
        }
		_logger.LogWarning("<<< Exiting GetParsedMonth()");
        return result;
    }
    public virtual DateTime? GetFromMonth()
    {
		_logger.LogError(">>> Entering GetFromMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
            return filterByMonth.Value;
        return null;
    }
    public virtual DateTime? GetToMonth()
    {
		_logger.LogError(">>> Entering GetToMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
            return filterByMonth.Value.AddMonths(1).AddSeconds(-1);
        return null;
    }
    #endregion

    #region Properties

    public string Month { get; set; }

    public string Tag { get; set; }

    #endregion
}