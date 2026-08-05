using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValidator : BaseNopValidator<AddressAttributeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AddressAttributeValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering AddressAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Fields.Name.Required"));

        SetDatabaseValidationRules<AddressAttribute>();
		_logger.LogDebug("<<< Exiting AddressAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}