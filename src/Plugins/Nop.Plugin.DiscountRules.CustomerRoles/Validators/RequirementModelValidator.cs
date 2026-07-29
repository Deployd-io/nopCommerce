using FluentValidation;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Validators;

/// <summary>
/// Represents an <see cref="RequirementModel"/> validator.
/// </summary>
public class RequirementModelValidator : BaseNopValidator<RequirementModel>
{
    public RequirementModelValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering RequirementModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.DiscountId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required"));
        RuleFor(model => model.CustomerRoleId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRoleId.Required"));
			logger.LogDebug("<<< Exiting RequirementModelValidator(localizationService={LocalizationService})", localizationService);
    }
}