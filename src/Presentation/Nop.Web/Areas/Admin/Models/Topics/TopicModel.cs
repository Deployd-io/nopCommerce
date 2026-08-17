using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic model
/// </summary>
public partial record TopicModel : BaseNopEntityModel, IAclSupportedModel, ILocalizedModel<TopicLocalizedModel>, IStoreMappingSupportedModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TopicModel()
    {
		_logger.LogDebug(">>> Entering TopicModel()");
        AvailableTopicTemplates = new List<SelectListItem>();
        Locales = new List<TopicLocalizedModel>();
		_logger.LogDebug("TopicModel(): Locales → {Locales}", Locales);
		_logger.LogDebug("TopicModel(): AvailableTopicTemplates → {AvailableTopicTemplates}", AvailableTopicTemplates);

        SelectedCustomerRoleIds = new List<int>();
		_logger.LogDebug("TopicModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("TopicModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);

        SelectedStoreIds = new List<int>();
		_logger.LogDebug("TopicModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting TopicModel()");
		_logger.LogDebug("TopicModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
    public string SystemName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
    public bool IncludeInSitemap { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed")]
    public bool AccessibleWhenStoreClosed { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
    public bool IsPasswordProtected { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
    public string Password { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
    public int TopicTemplateId { get; set; }

    public IList<SelectListItem> AvailableTopicTemplates { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
    public string SeName { get; set; }

    public IList<TopicLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    //ACL (customer roles)
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public string TopicName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AvailableStartDateTime")]
    [UIHint("DateTimeNullable")]
    public DateTime? AvailableStartDateTimeUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AvailableEndDateTime")]
    [UIHint("DateTimeNullable")]
    public DateTime? AvailableEndDateTimeUtc { get; set; }

    #endregion
}

public partial record TopicLocalizedModel : ILocalizedLocaleModel, IMetaTagsSupportedModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
    public string SeName { get; set; }
}