using FluentValidation;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ArtificialIntelligenceFullDescriptionValidator : BaseNopValidator<ArtificialIntelligenceFullDescriptionModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ArtificialIntelligenceFullDescriptionValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ArtificialIntelligenceFullDescriptionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.ProductName.Required"));

        RuleFor(x => x.Keywords)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.Keywords.Required"));

        RuleFor(x => x.CustomToneOfVoice)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice.Required"))
            .When(x => x.ToneOfVoiceId == (int)ToneOfVoiceType.Custom);
			_logger.LogDebug("<<< Exiting ArtificialIntelligenceFullDescriptionValidator(localizationService={LocalizationService})", localizationService);
    }
}