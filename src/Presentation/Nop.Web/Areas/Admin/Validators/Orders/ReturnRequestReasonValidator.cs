using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class ReturnRequestReasonValidator : BaseNopValidator<ReturnRequestReasonModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ReturnRequestReasonValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ReturnRequestReasonValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Required"));

        SetDatabaseValidationRules<ReturnRequestReason>();
		_logger.LogWarning("<<< Exiting ReturnRequestReasonValidator(localizationService={LocalizationService})", localizationService);
    }
}