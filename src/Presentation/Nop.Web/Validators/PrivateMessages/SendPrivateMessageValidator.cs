using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Validators.PrivateMessages;

public partial class SendPrivateMessageValidator : BaseNopValidator<SendPrivateMessageModel>
{
    public SendPrivateMessageValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering SendPrivateMessageValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
        RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));
		logger.LogDebug("<<< Exiting SendPrivateMessageValidator(localizationService={LocalizationService})", localizationService);
    }
}
