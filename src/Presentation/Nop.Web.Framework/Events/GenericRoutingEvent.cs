using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs when a generic route is processed and before default transformation
/// </summary>
public partial class GenericRoutingEvent : IStopProcessingEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GenericRoutingEvent(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord)
    {
		_logger.LogDebug(">>> Entering GenericRoutingEvent(httpContext={HttpContext},values={Values},urlRecord={UrlRecord})", httpContext, values, urlRecord);
        HttpContext = httpContext;
        RouteValues = values;
		_logger.LogDebug("GenericRoutingEvent(httpContext={HttpContext},values={Values},urlRecord={UrlRecord}): RouteValues → {RouteValues}", httpContext, values, urlRecord, RouteValues);
		_logger.LogDebug("GenericRoutingEvent(httpContext={HttpContext},values={Values},urlRecord={UrlRecord}): HttpContext → {HttpContext1}", httpContext, values, urlRecord, HttpContext);
        UrlRecord = urlRecord;
		_logger.LogDebug("GenericRoutingEvent(httpContext={HttpContext},values={Values},urlRecord={UrlRecord}): UrlRecord → {UrlRecord1}", httpContext, values, urlRecord, UrlRecord);
		_logger.LogDebug("<<< Exiting GenericRoutingEvent(httpContext={HttpContext},values={Values},urlRecord={UrlRecord})", httpContext, values, urlRecord);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets HTTP context
    /// </summary>
    public HttpContext HttpContext { get; protected set; }

    /// <summary>
    /// Gets route values associated with the current match
    /// </summary>
    public RouteValueDictionary RouteValues { get; protected set; }

    /// <summary>
    /// Gets record found by the URL slug
    /// </summary>
    public UrlRecord UrlRecord { get; protected set; }

    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    public bool StopProcessing { get; set; }

    #endregion
}