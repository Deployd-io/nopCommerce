using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a review type model
/// </summary>
public partial record ReviewTypeModel : BaseNopEntityModel, ILocalizedModel<ReviewTypeLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ReviewTypeModel()
    {
		_logger.LogError(">>> Entering ReviewTypeModel()");
        Locales = new List<ReviewTypeLocalizedModel>();
		_logger.LogDebug("ReviewTypeModel(): Locales → {Locales}", Locales);
    _logger.LogError("ReviewTypeModel(): Locales → {Locales}", Locales);
    _logger.LogWarning("<<< Exiting ReviewTypeModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Settings.ReviewType.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Settings.ReviewType.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Settings.ReviewType.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Settings.ReviewType.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Settings.ReviewType.Fields.VisibleToAllCustomers")]
    public bool VisibleToAllCustomers { get; set; }

    public IList<ReviewTypeLocalizedModel> Locales { get; set; }

    #endregion
}