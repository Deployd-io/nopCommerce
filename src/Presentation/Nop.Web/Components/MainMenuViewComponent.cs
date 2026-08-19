using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Menus;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class MainMenuViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public MainMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
		_logger.LogDebug(">>> Entering MainMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
        _menuModelFactory = menuModelFactory;
		_logger.LogDebug("MainMenuViewComponent(menuModelFactory={MenuModelFactory}): _menuModelFactory → {_menuModelFactory}", menuModelFactory, _menuModelFactory);
		_logger.LogDebug("<<< Exiting MainMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        var menus = await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Main);
		_logger.LogDebug("InvokeAsync(): menus → {Menus}", menus);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(menus?.FirstOrDefault() ?? new MenuModel());
    }

    #endregion
}
