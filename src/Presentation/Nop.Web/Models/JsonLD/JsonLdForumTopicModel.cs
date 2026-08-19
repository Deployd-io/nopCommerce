using Microsoft.Extensions.Logging;
﻿using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdForumTopicModel : JsonLdModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public JsonLdForumTopicModel()
    {
		_logger.LogDebug(">>> Entering JsonLdForumTopicModel()");
        Comments = new List<JsonLdForumTopicCommentModel>();
        InteractionStatistic = new List<JsonLdInteractionStatisticModel>();
		_logger.LogDebug("JsonLdForumTopicModel(): InteractionStatistic → {InteractionStatistic}", InteractionStatistic);
		_logger.LogDebug("JsonLdForumTopicModel(): Comments → {Comments}", Comments);
		_logger.LogDebug("<<< Exiting JsonLdForumTopicModel()");
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public string Context => "https://schema.org";

    [JsonProperty("@type")]
    public string Type => "DiscussionForumPosting";

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("author")]
    public JsonLdPersonModel Author { get; set; }

    [JsonProperty("datePublished")]
    public string DatePublished { get; set; }

    [JsonProperty("headline")]
    public string Subject { get; set; }

    [JsonProperty("comment")]
    public IList<JsonLdForumTopicCommentModel> Comments { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("interactionStatistic")]
    public IList<JsonLdInteractionStatisticModel> InteractionStatistic { get; set; }

    #endregion
}