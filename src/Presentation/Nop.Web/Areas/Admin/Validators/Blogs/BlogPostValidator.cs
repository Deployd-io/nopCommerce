using FluentValidation;
using Nop.Core.Domain.Blogs;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Blogs;

public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering BlogPostValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required"));

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required"));

        //blog tags should not contain dots
        //current implementation does not support it because it can be handled as file extension
        RuleFor(x => x.Tags)
            .Must(x => x == null || !x.Contains('.'))
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots"));

        RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength);

        SetDatabaseValidationRules<BlogPost>();
		_logger.LogDebug("<<< Exiting BlogPostValidator(localizationService={LocalizationService})", localizationService);
    }
}