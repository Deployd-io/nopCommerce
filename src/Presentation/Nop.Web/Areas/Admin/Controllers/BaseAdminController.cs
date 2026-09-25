using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[ValidateVendor]
[SaveSelectedTab]
[NotNullValidationMessage]
public abstract partial class BaseAdminController : BaseController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Creates an object that serializes the specified object to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The created object that serializes the specified data to JSON format for the response.</returns>
    public override JsonResult Json(object data)
    {
		_logger.LogError(">>> Entering Json(data={Data})", data);
        //use IsoDateFormat on writing JSON text to fix issue with dates in grid
        var useIsoDateFormat = EngineContext.Current.Resolve<AdminAreaSettings>()?.UseIsoDateFormatInJsonResult ?? false;
		_logger.LogError("Json(data={Data}): useIsoDateFormat → {UseIsoDateFormat}", data, useIsoDateFormat);
        var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcNewtonsoftJsonOptions>>()?.Value?.SerializerSettings
                                 ?? new JsonSerializerSettings();

        if (!useIsoDateFormat)
            return base.Json(data, serializerSettings);

        serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

		_logger.LogWarning("<<< Exiting Json(data={Data})", data);
        return base.Json(data, serializerSettings);
    }

}