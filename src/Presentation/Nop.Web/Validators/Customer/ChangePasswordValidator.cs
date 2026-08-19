using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class ChangePasswordValidator : BaseNopValidator<ChangePasswordModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ChangePasswordValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
		_logger.LogDebug(">>> Entering ChangePasswordValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
        RuleFor(x => x.OldPassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.ChangePassword.Fields.OldPassword.Required"));
        RuleFor(x => x.NewPassword).IsPassword(localizationService, customerSettings);
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.ChangePassword.Fields.ConfirmNewPassword.Required"));
        RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessageAwait(localizationService.GetResourceAsync("Account.ChangePassword.Fields.NewPassword.EnteredPasswordsDoNotMatch"));
		_logger.LogDebug("<<< Exiting ChangePasswordValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
    }
}