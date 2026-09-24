using FluentValidation;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Stores;

public partial class StoreValidator : BaseNopValidator<StoreModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public StoreValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering StoreValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Name.Required"));
        RuleFor(x => x.Url).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Url.Required"));

        SetDatabaseValidationRules<Store>();
		_logger.LogWarning("<<< Exiting StoreValidator(localizationService={LocalizationService})", localizationService);
    }
}