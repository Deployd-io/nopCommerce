using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class LoginValidator : BaseNopValidator<LoginModel>
{
    public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
		logger.LogDebug(">>> Entering LoginValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
        if (!customerSettings.UsernamesEnabled)
        {
            //login by email
            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Login.Fields.Email.Required"));
            RuleFor(x => x.Email)
                .IsEmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        }
		logger.LogDebug("<<< Exiting LoginValidator(localizationService={LocalizationService},customerSettings={CustomerSettings})", localizationService, customerSettings);
    }
}