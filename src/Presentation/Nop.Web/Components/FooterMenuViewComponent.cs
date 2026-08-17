using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class FooterMenuViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public FooterMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
		_logger.LogDebug(">>> Entering FooterMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
        _menuModelFactory = menuModelFactory;
		_logger.LogDebug("<<< Exiting FooterMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
		_logger.LogDebug("FooterMenuViewComponent(menuModelFactory={MenuModelFactory}): _menuModelFactory → {_menuModelFactory}", menuModelFactory, _menuModelFactory);
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Footer));
    }

    #endregion
}
