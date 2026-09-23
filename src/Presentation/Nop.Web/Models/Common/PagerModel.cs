using Nop.Services.Localization;
using Nop.Web.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Common;

public partial record PagerModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;

    protected int _individualPagesDisplayedCount;
    protected int _pageIndex = -2;
    protected int _pageSize;

    protected bool? _showFirst;
    protected bool? _showIndividualPages;
    protected bool? _showLast;
    protected bool? _showNext;
    protected bool? _showPagerItems;
    protected bool? _showPrevious;
    protected bool? _showTotalSummary;

    #endregion Fields

    #region Ctor

    public PagerModel(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering PagerModel(localizationService={LocalizationService})", localizationService);
        _localizationService = localizationService;
		_logger.LogDebug("PagerModel(localizationService={LocalizationService}): _localizationService → {_localizationService}", localizationService, _localizationService);
    _logger.LogWarning("<<< Exiting PagerModel(localizationService={LocalizationService})", localizationService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the first button text
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetFirstButtonTextAsync()
    {
		_logger.LogError(">>> Entering GetFirstButtonTextAsync()");
        _logger.LogWarning("<<< Exiting GetFirstButtonTextAsync()");
        return await _localizationService.GetResourceAsync("Pager.First");
    }

    /// <summary>
    /// Gets the last button text
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetLastButtonTextAsync()
    {
		_logger.LogError(">>> Entering GetLastButtonTextAsync()");
        _logger.LogWarning("<<< Exiting GetLastButtonTextAsync()");
        return await _localizationService.GetResourceAsync("Pager.Last");
    }

    /// <summary>
    /// Gets the next button text
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetNextButtonTextAsync()
    {
		_logger.LogError(">>> Entering GetNextButtonTextAsync()");
        _logger.LogWarning("<<< Exiting GetNextButtonTextAsync()");
        return await _localizationService.GetResourceAsync("Pager.Next");
    }

    /// <summary>
    /// Gets the previous button text
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetPreviousButtonTextAsync()
    {
		_logger.LogError(">>> Entering GetPreviousButtonTextAsync()");
        _logger.LogWarning("<<< Exiting GetPreviousButtonTextAsync()");
        return await _localizationService.GetResourceAsync("Pager.Previous");
    }

    /// <summary>
    /// Gets or sets the current page text
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetCurrentPageTextAsync()
    {
		_logger.LogError(">>> Entering GetCurrentPageTextAsync()");
        _logger.LogWarning("<<< Exiting GetCurrentPageTextAsync()");
        return await _localizationService.GetResourceAsync("Pager.CurrentPage");
    }

    /// <summary>
    /// Gets first individual page index
    /// </summary>
    /// <returns>Page index</returns>
    public int GetFirstIndividualPageIndex()
    {
		_logger.LogError(">>> Entering GetFirstIndividualPageIndex()");
        if ((TotalPages < IndividualPagesDisplayedCount) ||
            ((PageIndex - (IndividualPagesDisplayedCount / 2)) < 0))
            return 0;

        if ((PageIndex + (IndividualPagesDisplayedCount / 2)) >= TotalPages) 
            return (TotalPages - IndividualPagesDisplayedCount);

        return (PageIndex - (IndividualPagesDisplayedCount / 2));
    }

    /// <summary>
    /// Get last individual page index
    /// </summary>
    /// <returns>Page index</returns>
    public virtual int GetLastIndividualPageIndex()
    {
		_logger.LogError(">>> Entering GetLastIndividualPageIndex()");
        var num = IndividualPagesDisplayedCount / 2;
        if ((IndividualPagesDisplayedCount % 2) == 0) 
            num--;

        if ((TotalPages < IndividualPagesDisplayedCount) ||
            ((PageIndex + num) >= TotalPages))
            return (TotalPages - 1);

        if ((PageIndex - (IndividualPagesDisplayedCount / 2)) < 0) 
            return (IndividualPagesDisplayedCount - 1);

        return PageIndex + num;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Gets the current page index
    /// </summary>
    public int CurrentPage => PageIndex + 1;

    /// <summary>
    /// Gets or sets a count of individual pages to be displayed
    /// </summary>
    public int IndividualPagesDisplayedCount
    {
        get
        {
            if (_individualPagesDisplayedCount <= 0)
                return 5;

            return _individualPagesDisplayedCount;
        }
        set => _individualPagesDisplayedCount = value;
    }

    /// <summary>
    /// Gets the current page index
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex < 0 ? 0 : _pageIndex;
        set => _pageIndex = value;
    }

    /// <summary>
    /// Gets or sets a page size
    /// </summary>
    public int PageSize
    {
        get => (_pageSize <= 0) ? 10 : _pageSize;
        set => _pageSize = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "first"
    /// </summary>
    public bool ShowFirst
    {
        get => _showFirst ?? true;
        set => _showFirst = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "individual pages"
    /// </summary>
    public bool ShowIndividualPages
    {
        get => _showIndividualPages ?? true;
        set => _showIndividualPages = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "last"
    /// </summary>
    public bool ShowLast
    {
        get => _showLast ?? true;
        set => _showLast = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "next"
    /// </summary>
    public bool ShowNext
    {
        get => _showNext ?? true;
        set => _showNext = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show pager items
    /// </summary>
    public bool ShowPagerItems
    {
        get => _showPagerItems ?? true;
        set => _showPagerItems = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "previous"
    /// </summary>
    public bool ShowPrevious
    {
        get => _showPrevious ?? true;
        set => _showPrevious = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "total summary"
    /// </summary>
    public bool ShowTotalSummary
    {
        get => _showTotalSummary ?? false;
        set => _showTotalSummary = value;
    }

    /// <summary>
    /// Gets a total pages count
    /// </summary>
    public int TotalPages
    {
        get
        {
            if (TotalRecords == 0 || PageSize == 0)
                return 0;

            var num = TotalRecords / PageSize;

            if ((TotalRecords % PageSize) > 0)
                num++;

            return num;
        }
    }

    /// <summary>
    /// Gets or sets a total records count
    /// </summary>
    public int TotalRecords { get; set; }


    /// <summary>
    /// Gets or sets the route name or action name
    /// </summary>
    public string RouteActionName { get; set; }

    /// <summary>
    /// Gets or sets whether the links are created using RouteLink instead of Action Link 
    /// (for additional route values such as slugs or page numbers)
    /// </summary>
    public bool UseRouteLinks { get; set; }

    /// <summary>
    /// Gets or sets the RouteValues object. Allows for custom route values other than page.
    /// </summary>
    public IRouteValues RouteValues { get; set; }

    #endregion Properties
}