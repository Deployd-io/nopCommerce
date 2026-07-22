using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Boards;

namespace Nop.Web.Validators.Boards;

public partial class EditForumPostValidator : BaseNopValidator<EditForumPostModel>
{
    public EditForumPostValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering EditForumPostValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
		logger.LogDebug("<<< Exiting EditForumPostValidator(localizationService={LocalizationService})", localizationService);
    }
}
