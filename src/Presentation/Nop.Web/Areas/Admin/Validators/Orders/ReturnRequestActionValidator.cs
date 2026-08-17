using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class ReturnRequestActionValidator : BaseNopValidator<ReturnRequestActionModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ReturnRequestActionValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ReturnRequestActionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Name.Required"));

        SetDatabaseValidationRules<ReturnRequestAction>();
		_logger.LogDebug("<<< Exiting ReturnRequestActionValidator(localizationService={LocalizationService})", localizationService);
    }
}