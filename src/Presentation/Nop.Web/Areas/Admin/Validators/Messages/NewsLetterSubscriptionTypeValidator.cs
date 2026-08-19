using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class NewsLetterSubscriptionTypeValidator : BaseNopValidator<NewsLetterSubscriptionTypeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NewsLetterSubscriptionTypeValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering NewsLetterSubscriptionTypeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Fields.Name.Required"));

        SetDatabaseValidationRules<NewsLetterSubscriptionType>();
		_logger.LogDebug("<<< Exiting NewsLetterSubscriptionTypeValidator(localizationService={LocalizationService})", localizationService);
    }
}