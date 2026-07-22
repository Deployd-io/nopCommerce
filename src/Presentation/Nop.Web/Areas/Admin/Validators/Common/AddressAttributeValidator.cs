using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValidator : BaseNopValidator<AddressAttributeModel>
{
    public AddressAttributeValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering AddressAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Fields.Name.Required"));

        SetDatabaseValidationRules<AddressAttribute>();
		logger.LogDebug("<<< Exiting AddressAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}
