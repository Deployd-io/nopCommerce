using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.UI.Paging;

namespace Nop.Web.Models.Blogs;

public partial record BlogPagingFilteringModel : BasePageableModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    public virtual DateTime? GetParsedMonth()
    {
		_logger.LogDebug(">>> Entering GetParsedMonth()");
        DateTime? result = null;
        if (!string.IsNullOrEmpty(Month))
		_logger.LogDebug("GetParsedMonth(): result → {Result}", result);
        {
            var tempDate = Month.Split(['-']);
			_logger.LogDebug("GetParsedMonth(): tempDate → {TempDate}", tempDate);
            if (tempDate.Length == 2)
            {
                result = new DateTime(Convert.ToInt32(tempDate[0]), Convert.ToInt32(tempDate[1]), 1);
            }
        }
		_logger.LogDebug("<<< Exiting GetParsedMonth()");
        return result;
    }
    public virtual DateTime? GetFromMonth()
    {
		_logger.LogDebug(">>> Entering GetFromMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
			_logger.LogDebug("GetFromMonth(): filterByMonth → {FilterByMonth}", filterByMonth);
            return filterByMonth.Value;
        return null;
    }
    public virtual DateTime? GetToMonth()
    {
		_logger.LogDebug(">>> Entering GetToMonth()");
        var filterByMonth = GetParsedMonth();
        if (filterByMonth.HasValue)
			_logger.LogDebug("GetToMonth(): filterByMonth → {FilterByMonth}", filterByMonth);
            return filterByMonth.Value.AddMonths(1).AddSeconds(-1);
        return null;
    }
    #endregion

    #region Properties

    public string Month { get; set; }

    public string Tag { get; set; }

    #endregion
}