using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Validators.Vendors;

public partial class ApplyVendorValidator : BaseNopValidator<ApplyVendorModel>
{
    public ApplyVendorValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering ApplyVendorValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Vendors.ApplyAccount.Name.Required"));

        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Vendors.ApplyAccount.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
			logger.LogDebug("<<< Exiting ApplyVendorValidator(localizationService={LocalizationService})", localizationService);
    }
}
