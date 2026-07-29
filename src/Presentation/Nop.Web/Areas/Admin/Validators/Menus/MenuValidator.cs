using FluentValidation;
using Nop.Core.Domain.Menus;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Menus;

public partial class MenuValidator : BaseNopValidator<MenuModel>
{
    #region Ctor

    public MenuValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering MenuValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Fields.Name.Required"));

        SetDatabaseValidationRules<Menu>();
		logger.LogDebug("<<< Exiting MenuValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}
