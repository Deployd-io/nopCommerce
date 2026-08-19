using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription type model
/// </summary>
public partial record NewsLetterSubscriptionTypeModel : BaseNopEntityModel, ILocalizedModel<NewsLetterSubscriptionTypeLocalizedModel>, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NewsLetterSubscriptionTypeModel()
    {
		_logger.LogDebug(">>> Entering NewsLetterSubscriptionTypeModel()");
        Locales = new List<NewsLetterSubscriptionTypeLocalizedModel>();
        SelectedStoreIds = new List<int>();
		_logger.LogDebug("NewsLetterSubscriptionTypeModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
		_logger.LogDebug("NewsLetterSubscriptionTypeModel(): Locales → {Locales}", Locales);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("NewsLetterSubscriptionTypeModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("<<< Exiting NewsLetterSubscriptionTypeModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.TickedByDefault")]
    public bool TickedByDefault { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<NewsLetterSubscriptionTypeLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptionType.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}
