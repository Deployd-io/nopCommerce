using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerAttributeValueValidator : BaseNopValidator<CustomerAttributeValueModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAttributeValueValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CustomerAttributeValueValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required"));

        SetDatabaseValidationRules<CustomerAttributeValue>();
		_logger.LogWarning("<<< Exiting CustomerAttributeValueValidator(localizationService={LocalizationService})", localizationService);
    }
}