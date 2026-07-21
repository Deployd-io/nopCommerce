using FluentValidation;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Tasks;

public partial class ScheduleTaskValidator : BaseNopValidator<ScheduleTaskModel>
{
    public ScheduleTaskValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering ScheduleTaskValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Name.Required"));
        RuleFor(x => x.Seconds).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.ScheduleTasks.Seconds.Positive"));

        SetDatabaseValidationRules<ScheduleTask>();
		logger.LogDebug("<<< Exiting ScheduleTaskValidator(localizationService={LocalizationService})", localizationService);
    }
}
