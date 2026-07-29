using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a specification attribute model
/// </summary>
public partial record ProductSpecificationAttributeModel : BaseNopEntityModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the values
    /// </summary>
    public IList<ProductSpecificationAttributeValueModel> Values { get; set; }

    #endregion

    #region Ctor

    public ProductSpecificationAttributeModel()
    {
		logger.LogDebug(">>> Entering ProductSpecificationAttributeModel()");
        Values = new List<ProductSpecificationAttributeValueModel>();
		logger.LogDebug("<<< Exiting ProductSpecificationAttributeModel()");
		logger.LogDebug("ProductSpecificationAttributeModel(): Values → {Values}", Values);
    }

    #endregion
}