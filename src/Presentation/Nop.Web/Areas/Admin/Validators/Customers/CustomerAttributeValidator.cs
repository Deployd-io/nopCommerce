using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerAttributeValidator : BaseNopValidator<CustomerAttributeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAttributeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CustomerAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Fields.Name.Required"));

        SetDatabaseValidationRules<CustomerAttribute>();
		_logger.LogWarning("<<< Exiting CustomerAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}