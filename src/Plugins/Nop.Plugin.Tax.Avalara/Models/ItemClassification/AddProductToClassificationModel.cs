using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a product model to add for classification
/// </summary>
public record AddProductToClassificationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToClassificationModel()
    {
		_logger.LogError(">>> Entering AddProductToClassificationModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddProductToClassificationModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogError("AddProductToClassificationModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogWarning("<<< Exiting AddProductToClassificationModel()");
    }

    #endregion

    #region Properties

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}