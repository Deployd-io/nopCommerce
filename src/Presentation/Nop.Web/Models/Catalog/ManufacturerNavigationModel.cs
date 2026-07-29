using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record ManufacturerNavigationModel : BaseNopModel
{
    public ManufacturerNavigationModel()
    {
		logger.LogDebug(">>> Entering ManufacturerNavigationModel()");
        Manufacturers = new List<ManufacturerBriefInfoModel>();
		logger.LogDebug("<<< Exiting ManufacturerNavigationModel()");
		logger.LogDebug("ManufacturerNavigationModel(): Manufacturers → {Manufacturers}", Manufacturers);
    }

    public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

    public int TotalManufacturers { get; set; }
}

public partial record ManufacturerBriefInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }

    public bool IsActive { get; set; }
}