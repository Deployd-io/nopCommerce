using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Blogs;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Blogs;

public partial class BlogPostValidator : BaseNopValidator<BlogPostModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering BlogPostValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
		_logger.LogWarning("<<< Exiting BlogPostValidator(localizationService={LocalizationService})", localizationService);
    }
}