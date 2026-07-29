using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CategoryNavigationModel : BaseNopModel
{
    public CategoryNavigationModel()
    {
		logger.LogDebug(">>> Entering CategoryNavigationModel()");
        Categories = new List<CategorySimpleModel>();
		logger.LogDebug("<<< Exiting CategoryNavigationModel()");
		logger.LogDebug("CategoryNavigationModel(): Categories → {Categories}", Categories);
    }

    public int CurrentCategoryId { get; set; }
    public List<CategorySimpleModel> Categories { get; set; }

    #region Nested classes

    public partial record CategoryLineModel : BaseNopModel
    {
        public int CurrentCategoryId { get; set; }
        public CategorySimpleModel Category { get; set; }
    }

    #endregion
}