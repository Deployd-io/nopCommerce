using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription model
/// </summary>
public partial record NewsLetterSubscriptionModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NewsLetterSubscriptionModel()
    {
		_logger.LogError(">>> Entering NewsLetterSubscriptionModel()");
        AvailableNewsLetterSubscriptionTypes = new List<SelectListItem>();
        AvailableNewsLetterSubscriptionStores = new List<SelectListItem>();
		_logger.LogError("NewsLetterSubscriptionModel(): AvailableNewsLetterSubscriptionStores → {AvailableNewsLetterSubscriptionStores}", AvailableNewsLetterSubscriptionStores);
        AvailableNewsLetterSubscriptionLanguages = new List<SelectListItem>();
		_logger.LogError("NewsLetterSubscriptionModel(): AvailableNewsLetterSubscriptionLanguages → {AvailableNewsLetterSubscriptionLanguages}", AvailableNewsLetterSubscriptionLanguages);
    }

    #endregion

    #region Properties

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Email")]
    public string Email { get; set; }

    public string SubscriptionTypeName { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.SubscriptionType")]
    public int SelectedNewsLetterSubscriptionTypeId { get; set; }
    public IList<SelectListItem> AvailableNewsLetterSubscriptionTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Active")]
    public bool Active { get; set; }

    public string StoreName { get; set; }

    public IList<SelectListItem> AvailableNewsLetterSubscriptionStores { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Store")]
    public int SelectedNewsLetterSubscriptionStoreId { get; set; }

    public string LanguageName { get; set; }
    public IList<SelectListItem> AvailableNewsLetterSubscriptionLanguages { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.Language")]
    public int SelectedNewsLetterSubscriptionLanguageId { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscription.Fields.CreatedOn")]
    public string CreatedOn { get; set; }

    #endregion
}