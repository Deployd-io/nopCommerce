using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll model
/// </summary>
public partial record PollModel : BaseNopEntityModel, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PollModel()
    {
		_logger.LogError(">>> Entering PollModel()");
        AvailableLanguages = new List<SelectListItem>();
        AvailableStores = new List<SelectListItem>();
		_logger.LogError("PollModel(): AvailableStores → {AvailableStores}", AvailableStores);
        SelectedStoreIds = new List<int>();
		_logger.LogError("PollModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        PollAnswerSearchModel = new PollAnswerSearchModel();
		_logger.LogError("PollModel(): PollAnswerSearchModel → {PollAnswerSearchModel}", PollAnswerSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
    public int LanguageId { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
    public string LanguageName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.SystemKeyword")]
    public string SystemKeyword { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.ShowOnHomepage")]
    public bool ShowOnHomepage { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.AllowGuestsToVote")]
    public bool AllowGuestsToVote { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.StartDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? StartDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.EndDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? EndDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public PollAnswerSearchModel PollAnswerSearchModel { get; set; }

    #endregion
}