using Microsoft.Extensions.Logging;
﻿using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents the data url
/// </summary>
public partial class DataUrl
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="controllerName">Controller name</param>
    /// <param name="routeValues">Route values</param>
    public DataUrl(string actionName, string controllerName, RouteValueDictionary routeValues)
    {
		_logger.LogDebug(">>> Entering DataUrl(actionName={ActionName},controllerName={ControllerName},routeValues={RouteValues})", actionName, controllerName, routeValues);
        ActionName = actionName;
        ControllerName = controllerName;
		_logger.LogDebug("DataUrl(actionName={ActionName},controllerName={ControllerName},routeValues={RouteValues}): ControllerName → {ControllerName1}", actionName, controllerName, routeValues, ControllerName);
		_logger.LogDebug("DataUrl(actionName={ActionName},controllerName={ControllerName},routeValues={RouteValues}): ActionName → {ActionName1}", actionName, controllerName, routeValues, ActionName);
        RouteValues = routeValues;
		_logger.LogDebug("<<< Exiting DataUrl(actionName={ActionName},controllerName={ControllerName},routeValues={RouteValues})", actionName, controllerName, routeValues);
		_logger.LogDebug("DataUrl(actionName={ActionName},controllerName={ControllerName},routeValues={RouteValues}): RouteValues → {RouteValues1}", actionName, controllerName, routeValues, RouteValues);
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    public DataUrl(string url)
    {
		_logger.LogDebug(">>> Entering DataUrl(url={Url})", url);
        Url = url;
		_logger.LogDebug("<<< Exiting DataUrl(url={Url})", url);
		_logger.LogInformation("DataUrl(url={Url}): Url → {Url1}", url, Url);
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="dataId">Name of the column whose value is to be used as identifier in URL</param>
    public DataUrl(string url, string dataId)
    {
		_logger.LogDebug(">>> Entering DataUrl(url={Url},dataId={DataId})", url, dataId);
        Url = url;
        DataId = dataId;
		_logger.LogDebug("<<< Exiting DataUrl(url={Url},dataId={DataId})", url, dataId);
		_logger.LogDebug("DataUrl(url={Url},dataId={DataId}): DataId → {DataId1}", url, dataId, DataId);
		_logger.LogInformation("DataUrl(url={Url},dataId={DataId}): Url → {Url1}", url, dataId, Url);
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="trimEnd">Parameter indicating that you need to delete all occurrences of the character "/" at the end of the line</param>
    public DataUrl(string url, bool trimEnd)
    {
		_logger.LogDebug(">>> Entering DataUrl(url={Url},trimEnd={TrimEnd})", url, trimEnd);
        Url = url;
        TrimEnd = trimEnd;
		_logger.LogDebug("<<< Exiting DataUrl(url={Url},trimEnd={TrimEnd})", url, trimEnd);
		_logger.LogDebug("DataUrl(url={Url},trimEnd={TrimEnd}): TrimEnd → {TrimEnd1}", url, trimEnd, TrimEnd);
		_logger.LogInformation("DataUrl(url={Url},trimEnd={TrimEnd}): Url → {Url1}", url, trimEnd, Url);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Gets or sets the name of the controller.
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the route values.
    /// </summary>
    public RouteValueDictionary RouteValues { get; set; }

    /// <summary>
    /// Gets or sets data Id
    /// </summary>
    public string DataId { get; set; }

    /// <summary>
    /// Gets or sets parameter indicating that you need to delete all occurrences of the character "/" at the end of the line
    /// </summary>
    public bool TrimEnd { get; set; }

    #endregion
}