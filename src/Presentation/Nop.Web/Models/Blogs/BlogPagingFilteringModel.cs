using Nop.Web.Framework.UI.Paging;

namespace Nop.Web.Models.Blogs;

public partial record BlogPagingFilteringModel : BasePageableModel
{
    #region Methods

    public virtual DateTime? GetParsedMonth()
    {
		logger.LogDebug(">>> Entering GetParsedMonth()");
        DateTime? result = null;
        if (!string.IsNullOrEmpty(Month))
		logger.LogDebug("GetParsedMonth(): result → {Result}", result);
        {
            var tempDate = Month.Split(['-']);
            if (tempDate.Length == 2)
			logger.LogDebug("GetParsedMonth(): tempDate → {TempDate}", tempDate);
            {
                result = new DateTime(Convert.ToInt32(tempDate[0]), Convert.ToInt32(tempDate[1]), 1);
            }
        }
		logger.LogDebug("<<< Exiting GetParsedMonth()");
        return result;
    }
    public virtual DateTime? GetFromMonth()
    {
		logger.LogDebug(">>> Entering GetFromMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
			logger.LogDebug("GetFromMonth(): filterByMonth → {FilterByMonth}", filterByMonth);
            return filterByMonth.Value;
        return null;
    }
    public virtual DateTime? GetToMonth()
    {
		logger.LogDebug(">>> Entering GetToMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
			logger.LogDebug("GetToMonth(): filterByMonth → {FilterByMonth}", filterByMonth);
            return filterByMonth.Value.AddMonths(1).AddSeconds(-1);
        return null;
    }
    #endregion

    #region Properties

    public string Month { get; set; }

    public string Tag { get; set; }

    #endregion
}