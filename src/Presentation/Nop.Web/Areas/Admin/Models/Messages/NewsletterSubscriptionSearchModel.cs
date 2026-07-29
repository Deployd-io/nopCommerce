using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a newsletter subscription search model
/// </summary>
public partial record NewsLetterSubscriptionSearchModel : BaseSearchModel
{
    #region Ctor

    public NewsLetterSubscriptionSearchModel()
    {
		logger.LogDebug(">>> Entering NewsLetterSubscriptionSearchModel()");
        AvailableStores = new List<SelectListItem>();
        ActiveList = new List<SelectListItem>();
		logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("NewsLetterSubscriptionSearchModel(): ActiveList → {ActiveList}", ActiveList);
        AvailableCustomerRoles = new List<SelectListItem>();
        AvailableSubscriptionTypes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting NewsLetterSubscriptionSearchModel()");
		logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
		logger.LogDebug("NewsLetterSubscriptionSearchModel(): AvailableSubscriptionTypes → {AvailableSubscriptionTypes}", AvailableSubscriptionTypes);
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