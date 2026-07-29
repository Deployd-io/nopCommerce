using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValueValidator : BaseNopValidator<AddressAttributeValueModel>
{
    public AddressAttributeValueValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering AddressAttributeValueValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));

        SetDatabaseValidationRules<AddressAttributeValue>();
		logger.LogDebug("<<< Exiting AddressAttributeValueValidator(localizationService={LocalizationService})", localizationService);
    }
}