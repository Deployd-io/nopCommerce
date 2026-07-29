using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the manufacturer
/// </summary>
public partial record AddProductToManufacturerModel : BaseNopModel
{
    #region Ctor

    public AddProductToManufacturerModel()
    {
		logger.LogDebug(">>> Entering AddProductToManufacturerModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddProductToManufacturerModel()");
		logger.LogDebug("AddProductToManufacturerModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ManufacturerId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}