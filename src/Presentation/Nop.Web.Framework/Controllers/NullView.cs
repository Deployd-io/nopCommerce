using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Controllers;

public partial class NullView : IView
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static readonly NullView Instance = new();

    public string Path => string.Empty;

    public virtual Task RenderAsync(ViewContext context)
    {
		_logger.LogError(">>> Entering RenderAsync(context={Context})", context);
        ArgumentNullException.ThrowIfNull(context);

		_logger.LogWarning("<<< Exiting RenderAsync(context={Context})", context);
        return Task.CompletedTask;
    }
}