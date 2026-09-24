using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class LoginValidator : BaseNopValidator<LoginModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
		_logger.LogError(">>> Entering LoginValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
        if (!customerSettings.UsernamesEnabled)
        {
            //login by email
            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Login.Fields.Email.Required"));
            RuleFor(x => x.Email)
                .IsEmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        }
		_logger.LogWarning("<<< Exiting LoginValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
    }
}