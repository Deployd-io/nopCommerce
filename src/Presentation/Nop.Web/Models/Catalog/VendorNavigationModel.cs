using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record VendorNavigationModel : BaseNopModel
{
    public VendorNavigationModel()
    {
		logger.LogDebug(">>> Entering VendorNavigationModel()");
        Vendors = new List<VendorBriefInfoModel>();
		logger.LogDebug("<<< Exiting VendorNavigationModel()");
		logger.LogDebug("VendorNavigationModel(): Vendors → {Vendors}", Vendors);
    }

    public IList<VendorBriefInfoModel> Vendors { get; set; }

    public int TotalVendors { get; set; }
}

public partial record VendorBriefInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }
}