using FluentValidation;
using Nop.Core.Domain.Menus;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Menus;

public partial class MenuValidator : BaseNopValidator<MenuModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MenuValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering MenuValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Fields.Name.Required"));

        SetDatabaseValidationRules<Menu>();
		_logger.LogWarning("<<< Exiting MenuValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}
