using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class FooterMenuViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public FooterMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
		logger.LogDebug(">>> Entering FooterMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
        _menuModelFactory = menuModelFactory;
		logger.LogDebug("<<< Exiting FooterMenuViewComponent(menuModelFactory={MenuModelFactory})", menuModelFactory);
		logger.LogDebug("FooterMenuViewComponent(menuModelFactory={MenuModelFactory}): _menuModelFactory → {_menuModelFactory}", menuModelFactory, _menuModelFactory);
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Footer));
    }

    #endregion
}
