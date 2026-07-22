using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Menus;

namespace Nop.Web.Components;

public partial class MainMenuViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public MainMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
		logger.LogDebug(">>> Entering MainMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
        _menuModelFactory = menuModelFactory;
		logger.LogDebug("<<< Exiting MainMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
		logger.LogDebug("MainMenuViewComponent(menuModelFactory={MenuModelFactory}): _menuModelFactory → {_menuModelFactory}", menuModelFactory, _menuModelFactory);
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var menus = await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Main);
		logger.LogDebug("InvokeAsync(): menus → {Menus}", menus);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(menus?.FirstOrDefault() ?? new MenuModel());
    }

    #endregion
}
