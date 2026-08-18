using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class PasswordRecoveryConfirmValidator : BaseNopValidator<PasswordRecoveryConfirmModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PasswordRecoveryConfirmValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
		_logger.LogDebug(">>> Entering PasswordRecoveryConfirmValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
        RuleFor(x => x.NewPassword).IsPassword(localizationService, customerSettings);
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.ConfirmNewPassword.Required"));
        RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.NewPassword.EnteredPasswordsDoNotMatch"));
		_logger.LogDebug("<<< Exiting PasswordRecoveryConfirmValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
    }
}