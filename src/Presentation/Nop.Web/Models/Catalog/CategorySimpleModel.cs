using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CategorySimpleModel : BaseNopEntityModel
{
    public CategorySimpleModel()
    {
		logger.LogDebug(">>> Entering CategorySimpleModel()");
        SubCategories = new List<CategorySimpleModel>();
		logger.LogDebug("<<< Exiting CategorySimpleModel()");
		logger.LogDebug("CategorySimpleModel(): SubCategories → {SubCategories}", SubCategories);
    }

    public string Name { get; set; }

    public string SeName { get; set; }

    public int? NumberOfProducts { get; set; }

    public List<CategorySimpleModel> SubCategories { get; set; }

    public string Route { get; set; }
}