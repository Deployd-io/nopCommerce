using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Boards;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Boards;

public partial class EditForumTopicValidator : BaseNopValidator<EditForumTopicModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public EditForumTopicValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering EditForumTopicValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TopicSubjectCannotBeEmpty"));
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
		_logger.LogDebug("<<< Exiting EditForumTopicValidator(localizationService={LocalizationService})", localizationService);
    }
}