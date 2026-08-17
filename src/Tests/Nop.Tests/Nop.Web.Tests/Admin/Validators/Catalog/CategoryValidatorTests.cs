using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class CategoryValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private CategoryValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new CategoryValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()");
        var model = new CategoryModel
        {
            PageSizeOptions = "1, 2, 3, 5, 2"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PageSizeOptions);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()");
        var model = new CategoryModel
        {
            PageSizeOptions = "1, 2, 3, 5, 9"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()");
        var model = new CategoryModel
        {
            PageSizeOptions = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
        model.PageSizeOptions = string.Empty;
		_logger.LogDebug("ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty(): model.PageSizeOptions → {Arg0}", model.PageSizeOptions);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()");
    }
}