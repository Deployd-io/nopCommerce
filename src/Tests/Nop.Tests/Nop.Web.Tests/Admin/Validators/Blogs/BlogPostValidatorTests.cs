using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Validators.Blogs;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Blogs;

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

    #region Title Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTitleIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsNull()");
        var model = new BlogPostModel
        {
            Title = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsEmpty()");
        var model = new BlogPostModel
        {
            Title = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsWhitespace()");
        var model = new BlogPostModel
        {
            Title = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleIsSpecified()");
        var model = new BlogPostModel
        {
            Title = "Sample Blog Post Title"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleIsSpecified()");
    }

    #endregion

    #region Body Validation Tests

    [Test]
    public void ShouldHaveErrorWhenBodyIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsNull()");
        var model = new BlogPostModel
        {
            Body = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsEmpty()");
        var model = new BlogPostModel
        {
            Body = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenBodyIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenBodyIsWhitespace()");
        var model = new BlogPostModel
        {
            Body = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Body);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenBodyIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenBodyIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenBodyIsSpecified()");
        var model = new BlogPostModel
        {
            Body = "This is the blog post content."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Body);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenBodyIsSpecified()");
    }

    #endregion

    #region Tags Validation Tests

    [Test]
    public void ShouldHaveErrorWhenTagsContainDots()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTagsContainDots()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag.with.dots,tag3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTagsContainDots()");
    }

    [Test]
    public void ShouldHaveErrorWhenTagsContainSingleDot()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTagsContainSingleDot()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag.3"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTagsContainSingleDot()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsDoNotContainDots()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsDoNotContainDots()");
        var model = new BlogPostModel
        {
            Tags = "tag1,tag2,tag3"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsDoNotContainDots()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsNull()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsNull()");
        var model = new BlogPostModel
        {
            Tags = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsEmpty()");
        var model = new BlogPostModel
        {
            Tags = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTagsIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTagsIsWhitespace()");
        var model = new BlogPostModel
        {
            Tags = "   "
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Tags);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTagsIsWhitespace()");
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsNull()");
        var model = new BlogPostModel
        {
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsEmpty()");
        var model = new BlogPostModel
        {
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenSeNameExceedsMaxLength()");
        var model = new BlogPostModel
        {
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSeNameExceedsMaxLength()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsValidLength()");
        var model = new BlogPostModel
        {
            SeName = "sample-blog-post-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsValidLength()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
        var model = new BlogPostModel
        {
            Title = null,
            Body = string.Empty,
            Tags = "tag.with.dots",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1)
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Body);
        result.ShouldHaveValidationErrorFor(x => x.Tags);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
        var model = new BlogPostModel
        {
            Title = "Valid Blog Post Title",
            Body = "This is a valid blog post body content.",
            Tags = "tag1,tag2,tag3",
            SeName = "valid-blog-post-sename"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Body);
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    #endregion
}
