using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a templates model
/// </summary>
public partial record TemplatesModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TemplatesModel()
    {
		_logger.LogError(">>> Entering TemplatesModel()");
        TemplatesCategory = new CategoryTemplateSearchModel();
        TemplatesManufacturer = new ManufacturerTemplateSearchModel();
		_logger.LogError("TemplatesModel(): TemplatesManufacturer → {TemplatesManufacturer}", TemplatesManufacturer);
		_logger.LogDebug("TemplatesModel(): TemplatesCategory → {TemplatesCategory}", TemplatesCategory);
        TemplatesProduct = new ProductTemplateSearchModel();
		_logger.LogError("TemplatesModel(): TemplatesProduct → {TemplatesProduct}", TemplatesProduct);
        TemplatesTopic = new TopicTemplateSearchModel();
		_logger.LogError("TemplatesModel(): TemplatesTopic → {TemplatesTopic}", TemplatesTopic);

        AddCategoryTemplate = new CategoryTemplateModel();
		_logger.LogError("TemplatesModel(): AddCategoryTemplate → {AddCategoryTemplate}", AddCategoryTemplate);
        AddManufacturerTemplate = new ManufacturerTemplateModel();
		_logger.LogError("TemplatesModel(): AddManufacturerTemplate → {AddManufacturerTemplate}", AddManufacturerTemplate);
        AddProductTemplate = new ProductTemplateModel();
		_logger.LogError("TemplatesModel(): AddProductTemplate → {AddProductTemplate}", AddProductTemplate);
        AddTopicTemplate = new TopicTemplateModel();
		_logger.LogError("TemplatesModel(): AddTopicTemplate → {AddTopicTemplate}", AddTopicTemplate);
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