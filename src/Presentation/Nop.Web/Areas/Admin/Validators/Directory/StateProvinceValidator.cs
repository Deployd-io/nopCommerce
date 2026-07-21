using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class StateProvinceValidator : BaseNopValidator<StateProvinceModel>
{
    public StateProvinceValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering StateProvinceValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.States.Fields.Name.Required"));

        SetDatabaseValidationRules<StateProvince>();
		logger.LogDebug("<<< Exiting StateProvinceValidator(localizationService={LocalizationService})", localizationService);
    }
}
