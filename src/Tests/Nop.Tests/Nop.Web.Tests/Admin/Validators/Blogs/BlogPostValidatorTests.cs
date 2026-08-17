using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Validators.Blogs;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Blogs;

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
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Title Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTitleIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsNull()");
        var model = new BlogPostModel
        {
            Title = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsEmpty()");
        var model = new BlogPostModel
        {
            Title = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsWhitespace()");
        var model = new BlogPostModel
        {
            Title = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleIsSpecified()");
        var model = new BlogPostModel
        {
            Title = "Sample Blog Post Title"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Title);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleIsSpecified()");
    }

    #endregion

    #region Body Validation Tests

    [Test]
    public void ShouldHaveErrorWhenBodyIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsNull()");
        var model = new BlogPostModel
        {
            Body = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsEmpty()");
        var model = new BlogPostModel
        {
            Body = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsWhitespace()");
        var model = new BlogPostModel
        {
            Body = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenBodyIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenBodyIsSpecified()");
        var model = new BlogPostModel
        {
            Body = "This is the blog post content."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Body);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenBodyIsSpecified()");
    }

    #endregion

    #region Tags Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTagsContainDots()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTagsContainDots()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag.with.dots,tag3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTagsContainDots()");
    }

    [Test]
    public void ShouldHaveErrorWhenTagsContainSingleDot()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTagsContainSingleDot()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag.3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTagsContainSingleDot()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsDoNotContainDots()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsDoNotContainDots()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag3"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsDoNotContainDots()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsNull()");
        var model = new BlogPostModel
        {
            Tags = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsEmpty()");
        var model = new BlogPostModel
        {
            Tags = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsWhitespace()");
        var model = new BlogPostModel
        {
            Tags = "   "
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsWhitespace()");
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsNull()");
        var model = new BlogPostModel
        {
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsEmpty()");
        var model = new BlogPostModel
        {
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenSeNameExceedsMaxLength()");
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSeNameExceedsMaxLength()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsValidLength()");
        var model = new BlogPostModel
        {
            SeName = "sample-blog-post-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsValidLength()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
        var model = new BlogPostModel
        {
            Title = null,
            Body = string.Empty,
            Tags = "tag.with.dots",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1)
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Body);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
        var model = new BlogPostModel
        {
            Title = "Valid Blog Post Title",
            Body = "This is a valid blog post body content.",
            Tags = "tag1,tag2,tag3",
            SeName = "valid-blog-post-sename"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Body);
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    #endregion
}
