using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

public partial record ProductAttributeConditionModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductAttributeConditionModel()
    {
		_logger.LogDebug(">>> Entering ProductAttributeConditionModel()");
        ProductAttributes = new List<ProductAttributeModel>();
		_logger.LogDebug("ProductAttributeConditionModel(): ProductAttributes → {ProductAttributes}", ProductAttributes);
    }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Condition.EnableCondition")]
    public bool EnableCondition { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Condition.Attributes")]
    public int SelectedProductAttributeId { get; set; }
    public IList<ProductAttributeModel> ProductAttributes { get; set; }

    public int ProductAttributeMappingId { get; set; }

    #region Nested classes

    public partial record ProductAttributeModel : BaseNopEntityModel
    {
        public ProductAttributeModel()
        {
			_logger.LogDebug(">>> Entering ProductAttributeModel()");
            Values = new List<ProductAttributeValueModel>();
			_logger.LogDebug("ProductAttributeModel(): Values → {Values}", Values);
        }

        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public string TextPrompt { get; set; }

        public bool IsRequired { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<ProductAttributeValueModel> Values { get; set; }
    }

    public partial record ProductAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }

    #endregion
}