using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class CheckoutAttributeValidator : BaseNopValidator<CheckoutAttributeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutAttributeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CheckoutAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name.Required"));

        SetDatabaseValidationRules<CheckoutAttribute>();
		_logger.LogWarning("<<< Exiting CheckoutAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}