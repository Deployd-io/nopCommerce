using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ManufacturerValidatorTests : BaseNopTest
{
    private ManufacturerValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ManufacturerValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()");
        var model = new ManufacturerModel
        {
            PageSizeOptions = "1, 2, 3, 5, 2"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PageSizeOptions);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPageSizeOptionsHasDuplicateItems()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()");
        var model = new ManufacturerModel
        {
            PageSizeOptions = "1, 2, 3, 5, 9"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPageSizeOptionsHasNotDuplicateItems()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()");
        var model = new ManufacturerModel
        {
            PageSizeOptions = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
        model.PageSizeOptions = string.Empty;
		logger.LogDebug("ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty(): model.PageSizeOptions → {Arg0}", model.PageSizeOptions);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PageSizeOptions);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPageSizeOptionsIsNullOrEmpty()");
    }
}
