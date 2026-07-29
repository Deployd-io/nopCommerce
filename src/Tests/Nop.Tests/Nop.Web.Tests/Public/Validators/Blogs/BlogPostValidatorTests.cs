using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Blogs;
using Nop.Web.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Blogs;

[TestFixture]
public class BlogPostValidatorTests : BaseNopTest
{
    private BlogPostValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new BlogPostValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenCommentIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCommentIsNullOrEmpty()");
        var model = new BlogPostModel { AddNewComment = { CommentText = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        model.AddNewComment.CommentText = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCommentIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCommentIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCommentIsSpecified()");
        var model = new BlogPostModel { AddNewComment = { CommentText = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCommentIsSpecified()");
		logger.LogDebug("ShouldNotHaveErrorWhenCommentIsSpecified(): model → {Model}", model);
    }
}