using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Boards;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Boards;

public partial class EditForumPostValidator : BaseNopValidator<EditForumPostModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public EditForumPostValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering EditForumPostValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Forum.TextCannotBeEmpty"));
		_logger.LogDebug("<<< Exiting EditForumPostValidator(localizationService={LocalizationService})", localizationService);
    }
}