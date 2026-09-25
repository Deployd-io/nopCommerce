using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class StateProvinceValidator : BaseNopValidator<StateProvinceModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public StateProvinceValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering StateProvinceValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.States.Fields.Name.Required"));

        SetDatabaseValidationRules<StateProvince>();
		_logger.LogWarning("<<< Exiting StateProvinceValidator(localizationService={LocalizationService})", localizationService);
    }
}