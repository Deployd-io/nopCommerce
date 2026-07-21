using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class NewsLetterSubscriptionValidator : BaseNopValidator<NewsLetterSubscriptionModel>
{
    public NewsLetterSubscriptionValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering NewsLetterSubscriptionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

        SetDatabaseValidationRules<NewsLetterSubscription>();
		logger.LogDebug("<<< Exiting NewsLetterSubscriptionValidator(localizationService={LocalizationService})", localizationService);
    }
}
