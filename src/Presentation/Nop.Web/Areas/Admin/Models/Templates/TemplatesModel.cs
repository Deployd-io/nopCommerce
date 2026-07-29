using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a templates model
/// </summary>
public partial record TemplatesModel : BaseNopModel
{
    #region Ctor

    public TemplatesModel()
    {
		logger.LogDebug(">>> Entering TemplatesModel()");
        TemplatesCategory = new CategoryTemplateSearchModel();
        TemplatesManufacturer = new ManufacturerTemplateSearchModel();
		logger.LogDebug("TemplatesModel(): TemplatesCategory → {TemplatesCategory}", TemplatesCategory);
		logger.LogDebug("TemplatesModel(): TemplatesManufacturer → {TemplatesManufacturer}", TemplatesManufacturer);
        TemplatesProduct = new ProductTemplateSearchModel();
        TemplatesTopic = new TopicTemplateSearchModel();
		logger.LogDebug("TemplatesModel(): TemplatesProduct → {TemplatesProduct}", TemplatesProduct);
		logger.LogDebug("TemplatesModel(): TemplatesTopic → {TemplatesTopic}", TemplatesTopic);

        AddCategoryTemplate = new CategoryTemplateModel();
        AddManufacturerTemplate = new ManufacturerTemplateModel();
		logger.LogDebug("TemplatesModel(): AddCategoryTemplate → {AddCategoryTemplate}", AddCategoryTemplate);
		logger.LogDebug("TemplatesModel(): AddManufacturerTemplate → {AddManufacturerTemplate}", AddManufacturerTemplate);
        AddProductTemplate = new ProductTemplateModel();
        AddTopicTemplate = new TopicTemplateModel();
		logger.LogDebug("<<< Exiting TemplatesModel()");
		logger.LogDebug("TemplatesModel(): AddProductTemplate → {AddProductTemplate}", AddProductTemplate);
		logger.LogDebug("TemplatesModel(): AddTopicTemplate → {AddTopicTemplate}", AddTopicTemplate);
    }

    #endregion

    #region Properties

    public CategoryTemplateSearchModel TemplatesCategory { get; set; }

    public ManufacturerTemplateSearchModel TemplatesManufacturer { get; set; }

    public ProductTemplateSearchModel TemplatesProduct { get; set; }

    public TopicTemplateSearchModel TemplatesTopic { get; set; }

    public CategoryTemplateModel AddCategoryTemplate { get; set; }

    public ManufacturerTemplateModel AddManufacturerTemplate { get; set; }

    public ProductTemplateModel AddProductTemplate { get; set; }

    public TopicTemplateModel AddTopicTemplate { get; set; }

    #endregion
}