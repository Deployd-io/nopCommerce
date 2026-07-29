using FluentValidation;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Polls;

public partial class PollValidator : BaseNopValidator<PollModel>
{
    public PollValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering PollValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Fields.Name.Required"));

        SetDatabaseValidationRules<Poll>();
		logger.LogDebug("<<< Exiting PollValidator(localizationService={LocalizationService})", localizationService);
    }
}