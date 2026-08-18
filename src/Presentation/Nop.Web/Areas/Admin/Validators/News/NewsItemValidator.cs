using FluentValidation;
using Nop.Core.Domain.News;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.News;

public partial class NewsItemValidator : BaseNopValidator<NewsItemModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NewsItemValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering NewsItemValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

        RuleFor(x => x.Short).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

        RuleFor(x => x.Full).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));

        RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength);

        SetDatabaseValidationRules<NewsItem>();
		_logger.LogDebug("<<< Exiting NewsItemValidator(localizationService={LocalizationService})", localizationService);
    }
}