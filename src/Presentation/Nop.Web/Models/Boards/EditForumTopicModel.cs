using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Boards;

public partial record EditForumTopicModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public EditForumTopicModel()
    {
		_logger.LogDebug(">>> Entering EditForumTopicModel()");
        TopicPriorities = new List<SelectListItem>();
		_logger.LogDebug("EditForumTopicModel(): TopicPriorities → {TopicPriorities}", TopicPriorities);
    }

    #endregion

    #region Properties

    public bool IsEdit { get; set; }

    public int Id { get; set; }

    public int ForumId { get; set; }

    public string ForumName { get; set; }

    public string ForumSeName { get; set; }

    public int TopicTypeId { get; set; }

    public EditorType ForumEditor { get; set; }

    public string Subject { get; set; }

    public string Text { get; set; }

    public bool IsCustomerAllowedToSetTopicPriority { get; set; }

    public IEnumerable<SelectListItem> TopicPriorities { get; set; }

    public bool IsCustomerAllowedToSubscribe { get; set; }

    public bool Subscribed { get; set; }

    public bool DisplayCaptcha { get; set; }

    #endregion
}