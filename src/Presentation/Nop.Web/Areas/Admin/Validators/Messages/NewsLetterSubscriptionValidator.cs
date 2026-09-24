using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class NewsLetterSubscriptionValidator : BaseNopValidator<NewsLetterSubscriptionModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NewsLetterSubscriptionValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering NewsLetterSubscriptionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

        SetDatabaseValidationRules<NewsLetterSubscription>();
		_logger.LogWarning("<<< Exiting NewsLetterSubscriptionValidator(localizationService={LocalizationService})", localizationService);
    }
}