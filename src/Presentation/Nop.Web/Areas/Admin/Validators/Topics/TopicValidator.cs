using FluentValidation;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Topics;

public partial class TopicValidator : BaseNopValidator<TopicModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TopicValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering TopicValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.SeName)
            .Length(0, NopSeoDefaults.ForumTopicLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.ForumTopicLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => x.IsPasswordProtected)
            .WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));

        RuleFor(x => x.AvailableEndDateTimeUtc)
            .GreaterThanOrEqualTo(x => x.AvailableStartDateTimeUtc)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Fields.AvailableEndDateTime.GreaterThanOrEqualToStartDate"));
        
        SetDatabaseValidationRules<Topic>();
		_logger.LogWarning("<<< Exiting TopicValidator(localizationService={LocalizationService})", localizationService);
    }
}