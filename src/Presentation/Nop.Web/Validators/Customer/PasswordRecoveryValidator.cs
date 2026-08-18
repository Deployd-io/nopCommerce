using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class PasswordRecoveryValidator : BaseNopValidator<PasswordRecoveryModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PasswordRecoveryValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering PasswordRecoveryValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
			_logger.LogDebug("<<< Exiting PasswordRecoveryValidator(localizationService={LocalizationService})", localizationService);
    }
}