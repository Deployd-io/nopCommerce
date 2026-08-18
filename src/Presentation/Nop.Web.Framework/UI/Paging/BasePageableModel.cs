//Contributor : MVCContrib

using Nop.Core;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.UI.Paging;

/// <summary>
/// Base class for pageable models
/// </summary>
public abstract record BasePageableModel : BaseNopModel, IPageableModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Ctor
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="pagedList">Entities (models)</param>
    public virtual void LoadPagedList<T>(IPagedList<T> pagedList)
    {
		_logger.LogDebug(">>> Entering LoadPagedList(pagedList={PagedList})", pagedList);
        FirstItem = (pagedList.PageIndex * pagedList.PageSize) + 1;
        HasNextPage = pagedList.HasNextPage;
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): FirstItem → {FirstItem}", pagedList, FirstItem);
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): HasNextPage → {HasNextPage}", pagedList, HasNextPage);
        HasPreviousPage = pagedList.HasPreviousPage;
        LastItem = Math.Min(pagedList.TotalCount, ((pagedList.PageIndex * pagedList.PageSize) + pagedList.PageSize));
        PageNumber = pagedList.PageIndex + 1;
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): HasPreviousPage → {HasPreviousPage}", pagedList, HasPreviousPage);
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): LastItem → {LastItem}", pagedList, LastItem);
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): PageNumber → {PageNumber}", pagedList, PageNumber);
        PageSize = pagedList.PageSize;
        TotalItems = pagedList.TotalCount;
        TotalPages = pagedList.TotalPages;
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): PageSize → {PageSize}", pagedList, PageSize);
		_logger.LogDebug("<<< Exiting LoadPagedList(pagedList={PagedList})", pagedList);
		_logger.LogDebug("LoadPagedList(pagedList={PagedList}): TotalItems → {TotalItems}", pagedList, TotalItems);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The current page index (starts from 0)
    /// </summary>
    public int PageIndex
    {
        get
        {
            if (PageNumber > 0)
                return PageNumber - 1;

            return 0;
        }
    }

    /// <summary>
    /// The current page number (starts from 1)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items in each page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The index of the first item in the page.
    /// </summary>
    public int FirstItem { get; set; }

    /// <summary>
    /// The index of the last item in the page.
    /// </summary>
    public int LastItem { get; set; }

    /// <summary>
    /// Whether there are pages before the current page.
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Whether there are pages after the current page.
    /// </summary>
    public bool HasNextPage { get; set; }

    #endregion
}