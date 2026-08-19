using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValueValidator : BaseNopValidator<AddressAttributeValueModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AddressAttributeValueValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering AddressAttributeValueValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));

        SetDatabaseValidationRules<AddressAttributeValue>();
		_logger.LogDebug("<<< Exiting AddressAttributeValueValidator(localizationService={LocalizationService})", localizationService);
    }
}