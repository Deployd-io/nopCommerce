using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a message template search model
/// </summary>
public partial record MessageTemplateSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MessageTemplateSearchModel()
    {
		_logger.LogError(">>> Entering MessageTemplateSearchModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableActiveOptions = new List<SelectListItem>();
		_logger.LogError("MessageTemplateSearchModel(): AvailableActiveOptions → {AvailableActiveOptions}", AvailableActiveOptions);
        AvailableEmailAccounts = new List<SelectListItem>();
		_logger.LogError("MessageTemplateSearchModel(): AvailableEmailAccounts → {AvailableEmailAccounts}", AvailableEmailAccounts);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchKeywords")]
    public string SearchKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchStore")]
    public int SearchStoreId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.IsActive")]
    public int IsActiveId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchEmailAccount")]
    public int EmailAccountId { get; set; }
  
    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableActiveOptions { get; set; }

    public IList<SelectListItem> AvailableEmailAccounts { get; set; }

    public bool HideStoresList { get; set; }
    public bool HideEmailAccount { get; set; }

    #endregion
}