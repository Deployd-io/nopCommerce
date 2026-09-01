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
		_logger.LogDebug(">>> Entering TemplatesModel()");
        TemplatesCategory = new CategoryTemplateSearchModel();
        TemplatesManufacturer = new ManufacturerTemplateSearchModel();
        TemplatesProduct = new ProductTemplateSearchModel();
		_logger.LogDebug("TemplatesModel(): TemplatesProduct → {TemplatesProduct}", TemplatesProduct);
		_logger.LogDebug("TemplatesModel(): TemplatesCategory → {TemplatesCategory}", TemplatesCategory);
		_logger.LogDebug("TemplatesModel(): TemplatesManufacturer → {TemplatesManufacturer}", TemplatesManufacturer);
        TemplatesTopic = new TopicTemplateSearchModel();
		_logger.LogDebug("TemplatesModel(): TemplatesTopic → {TemplatesTopic}", TemplatesTopic);

        AddCategoryTemplate = new CategoryTemplateModel();
		_logger.LogDebug("TemplatesModel(): AddCategoryTemplate → {AddCategoryTemplate}", AddCategoryTemplate);
        AddManufacturerTemplate = new ManufacturerTemplateModel();
		_logger.LogDebug("TemplatesModel(): AddManufacturerTemplate → {AddManufacturerTemplate}", AddManufacturerTemplate);
        AddProductTemplate = new ProductTemplateModel();
		_logger.LogDebug("TemplatesModel(): AddProductTemplate → {AddProductTemplate}", AddProductTemplate);
        AddTopicTemplate = new TopicTemplateModel();
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