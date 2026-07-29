using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ExternalMethodsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;

    #endregion

    #region Ctor

    public ExternalMethodsViewComponent(IExternalAuthenticationModelFactory externalAuthenticationModelFactory)
    {
		logger.LogDebug(">>> Entering ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory})", externalAuthenticationModelFactory);
        _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
		logger.LogDebug("<<< Exiting ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory})", externalAuthenticationModelFactory);
		logger.LogDebug("ExternalMethodsViewComponent(externalAuthenticationModelFactory={ExternalAuthenticationModelFactory}): _externalAuthenticationModelFactory → {_externalAuthenticationModelFactory}", externalAuthenticationModelFactory, _externalAuthenticationModelFactory);
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _externalAuthenticationModelFactory.PrepareExternalMethodsModelAsync();

		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }

    #endregion
}