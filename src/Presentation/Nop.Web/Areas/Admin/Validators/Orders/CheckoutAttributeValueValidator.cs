using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class CheckoutAttributeValueValidator : BaseNopValidator<CheckoutAttributeValueModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutAttributeValueValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering CheckoutAttributeValueValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.Name.Required"));

        SetDatabaseValidationRules<CheckoutAttributeValue>();
		_logger.LogDebug("<<< Exiting CheckoutAttributeValueValidator(localizationService={LocalizationService})", localizationService);
    }
}