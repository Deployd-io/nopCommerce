using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription search model
/// </summary>
public partial record NewsLetterSubscriptionSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NewsLetterSubscriptionSearchModel()
    {
		_logger.LogDebug(">>> Entering NewsLetterSubscriptionSearchModel()");
        AvailableStores = new List<SelectListItem>();
        ActiveList = new List<SelectListItem>();
		_logger.LogDebug("NewsLetterSubscriptionSearchModel(): ActiveList → {ActiveList}", ActiveList);
		_logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
        AvailableSubscriptionTypes = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting NewsLetterSubscriptionSearchModel()");
		_logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableSubscriptionTypes → {AvailableSubscriptionTypes}", AvailableSubscriptionTypes);
    }

    #endregion

    #region Properties

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchEmail")]
    public string SearchEmail { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchStore")]
    public int StoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
    public int ActiveId { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
    public IList<SelectListItem> ActiveList { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.CustomerRoles")]
    public int CustomerRoleId { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SubscriptionTypes")]
    public int SubscriptionTypeId { get; set; }
    public IList<SelectListItem> AvailableSubscriptionTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}