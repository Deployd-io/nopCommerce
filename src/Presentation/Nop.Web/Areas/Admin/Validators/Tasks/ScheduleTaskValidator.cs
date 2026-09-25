using FluentValidation;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Tasks;

public partial class ScheduleTaskValidator : BaseNopValidator<ScheduleTaskModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ScheduleTaskValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ScheduleTaskValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Name.Required"));
        RuleFor(x => x.Seconds).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Seconds.Positive"));

        SetDatabaseValidationRules<ScheduleTask>();
		_logger.LogWarning("<<< Exiting ScheduleTaskValidator(localizationService={LocalizationService})", localizationService);
    }
}