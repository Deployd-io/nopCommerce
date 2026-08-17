using Microsoft.Extensions.Logging;
﻿using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdBreadcrumbListModel : JsonLdModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public JsonLdBreadcrumbListModel()
    {
		_logger.LogDebug(">>> Entering JsonLdBreadcrumbListModel()");
        ItemListElement = new List<JsonLdBreadcrumbListItemModel>();
		_logger.LogDebug("<<< Exiting JsonLdBreadcrumbListModel()");
		_logger.LogDebug("JsonLdBreadcrumbListModel(): ItemListElement → {ItemListElement}", ItemListElement);
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public static string Context => "https://schema.org";

    [JsonProperty("@type")]
    public static string Type => "BreadcrumbList";

    [JsonProperty("itemListElement")]
    public IList<JsonLdBreadcrumbListItemModel> ItemListElement { get; set; }

    #endregion
}