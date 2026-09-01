using Microsoft.Extensions.Logging;
using Nop.Core;

namespace Nop.Web.Framework.Models.Extensions;

/// <summary>
/// Represents model extensions
/// </summary>
public static class ModelExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Convert list to paged list according to paging request
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="list">List of objects</param>
    /// <param name="pagingRequestModel">Paging request model</param>
    /// <returns>Paged list</returns>
    public static IPagedList<T> ToPagedList<T>(this IList<T> list, IPagingRequestModel pagingRequestModel)
    {
        return new PagedList<T>(list, pagingRequestModel.Page - 1, pagingRequestModel.PageSize);
    }

    /// <summary>
    /// Convert async-enumerable sequence to paged list according to paging request
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="collection">Async-enumerable sequence of objects</param>
    /// <param name="pagingRequestModel">Paging request model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list
    /// </returns>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IAsyncEnumerable<T> collection, IPagingRequestModel pagingRequestModel)
    {
		_logger.LogDebug(">>> Entering ToPagedListAsync(collection={Collection},pagingRequestModel={PagingRequestModel})", collection, pagingRequestModel);
        var list = await collection.ToListAsync();
		_logger.LogDebug("ToPagedListAsync(collection={Collection},pagingRequestModel={PagingRequestModel}): list → {List}", collection, pagingRequestModel, list);
        return list.ToPagedList(pagingRequestModel);
    }

    /// <summary>
    /// Prepare passed list model to display in a grid
    /// </summary>
    /// <typeparam name="TListModel">List model type</typeparam>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TObject">Object type</typeparam>
    /// <param name="listModel">List model</param>
    /// <param name="searchModel">Search model</param>
    /// <param name="objectList">Paged list of objects</param>
    /// <param name="dataFillFunction">Function to populate model data</param>
    /// <returns>List model</returns>
    public static TListModel PrepareToGrid<TListModel, TModel, TObject>(this TListModel listModel,
        BaseSearchModel searchModel, IPagedList<TObject> objectList, Func<IEnumerable<TModel>> dataFillFunction)
        where TListModel : BasePagedListModel<TModel>
        where TModel : BaseNopModel
    {
		_logger.LogDebug(">>> Entering PrepareToGrid(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction})", listModel, searchModel, objectList, dataFillFunction);
        ArgumentNullException.ThrowIfNull(listModel);

        listModel.Data = dataFillFunction?.Invoke();
        listModel.Draw = searchModel?.Draw;
		_logger.LogDebug("PrepareToGrid(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.Draw → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.Draw);
		_logger.LogDebug("PrepareToGrid(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.RecordsTotal → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.RecordsTotal);
		_logger.LogDebug("PrepareToGrid(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.RecordsFiltered → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.RecordsFiltered);
        listModel.RecordsTotal = objectList?.TotalCount ?? 0;
        listModel.RecordsFiltered = objectList?.TotalCount ?? 0;

		_logger.LogDebug("<<< Exiting PrepareToGrid(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction})", listModel, searchModel, objectList, dataFillFunction);
        return listModel;
    }

    /// <summary>
    /// Prepare passed list model to display in a grid
    /// </summary>
    /// <typeparam name="TListModel">List model type</typeparam>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TObject">Object type</typeparam>
    /// <param name="listModel">List model</param>
    /// <param name="searchModel">Search model</param>
    /// <param name="objectList">Paged list of objects</param>
    /// <param name="dataFillFunction">Function to populate model data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list model
    /// </returns>
    public static async Task<TListModel> PrepareToGridAsync<TListModel, TModel, TObject>(this TListModel listModel,
        BaseSearchModel searchModel, IPagedList<TObject> objectList, Func<IAsyncEnumerable<TModel>> dataFillFunction)
        where TListModel : BasePagedListModel<TModel>
        where TModel : BaseNopModel
    {
		_logger.LogDebug(">>> Entering PrepareToGridAsync(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction})", listModel, searchModel, objectList, dataFillFunction);
        ArgumentNullException.ThrowIfNull(listModel);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug("PrepareToGridAsync(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.Draw → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.Draw);
		_logger.LogDebug("PrepareToGridAsync(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.RecordsTotal → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.RecordsTotal);
		_logger.LogInformation("PrepareToGridAsync(listModel,searchModel,objectList,dataFillFunction)={ListModel},{SearchModel},{ObjectList},{DataFillFunction}: database query executed in {Arg4} ms", listModel, searchModel, objectList, dataFillFunction, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("PrepareToGridAsync(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction}): listModel.RecordsFiltered → {Arg4}", listModel, searchModel, objectList, dataFillFunction, listModel.RecordsFiltered);
        listModel.Data = await (dataFillFunction?.Invoke()).ToListAsync();
		_logger.LogDebug("<<< Exiting PrepareToGridAsync(listModel={ListModel},searchModel={SearchModel},objectList={ObjectList},dataFillFunction={DataFillFunction})", listModel, searchModel, objectList, dataFillFunction);
        listModel.Draw = searchModel?.Draw;
        listModel.RecordsTotal = objectList?.TotalCount ?? 0;
        listModel.RecordsFiltered = objectList?.TotalCount ?? 0;

        return listModel;
    }
}