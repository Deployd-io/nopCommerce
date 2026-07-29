using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a product attribute model
/// </summary>
public partial record ProductAttributeModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the attribute id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the value IDs of the attribute
    /// </summary>
    public IList<int> ValueIds { get; set; }

    #endregion

    #region Ctor

    public ProductAttributeModel()
    {
		logger.LogDebug(">>> Entering ProductAttributeModel()");
        ValueIds = new List<int>();
		logger.LogDebug("<<< Exiting ProductAttributeModel()");
		logger.LogDebug("ProductAttributeModel(): ValueIds → {ValueIds}", ValueIds);
    }

    #endregion
}