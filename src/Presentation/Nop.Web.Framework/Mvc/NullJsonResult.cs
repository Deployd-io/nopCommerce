using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Framework.Mvc;

/// <summary>
/// Null JSON result
/// </summary>
public partial class NullJsonResult : JsonResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    public NullJsonResult() : base(null)
    {
	_logger.LogDebug(">>> Entering NullJsonResult()");
    }
}