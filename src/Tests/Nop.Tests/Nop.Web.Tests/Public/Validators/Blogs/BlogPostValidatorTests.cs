using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Blogs;
using Nop.Web.Validators.Blogs;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Blogs;

[TestFixture]
public class BlogPostValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private BlogPostValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new BlogPostValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenCommentIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCommentIsNullOrEmpty()");
        var model = new BlogPostModel { AddNewComment = { CommentText = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        model.AddNewComment.CommentText = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCommentIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCommentIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCommentIsSpecified()");
        var model = new BlogPostModel { AddNewComment = { CommentText = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText);
		_logger.LogDebug("ShouldNotHaveErrorWhenCommentIsSpecified(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCommentIsSpecified()");
    }
}