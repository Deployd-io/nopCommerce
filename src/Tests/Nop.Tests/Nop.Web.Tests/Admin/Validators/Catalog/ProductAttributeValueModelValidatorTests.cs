using FluentValidation.TestHelper;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductAttributeValueModelValidatorTests : BaseNopTest
{
    private ProductAttributeValueModelValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductAttributeValueModelValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductAttributeValueModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ProductAttributeValueModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ProductAttributeValueModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ProductAttributeValueModel
        {
            Name = "Red"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region Quantity Validation Tests

    [Test]
    public void ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenQuantityIsNegativeAndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIs1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 5
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsGreaterThan1AndAttributeValueTypeIsAssociatedToProductAndCustomerDoesNotEnterQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsAssociatedToProductButCustomerEntersQty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Quantity);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenQuantityIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
    }

    #endregion

    #region AssociatedProductId Validation Tests

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAssociatedProductIdIsNegativeAndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 1
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIs1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            AssociatedProductId = 10
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIsGreaterThan1AndAttributeValueTypeIsAssociatedToProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Valid Name",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            AssociatedProductId = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAssociatedProductIdIsLessThan1AndAttributeValueTypeIsNotAssociatedToProduct()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = null, // Invalid
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 0, // Invalid
            AssociatedProductId = 0 // Invalid
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
        result.ShouldHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalidForAssociatedProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Bundle Product",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = false,
            Quantity = 2,
            AssociatedProductId = 5
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidForAssociatedProduct()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()");
        var model = new ProductAttributeValueModel
        {
            Name = "Large Size",
            AttributeValueTypeId = (int)AttributeValueType.Simple,
            CustomerEntersQty = false,
            Quantity = 0, // Not validated for Simple type
            AssociatedProductId = 0 // Not validated for Simple type
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidForSimpleAttribute()");
    }

    [Test]
    public void ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()
    {
		logger.LogDebug(">>> Entering ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()");
        var model = new ProductAttributeValueModel
        {
            Name = "Custom Bundle",
            AttributeValueTypeId = (int)AttributeValueType.AssociatedToProduct,
            CustomerEntersQty = true, // Customer enters quantity
            Quantity = 0, // Not validated when customer enters qty
            AssociatedProductId = 3
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.AssociatedProductId);
		logger.LogDebug("<<< Exiting ShouldPassValidationWhenCustomerEntersQtyForAssociatedProduct()");
    }

    #endregion
}
