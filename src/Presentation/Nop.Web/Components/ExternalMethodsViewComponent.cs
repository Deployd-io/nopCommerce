using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ExternalMethodsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;

    #endregion

    #region Ctor

    public ExternalMethodsViewComponent(IExternalAuthenticationModelFactory externalAuthenticationModelFactory)
    {
		_logger.LogError(">>> Entering ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory})", externalAuthenticationModelFactory);
        _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
		_logger.LogDebug("ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory}): _externalAuthenticationModelFactory → {_externalAuthenticationModelFactory}", externalAuthenticationModelFactory, _externalAuthenticationModelFactory);
    _logger.LogWarning("<<< Exiting ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory})", externalAuthenticationModelFactory);
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _externalAuthenticationModelFactory.PrepareExternalMethodsModelAsync();

		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }

    #endregion
}