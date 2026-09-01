using FluentValidation;
using Nop.Core.Domain.Polls;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Polls;

public partial class PollValidator : BaseNopValidator<PollModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PollValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering PollValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Polls.Fields.Name.Required"));

        SetDatabaseValidationRules<Poll>();
		_logger.LogDebug("<<< Exiting PollValidator(localizationService={LocalizationService})", localizationService);
    }
}