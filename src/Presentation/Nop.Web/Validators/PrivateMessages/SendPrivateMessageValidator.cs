using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.PrivateMessages;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.PrivateMessages;

public partial class SendPrivateMessageValidator : BaseNopValidator<SendPrivateMessageModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SendPrivateMessageValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering SendPrivateMessageValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
        RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));
		_logger.LogDebug("<<< Exiting SendPrivateMessageValidator(localizationService={LocalizationService})", localizationService);
    }
}