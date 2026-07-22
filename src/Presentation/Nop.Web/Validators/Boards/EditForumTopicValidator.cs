using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Boards;

namespace Nop.Web.Validators.Boards;

public partial class EditForumTopicValidator : BaseNopValidator<EditForumTopicModel>
{
    public EditForumTopicValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering EditForumTopicValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TopicSubjectCannotBeEmpty"));
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
		logger.LogDebug("<<< Exiting EditForumTopicValidator(localizationService={LocalizationService})", localizationService);
    }
}
