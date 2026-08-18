using FluentValidation.TestHelper;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductAttributeValueModelValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductAttributeValueModelValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductAttributeValueModelValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductAttributeValueModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ProductAttributeValueModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ProductAttributeValueModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ProductAttributeValueModel
        {
            Name = "Red"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region Quantity Validation Tests

    [Test]
    public void ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 5
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
    }

    #endregion

    #region AssociatedProductId Validation Tests

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 10
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()
    {
		_logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = null, // Invalid
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0, // Invalid
            AssociatedProductId = 0 // Invalid
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
        result.ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Bundle Product",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 2,
            AssociatedProductId = 5
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()");
        var model = new ProductAttributeValueModel
        {
            Name = "Large Size",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0, // Not validated for Simple type
            AssociatedProductId = 0 // Not validated for Simple type
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()");
    }

    [Test]
    public void ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()
    {
		_logger.LogDebug(">>> Entering ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Custom Bundle",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true, // Customer enters quantity
            Quantity = 0, // Not validated when customer enters qty
            AssociatedProductId = 3
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		_logger.LogDebug("<<< Exiting ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()");
    }

    #endregion
}
